using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzNginx.Provision.Core.ServiceSettings
{
    /// <summary>
    /// The possible types of job dispatching that JobDispatcher supports.
    /// 
    /// Note: JobDispatcher is responsible for fetching jobs 
    /// from one or more sources (e.g. Windows Azure Queue) and dispatching these jobs to 
    /// handlers for processing.
    /// </summary>
    public enum JobDispatchingType
    {
        // This options cause dispatcher to always spin up a thread as soon as a new job is available 
        // in the job source (e.g. Windows Azure Queue). Dispatcher will not put any limit on the number 
        // of threads that can run at a given point of  time. As more jobs appear in the queue more 
        // threads gets spinned up. 
        // 
        // Note-1: This type of processing is recommended if you know the jobs finishes quickly and less 
        // CPU intensive.
        //
        // Note-2: If more jobs comes and if CPU usage reaches beyond a limit then its the time to spinup 
        // new instances to handle the load.
        //
        NoWaitAnyTime,

        // This option cause dispatcher to limit the number of threads running at a time. There is a max limit
        // for number of threads runs at a given point of time. Lets call this max limit to max_batch_size. 
        // If there is already max_batch_size threads running and if new jobs appear in the job source (e.g.
        // Windows Azure Queue) then those jobs needs to wait for threads to be available.
        //
        // Note-1: This type of processing is recommended if you know the jobs takes more time to finish and 
        // more CPU intensive.
        //
        // Note-2: If many jobs comes and found the 'wait time for processing' of these jobs in the queue then 
        // its the time to spinup new instnces to handle the load.
        //
        WaitIfLimitReached
    };
}
