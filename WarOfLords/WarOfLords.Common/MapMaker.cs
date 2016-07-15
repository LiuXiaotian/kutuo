using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarOfLords.Common.Models;

namespace WarOfLords.Common
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

        public static Map MakeRectMap(MapVertex startPos, int width, int height)
        {
            Map map = new Map(NewId(), "GridMap");
            MapRegion cellRegion = new MapRegion(NewId(), "RectRegion");
            cellRegion.RegionType = MapRegionType.Plain;
            var leftUp = new MapVertex
            {
                X = startPos.X,
                Y = startPos.Y,
                Z = startPos.Z
            };
            var rightUp = new MapVertex
            {
                X = startPos.X + width - 1,
                Y = startPos.Y,
                Z = startPos.Z
            };
            var rightBottom = new MapVertex
            {
                X = startPos.X + width - 1,
                Y = startPos.Y + height - 1,
                Z = startPos.Z
            };
            var leftBottom = new MapVertex
            {
                X = startPos.X,
                Y = startPos.Y + height -1,
                Z = startPos.Z
            };
            cellRegion.Vertexs.Add(leftUp);
            cellRegion.Vertexs.Add(rightUp);
            cellRegion.Vertexs.Add(rightBottom);
            cellRegion.Vertexs.Add(leftBottom);
            map.RegionDicts.Add( cellRegion.Id, cellRegion);
            return map;
        }
    }
}
