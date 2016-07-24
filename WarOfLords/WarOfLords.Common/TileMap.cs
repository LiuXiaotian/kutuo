
using System;
using System.Linq;
using System.Collections.Generic;
using WarOfLords.Common.Models;

namespace WarOfLords.Common
{ 
    public class TileMap : Map
    {
        public int Row = 26;
        public int Column = 20;
        public int TileWidth = 40;
        public int TileHeight = 40;
       
        

        public TileMap(int row, int column, int tileWidth, int tileHeight)
        {
            this.Row = row;
            this.Column = column;
            this.TileWidth = tileWidth;
            this.TileHeight = tileHeight;
        }

        public Dictionary<long, MapTileIndex> ReachableTiles = new Dictionary<long, MapTileIndex>();

        public Dictionary<long, MapTileIndex> HubTiles = new Dictionary<long, MapTileIndex>();

        public Dictionary<long, MapTileIndex> Camp1Tiles = new Dictionary<long, MapTileIndex>();

        public Dictionary<long, MapTileIndex> Camp2Tiles = new Dictionary<long, MapTileIndex>();

        Dictionary<TileNavigationKey, TileNavigationResult> hubNavCache = new Dictionary<TileNavigationKey, TileNavigationResult>();

        class TileNavigationKey : IEquatable<TileNavigationKey>
        {
            public MapTileIndex FromTile;

            public MapTileIndex ToTile;

            public bool Equals(TileNavigationKey other)
            {
                return FromTile.HashValue == other.FromTile.HashValue && ToTile.HashValue == other.ToTile.HashValue;
            }

            public override int GetHashCode()
            {
                return FromTile.X;
            }
        }

        public void AddReachableTilesInRow(int fromColumn, int toColumn, int row)
        {
            for (int column = fromColumn; column <= toColumn; column++)
            {
                var tile = new MapTileIndex(column, row);
                if (!this.ReachableTiles.ContainsKey(tile.HashValue))
                {
                    AddReachableTile(tile);
                }
            }
        }

        public void AddReachableTilesInColumn(int fromRow, int toRow, int column)
        {
            for (int row = fromRow; row <= toRow; row++)
            {
                var tile = new MapTileIndex(column, row);
                AddReachableTile(tile);
            }
        }

        public void AddReachableTile(MapTileIndex tile)
        {
            if (!this.ReachableTiles.ContainsKey(tile.HashValue))
            {
                this.ReachableTiles.Add(tile.HashValue, tile);
            }
        }

        public void RemoveReachableTile(MapTileIndex tile)
        {
            if (this.ReachableTiles.ContainsKey(tile.HashValue))
            {
                this.ReachableTiles.Remove(tile.HashValue);
            }
        }

        public bool IsReachable(MapTileIndex tile)
        {
            return this.ReachableTiles.ContainsKey(tile.HashValue);
        }

        public MapTileIndex GetClosestReachableTile(float x, float y)
        {
            var tile = this.GetTileIndex(x, y);
            if(this.ReachableTiles.ContainsKey(tile.HashValue))return tile;
            MapTileIndex closestTile = null;
            int currentDistance = int.MaxValue;
            foreach (var hubTile in this.ReachableTiles.Values)
            {
                int distanceSq = (tile.X - hubTile.X) * (tile.X - hubTile.X) + (tile.Y - hubTile.Y) * (tile.Y - hubTile.Y);
                if (distanceSq < currentDistance)
                {
                    currentDistance = distanceSq;
                    closestTile = hubTile;
                }
            }
            return closestTile;
        }

        internal void AddCamp1Tile(MapTileIndex mapTileIndex)
        {

            if(!this.Camp1Tiles.ContainsKey(mapTileIndex.HashValue))
            {
                this.Camp1Tiles.Add(mapTileIndex.HashValue, mapTileIndex);
            }
        }

