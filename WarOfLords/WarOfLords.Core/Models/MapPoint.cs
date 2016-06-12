using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarOfLords.Core.Models
{
    public class MapPoint /*: IEqualityComparer<MapPoint>*/

    {
        public int X;
        public int Y;

        private double xDecimal = 0;
        private double yDecimal = 0;

        public int DistanceTo(MapPoint pos)
        {
            return (int)Math.Sqrt((pos.X - this.X) * (pos.X - this.X) + (pos.Y - this.Y) * (pos.Y - this.Y));
        }

        public void Moved(MapPoint moveTo, int moved)
        {
            int distance = DistanceTo(moveTo);
            if (distance == 0) return;
            double ratio = (double)moved / DistanceTo(moveTo);

            double actualX = (moveTo.X - this.X) * ratio + this.xDecimal;
            double actualY = (moveTo.Y - this.Y) * ratio + this.yDecimal;

            int deltaX = (int)actualX;
            int deltaY = (int)actualY;

            this.xDecimal = actualX - deltaX;
            this.yDecimal = actualY - deltaY;

            this.X += deltaX;
            this.Y += deltaY;
            if(this.X < 0)
            {
                LoggerFactory.Logger.Log(LogEntryType.Error, "{0},{1},{2},{3}", actualX, actualY, xDecimal, yDecimal);
            }
        }

        public void Moved(MapPoint moveTo, int moved, Map map)
        {
            if (moved <= 0) return;
            int distance = DistanceTo(moveTo);
            if (distance == 0) return;
            double ratio = (double)moved / DistanceTo(moveTo);

            double actualX = (moveTo.X - this.X) * ratio + this.xDecimal;
            double actualY = (moveTo.Y - this.Y) * ratio + this.yDecimal;

            int deltaX = (int)actualX;
            int deltaY = (int)actualY;

            this.xDecimal = actualX - deltaX;
            this.yDecimal = actualY - deltaY;

            int x = this.X + deltaX;
            int y = this.Y + deltaY;

            MapPoint point = new MapPoint
            {
                X = x,
                Y = y
            };

            if(map.PointReachable(point))
            {
                map.LeavePoint(this);
                this.X = x;
                this.Y = y;
                map.EnterPoint(point);
            }
            else
            {
                this.Moved(moveTo, moved - 1, map);
            }
            if (this.X < 0)
            {
                LoggerFactory.Logger.Log(LogEntryType.Error, "{0},{1},{2},{3}", actualX, actualY, xDecimal, yDecimal);
            }
        }

        public override bool Equals(object obj)
        {
            if(obj is MapPoint)
            {
                MapPoint pos = obj as MapPoint;
                return (this.X == pos.X && this.Y == pos.Y);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return this.X ^ this.Y;
        }

        public static bool operator ==(MapPoint p1, MapPoint p2)
        {
            return p1.X == p2.X && p1.Y == p2.Y;
        }

        public static bool operator !=(MapPoint p1, MapPoint p2)
        {
            return p1.X != p2.X || p1.Y != p2.Y;
        }
    }
}

