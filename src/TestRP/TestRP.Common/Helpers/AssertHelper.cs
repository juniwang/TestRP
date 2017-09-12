using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestRP.Common.Helpers
{
    public class AssertHelper
    {
        public static void True(bool expr, string msg)
        {
            if (!expr)
            {
                AzureLog.Fatal("** FATAL ERROR ** Calling Environment.FailFast(\"{0}\").  Callstack: {1}", msg, Environment.StackTrace);

                if (IsUnitTestProcess())
                {
                    throw new InvalidOperationException("AssertHelper failed: " + msg);
                }
                else
                {
                    Environment.FailFast(msg);
                }
            }
        }

        public static void False(bool expr, string msg)
        {
            True(!expr, msg);
        }

        public static void IsNull(object o, string msg)
        {
            True(o == null, msg);
        }

        public static void IsNotNull(object o, string msg)
        {
            True(o != null, msg);
        }

        public static void Never(string msg)
        {
            True(false, msg);
        }

        public static void InUnitTests(string msg = "")
        {
            if (string.IsNullOrEmpty(msg))
            {
                msg = "This method should only be called from unit tests.  Current Process: " + Process.GetCurrentProcess().ProcessName;
            }

            True(IsUnitTestProcess(), msg);
        }

        public static string[] KnownProcessNames = {
            "vstest",
            "te.processhost.managed",
            "msbuild",
            "simplifieddeployment",
            "xunit.console",
            "testrunner",
        };

        static string cachedProcessName;
        public static bool IsUnitTestProcess()
        {
            cachedProcessName = (cachedProcessName ?? Process.GetCurrentProcess().ProcessName.ToLowerInvariant());
            return KnownProcessNames.Any(n => cachedProcessName.Contains(n));
        }
    }
}