        public TileNavigationResult AreReachableInRow(int fromColumn, int toColumn, int row)
        {
            //int minColumn = Math.Min(fromColumn, toColumn);
            //int maxColumn = Math.Max(fromColumn, toColumn);
            TileNavigationResult navResult = new TileNavigationResult
            {
                IsReachable = false,
                FromTile = new MapTileIndex(fromColumn, row),
                ToTile = new MapTileIndex(toColumn, row),
                RoutingTiles = new List<MapTileIndex>()
            };

            if (fromColumn <= toColumn)
            {
                for (int column = fromColumn; column <= toColumn; column++)
                {
                    var tile = new MapTileIndex(column, row);
                    if (!IsReachable(tile))
                    {
                        return navResult;
                    }
                    navResult.RoutingTiles.Add(tile);
                }
            }
            else
            {
                for (int column = fromColumn; column >= toColumn; column--)
                {
                    var tile = new MapTileIndex(column, row);
                    if (!IsReachable(tile))
                    {
                        return navResult;
                    }
                    navResult.RoutingTiles.Add(tile);
                }
            }
            navResult.IsReachable = true;
            return navResult;
        }

        internal void AddCamp2Tile(MapTileIndex mapTileIndex)
        {
            this.Camp2Tiles.Add(mapTileIndex.HashValue, mapTileIndex);
        }

        public TileNavigationResult AreReachableInColumn(int fromRow, int toRow, int column)
        {
            //int minRow = Math.Min(fromRow, toRow);
            //int maxRow = Math.Max(fromRow, toRow);
            TileNavigationResult navResult = new TileNavigationResult
            {
                IsReachable = false,
                FromTile = new MapTileIndex(column, fromRow),
                ToTile = new MapTileIndex(column, toRow),
                RoutingTiles = new List<MapTileIndex>()
            };

            if (fromRow <= toRow)
            {
                for (int row = fromRow; row <= toRow; row++)
                {
                    var tile = new MapTileIndex(column, row);
                    if (!IsReachable(tile))
                    {
                        return navResult;
                    }
                    navResult.RoutingTiles.Add(tile);
                }
            }
            else
            {
                for (int row = fromRow; row >= toRow; row--)
                {
                    var tile = new MapTileIndex(column, row);
                    if (!IsReachable(tile))
                    {
                        return navResult;
                    }
                    navResult.RoutingTiles.Add(tile);
                }
            }
            navResult.IsReachable = true;
            return navResult;
        }

        public void AddHubTile(MapTileIndex tile)
        {
            if (!this.HubTiles.ContainsKey(tile.HashValue))
            {
                this.HubTiles.Add(tile.HashValue, tile);
            }
        }

        public bool IsHubTile(MapTileIndex tile)
        {
            return this.HubTiles.ContainsKey(tile.HashValue);
        }

        public TileNavigationResult GetStraightWayToHubTile(MapTileIndex tile)
        {
            int currentDistance = int.MaxValue;
            TileNavigationResult navResult = null;

            //SortedDictionary<int, TileNavigationResult> possiableWays = new SortedDictionary<int, TileNavigationResult>();
            foreach (var hubTile in this.HubTiles.Values)
            {
                if (hubTile.X == tile.X && hubTile.Y == tile.Y)
                {
                    continue;
                }
                if (hubTile.X == tile.X || hubTile.Y == tile.Y)
                {
                    int distanceSq = (tile.X - hubTile.X) * (tile.X - hubTile.X) + (tile.Y - hubTile.Y) * (tile.Y - hubTile.Y);
                    if (hubTile.X == tile.X)
                    {
                        var way = AreReachableInColumn(tile.Y, hubTile.Y, tile.X);
                        if (way != null && way.IsReachable)
                        {
                            if (distanceSq < currentDistance)
                            {
                                currentDistance = distanceSq;
                                navResult = way;
                            }
                        }
                    }
                    else
                    {
                        var way = AreReachableInRow(tile.X, hubTile.X, tile.Y);
                        if (way != null && way.IsReachable)
                        {
                            if (distanceSq < currentDistance)
                            {
                                currentDistance = distanceSq;
                                navResult = way;
                            }
                        }
                    }
                }
            }

            return navResult;
        }

