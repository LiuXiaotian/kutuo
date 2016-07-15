using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarOfLords.Common.Models;

namespace WarOfLords.Common
{
    class DetectHelper
    {
        public static bool IsInRange(MapVertex pnt, IEnumerable<MapCircle> circles)
        {
            if (circles == null || circles.Count() <= 0)
            {
                return false;
            }

           return circles.Any(_ => Util.IsInRegion(pnt, _));
        }

        public static bool IsInRange(MapVertex pnt, IEnumerable<BattleUnit> bUnits)
        {
            if (bUnits == null || bUnits.Count() <= 0)
            {
                return false;
            }
            var circles = bUnits.Select(_ => new MapCircle(_.Position, _.SightRange));
            return circles.Any(_ => Util.IsInRegion(pnt, _));
        }
    }
}
