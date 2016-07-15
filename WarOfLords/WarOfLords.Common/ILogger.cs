using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarOfLords.Common
{
    public interface ILogger
    {
        void Log(LogEntryType type, string format, params object[] args);
    }

    public enum LogEntryType
    {
        Info,
        Warning,
        Error,
    }
}
