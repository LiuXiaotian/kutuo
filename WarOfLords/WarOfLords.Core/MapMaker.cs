using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarOfLords.Core.Models;

namespace WarOfLords.Core
{
    public class MapMaker
    {
        private static int lastId = 0;
        private static object idLock = new object();

        public static int NewId()
        {
            lock (idLock)
            {
                return lastId++;
            }
        }

        public static Map MakeGridMap(MapVertex originPos, int columnCount, int rowCount, int cellWidth)
        {
            Map map = new Map(NewId(), "GridMap");
            for (int i = 0; i < columnCount; i++)
            {
                for (int j = 0; j < rowCount; j++)
                {
                    MapRegion cellRegion = new MapRegion(NewId(), string.Format("Region({0},{1})", i, j));
                    cellRegion.RegionType = MapRegionType.Plain;
                    var leftUp = new MapVertex
                    {
                        X = originPos.X + (i) * cellWidth,
                        Y = originPos.Y + (j) * cellWidth,
                        Z = originPos.Z
                    };
                    var rightUp = new MapVertex
                    {
                        X = originPos.X + (i + 1) * cellWidth,
                        Y = originPos.Y + (j) * cellWidth,
                        Z = originPos.Z
                    };
                    var rightBottom = new MapVertex
                    {
                        X = originPos.X + (i + 1) * cellWidth,
                        Y = originPos.Y + (j + 1) * cellWidth,
                        Z = originPos.Z
                    };
                    var leftBottom = new MapVertex
                    {
                        X = originPos.X + (i) * cellWidth,
                        Y = originPos.Y + (j + 1) * cellWidth,
                        Z = originPos.Z
                    };

                    cellRegion.Vertexs.Add(leftUp);
                    cellRegion.Vertexs.Add(rightUp);
                    cellRegion.Vertexs.Add(rightBottom);
                    cellRegion.Vertexs.Add(leftBottom);
                    map.RegionDicts.Add(cellRegion.Id, cellRegion);
                }
            }
            return map;
        } 
    }
}
