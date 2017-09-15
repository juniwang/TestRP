using AzNginx.Common;
using AzNginx.Provision.Core.ServiceSettings;
using Microsoft.WindowsAzure.Storage.Queue;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AzNginx.Provision.Core.Scheduler
{
    /// <summary>
    /// Define a delegate for the callback which will be invoked when the task did not do any work 
    /// during an iteration. This delgate helps to auto scale down of the dequeue task(s) or decrease 
    /// the queue polling rate based on the transition from non-empty to empty queue.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="idleCount">A value indicating how many times the task has been idle.</param>
    /// <param name="newDelay">A delay by which the task needs to be sleep before next iteration.</param>
    /// <returns>A boolean value indicating the task should stop retrieving further message and terminate</returns>
    /// <remarks></remarks>
    public delegate bool HandleQueueEmptyDelegate(object sender, int queueId, int idleCount, out TimeSpan newDelay);

    /// <summary>
    /// Define a delegate for the call back which will be invoked when the task found messages in the queue
    /// while it was in sleep state. This delegate helps to auto scale up the dequeue tasks based on the 
    /// tansition from empty to non-empty queue.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <returns></returns>
    /// <remarks></remarks>
    public delegate void HandleQueueWorkDetectedDelegate(object sender, int queueId);

    /// <summary>
    /// JobDispatcher responsible for creating and managing the tasks (.NET task) those
    /// gets the job from IJobProducer and get it processed with the help of IJobConsumer.
    /// Note: Dispatcher can be treated as the channel that connects producer and consumer.
    /// </summary>
    public class JobDispatcher : IDisposable
    {
        private bool disposed = false;
        private readonly CancellationTokenSource cancellationSignal = new CancellationTokenSource();
        private IList<ConcurrentBag<Task>> dequeueTaskLists;
        private IList<BlockingCollection<Task>> dequeueTasks;
        private IList<IProducerQueueInfo> producerQueueInfos;
        private int producerQueueCount;
        private IJobConsumer jobConsumer;
        private IJobManager jobManager;

        public HandleQueueEmptyDelegate QueueEmpty;
        public HandleQueueWorkDetectedDelegate QueueWorkDetected;

        /// <summary>
        /// Creates an instance of 'JobDispatcher'.
        /// </summary>
        /// <param name="jobManager"></param>
        /// <param name="serviceSettings"></param>
        public JobDispatcher(IJobManager jobManager, IServiceSettings serviceSettings)
        {
            this.jobManager = jobManager;
            this.jobManager.Initialize(serviceSettings);

            this.producerQueueInfos = jobManager.JobProducer.ProducerQueueInfos;
            this.jobConsumer = jobManager.JobConsumer;
            this.producerQueueCount = this.producerQueueInfos.Count();

            this.dequeueTaskLists = new List<ConcurrentBag<Task>>();
            this.dequeueTasks = new List<BlockingCollection<Task>>();

            for (int i = 0; i < producerQueueCount; i++)
            {
                this.dequeueTaskLists.Add(new ConcurrentBag<Task>());
                this.dequeueTasks.Add(new BlockingCollection<Task>(this.dequeueTaskLists[i]));
            }
        }

        /// <summary>
        /// Starts retrieval and dispatching of jobs.
        /// </summary>
        public void Start()
        {
            try
            {
                Action<object> DequeueJobsAndDispatchInParallelMain = null;
                if (this.jobManager.JobDispatchingType == JobDispatchingType.NoWaitAnyTime)
                {
                    DequeueJobsAndDispatchInParallelMain
                        = DequeueJobsAndDispatchInParallelMain_NoWaitAnyTime;
                }
                else if (this.jobManager.JobDispatchingType == JobDispatchingType.WaitIfLimitReached)
                {
                    DequeueJobsAndDispatchInParallelMain
                        = DequeueJobsAndDispatchInParallelMain_WaitIfLimitReached;
                }
                else
                {
                    throw new NotImplementedException("Dispatching logic for the given JobDispatchingType is not implemented");
                }


                for (int i = 0; i < this.producerQueueCount; i++)
                {
                    if (this.dequeueTasks[i].IsAddingCompleted)
                    {
                        this.dequeueTasks[i] = new BlockingCollection<System.Threading.Tasks.Task>(this.dequeueTaskLists[i]);
                    }

                    for (int j = 0; j < this.producerQueueInfos[i].Concurrency; j++)
                    {
                        CancellationToken cancellationToken = this.cancellationSignal.Token;
                        DequeueAndDispatchJobState workerState = new DequeueAndDispatchJobState(this.jobManager.OnError,
                            this.producerQueueInfos[i],
                            this.jobConsumer,
                            cancellationToken);

                        this.dequeueTasks[i].Add(Task.Factory.StartNew(
                            DequeueJobsAndDispatchInParallelMain,
                            workerState,
                            cancellationToken,
                            TaskCreationOptions.LongRunning,
                            TaskScheduler.Default)
                        );
                    }

                    this.dequeueTasks[i].CompleteAdding();
                }
            }
            finally
            {
                // TraceInformation("Started Dequeue Tasks and Dispatching");
            }
        }

        /// <summary>
        /// Stop retrieval and dispatching of jobs.
        /// </summary>
        public void Stop()
        {
            try
            {
                // Send Cancellation to all task and wait them to finish
                cancellationSignal.Cancel();
                for (int i = 0; i < this.dequeueTasks.Count(); i++)
                {
                    foreach (var task in this.dequeueTasks[i])
                    {
                        // Log this event
                        try
                        {
                            // Block until the task completes (if it is running).
                            if (task.Status != TaskStatus.Canceled && task.Status != TaskStatus.Faulted && task.Status != TaskStatus.RanToCompletion)
                            {
                                task.Wait();
                            }
                        }
                        catch (AggregateException)
                        {
                            // Ignore this exception, catching this to ensure a safe stop
                            // http://www.albahari.com/threading/part5.aspx#_Working_with_AggregateException
                        }
                        catch (OperationCanceledException)
                        {
                            // Should ensure a safe stop, just ignore this exception and don't let it damage 
                            // the rest of the stop logic.
                        }
                        catch
                        {
                            // TODO: Log Exception
                            //Just log an exception and  ensure a safe stop
                            // LogError this exception
                        }
                        finally
                        {
                            // Log Info
                            task.Dispose();
                        }
                    }
                }
            }
            finally
            {
                // Log completion
            }
        }

        /// <summary>
        /// The code behind the long running task responsible for reading batch of jobs from 
        /// a queue and processing it in parallel.
        /// 
        /// Dispatcher uses this style of processing if jobManager.JobDispatchingType is 'WaitIfLimitReached'
        /// @see Marketplace.Common.JobDispatchingType.WaitIfLimitReached
        /// </summary>
        /// <param name="state"></param>
        private void DequeueJobsAndDispatchInParallelMain_WaitIfLimitReached(object state)
        {
            DequeueAndDispatchJobState workerState = (DequeueAndDispatchJobState)state;
            TimeSpan sleepInterval = (TimeSpan)workerState.ProdcuerQueueInfo.DequeueInterval;
            short max_batch_size = workerState.ProdcuerQueueInfo.DequeueBatchSize;
            TimeSpan visibilityTimeOut = (TimeSpan)workerState.ProdcuerQueueInfo.VisibilityTimeOut;
            int queueId = workerState.ProdcuerQueueInfo.QueueId;

            List<Task> tasks = new List<Task>();
            int idleStateCount = 0;
            short currentBatchSize = max_batch_size;

            try
            {
                // Run a dequeue task until asked to terminate (from stop() method) or 
                // until a break condition is encountered.
                while (workerState.CanRun)
                {
                    try
                    {
                        ExceptionDetails exceptionDetails = null;
                        var queueMessages = from msg
                            in workerState.ProdcuerQueueInfo.GetJobs(currentBatchSize,
                                visibilityTimeOut, out exceptionDetails)
                                            where msg != null
                                            select msg;
                        int messageCount = 0;

                        // Check whether or not work items arrived to a queue while the 
                        // listener was idle.
                        if (idleStateCount > 0 && queueMessages.Count() > 0)
                        {
                            if (QueueWorkDetected != null)
                            {
                                QueueWorkDetected(this, workerState.ProdcuerQueueInfo.QueueId);
                            }
                        }

                        /**
                         * Can we replace foreach to use code similar to below Select?
                          tasks = queueMessages.Select(queueMessage => Task.Factory.StartNew((taskParam) =>
                          {
                              CloudQueueMessage message = taskParam as CloudQueueMessage;
                              idleStateCount = 0;
                              ExceptionDetails exceptionDetails2 = null;
                              workerState.JobConsumer.OnJob(queueId, message, out exceptionDetails2);
                              messageCount++;
                          }, queueMessage, TaskCreationOptions.None)).ToList();
                         **/

                        foreach (var queueMessage in queueMessages)
                        {
                            Task task = Task.Factory.StartNew((taskParam) =>
                            {
                                CloudQueueMessage message = taskParam as CloudQueueMessage;
                                idleStateCount = 0;
                                ExceptionDetails exceptionDetails2 = null;
                                workerState.JobConsumer.OnJob(queueId, message, out exceptionDetails2);
                                messageCount++;
                            }, queueMessage, TaskCreationOptions.None);

                            tasks.Add(task);
                        }

                        var tasksAsArray = tasks.ToArray();
                        short numTasks = (short)tasksAsArray.Count();

                        if (numTasks != 0)
                        {
                            int index = Task.WaitAny(tasksAsArray);
                            tasks.RemoveAt(index);
                            currentBatchSize = (short)(max_batch_size - numTasks);
                        }
                        else
                        {
                            currentBatchSize = max_batch_size;
                        }

                        if (0 == messageCount)
                        {
                            idleStateCount++;
                            if (QueueEmpty != null)
                            {
                                if (QueueEmpty(this, queueId, idleStateCount, out sleepInterval))
                                {
                                    break;
                                }
                            }

                            Thread.Sleep(sleepInterval);
                        }
                    }
                    catch (Exception exception)
                    {
                        if (exception is OperationCanceledException)
                        {
                            throw;
                        }
                        else
                        {
                            // handover the responsibility for handling or reporting the error 
                            // to IJobManager.OnError.
                            workerState.OnError(exception);
                            // Sleep for some time to avoid accumulating lots of error
                            Thread.Sleep(sleepInterval);
                        }
                    }
                }
            }
            finally
            {
                // workerState.JobManager.OnCompleted((workerState.ProdcuerQueueInfo.QueueId);
                // Log
            }
        }

        /// <summary>
        /// The code behind the long running task responsible for reading batch of jobs from queue and
        /// processing them in parallel.
        /// 
        /// Dispatcher uses this style of processing if jobManager.JobDispatchingType is 'NoWaitAnyTime'
        /// @see Marketplace.Common.JobDispatchingType.NoWaitAnyTime
        /// </summary>
        /// <param name="state">a reference to instance of DequeueAndDispatchJobState structure.</param>
        private void DequeueJobsAndDispatchInParallelMain_NoWaitAnyTime(object state)
        {
            DequeueAndDispatchJobState workerState = (DequeueAndDispatchJobState)state;
            TimeSpan sleepInterval = (TimeSpan)workerState.ProdcuerQueueInfo.DequeueInterval;
            short dequeueBatchSize = workerState.ProdcuerQueueInfo.DequeueBatchSize;
            TimeSpan visibilityTimeOut = (TimeSpan)workerState.ProdcuerQueueInfo.VisibilityTimeOut;
            int queueId = workerState.ProdcuerQueueInfo.QueueId;

            int idleStateCount = 0;

            try
            {
                // Run a dequeue task until asked to terminate (from stop() method) or 
                // until a break condition is encountered.
                while (workerState.CanRun)
                {
                    try
                    {
                        ExceptionDetails exceptionDetails = null;
                        var queueMessages = from msg
                            in workerState.ProdcuerQueueInfo.GetJobs(dequeueBatchSize,
                                visibilityTimeOut, out exceptionDetails).AsParallel()
                                            where msg != null
                                            select msg;
                        int messageCount = 0;

                        // Check whether or not work items arrived to a queue while the listener was idle.
                        if (idleStateCount > 0 && queueMessages.Count() > 0)
                        {
                            QueueWorkDetected?.Invoke(this, workerState.ProdcuerQueueInfo.QueueId);
                        }

                        queueMessages.ForAll((message) =>
                        {
                            idleStateCount = 0;
                            ExceptionDetails exceptionDetails2 = null;
                            workerState.JobConsumer.OnJob(queueId, message, out exceptionDetails2);
                            messageCount++;
                        });

                        if (0 == messageCount)
                        {
                            idleStateCount++;
                            if (QueueEmpty != null)
                            {
                                if (QueueEmpty(this, queueId, idleStateCount, out sleepInterval))
                                {
                                    break;
                                }
                            }

                            Thread.Sleep(sleepInterval);
                        }
                    }
                    catch (Exception exception)
                    {
                        if (exception is OperationCanceledException)
                        {
                            throw;
                        }
                        else
                        {
                            // handover the responsibility for handling or reporting the error 
                            // to IJobManager.OnError.
                            workerState.OnError(exception);
                            // Sleep for some time to avoid accumulating lots of error
                            Thread.Sleep(sleepInterval);
                        }
                    }
                }
            }
            finally
            {
                // workerState.JobManager.OnCompleted((workerState.ProdcuerQueueInfo.QueueId);
                // Log
            }
        }

        /// <summary>
        /// Types to wrap all datas that needs to be passed to task (thread) function.
        /// </summary>
        private sealed class DequeueAndDispatchJobState
        {
            private readonly CancellationToken cancellationToken;
            private readonly IProducerQueueInfo producerQueueInfo;
            private readonly IJobConsumer jobConsumer;
            private readonly Action<Exception> onError;

            public IProducerQueueInfo ProdcuerQueueInfo
            {
                get { return this.producerQueueInfo; }
            }

            public IJobConsumer JobConsumer
            {
                get { return this.jobConsumer; }
            }

            public Action<Exception> OnError
            {
                get { return this.onError; }
            }

            public bool CanRun
            {
                get
                {
                    return !this.cancellationToken.IsCancellationRequested;
                }
            }

            public DequeueAndDispatchJobState(Action<Exception> onError, IProducerQueueInfo producerQueueInfo, IJobConsumer jobConsumer, CancellationToken cancellationToken)
            {
                this.cancellationToken = cancellationToken;
                this.producerQueueInfo = producerQueueInfo;
                this.jobConsumer = jobConsumer;
                this.onError = onError;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    if (cancellationSignal != null)
                    {
                        cancellationSignal.Dispose();
                    }

                    if (dequeueTasks != null)
                    {
                        for (int i = 0; i < dequeueTasks.Count; i++)
                        {
                            if (dequeueTasks[i] != null)
                            {
                                dequeueTasks[i].Dispose();
                            }
                        }
                    }
                }

                disposed = true;
            }
        }
    }
}
