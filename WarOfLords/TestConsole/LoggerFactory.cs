using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarOfLords.Common
{
    public class LoggerFactory
    {
        private static ILogger logger;
        private static object logLocker = new object();

        public static ILogger Logger
        {
            get
            {
                if(logger == null)
                {
                    lock(logLocker)
                    {
                        if(logger != null)
                        {
                            logger = new TraceLogger();
                        }
                    }
                }
                return logger;
            }
        }
    }
}
