using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarOfLords.Common
{
    public class TraceLogger : ILogger
    {
        public void Log(LogEntryType type, string format, params object[] args)
        {
            if(type == LogEntryType.Error)
            {
                Trace.TraceError(format, args);
            }
            else if(type == LogEntryType.Warning)
            {
                Trace.TraceWarning(format, args);
            }
            else
            {
                Trace.TraceInformation(format, args);
            }

        }
    }
}
