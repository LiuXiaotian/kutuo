using System;
using System.Collections.Generic;
using System.Text;
using WarOfLords.Client;

namespace WarOfLords.Common
{
    public class TileNavigationResult : IComparable<TileNavigationResult>
    {
        public bool IsReachable;
        public MapTileIndex FromTile;
        public MapTileIndex ToTile;
        public List<MapTileIndex> RoutingTiles = new List<MapTileIndex>();

        public int computeDisSq()
        {
            return (FromTile.X - ToTile.X) * (FromTile.X - ToTile.X) + (FromTile.Y - ToTile.Y) * (FromTile.Y - ToTile.Y);
        }

        public void Optimize()
        {
            List<MapTileIndex> removingTiles = new List<MapTileIndex>();
            var tileEnum = RoutingTiles.GetEnumerator();
            MapTileIndex last = null;
            MapTileIndex current = null;
            bool hasMore = tileEnum.MoveNext();
            while (hasMore)
            {
                if (last == null)
                {
                    last = tileEnum.Current;
                    hasMore = tileEnum.MoveNext();
                }
                if (hasMore)
                {
                    current = tileEnum.Current;
                    if (last.HashValue == current.HashValue)
                    {
                        removingTiles.Add(last);
                    }
                    last = current;
                    hasMore = tileEnum.MoveNext();
                }
                else
                {
                    break;
                }
            }

            foreach (var removeTile in removingTiles)
            {
                this.RoutingTiles.Remove(removeTile);
            }
        }

        public List<long> GetAllPassingHashs()
        {
            List<long> hashs = new List<long>();
            foreach (var tile in RoutingTiles)
            {
                hashs.Add(tile.HashValue);
            }
            return hashs;
        }

        public int CompareTo(TileNavigationResult other)
        {
            return this.computeDisSq() - other.computeDisSq();
        }
    }
}
