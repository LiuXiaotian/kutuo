using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarOfLords.Common
{
    public class TheLordTime
    {
        static DateTime stardardBaseTime = (new DateTime(2016, 1, 26)).ToUniversalTime();
        static DateTime localBaseTime = new DateTime(2011, 10, 25);
        static double localToStartdard = 5;

        public static DateTime Now
        {
            get
            {
                TimeSpan deltaTime = DateTime.UtcNow - stardardBaseTime;
                long deltaMilliseconds = (long)(deltaTime.TotalMilliseconds * localToStartdard);
                DateTime localNow = localBaseTime.AddMilliseconds(deltaMilliseconds);
                return localNow;
            }
        }

        public static DateTime EarthTimeNow
        {
            get
            {
               return DateTime.UtcNow;
            }
        }

        public static DateTime ToEarthTime(DateTime thelordTime)
        {
            TimeSpan deltaTime = thelordTime - localBaseTime;
            long deltaMilliseconds = (long)(deltaTime.TotalMilliseconds / localToStartdard);
            DateTime earthTime = stardardBaseTime.AddMilliseconds(deltaMilliseconds);
            return earthTime;
        }

        public static bool IsPast(DateTime time)
        {
            return (Now > time);
        }

        public static int DaysToDayZero
        {
            get
            {
                return (Now - localBaseTime).Days;
            }
        }

        public static TimeSpan FromMilliseconds(int actualMs)
        {
            return TimeSpan.FromMilliseconds(actualMs / localToStartdard);
        }
    }
}
