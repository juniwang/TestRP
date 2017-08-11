using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestRP.Common
{
    public static class AzureLog
    {
        private static ILog log = LogManager.GetLogger("AzureLog.Logging");

        public static void Debug(string message)
        {
            log.Debug(message);
        }
        public static void Error(string message)
        {
            log.Error(message);
        }

        public static void Error(string messageFormat, params string[] args)
        {
            log.Error(string.Format(messageFormat, args));
        }

        public static void Error(string message, Exception exception)
        {
            log.Error(message, exception);
        }
        public static void Fatal(string message)
        {
            log.Fatal(message);
        }
        public static void Fatal(string message, Exception exception)
        {
            log.Fatal(message, exception);
        }
        public static void Info(string message)
        {
            log.Info(message);
        }

        public static void Info(string messageFormat, params string[] args)
        {
            log.Info(string.Format(messageFormat, args));
        }

        public static void Warn(string message)
        {
            log.Warn(message);
        }
    }
}
