using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Diagnostics.Tracing;

namespace TestRP
{
    [EventSource(
        Guid = ProviderConstants.SpringBoardProviderGuid,
        Name = ProviderConstants.SpringBoardProviderName)]
    public sealed class SpringBoardEventSource : EventSource
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
        public static readonly SpringBoardEventSource Instance = new SpringBoardEventSource();

        private SpringBoardEventSource()
        {

        }

        [Event(0)]
        public void LogDefault(int Level, string Message)
        {
            this.WriteEventInternal(0, Level, Message);
        }

        [Event(1)]
        public void LogConsoleOutFromPayLoad(int Level, string Message)
        {
            this.WriteEventInternal(1, Level, Message);
        }

        [Event(2)]
        public void LogAssemblyVersionLog(int Level, string Message)
        {
            this.WriteEventInternal(2, Level, Message);
        }

        [NonEvent]
        void WriteEventInternal(int eventId, int level, string message)
        {
            WriteEvent(eventId, level, message);
        }
    }
}
