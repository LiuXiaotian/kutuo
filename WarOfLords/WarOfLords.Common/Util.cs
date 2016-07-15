using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarOfLords.Common.Models;

namespace WarOfLords.Common
{
    public class Util
    {
        public static bool IsInSight(MapPoint sourcePos, int sightRange, MapPoint targetPos)
        {
            return sourcePos.DistanceTo(targetPos) <= sightRange;
        }

        public static bool IsInMiddle(MapPoint p, MapPoint p1, MapPoint p2)
        {
            return IsOnRectSegment(p1, p2, p) && IsStraight(p1, p2, p);
        }

        public static bool IsStraight(MapPoint p, MapPoint p1, MapPoint p2)
        {
            return isLeft(p, p1, p2) == 0;
        }

        public static bool IsOnRectSegment(MapPoint p, MapPoint p1, MapPoint p2)
        {
            int minX = Math.Min(p1.X, p2.X);
            int maxX = Math.Max(p1.X, p2.X);
            int minY = Math.Min(p1.Y, p2.Y);
            int maxY = Math.Max(p1.Y, p2.Y);

            return p.X >= minX && p.X <= maxX && p.Y >= minY && p.Y <= maxY;
        }

        private static int isLeft(MapPoint P0, MapPoint P1, MapPoint P2)
        {
            int abc = (int)((P1.X - P0.X) * (P2.Y - P0.Y) - (P2.X - P0.X) * (P1.Y - P0.Y));
            return abc;

        }
        public static bool IsInRegion(MapPoint pnt, List<MapPoint> region)
        {
            int wn = 0, j = 0; 
            for (int i = 0; i < region.Count; i++)
            {
                if (i == region.Count - 1)
                {
                    j = 0;
                }
                else
                {
                    j = j + 1;
                }

                if (region[i].Y <= pnt.Y) 
                {
                    if (region[j].Y > pnt.Y) 
                    {
                        if (isLeft(region[i], region[j], pnt) > 0)
                        {
                            wn++;
                        }
                    }
                }
                else
                {
                    if (region[j].Y <= pnt.Y)
                    {
                        if (isLeft(region[i], region[j], pnt) < 0)
                        {
                            wn--;
                        }
                    }
                }
            }
            if (wn == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public static bool IsInRegion(MapPoint pnt, MapRegion region)
        {
            var pList = region.Vertexs.Select(p => (MapPoint)p);
            return IsInRegion(pnt, pList.ToList());
        }

        public static bool IsInRegion(MapPoint pnt, MapVertex center, int radius)
        {
            return pnt.DistanceTo(center) <= radius;
        }

        public static bool IsInRegion(MapPoint pnt, MapCircle circle)
        {
            return IsInRegion(pnt, circle.CenterVertex, circle.Radius);
        }
    }
}
