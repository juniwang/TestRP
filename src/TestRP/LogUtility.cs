using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using TestRP.Extensions;
using Microsoft.Diagnostics.Tracing;

namespace TestRP
{
    public class LogUtility
    {
        /// <summary>
        /// 'Pit of Success' String Formatting.
        /// Never throws FormatException.
        /// </summary>
        public static string SafeStringFormat(string format, params object[] args)
        {
            try
            {
                if (args == null || args.Length == 0) return format;
                return string.Format(format, args);
            }
            catch (FormatException)
            {
                string argsDetail =
                    string.Join("'", Array.ConvertAll(args, obj => obj?.ToString()));

                return $"StringFormattingFailed. Format String: '{format}'. Arguments: {argsDetail}.";
            }
        }

        static object lockObject = new object();
        static volatile TraceSource _source;
        static int _defaultEventId;

        static AggregateLog _aggregateLog = new AggregateLog();

        public static Func<TraceEventType, string, int, bool> Filter;
        public static TraceSource PreFilterTraceSource;
        public static bool ConfigureTextWriterTracing = false;


        public static void AggregateLogException1(Exception e, string msg, string d1, params object[] args)
        {
            _aggregateLog.AggregateLogException1(e, msg, d1, args);
        }
        public static void AggregateLogException2(Exception e, string msg, string d1, string d2, params object[] args)
        {
            _aggregateLog.AggregateLogException2(e, msg, d1, d2, args);
        }
        public static void AggregateLogException3(Exception e, string msg, string d1, string d2, string d3, params object[] args)
        {
            _aggregateLog.AggregateLogException3(e, msg, d1, d2, d3, args);
        }

        public static void AggregateLogException0(Exception e, string msg, params object[] args)
        {
            _aggregateLog.AggregateLogException0(e, msg, args);
        }

        public static void DumpAggregateLog()
        {
            try
            {
                var contents = _aggregateLog.GetLogString();
                WriteEvent(TraceEventType.Information, contents);
            }
            catch (Exception e)
            {
                LogException(e);
            }
        }

        public static void LogAggregate(params object[] args)
        {
            _aggregateLog.Log(args);
        }
        public static void LogInfo(int eventid, string msg, params object[] args)
        {
            string content = GetFormattedMessage(msg, args);
            WriteEvent(TraceEventType.Information, content, eventid);
        }

        public static void LogGeneric(TraceEventType level, string msg, params object[] args)
        {
            LogGeneric(level, GetDefaultEventId(), msg, args);
        }

        public static void LogGeneric(TraceEventType level, int id, string msg, params object[] args)
        {
            string content = GetFormattedMessage(msg, args);
            WriteEvent(level, content, id);
        }


        public static void LogInfo(string msg, params object[] args)
        {
            LogInfo(GetDefaultEventId(), msg, args);
        }
        public static void LogError(string msg, params object[] args)
        {
            LogError(GetDefaultEventId(), msg, args);
        }

        public static void LogError(int eventid, string msg, params object[] args)
        {
            string content = GetFormattedMessage(msg, args);
            WriteEvent(TraceEventType.Error, content, eventid);
        }

        public static void LogWarning(int eventid, string msg, params object[] args)
        {
            string content = GetFormattedMessage(msg, args);
            WriteEvent(TraceEventType.Warning, content, eventid);
        }
        public static void LogWarning(string msg, params object[] args)
        {
            LogWarning(GetDefaultEventId(), msg, args);
        }

        public static void LogVerbose(string msg, params object[] args)
        {
            LogVerbose(GetDefaultEventId(), msg, args);
        }

        public static void LogVerbose(int eventid, string msg, params object[] args)
        {
            string content = GetFormattedMessage(msg, args);
            WriteEvent(TraceEventType.Verbose, content, eventid);
        }

        public static void LogCritical(string msg)
        {
            WriteEvent(TraceEventType.Critical, msg);
            //TenantMdmMetrics.Instance.LogMetrics(TenantMdmMetrics.GenericCriticalEventsMetric);
        }

        public static void LogCritical(string metricName, string msg)
        {
            WriteEvent(TraceEventType.Critical, msg);
            //TenantMdmMetrics.Instance.LogMetrics(metricName);
        }

        public static void LogException(Exception ex, HttpRequestMessage request = null, TraceEventType traceEventType = TraceEventType.Warning, bool handled = true, int? eventId = null)
        {
            string content = SafeStringFormat("{0} {1} Exception: {2}",
                request != null ? GetLogPrefix(request) : string.Empty,
                handled ? "Handled" : "Unhandled",
                ex.ToString());
            WriteEvent(traceEventType, content, eventId: eventId);
        }

        public static string GetLogPrefix(Guid requestId)
        {
            return SafeStringFormat("[RequestId: {0}]", requestId);
        }

        public static string GetLogPrefix(HttpRequestMessage request)
        {
            StringBuilder logPrefixBuilder = new StringBuilder();
            logPrefixBuilder.AppendFormat("[RequestId: {0}", request.GetRequestId());
            string correlationRequestId = request.GetCsmCorrelationId();
            if (!string.IsNullOrEmpty(correlationRequestId))
            {
                logPrefixBuilder.AppendFormat(",CorrelationRequestId: {0}", correlationRequestId);
            }

            string clientRequestId = request.GetClientRequestId();
            if (!string.IsNullOrEmpty(clientRequestId))
            {
                logPrefixBuilder.AppendFormat(",ClientRequestId: {0}", clientRequestId);
            }

            logPrefixBuilder.Append("]");
            return logPrefixBuilder.ToString();
        }

