using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarOfLords.Common.Models
{
    public class MapRegion
    {
        public virtual int Id { get; set; }

        public virtual string Name { get; set; }

        public virtual List<MapVertex> Vertexs { get; set; }

        public virtual MapRegionType RegionType { get; set; }

        public MapRegion(int id, string name)
        {
            this.Id = id;
            this.Name = name;
            this.Vertexs = new List<MapVertex>();
            RegionType = MapRegionType.Plain;
        }

        public virtual IEnumerable<MapRoute> Routes
        {
            get
            {
                List<MapRoute> routeList = new List<MapRoute>();
                for (int i = 0; i < this.Vertexs.Count; i++)
                {
                    if (i < this.Vertexs.Count - 1)
                    {
                        routeList.Add(new MapRoute
                        {
                            StartPoint = this.Vertexs[i],
                            EndPoint = this.Vertexs[i + 1]
                        });
                    }
                    else
                    {
                        routeList.Add(new MapRoute
                        {
                            StartPoint = this.Vertexs[i],
                            EndPoint = this.Vertexs[0]
                        });
                    }
                }
                return routeList.AsEnumerable();
            }
        }
    }
}