        public SortedSet<TileNavigationResult> GetAllPossiableWaysToHubTile(MapTileIndex tile, List<long> excludedTileHashs)
        {
            //List<TileNavigationResult> possiableWays = new List<TileNavigationResult>();
            SortedSet<TileNavigationResult> possiableWays = new SortedSet<TileNavigationResult>();
            foreach (var hubTile in this.HubTiles.Values)
            {
                if (excludedTileHashs.Contains(hubTile.HashValue))
                {
                    continue;
                }
                if (hubTile.X == tile.X && hubTile.Y == tile.Y)
                {
                    continue;
                }
                if (hubTile.X == tile.X || hubTile.Y == tile.Y)
                {
                    //int distanceSq = (tile.X - hubTile.X) * (tile.X - hubTile.X) + (tile.X - hubTile.Y) * (tile.X - hubTile.Y);
                    if (hubTile.X == tile.X)
                    {
                        var way = AreReachableInColumn(tile.Y, hubTile.Y, tile.X);
                        if (way != null && way.IsReachable)
                        {
                            possiableWays.Add(way);
                        }
                    }
                    else
                    {
                        var way = AreReachableInRow(tile.X, hubTile.X, tile.Y);
                        if (way != null && way.IsReachable)
                        {
                            possiableWays.Add(way);
                        }
                    }
                }
            }
            return possiableWays;
        }

        public TileNavigationResult TryGetStraightWay(MapTileIndex fromTile, MapTileIndex toTile)
        {
            if (fromTile.X == toTile.X || fromTile.Y == toTile.Y)
            {
                //int distanceSq = (tile.X - hubTile.X) * (tile.X - hubTile.X) + (tile.X - hubTile.Y) * (tile.X - hubTile.Y);
                if (fromTile.X == toTile.X)
                {
                    var way = AreReachableInColumn(fromTile.Y, toTile.Y, toTile.X);
                    if (way != null && way.IsReachable)
                    {
                        return way;
                    }
                }
                else
                {
                    var way = AreReachableInRow(fromTile.X, toTile.X, toTile.Y);
                    if (way != null && way.IsReachable)
                    {
                        return way;
                    }
                }
            }
            return null;
        }

        public TileNavigationResult SearchWayBetweenTwoHubTiles(MapTileIndex fromHubTile, MapTileIndex toHubTile, List<long> excludedTileHashs/*, TileNavigationResult previousWay*/)
        {
            TileNavigationKey navKey = new TileNavigationKey
            {
                FromTile = fromHubTile,
                ToTile = toHubTile,
            };

            if(this.hubNavCache.ContainsKey(navKey))
            {
                return this.hubNavCache[navKey];
            }

            if (excludedTileHashs == null)
            {
                excludedTileHashs = new List<long>();
                excludedTileHashs.Add(fromHubTile.HashValue);
            }

            var straightWay = TryGetStraightWay(fromHubTile, toHubTile);
            if (straightWay != null && straightWay.IsReachable)
            {
                this.hubNavCache[navKey] = straightWay;
                return straightWay;
            }

            var possiableWays = GetAllPossiableWaysToHubTile(fromHubTile, excludedTileHashs);

            if (possiableWays.Count == 0)
            {
                this.hubNavCache[navKey] = null;
                return null;
            }

            int distance = int.MaxValue;
            TileNavigationResult shortestWay = null;

            foreach (var way in possiableWays)
            {
                excludedTileHashs.AddRange(way.GetAllPassingHashs());
                if (way.ToTile.HashValue == toHubTile.HashValue)
                {
                    if (distance > way.RoutingTiles.Count)
                    {
                        distance = way.RoutingTiles.Count;
                        shortestWay = way;
                    }
                    //return way;
                    continue;
                }
                
                var nextWay = SearchWayBetweenTwoHubTiles(way.ToTile, toHubTile, excludedTileHashs);
                if (nextWay != null)
                {
                    way.RoutingTiles.AddRange(nextWay.RoutingTiles);
                    way.IsReachable = true;
                    way.ToTile = toHubTile;
                    ///way.Optimize();
                    if (distance > way.RoutingTiles.Count)
                    {
                        distance = way.RoutingTiles.Count;
                        shortestWay = way;
                    }
                }
            }

            this.hubNavCache[navKey] = shortestWay;
            return shortestWay;
        }

        public MapTileIndex GetClosestHubTile(MapTileIndex tile)
        {
            SortedDictionary<int, MapTileIndex> distanceDic = new SortedDictionary<int, MapTileIndex>();
            MapTileIndex closestTile = null;
            int currentDistance = int.MaxValue;
            foreach (var hubTile in this.HubTiles.Values)
            {
                int distanceSq = (tile.X - hubTile.X) * (tile.X - hubTile.X) + (tile.Y - hubTile.Y) * (tile.Y - hubTile.Y);
                if (distanceSq < currentDistance)
                {
                    currentDistance = distanceSq;
                    closestTile = hubTile;
                }
            }
            return closestTile;
        }