        public static void WriteEvent(TraceEventType eventType, string content, int? eventId = null, bool includeTime = true)
        {
            EnsureInitialized();
            // only write events if content is not empty
            if (string.IsNullOrWhiteSpace(content))
            {
                return;
            }

            int thisEventId = eventId.HasValue ? eventId.Value : _defaultEventId;
            int origEventId = thisEventId;
            try
            {
                //if (RoleEnvironmentHelper.IsRunningSingleBox())
                //{
                //    if (thisEventId == _defaultEventId)
                //    {
                //        thisEventId = Utility.GetRoleInstanceNumber();
                //    }
                //    content = "(" + Utility.GetRoleInstanceNumber() + ")" + content;
                //}
                if (includeTime)
                {
                    content = "[" + DateTime.UtcNow.TimeOfDay.ToString() + "] " + content;
                }
                if (PreFilterTraceSource != null)
                {
                    PreFilterTraceSource.TraceEvent(eventType, thisEventId, content);
                    PreFilterTraceSource.Flush();
                }
                if (Filter != null && !Filter(eventType, content, origEventId))
                    return;
                _source.TraceEvent(eventType, thisEventId, content);
            }
            catch (Exception e)
            {
                _source.TraceEvent(TraceEventType.Critical, _defaultEventId, "Logging exception " + e.ToString());
                _source.TraceEvent(TraceEventType.Critical, _defaultEventId, "original message: " + content);
            }

            // etw logger
            WriteToEventSource(thisEventId, eventType, content);
        }

        public static void WriteToEventSource(int eventId, TraceEventType eventType, string content)
        {

            switch (eventId)
            {
                default:
                    SpringBoardEventSource.Instance.LogDefault(GetLogLevel(eventType), content);
                    break;
            }
        }

        private static int GetLogLevel(TraceEventType eventType)
        {
            switch (eventType)
            {
                case TraceEventType.Verbose:
                    return GetLogLevel(EventLevel.Verbose);
                case TraceEventType.Information:
                    return GetLogLevel(EventLevel.Informational);
                case TraceEventType.Warning:
                    return GetLogLevel(EventLevel.Warning);
                case TraceEventType.Error:
                    return GetLogLevel(EventLevel.Error);
                case TraceEventType.Critical:
                    return GetLogLevel(EventLevel.Critical);
                default:
                    return GetLogLevel(EventLevel.Informational);
            }
        }

        public static int GetLogLevel(EventLevel level)
        {
            return (int)level;
        }

        /// <summary>
        ///  // msg can contain "{" and "}". To keep it simple we will ignore format incase args is NOT present 
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static string GetFormattedMessage(string msg, object[] args)
        {
            return (args != null && (args.Length > 0)) ? SafeStringFormat(msg, args) : msg;
        }

        private static void EnsureInitialized()
        {
            // Using .net 2.0 tracing, 
            // http://msdn.microsoft.com/en-us/library/ms228993(v=vs.110).aspx
            // with Azure Diagnostics Trace Listener.
            if (_source == null)
            {
                lock (lockObject)
                {
                    if (_source == null)
                    {
                        StartAggregateDumper();
                        _defaultEventId = GetDefaultEventId();
                        Trace.Listeners.Clear();

                        if (!RoleEnvironmentHelper.IsRunningInRealAzure())
                        {
                            Trace.Listeners.Add(new DefaultTraceListener());
                            Trace.Listeners.Add(new ConsoleTraceListener());
                        }

                        // SourceLevels.All - EMIT all events from the trace source by default
                        _source = new TraceSource(GetTraceSourceName(), SourceLevels.All);

                        // Workaround-ish
                        // In Emulator, for some reason, source.Listeners != System.Diganostics.Trace.Listeners
                        // Which clearly breaks everyone's intuition of how tracing should work.
                        _source.Listeners.Clear();
                        _source.Listeners.AddRange(Trace.Listeners);

                        if (ConfigureTextWriterTracing)
                        {
                            ConfigureTextTracing();
                        }
                    }
                }
            }
        }

        private static void ConfigureTextTracing()
        {
            Trace.Listeners.Clear();
            string textLogBaseDir = GetTextLogsBaseDir();
            Directory.CreateDirectory(textLogBaseDir);
            string textLogFilePath = textLogBaseDir + "\\" + Process.GetCurrentProcess().ProcessName + ".log";
            Trace.Listeners.Add(new TextWriterTraceListener(textLogFilePath));
        }

        private static string GetTextLogsBaseDir() {    
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "1","logs");
        }
        

        private static void StartAggregateDumper()
        {
            Task.Run(async () => {
                while (true)
                {
                    await Task.Delay(TimeSpan.FromSeconds(60));
                    DumpAggregateLog();
                }
            });
        }

        public static void AddListener(TraceListener listener)
        {
            if (listener != null)
            {
                EnsureInitialized();
                _source.Listeners.Add(listener);
            }
        }

        public static void RemoveListener(TraceListener listener)
        {
            if (listener != null)
            {
                EnsureInitialized();
                _source.Listeners.Remove(listener);
            }
        }

        static string _processName = null;
        private static string GetTraceSourceName()
        {
            _processName = (_processName ?? Process.GetCurrentProcess().ProcessName);
            return _processName;
        }

        private static int GetDefaultEventId()
        {
            //fallback
            return 0;
        }
    }
}
