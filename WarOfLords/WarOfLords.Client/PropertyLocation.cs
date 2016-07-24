using System;
using System.Collections.Generic;
using CocosSharp;

namespace WarOfLords.Client
{
	public struct PropertyLocation
	{
		public CCTileMapLayer Layer;
		public CCTileMapCoordinates TileCoordinates;

		public float WorldX;
		public float WorldY;
		public Dictionary<string, string> Properties;
	}
}

