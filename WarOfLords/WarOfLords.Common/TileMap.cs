
using System.Collections.Generic;

namespace WarOfLords.Common
{ 
    public class TileMap
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
                    int distanceSq = (tile.X - hubTile.X) * (tile.X - hubTile.X) + (tile.X - hubTile.Y) * (tile.X - hubTile.Y);
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
            if (excludedTileHashs == null)
            {
                excludedTileHashs = new List<long>();
                excludedTileHashs.Add(fromHubTile.HashValue);
            }

            var straightWay = TryGetStraightWay(fromHubTile, toHubTile);
            if (straightWay != null && straightWay.IsReachable)
            {
                return straightWay;
            }

            var possiableWays = GetAllPossiableWaysToHubTile(fromHubTile, excludedTileHashs);

            if (possiableWays.Count == 0) return null;

            foreach (var way in possiableWays)
            {
                if (way.ToTile.HashValue == toHubTile.HashValue)
                {
                    return way;
                }
                excludedTileHashs.AddRange(way.GetAllPassingHashs());
            }

            int distance = int.MaxValue;
            TileNavigationResult shortestWay = null;

            foreach (var way in possiableWays)
            {
                var nextWay = SearchWayBetweenTwoHubTiles(way.ToTile, toHubTile, excludedTileHashs);
                if (nextWay != null)
                {
                    way.RoutingTiles.AddRange(nextWay.RoutingTiles);
                    way.IsReachable = true;
                    way.ToTile = toHubTile;
                    way.Optimize();
                    if (distance > way.RoutingTiles.Count)
                    {
                        distance = way.RoutingTiles.Count;
                        shortestWay = way;
                    }
                }
            }

            return shortestWay;
        }

        public MapTileIndex GetClosestHubTile(MapTileIndex tile)
        {
            SortedDictionary<int, MapTileIndex> distanceDic = new SortedDictionary<int, MapTileIndex>();
            MapTileIndex closestTile = null;
            int currentDistance = int.MaxValue;
            foreach (var hubTile in this.HubTiles.Values)
            {
                int distanceSq = (tile.X - hubTile.X) * (tile.X - hubTile.X) + (tile.X - hubTile.Y) * (tile.X - hubTile.Y);
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
                    return navResult;
                }
            }
            if (lastHubWayToEnd != null)
            {
                lastHubWayToEnd.RoutingTiles.Reverse();
                navResult.RoutingTiles.AddRange(navResultBetweenHubs.RoutingTiles);
                navResult.ToTile = navResultBetweenHubs.ToTile;
                navResult.IsReachable = true;
                return navResult;
            }
            return navResult;
        }
    }
}