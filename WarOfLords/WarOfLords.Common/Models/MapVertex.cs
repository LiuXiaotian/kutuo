using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarOfLords.Common.Models
{
    public class MapVertex : MapPoint
    {
        public int Z { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is MapVertex)
            {
                var vert = obj as MapVertex;
                return (this.X == vert.X && this.Y == vert.Y && this.Z == vert.Z);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() ^ this.Z;
        }

        //public static bool operator ==(MapVertex v1, MapVertex v2)
        //{
        //    return v1.X == v2.X && v1.Y == v2.Y && v1.Z == v2.Z;
        //}

        //public static bool operator !=(MapVertex v1, MapVertex v2)
        //{
        //    return v1.X != v2.X || v1.Y != v2.Y || v1.Z != v2.Z; ;
        //}

        public MapVertex Clone()
        {
            return new MapVertex
            {
                X = this.X,
                Y = this.Y,
                Z = this.Z
            };
        }

        private const string MapVertexFormatString = "({0},{1},{2})";
        public override string ToString()
        {
            return string.Format(MapVertexFormatString, this.X, this.Y, this.Z);
        }
    }
}