        public TileNavigationResult SearchWay(MapTileIndex fromTile, MapTileIndex toTile)
        {
            List<MapTileIndex> routeTiles = new List<MapTileIndex>();
            MapTileIndex fromHubTile;
            MapTileIndex toHubTile = null;
            TileNavigationResult lastHubWayToEnd = null;

            var straightWay = TryGetStraightWay(fromTile, toTile);
            if (straightWay != null && straightWay.IsReachable)
            {
                return straightWay;
            }

            TileNavigationResult navResult = new TileNavigationResult
            {
                IsReachable = false,
                FromTile = fromTile,
                ToTile = toTile,
                RoutingTiles = new List<MapTileIndex>()
            };

            if (!IsHubTile(fromTile))
            {
                var tempNavResult = GetStraightWayToHubTile(fromTile);
                if (tempNavResult == null)
                {
                    //No way
                    navResult.RoutingTiles.Add(fromTile);
                    return navResult;
                }
                else
                {
                    navResult.RoutingTiles.AddRange(tempNavResult.RoutingTiles);
                    if (!tempNavResult.IsReachable)
                    {
                        return navResult;
                    }
                    fromHubTile = tempNavResult.ToTile;
                }
            }
            else
            {
                fromHubTile = fromTile;
            }

            if (!IsHubTile(toTile))
            {
                var tempNavResult = GetStraightWayToHubTile(toTile);
                if (tempNavResult == null || !tempNavResult.IsReachable)
                {
                    toHubTile = GetClosestHubTile(toTile);
                    if (toHubTile == null) return navResult;
                }
                else
                {
                    toHubTile = tempNavResult.ToTile;
                    lastHubWayToEnd = tempNavResult;
                }
            }
            else
            {
                toHubTile = toTile;
            }

            // fromHub to toHub
            var navResultBetweenHubs = SearchWayBetweenTwoHubTiles(fromHubTile, toHubTile, null);
            if (navResultBetweenHubs != null)
            {

                navResult.RoutingTiles.AddRange(navResultBetweenHubs.RoutingTiles);
                navResult.ToTile = navResultBetweenHubs.ToTile;
                if (toHubTile.HashValue == toTile.HashValue)
                {
                    navResult.IsReachable = true;
                    navResult.Optimize();
                    return navResult;
                }
            }
            else
            {
                navResult.IsReachable = false;
                return navResult;
            }


            if (lastHubWayToEnd != null)
            {
                lastHubWayToEnd.RoutingTiles.Reverse();
                navResult.RoutingTiles.AddRange(lastHubWayToEnd.RoutingTiles);
                navResult.ToTile = toTile;
                navResult.IsReachable = true;
            }

            if (navResult != null) navResult.Optimize();

            return navResult;
        }

        public MapTileIndex GetTileIndex(float x, float y)
        {
            return GetTileIndex((int)x, (int)y);
        }

        public MapTileIndex GetTileIndex(int x, int y)
        {
            return new MapTileIndex(x / TileWidth, y / TileHeight);
        }

        public MapTileIndex GetTileIndex(MapVertex pos)
        {
            return GetTileIndex(pos.X, pos.Y);
        }

        public bool InTile(int x, int y, MapTileIndex tile)
        {
            return tile.X ==  x / TileWidth && tile.Y == y / TileHeight;
        }

        public MapVertex TileCenter(MapTileIndex tile)
        {
            int minX = tile.X * TileWidth;
            int minY = tile.Y * TileHeight;

            return new MapVertex { X = minX + TileWidth / 2, Y = minY + TileHeight / 2 , Z = 0};
        }

        public MapVertex FindReachablePointInTile(IMovementCapability entity, MapTileIndex tile)
        {
            MapVertex center = TileCenter(tile);
            if (InTile(entity.Position.X, entity.Position.Y, tile))
            {
                return center;
            }

            if (this.CheckCollisionMethod == null || !this.CheckCollisionMethod(entity, center))
            {
                return center;
            }

            Random ran = new Random();
            int tryTimes = 3;
            int xRange = TileWidth / 2 - 1;
            int yRange = TileHeight / 2 - 1;

            while (tryTimes > 0)
            {
                int xOff = ran.Next(-xRange,  xRange);
                int yOff = ran.Next(-yRange, yRange);

                MapVertex temp = new MapVertex
                {
                    X = center.X + xOff,
                    Y = center.Y + yOff,
                    Z = 0
                };

                if(!this.CheckCollisionMethod(entity, temp))
                {
                    return temp;
                }

                tryTimes--;
            }

            return center;
        }

