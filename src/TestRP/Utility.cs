using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography.X509Certificates;
using System.Runtime.CompilerServices;

namespace TestRP
{
    public static class Utility
    {

        public static bool IsUnitTest = false;

        public static Task TaskCompleted = Task.FromResult(0);

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1060:MovePInvokesToNativeMethodsClass"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Interoperability", "CA1401:PInvokesShouldNotBeVisible"), DllImport("kernel32.dll", SetLastError = true)]
        static extern bool SetHandleInformation(IntPtr hObject, HANDLE_FLAGS dwMask, HANDLE_FLAGS dwFlags);


        [Flags]
        enum HANDLE_FLAGS : uint
        {
            None = 0,
            INHERIT = 1,
            PROTECT_FROM_CLOSE = 2
        }


        static public bool MakeHandleNonInheritable(IntPtr handle)
        {
            var ok = SetHandleInformation(handle, HANDLE_FLAGS.INHERIT, 0);
            if (!ok)
            {
                LogUtility.LogError("Cannot make handle non inheritable." + Environment.NewLine + Environment.StackTrace);
            }
            return ok;
        }

        public static void Write32(byte[] array, int index, uint val)
        {
            array[index] = (byte)val;
            array[index + 1] = (byte)(val >> 8);
            array[index + 2] = (byte)(val >> 16);
            array[index + 3] = (byte)(val >> 24);
        }

        // do i really have to write this
        public static bool AreArrayRangesSame(byte[] a1, int o1, byte[] a2, int o2, int l, bool caseSensitive)
        {
            if (o1 + l > a1.Length) return false;
            if (o2 + l > a2.Length) return false;
            for (int x = 0; x < l; x++)
            {
                byte b1 = a1[o1 + x];
                if (!caseSensitive)
                {
                    if (b1 >= 'a' && b1 <= 'z')
                    {
                        b1 -= (byte)('a' - 'A');
                    }
                }
                if (b1 != a2[o2 + x]) return false;
            }
            return true;
        }

        public static ArraySegment<T> Move<T>(this ArraySegment<T> arr, int count)
        {
            return new ArraySegment<T>(arr.Array, arr.Offset + count, arr.Count - count);
        }

        public static IEnumerable<int> GetIndexes(this Array arr)
        {
            if (arr == null) return Enumerable.Range(0, 0);
            return Enumerable.Range(0, arr.Length);
        }


        static public void ContinueWithSafe(this Task taskToContinue, Action<Task> continuation)
        {
            taskToContinue.ContinueWith(t =>
            {
                try
                {
                    continuation(t);
                }
                catch (Exception e)
                {
                    LogUtility.LogException(e);
                }
            });
        }


        public static async Task TimeoutAfter(this Task t, int millisecondsTimeout)
        {
            if (t.IsCompleted || t.IsFaulted || t.IsCanceled)
            {
                await t; //await on the completed task to transition back the exception
            }
            else
            {
                CancellationTokenSource cts = new CancellationTokenSource();
                Task delay = Task.Delay(millisecondsTimeout, cts.Token);
                if (t == await Task.WhenAny(t, delay))
                {
                    cts.Cancel(); //cancel delay task if operation has already completed to reduce lingering tasks
                    await t;
                }
                else
                {
                    // if the underlying task throws, we need to observe that exception
                    t.ContinueWithSafe((innert) =>
                    {
                        if (innert.Exception != null)
                        {
                            LogUtility.AggregateLogException0(innert.Exception, "Exception in timedout task");
                        }
                    });
                    throw new TimeoutException();
                }
                cts.Dispose();
                delay.Dispose();
            }
        }

        public static async Task<TResult> TimeoutAfter<TResult>(this Task<TResult> task, int millisecondsTimeout)
        {
            await TimeoutAfter((Task)task, millisecondsTimeout);
            return await task; //awaiting on the completed task to transition back exception if any or the result
        }

        public async static Task<bool> RetryAsync(string description, Func<Task<bool>> func, int? number = null, TimeSpan? delay = null, TimeSpan? maxTime = null, Func<Exception, Task> onFail = null, Func<Task> returnFail = null, Action maxTimeExceededHandler = null)
        {
            if (!number.HasValue && !maxTime.HasValue) throw new ArgumentException("either number or maxTime must be specified");
            int attempt = 1;
            DateTime stop = DateTime.UtcNow + (maxTime.HasValue ? maxTime.Value : TimeSpan.Zero);
            while (true)
            {
                Exception exception = null;
                try
                {
                    LogUtility.LogInfo("RoleShared.Utility.RetryAsync({0}) -> Try {1}", description, attempt);
                    bool succeeded = await func();
                    if (succeeded)
                    {
                        LogUtility.LogInfo("RoleShared.Utility.RetryAsync({0}) -> Retry success!", description);
                        return true;
                    }
                }
                catch (Exception e)
                {
                    LogUtility.LogException(e);
                    exception = e;
                }
                if (onFail != null)
                {
                    try
                    {
                        LogUtility.LogError("RoleShared.Utility.RetryAsync({0}) -> invoking onFail", description);
                        await onFail(exception);
                    }
                    catch (Exception e2)
                    {
                        LogUtility.LogException(e2);
                        throw;
                    }
                }
                if (delay.HasValue)
                {
                    try
                    {
                        await Task.Delay(delay.Value);
                    }
                    catch (Exception e3)
                    {
                        LogUtility.LogException(e3);
                    }
                }
                attempt++;
                if (number.HasValue && attempt > number.Value) break;
                if (maxTime.HasValue && DateTime.UtcNow > stop)
                {
                    if (maxTimeExceededHandler != null)
                    {
                        maxTimeExceededHandler();
                        // reset the stop time
                        stop = DateTime.UtcNow + maxTime.Value;
                        continue;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            LogUtility.LogError("RoleShared.Utility.RetryAsync({0}) -> Giving up in Retry", description);
            if (returnFail != null)
            {
                await returnFail();
            }
            return false;
        }

        private static Tuple<int, int> GetNextTupleOfPair(List<Tuple<int, int>> orderedUpdateDomains)
        {
            for (int index = 1; index < orderedUpdateDomains.Count; ++index)
            {
                if (orderedUpdateDomains[index].Item2 != orderedUpdateDomains[0].Item2)
                {
                    return orderedUpdateDomains[index];
                }
            }

            return null;
        }

        public static int GetRoleInstanceNumber(string instanceId)
        {
            int x1 = instanceId.LastIndexOf('.');
            int x2 = instanceId.LastIndexOf('_');
            int x = Math.Max(x1, x2);

            var val = instanceId.Substring(x + 1);

            int instanceIndex = int.Parse(val);

            return instanceIndex;
        }

        private static Tuple<int, int> GetInstanceUpdateDomain(string instanceIdUpdateDomain)
        {
            var parts = instanceIdUpdateDomain.Split(new char[] { ':' });
            return new Tuple<int, int>(int.Parse(parts[0]), int.Parse(parts[1]));
        }

        /// <summary>
        /// Returns int(port) when passed "a.b.c.d:port"
        /// </summary>
        public static int GetPortFromEndpint(string ep)
        {
            var port = ep.Split(':')[1];
            return int.Parse(port);
        }


        public static void DirectoryCopy(string sourceDirName, string destDirName, bool recursive)
        {
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, true);
            }

            if (recursive)
            {
                DirectoryInfo[] dirs = dir.GetDirectories();
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, recursive);
                }
            }
        }

        [DebuggerNonUserCode]
        static string GetProcMainPath(Process proc)
        {
            try
            {
                string path = proc.MainModule.FileName;
                return path;
            }
            catch
            {
                return null;
            }
        }

        // Like Task.Delay(), but doesn't throw exception when cancelled
        [DebuggerNonUserCode]
        public static async Task DelayNoThrow(int milliseconds, CancellationToken token)
        {
            // flush frequency 
            try
            {
                await Task.Delay(milliseconds, token);
            }
            catch
            {
            }
        }

        [DebuggerNonUserCode]
        public static async Task DelayNoThrow(TimeSpan delay, CancellationToken token)
        {
            try
            {
                await Task.Delay(delay, token);
            }
            catch
            {
            }
        }


        public static async Task<bool> DelayNoThrow(string descr, TimeSpan timeout, Func<Task<bool>> waitUntil, CancellationToken token, int maxSleepTimeMS = 1000, TimeSpan? period = null, Func<Task> eachPeriodDo = null)
        {
            DateTime now = DateTime.UtcNow;
            DateTime maxtime = now + timeout;
            DateTime? maxtimeperiod = period.HasValue ? (now + period.Value) : (DateTime?)null;
            TimeSpan maxsleep = TimeSpan.FromMilliseconds(maxSleepTimeMS);
            LogUtility.LogInfo("Starting delay loop for: {0} timeout:{1}", descr, timeout);

            while (true)
            {
                if (token.IsCancellationRequested)
                {
                    LogUtility.LogInfo("DelayLoop: return on cancel");
                    return false;
                }
                if (maxtimeperiod.HasValue && now > maxtimeperiod.Value && eachPeriodDo != null)
                {
                    await eachPeriodDo();
                    now = DateTime.UtcNow;
                    maxtimeperiod = now + period.Value;
                }
                if (await waitUntil())
                {
                    LogUtility.LogInfo("DelayLoop: return on condition satisfied");
                    return true;
                }
                if (token.IsCancellationRequested)
                {
                    LogUtility.LogInfo("DelayLoop: return on cancel");
                    return false;
                }
                now = DateTime.UtcNow;
                if (now >= maxtime)
                {
                    LogUtility.LogInfo("DelayLoop: return on timeout");
                    return false;
                }
                TimeSpan sleeptime;
                if (maxtimeperiod.HasValue && now >= maxtimeperiod.Value)
                {
                    sleeptime = TimeSpan.Zero;
                }
                else if (!maxtimeperiod.HasValue || maxtimeperiod.Value > maxtime)
                {
                    sleeptime = maxtime - now;
                }
                else
                {
                    sleeptime = maxtimeperiod.Value - now;
                }
                if (sleeptime > maxsleep)
                {
                    sleeptime = maxsleep;
                }
                if (sleeptime > TimeSpan.Zero)
                {
                    try
                    {
                        await Task.Delay(sleeptime, token);
                    }
                    catch
                    {
                        LogUtility.LogInfo("DelayLoop: return on cancel exception");
                        return false;
                    }
                }
                now = DateTime.UtcNow;
            }
        }

        public static void CancelNoThrow(this CancellationTokenSource tokenSource)
        {
            if (tokenSource != null)
            {
                try
                {
                    tokenSource.Cancel(false);
                }
                catch
                {
                }
            }
        }

        public static T[] FillArray<T>(this T[] arr, Func<int, T> create)
        {
            for (int x = 0; x < arr.Length; x++)
            {
                arr[x] = create(x);
            }
            return arr;
        }

        static public IEnumerable<Exception> Flatten(this Exception e)
        {
            if (e == null) return new Exception[0];
            var agg = e as AggregateException;
            if (agg == null || agg.InnerExceptions == null)
            {
                return new Exception[] { e }.Concat(e.InnerException.Flatten());
            }
            else
            {
                return new Exception[] { e }.Concat(agg.InnerExceptions.SelectMany(Flatten));
            }
        }

        public static string GenerateTimeBasedName(string prefix, string suffix)
        {
            var t = DateTime.UtcNow;
            var dt = t.Date.ToShortDateString() + " " + t.TimeOfDay.ToString();

            dt = dt.Replace(' ', '-').Replace(':', '-').Replace('/', '-');
            return prefix + dt + suffix;
        }

        public static string EndpointToString(EndPoint ep, bool includePort = true)
        {
            IPEndPoint ipe = ep as IPEndPoint;
            IPAddress addr = ipe.Address;
            string sa;
            if (addr.IsIPv4MappedToIPv6)
            {
                var dest = new byte[4];
                var bytes = addr.GetAddressBytes();
                Array.Copy(bytes, bytes.Length - 4, dest, 0, 4);
                sa = new IPAddress(dest).ToString();
            }
            else
            {
                sa = addr.ToString();
            }
            if (!includePort) return sa;
            return sa + ":" + ipe.Port.ToString();
        }

        public static string GetAssemblyInfo(Assembly assembly)
        {
            FileVersionInfo fileInfo = FileVersionInfo.GetVersionInfo(assembly.Location);

            var title = GetAssemblyTitle(assembly);

            var format = "FileVersion={0}, AssemblyTitle={1}";
            return string.Format(format, fileInfo.FileVersion, title);
        }

        public static string GetAssemblyInfo(string assemblyName)
        {

            var specificAssembly = AppDomain.CurrentDomain.GetAssemblies().
                   SingleOrDefault(assembly => assembly.GetName().Name == assemblyName);
            if (specificAssembly != null)
            {
                return GetAssemblyInfo(specificAssembly);
            }

            return null;
        }

        public static T[] CombineLists<T>(List<T> list1, List<T> list2)
        {
            list1.AddRange(list2);
            return list1.Distinct().ToArray();
        }

        public static T[] ShiftArray<T>(IReadOnlyList<T> arr, int position)
        {
            if (position >= arr.Count) return arr.ToArray();
            var arrFirst = arr.Take(position);
            var arrLast = arr.Skip(position);
            return arrLast.Concat(arrFirst).ToArray();
        }

        public static string GetAssemblyTitle(Assembly assembly)
        {
            var attribute = assembly.GetCustomAttribute(typeof(AssemblyTitleAttribute)) as AssemblyTitleAttribute;
            return attribute != null ? attribute.Title : "TitleNotFound";
        }

        public static string GetExceptionCallstack(Exception e)
        {
            try
            {
                if (e == null) return "";
                if (e is AggregateException)
                {
                    var a = e as AggregateException;
                    var inner = a.InnerException;
                    if (inner == null)
                    {
                        return e.StackTrace ?? "No callstack available";
                    }
                    else
                    {
                        var innere = GetExceptionCallstack(inner) ?? "No Inner callstack";
                        var outer = e.StackTrace ?? "No outer callstack";
                        return inner + Environment.NewLine + outer;
                    }
                }
                else
                {
                    return e.StackTrace ?? "Null callstack";
                }
            }
            catch
            {
                return "No callstack obtained";
            }
        }

        public static string GetExceptionMessage(Exception e)
        {
            try
            {
                if (e == null) return "";
                if (e is AggregateException)
                {
                    var a = e as AggregateException;
                    var inner = a.InnerExceptions;
                    if (inner == null || inner.Count == 0)
                    {
                        return string.Format("{0}({1})", e.Message, GetExceptionMessage(e.InnerException));
                    }
                    else
                    {
                        string i = string.Join("|", a.InnerExceptions.Select(GetExceptionMessage));
                        return string.Format("{0}({1})", e.Message, i);
                    }
                }
                else
                {
                    return string.Format("{0}({1})", e.Message, GetExceptionMessage(e.InnerException));
                }
            }
            catch (Exception e2)
            {
                return e2.ToString();
            }
        }

        public static DateTime ParseUnixTimeInS(long p)
        {
            return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc) + TimeSpan.FromSeconds(p);
        }

        public static long UnixInMS(this DateTime dt)
        {
            return (long)((dt - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds);
        }


        static public Task RunAfterDelay(TimeSpan delay, Action what, CancellationToken token)
        {
            if (token.IsCancellationRequested) return Task.FromResult(1);

            if (delay > TimeSpan.Zero)
            {
                return Task.Run(async () =>
                {
                    try
                    {
                        await Task.Delay(delay, token);
                    }
                    catch
                    {
                    }
                    if (token.IsCancellationRequested) return;
                    what();
                });
            }
            else
            {
                return Task.Run(what);
            }
        }

        public static bool NeverFatal(Exception e)
        {
            return false;
        }

        static bool IsCancel(Exception e)
        {
            if (e == null) return false;
            if (e is OperationCanceledException || e is TaskCanceledException) return true;
            if (e is AggregateException)
            {
                var ae = e as AggregateException;
                if (IsCancel(ae.InnerException)) return true;
                foreach (var ie in ae.InnerExceptions)
                {
                    if (IsCancel(ie)) return true;
                }
            }
            return false;
        }


        public static void RunAndTime(string descr, Action a)
        {
            Stopwatch sw = new Stopwatch();
            try
            {
                sw.Start();
                a();
            }
            finally
            {
                sw.Stop();
                LogUtility.LogVerbose("Operation timing: {0} took {1}", descr, sw.Elapsed);
            }
        }

        public static R RunAndTime<R>(string descr, Func<R> a)
        {
            Stopwatch sw = new Stopwatch();
            try
            {
                sw.Start();
                return a();
            }
            finally
            {
                sw.Stop();
                LogUtility.LogVerbose("Operation timing: {0} took {1}", descr, sw.Elapsed);
            }
        }


        public static R RunAndTime<R, A1, A2>(string descr, Func<Func<A1, A2, R>, A1, A2, R> wrapper, Func<A1, A2, R> target, A1 a1, A2 a2)
        {
            Stopwatch sw = new Stopwatch();
            try
            {
                sw.Start();
                return wrapper(target, a1, a2);
            }
            finally
            {
                sw.Stop();
                LogUtility.LogVerbose("Operation timing: {0} took {1}", descr, sw.Elapsed);
            }
        }

        //RunNoThrow has IComparable constraint so that a lambda returning Task is not passed to it.
        //for lambda returning Task use RunNoThrowAsync instead.
        public static R RunNoThrow<T, R>(Func<T, R> f, T arg1) where R : IComparable
        {
            try
            {
                return f(arg1);
            }
            catch (Exception e)
            {
                LogUtility.LogException(e);
                return default(R);
            }
        }
        public static R RunNoThrow<T1, T2, R>(Func<T1, T2, R> f, T1 arg1, T2 arg2) where R : IComparable
        {
            try
            {
                return f(arg1, arg2);
            }
            catch (Exception e)
            {
                LogUtility.LogException(e);
                return default(R);
            }
        }

        public static R RunNoThrow<R>(Func<R> f) where R : IComparable
        {
            try
            {
                return f();
            }
            catch (Exception e)
            {
                LogUtility.LogException(e);
                return default(R);
            }
        }

        public static R? RunNoThrow<R>(Func<R?> f) where R : struct
        {
            try
            {
                return f();
            }
            catch (Exception e)
            {
                LogUtility.LogException(e);
                return default(R);
            }
        }

        public static bool RunNoThrow(Action a)
        {
            try
            {
                a();
                return true;
            }
            catch (Exception e)
            {
                LogUtility.LogException(e);
            }
            return false;
        }
        public static void RunNoThrow<A>(Action<A> a, A v)
        {
            try
            {
                a(v);
            }
            catch (Exception e)
            {
                LogUtility.LogException(e);
            }
        }

        static async public Task<bool> RunNoThrowAsync<T>(Func<T, Task> action, T arg, Func<Exception, bool> IsFatal = null)
        {
            if (IsFatal == null) IsFatal = IsCancel;

            try
            {
                await action(arg);
                return true;
            }
            catch (Exception e)
            {
                LogUtility.LogException(e);
                if (IsFatal(e)) throw;
            }
            return false;
        }

        static async public Task RunNoThrowAsync(Func<Task> action, Func<Exception, bool> IsFatal = null)
        {
            if (IsFatal == null) IsFatal = IsCancel;

            try
            {
                await action();
            }
            catch (Exception e)
            {
                LogUtility.LogException(e);
                if (IsFatal(e)) throw;
            }
        }

        static async public Task RunNoThrowAsync(Task t)
        {
            try
            {
                await t;
            }
            catch (Exception e)
            {
                LogUtility.LogException(e);
            }
        }


        public static void RunOnceAndClear(ref Action a)
        {
            if (a != null)
            {
                a();
                a = null;
            }
        }

        // Security: timing safe comparison using Bitwise OR and Bitwise XOR
        // that hopefully doesn't leak any timing information usable for side channel attacks
        // other than, possibly, the length of the key, which since we generate keys everyone
        // can deduce anyway.
        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        public static bool TimingSafeEquals(byte[] key, byte[] requestKey)
        {
            if (key.Length <= 0)
            {
                throw new ArgumentException("array must not be empty", "key");
            }
            if (requestKey.Length < key.Length)
            {
                throw new ArgumentException("request buffer is too short", "requestKey");
            }

            // note: this XOR,OR accumulation is same strategy as redis uses to compare buffers without leaking timing information
            int diff = 0;
            for (int i = 0; i < key.Length; i++)
            {
                int xor = key[i] ^ requestKey[i]; // xor will be nonzero if any bits differ
                diff = diff | xor; // diff therefore becomes nonzero if any bits differed ever
            }

            return diff == 0;
        }


        static public void ForEachTask<T>(IEnumerable<Task<T>> tasks, Action<Exception> whenFailed = null, Action<T> whenCompleted = null)
        {
            if (tasks == null || (whenFailed == null && whenCompleted == null)) return;

            foreach (var t in tasks)
            {
                if (t.IsFaulted && t.Exception != null)
                {
                    if (whenFailed != null)
                    {
                        whenFailed(t.Exception);
                    }
                    continue;
                }
                if (t.IsCompleted && t.Exception == null && !t.IsCanceled)
                {
                    if (whenCompleted != null)
                    {
                        whenCompleted(t.Result);
                    }
                    continue;
                }
            }
        }

        public static void RunLoopInBackground(Func<Task> doWhat, TimeSpan interval, CancellationToken? cancel = null)
        {
            RunLoopInBackground(c => doWhat(), interval, cancel);
        }

        public static async void RunLoopInBackground(Func<CancellationToken, Task> doWhat, TimeSpan interval, CancellationToken? cancel = null)
        {
            await Task.Yield();
            var ct = cancel.HasValue ? cancel.Value : CancellationToken.None;
            while (!ct.IsCancellationRequested)
            {
                await RunNoThrowAsync<CancellationToken>(doWhat, ct, x => false);
                await Utility.DelayNoThrow(interval, ct);
            }
        }

        public static Action RunLoopInBackgroundUntilStopped(Func<CancellationToken, Task> what, TimeSpan interval, CancellationToken? cancel = null)
        {
            CancellationTokenSource cts = cancel.HasValue ? CancellationTokenSource.CreateLinkedTokenSource(cancel.Value) : new CancellationTokenSource();
            RunLoopInBackground(what, interval, cts.Token);
            return () => { cts.Cancel(true); };
        }

        public static Action RunLoopAsyncUntilStopped(Func<Task> what, TimeSpan interval, CancellationToken? cancel = null)
        {
            return RunLoopInBackgroundUntilStopped(c => what(), interval, cancel);
        }

        static public void RecordMax(ref long maxValue, long newValue)
        {
            while (true)
            {
                long temp = maxValue;
                if (temp < newValue)
                {
                    if (temp == Interlocked.CompareExchange(ref maxValue, newValue, temp))
                    {
                        break;
                    }
                }
                else
                {
                    break;
                }
            }
        }

    }
}
