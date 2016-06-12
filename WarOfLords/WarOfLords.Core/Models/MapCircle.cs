using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarOfLords.Core.Models
{
    public class MapCircle
    {
        public MapVertex CenterVertex { get; set; }
        public int Radius { get; set; }

        public MapCircle(MapVertex center, int r)
        {
            this.CenterVertex = center;
            this.Radius = r;
        }
    }
}
