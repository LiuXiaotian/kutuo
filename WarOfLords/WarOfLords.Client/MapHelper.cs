using CocosSharp;
using System;
using System.Collections.Generic;
using System.Text;
using WarOfLords.Common.Models;

namespace WarOfLords.Client
{
    class MapHelper
    {
        public static float ViewWidth = 768;
        public static float ViewHeight = 1024;

        public class DefaultMap
        {
            public static int TileSize = 200;
            public static MapVertex StartPoint = new MapVertex
            {
                X = 86,
                Y = 212
            };

            public static MapVertex IndexToPoint(MapTileIndex index)
            {
                MapVertex point = new MapVertex();
                point.X = StartPoint.X + index.X * TileSize;
                point.Y = StartPoint.Y + index.Y * TileSize;
                return point;
            }

            public static MapVertex NeareastOnTrackPoint(MapVertex orgPos)
            {
                int deltaX, deltaY;
                int x = orgPos.X;
                int y = orgPos.Y;
                deltaX = Math.Abs(orgPos.X - StartPoint.X) % TileSize;
                deltaY = Math.Abs(orgPos.Y - StartPoint.Y) % TileSize;
                if (deltaX == 0 || deltaY == 0) return new MapVertex
                {
                    X = x,
                    Y = y
                };

                if (deltaX < deltaY)
                {
                    if (deltaX < TileSize / 2)
                    {
                        x = orgPos.X - deltaX;
                    }
                    else
                    {
                        x = orgPos.X + (TileSize - deltaX);
                    }
                }
                else
                {
                    if (deltaY < TileSize / 2)
                    {
                        y = orgPos.Y - deltaY;
                    }
                    else
                    {
                        y = orgPos.Y + (TileSize - deltaY);
                    }
                }
                if (orgPos.X < StartPoint.X)
                {
                    x = StartPoint.X;
                }
                if (orgPos.Y < StartPoint.Y)
                {
                    y = StartPoint.Y;
                }
                return new MapVertex
                {
                    X = x,
                    Y = y
                };
            }

            public static MapTileIndex NeareastTileIndex(MapVertex orgPos)
            {
                int deltaX = Math.Abs(orgPos.X - StartPoint.X) % TileSize;
                int deltaY = Math.Abs(orgPos.Y - StartPoint.Y) % TileSize;


                int x = 0;
                int y = 0;
                x = Math.Abs(orgPos.X - StartPoint.X) / TileSize;
                y = Math.Abs(orgPos.Y - StartPoint.Y) / TileSize;
                if (orgPos.X < StartPoint.X) x = 0;
                if (orgPos.Y < StartPoint.Y) y = 0;
                return new MapTileIndex
                {
                    X = x,
                    Y = y
                };

            }

            public static IEnumerable<MapVertex> GetRoutePoints(MapVertex fromPos, MapVertex toPos)
            {
                List<MapVertex> routePoints = new List<MapVertex>();
                
                MapVertex currentPos = NeareastOnTrackPoint(fromPos);
                if (fromPos != currentPos)
                {
                    routePoints.Add(currentPos);
                }
                MapVertex lastPos = NeareastOnTrackPoint(toPos);
                int xDis = currentPos.X - lastPos.X;
                int yDis = currentPos.Y - lastPos.Y;

                while (xDis != 0 || yDis != 0)
                {
                    int deltaX = Math.Abs(currentPos.X - StartPoint.X) % TileSize;
                    int deltaY = Math.Abs(currentPos.Y - StartPoint.Y) % TileSize;
                    int nextX, nextY;

                    if (xDis < 0)
                    {
                        if (deltaY == 0)
                        {
                            nextX = Math.Min(lastPos.X, currentPos.X + TileSize - deltaX);
                            nextY = currentPos.Y;
                        }
                        else
                        {
                            nextX = currentPos.X;
                            if (yDis < 0)
                            {
                                
                                nextY = currentPos.Y + TileSize - deltaY;
                            }
                            else if (yDis > 0)
                            {
                                nextY = currentPos.Y - deltaY;
                            }
                            else
                            {
                                if(deltaY < TileSize/2) nextY = currentPos.Y - deltaY;
                                else nextY = currentPos.Y + TileSize - deltaY;
                            }
                        }
                    }
                    else if(xDis > 0)
                    {
                        if (deltaY == 0)
                        {
                            nextX = Math.Max(lastPos.X, currentPos.X - deltaX);
                            nextY = currentPos.Y;
                        }
                        else
                        {
                            nextX = currentPos.X;
                            if (yDis < 0)
                            {

                                nextY = currentPos.Y + TileSize - deltaY;
                            }
                            else if (yDis > 0)
                            {
                                nextY = currentPos.Y - deltaY;
                            }
                            else
                            {
                                if (deltaY < TileSize / 2) nextY = currentPos.Y - deltaY;
                                else nextY = currentPos.Y + TileSize - deltaY;
                            }
                        }
                    }
                    else
                    {
                        nextX = currentPos.X;
                        nextY = currentPos.Y;
                        if (yDis < 0)
                        {

                            nextY = Math.Min(lastPos.Y, currentPos.Y + TileSize - deltaY);
                        }
                        else if (yDis > 0)
                        {
                            nextY = Math.Max(currentPos.Y - deltaY , lastPos.Y);
                        }
                    }
                    routePoints.Add(new MapVertex
                    {
                        X = nextX,
                        Y = nextY
                    });
                }

                if(toPos != lastPos)
                {
                    routePoints.Add(lastPos);
                }
                routePoints.Add(toPos);

                return routePoints;
            }

        }
    }

    public class MapTileIndex
    {
        public int X;
        public int Y;
    }
}
