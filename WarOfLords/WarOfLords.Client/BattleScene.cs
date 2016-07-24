using CocosSharp;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using WarOfLords.Common;
using WarOfLords.Common.Models;

namespace WarOfLords.Client
{
    class BattleScene : CCScene
    {
        string mapFile;
        BattleInfo BattleInfo;
        CCTileMap tileMap;
        CCTileMapLayer backgroundLayer;
        int tileWidthDimension;
        int tileHeightDimension;
        // Find out how many rows and columns are in our tile map
        int numberOfColumns;
        int numberOfRows;
        TileMap battleMap; 

        public BattleScene(CCWindow window, string tmxFile, BattleInfo battleInfo) : base(window)
        {
            mapFile = tmxFile;
            BattleInfo = battleInfo;
            this.Init();
        }
        public BattleScene(CCScene scene, string tmxFile, BattleInfo battleInfo) : base(scene)
        {
            mapFile = tmxFile;
            BattleInfo = battleInfo;
            this.Init();
        }
        public BattleScene(CCWindow window, CCDirector director, string tmxFile, BattleInfo battleInfo) : base(window, director)
        {
            mapFile = tmxFile;
            BattleInfo = battleInfo;
            this.Init();
        }
        public BattleScene(CCWindow window, CCViewport viewport, string tmxFile, BattleInfo battleInfo, CCDirector director = null) : base(window, viewport, director)
        {
            mapFile = tmxFile;
            BattleInfo = battleInfo;
            this.Init();
        }

        private void Init()
        {
            addMapLayers();
            createBattleMap();
            addBattleLayer();
        }

        private void addMapLayers()
        {
            tileMap = new CCTileMap(mapFile);
            backgroundLayer = tileMap.LayerNamed("background");
            tileWidthDimension = (int)tileMap.TileTexelSize.Width;
            tileHeightDimension = (int)tileMap.TileTexelSize.Height;
            // Find out how many rows and columns are in our tile map
            numberOfColumns = (int)tileMap.MapDimensions.Size.Width;
            numberOfRows = (int)tileMap.MapDimensions.Size.Height;
            AddChild(tileMap);
        }

        private void addBattleLayer()
        {
            GameLayer battleLayer = new GameLayer(BattleInfo, battleMap);
            AddChild(battleLayer);
        }

        private void createBattleMap()
        {
            battleMap = new TileMap(numberOfRows, numberOfColumns, tileWidthDimension, tileHeightDimension);
            TileMapPropertyFinder finder = new TileMapPropertyFinder(tileMap);
            var allProps = finder.GetPropertyLocations().Where(_=>_.Layer.Name == backgroundLayer.Name);
            foreach(var tileProp in allProps)
            {
                if(tileProp.Properties.ContainsKey("Reachable") && tileProp.Properties["Reachable"].Equals( "true", StringComparison.CurrentCultureIgnoreCase))
                {
                    battleMap.AddReachableTile(new MapTileIndex(tileProp.TileCoordinates.Column, tileProp.TileCoordinates.Row));
                    if (tileProp.Properties.ContainsKey("Type"))
                    {
                        if(tileProp.Properties["Type"].Equals("Hub", StringComparison.CurrentCultureIgnoreCase))
                        {
                            battleMap.AddHubTile(new MapTileIndex(tileProp.TileCoordinates.Column, tileProp.TileCoordinates.Row));
                        }
                        else if (tileProp.Properties["Type"].Equals("Camp1", StringComparison.CurrentCultureIgnoreCase))
                        {
                            battleMap.AddCamp1Tile(new MapTileIndex(tileProp.TileCoordinates.Column, tileProp.TileCoordinates.Row));
                        }
                        else if (tileProp.Properties["Type"].Equals("Camp2", StringComparison.CurrentCultureIgnoreCase))
                        {
                            battleMap.AddCamp2Tile(new MapTileIndex(tileProp.TileCoordinates.Column, tileProp.TileCoordinates.Row));
                        }
                    }
                }   
            }
            battleMap.CheckCollisionMethod = CheckCollision;
        }

        private bool CheckCollision(IMovementCapability entity, MapPoint point)
        {
                Random ran = new Random();
                if (ran.Next(0, 100) > 50)
                {
                    return true;
                }
                return false;
        }
    }
}
