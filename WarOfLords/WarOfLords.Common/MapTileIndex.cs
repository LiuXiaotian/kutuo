
namespace WarOfLords.Common
{
    public class MapTileIndex
    {
        public int X;
        public int Y;

        public MapTileIndex(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        public long HashValue
        {
            get
            {
                return ((long)X) << 32 | Y;
            }
        }
    }
}