        public MapVertex FindReachablePointInTiles(IMovementCapability entity, IEnumerable<MapTileIndex> tiles)
        {
            MapVertex center = null;
            foreach (var tile in tiles)
            {
                center = TileCenter(tile);

                if (this.CheckCollisionMethod == null || !this.CheckCollisionMethod(entity, center))
                {
                    return center;
                }

                Random ran = new Random();
                int tryTimes = 3;
                int xRange = TileWidth / 2 - 1;
                int yRange = TileHeight / 2 - 1;

                while (tryTimes > 0)
                {
                    int xOff = ran.Next(-xRange, xRange);
                    int yOff = ran.Next(-yRange, yRange);

                    MapVertex temp = new MapVertex
                    {
                        X = center.X + xOff,
                        Y = center.Y + yOff
                    };

                    if (!this.CheckCollisionMethod(entity, temp))
                    {
                        return temp;
                    }

                    tryTimes--;
                }
            }
            return center;
        }

        public static TileMap DefaultInstance()
        {
            TileMap map = new TileMap(20, 26, 40, 40);
            map.AddReachableTilesInColumn(5, 20, 2);
            map.AddReachableTilesInColumn(5, 20, 7);
            map.AddReachableTilesInColumn(5, 20, 12);
            map.AddReachableTilesInColumn(5, 20, 17);
            map.AddReachableTilesInColumn(11, 14, 1);
            map.AddReachableTilesInColumn(11, 14, 18);

            map.AddReachableTilesInRow(2, 17, 5);
            map.AddReachableTilesInRow(2, 17, 10);
            map.AddReachableTilesInRow(2, 17, 15);
            map.AddReachableTilesInRow(2, 17, 20);
            map.AddReachableTilesInRow(8, 11, 4);
            map.AddReachableTilesInRow(8, 11, 21);

            map.AddHubTile(new MapTileIndex(2, 5));
            map.AddHubTile(new MapTileIndex(7, 5));
            map.AddHubTile(new MapTileIndex(12, 5));
            map.AddHubTile(new MapTileIndex(17, 5));

            map.AddHubTile(new MapTileIndex(2, 10));
            map.AddHubTile(new MapTileIndex(7, 10));
            map.AddHubTile(new MapTileIndex(12, 10));
            map.AddHubTile(new MapTileIndex(17, 10));

            map.AddHubTile(new MapTileIndex(2, 15));
            map.AddHubTile(new MapTileIndex(7, 15));
            map.AddHubTile(new MapTileIndex(12, 15));
            map.AddHubTile(new MapTileIndex(17, 15));

            map.AddHubTile(new MapTileIndex(2, 20));
            map.AddHubTile(new MapTileIndex(7, 20));
            map.AddHubTile(new MapTileIndex(12, 20));
            map.AddHubTile(new MapTileIndex(17, 20));

            map.AddHubTile(new MapTileIndex(8, 5));
            map.AddHubTile(new MapTileIndex(9, 5));
            map.AddHubTile(new MapTileIndex(10, 5));
            map.AddHubTile(new MapTileIndex(11, 5));

            map.AddHubTile(new MapTileIndex(8, 20));
            map.AddHubTile(new MapTileIndex(9, 20));
            map.AddHubTile(new MapTileIndex(10, 20));
            map.AddHubTile(new MapTileIndex(11, 20));

            map.AddHubTile(new MapTileIndex(2, 11));
            map.AddHubTile(new MapTileIndex(2, 12));
            map.AddHubTile(new MapTileIndex(2, 13));
            map.AddHubTile(new MapTileIndex(2, 14));

            map.AddHubTile(new MapTileIndex(17, 11));
            map.AddHubTile(new MapTileIndex(17, 12));
            map.AddHubTile(new MapTileIndex(17, 13));
            map.AddHubTile(new MapTileIndex(17, 14));

            map.CheckCollisionMethod = (a, b) =>
            {
                Random ran = new Random();
                if (ran.Next(0, 100) > 50)
                {
                    return true;
                }
                return false;
            };
            return map;
        }
    }

    
}