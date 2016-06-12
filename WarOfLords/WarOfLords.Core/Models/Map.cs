using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarOfLords.Core.Models
{
    public class Map
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Dictionary<int, MapRegion> RegionDicts { get; set; }
        public ConcurrentDictionary<MapPoint, int> PointUnitCountDic { get; set; }

        public MapSetting Setting { get; set; }

        public Map(int id, string name)
        {
            this.Id = id;
            this.Name = name;
            this.RegionDicts = new Dictionary<int, MapRegion>();
            this.PointUnitCountDic = new ConcurrentDictionary<MapPoint, int>();
            this.Setting = new MapSetting();
        }

        public IEnumerable<MapRoute> AllRoutes
        {
            get
            {
                List<MapRoute> all = new List<MapRoute>();
                foreach (var region in this.RegionDicts.Values)
                {
                    all.AddRange(region.Routes);
                }
                return all.AsEnumerable();
            }
        }

        public MapRegion QueryRegion(MapPoint p)
        {
            foreach(var region in this.RegionDicts.Values)
            {
                if (Util.IsInRegion(p, region)) return region;
            }
            return null;
        }

        public IEnumerable< MapVertex> StrongPoints
        {
            get
            {
                List<MapVertex> allPoints = new List<MapVertex>();
                this.RegionDicts.Values.ToList().ForEach(reg => allPoints.AddRange(reg.Vertexs));
                return allPoints.Distinct();
            }
        }

        public void EnterPoint(MapPoint point)
        {
            this.PointUnitCountDic.AddOrUpdate(point, 1, (key, oldValue)=>
            {
                return oldValue + 1;
            });
        }

        public void LeavePoint(MapPoint point)
        {
            this.PointUnitCountDic.AddOrUpdate(point, 0, (key, oldValue) =>
            {
                return oldValue - 1 > 0 ? oldValue - 1 : 0;
            });
        }

        public bool PointReachable(MapPoint point)
        {
            int count = 0;
            if (this.PointUnitCountDic.TryGetValue(point, out count))
            {
                return count < this.Setting.UnitCapacityPerPoint;
            }
            else
            {
                return true;
            }
        }
    }
}
