using System;
using System.Collections.Generic;
using CocosSharp;
using Microsoft.Xna.Framework;
using WarOfLords.Common.Models;
using WarOfLords.Common;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;

namespace WarOfLords.Client
{
    public class GameLayer : CCLayerColor, ICCUpdatable
    {

        // Define a label variable
        CCLabel team1Label;
        CCLabel team2Label;
        SwordMan swordMan;
        BattleTeam battleTeam1;
        BattleTeam battleTeam2;
        BattleManager battleManager;
        CancellationTokenSource cancelSourceForMessageLoopUp = new CancellationTokenSource();
        BattleInfo battleInfo;
        CCTileMap tileMap;

        public GameLayer(BattleInfo info) : base(CCColor4B.White)
        {
            battleInfo = info;
            var touchListener = new CCEventListenerTouchAllAtOnce(); 
            touchListener.OnTouchesEnded += this.OnTouchesEnded;
            AddEventListener(touchListener, this);
            
            this.Opacity = 0;
            //CCSprite backgroupSprite = new CCSprite("background");
            //backgroupSprite.ZOrder = 0;
            //backgroupSprite.AnchorPoint = new CCPoint(0, 0);
            //backgroupSprite.Position = new CCPoint(0, 0);
            //AddChild(backgroupSprite);
        }

        protected override void AddedToScene()
        {
            base.AddedToScene();

           

            //CCTileMapLayer tileLayer = tileMap.LayerNamed("Tile Layer 1");
            //CCTileMapCoordinates coor = new CCTileMapCoordinates(1, 1);
            //CCSprite tileSprite = tileLayer.ExtractTile(coor);

            // Use the bounds to layout the positioning of our drawable assets
            var bounds = VisibleBoundsWorldspace;

            // position the label on the center of the screen
            //label.Position = bounds.Center;

            // Register for touch events
            var touchListener = new CCEventListenerTouchAllAtOnce();
            touchListener.OnTouchesEnded = OnTouchesEnded;
            AddEventListener(touchListener, this);

            CCSize size = VisibleBoundsWorldspace.Size;

            team1Label = new CCLabel("Team1Label", "Arial", 18, CCLabelFormat.SystemFont);
            team1Label.Color = new CCColor3B(CCColor4B.Red);
            team1Label.PositionX = 0;
            team1Label.PositionY = 0;
            team1Label.AnchorPoint = new CCPoint(0, 0);
            AddChild(team1Label);

            team2Label = new CCLabel("Team2Label", "Arial", 18, CCLabelFormat.SystemFont);
            team2Label.Color = new CCColor3B(CCColor4B.Red);
            team2Label.Position = this.VisibleBoundsWorldspace.LeftTop();
            //team2Label.PositionY = this.VisibleBoundsWorldspace.top;
            team2Label.AnchorPoint = new CCPoint(0, 1);
            AddChild(team2Label);

            //CCCallFuncN moveBananaComplete = new CCCallFuncN(node =>
            //{
            //    CCScene scene = new CCScene(GameView);
            //    scene.AddLayer(new GameOverLayer(500));
            //    GameView.RunWithScene(scene);
            //});

            initBattleTeams();

        }

        async void OnTouchesEnded(List<CCTouch> touches, CCEvent touchEvent)
        {
            if (touches.Count > 0)
            {
                //await swordMan.MoveTo(new MapVertex { X = (int)touches[0].Location.X, Y = (int)touches[0].Location.Y });
                //MapVertex attackPos = new MapVertex
                //{
                //    X = (int)touches[0].Location.X,
                //    Y = (int)touches[0].Location.Y,
                //    Z = 0
                //};
                //var onTrackPos = MapHelper.DefaultMap.NeareastOnTrackPoint(attackPos);
                var targeTile = battleTeam1.BattleFieldMap.GetClosestReachableTile(touches[0].Location.X, touches[0].Location.Y);
                var targetPos = battleTeam1.BattleFieldMap.TileCenter(targeTile);
                var task1 = battleTeam1.Attack(targetPos);
                var task2 = battleTeam2.Attack(targetPos);
                await Task.WhenAll(task1, task2);
            }
        }

        void initBattleTeams()
        {
            string country1 = "C1";
            string federation1 = "F1";

            string country2 = "C2";
            string federation2 = "F2";

            ArmyMaker armyMaker = new ArmyMaker();
            TileMap map = TileMap.DefaultInstance();

            battleManager = new BattleManager();

            battleManager.CountryFederationMapDic.AddOrUpdate(country1, federation1, (key, oldvalue) => federation1);
            battleManager.CountryFederationMapDic.AddOrUpdate(country2, federation2, (key, oldvalue) => federation2);

            battleManager.CountryBattleTeamsMapDic.AddOrUpdate(country1, new List<BattleTeam>(), (key, oldValue) => oldValue);
            battleManager.CountryBattleTeamsMapDic.AddOrUpdate(country2, new List<BattleTeam>(), (key, oldValue) => oldValue);

            List<MapTileIndex> team1Tiles = new List<MapTileIndex>();
            team1Tiles.Add(new MapTileIndex(8, 4));
            team1Tiles.Add(new MapTileIndex(9, 4));
            team1Tiles.Add(new MapTileIndex(10, 4));
            team1Tiles.Add(new MapTileIndex(11, 4));
            List<MapTileIndex> team2Tiles = new List<MapTileIndex>();
            team2Tiles.Add(new MapTileIndex(8, 21));
            team2Tiles.Add(new MapTileIndex(9, 21));
            team2Tiles.Add(new MapTileIndex(10, 21));
            team2Tiles.Add(new MapTileIndex(11, 21));

            battleTeam1 = new BattleTeam(ArmyMaker.NewId(), "C1.T1", country1, federation1, map, team1Tiles,  this.battleManager);
            battleTeam2 = new BattleTeam(ArmyMaker.NewId(), "C2.T2", country2, federation2, map, team2Tiles, this.battleManager);

            battleManager.CountryBattleTeamsMapDic[country1].Add(battleTeam1);
            battleManager.CountryBattleTeamsMapDic[country2].Add(battleTeam2);

            battleTeam1.OnAddBattleUnitSucceeded += this.OnAddBattleUnitToBattleTeamSucceeded;
            battleTeam2.OnAddBattleUnitSucceeded += this.OnAddBattleUnitToBattleTeamSucceeded;

            ArmyMaker.MakeArmy(battleTeam1, battleInfo.Team1SwordNumber, battleInfo.Team1BowNumber, 2, 2, 1);
            ArmyMaker.MakeArmy(battleTeam2, battleInfo.Team2SwordNumber, battleInfo.Team2BowNumber, 2, 2, 1);
            battleTeam1.Setting.MaxLockPerEnemy = 5;
            battleTeam2.Setting.MaxLockPerEnemy = 5;

            battleManager.CountryBattleTeamsMapDic[country1].Add(battleTeam1);
            battleManager.CountryBattleTeamsMapDic[country2].Add(battleTeam2);

            var center = new MapVertex
            {
                X = (int)this.VisibleBoundsWorldspace.MaxX / 2,
                Y = (int)this.VisibleBoundsWorldspace.MaxY / 2,
                Z = 0
            };

            battleTeam1.SelectAll();
            battleTeam2.SelectAll();

            battleTeam1.SetEnemy(battleTeam2.Country, battleTeam2.Federation);
            battleTeam2.SetEnemy(battleTeam1.Country, battleTeam1.Federation);

            var detectTask = battleTeam1.Detect(cancelSourceForMessageLoopUp.Token);
            var detectTask2 = battleTeam2.Detect(cancelSourceForMessageLoopUp.Token);

            this.Schedule();
            //CCScheduler scheduler = new CCScheduler(); 
            //var updateLoop = labelUpdateLoop();

            //battleTeam1.CreateSubTeam<BowMan>(1);
            //battleTeam1.CreateSubTeam(2, 500, 0, 0, 0, 0);
            //battleTeam1.CreateSubTeam(3, 0, 0, 2, 2, 2);
        }

        public override void Update(float dt)
        {
            base.Update(dt);
            string team1 = battleTeam1.Name;
            string team2 = battleTeam2.Name;
            int team1Alive = battleTeam1.AllAliveUnits.Count();
            int team2Alive = battleTeam2.AllAliveUnits.Count();

            if (team1Alive > 0 && team2Alive > 0)
            {
                this.team1Label.Text = string.Format("{0}:{1}, {2}", team1, team1Alive, battleTeam1.TotalHealth);
                this.team2Label.Text = string.Format("{0}:{1},  {2}", team2, team2Alive, battleTeam2.TotalHealth);
                //await Task.Delay(500);
                //team1Alive = battleTeam1.AllAliveUnits.Count();
                //team2Alive = battleTeam2.AllAliveUnits.Count();
            }
            else
            {
                //this.team1Label.Text = string.Format("{0}:{1},  {2}:{3}", team1, team1Alive, team2, team2Alive);
                
                BattleResult result = new BattleResult
                {
                    team1 = team1,
                    team1Alive = team1Alive,
                    team2 = team2,
                    team2Alive = team2Alive
                };

                GameOverLayer overLayer = new GameOverLayer(result);
                CCScene scene = new CCScene(Window);
                scene.AddChild(overLayer);
                Window.RunWithScene(scene);
            }
        }

        void OnAddBattleUnitToBattleTeamSucceeded(BattleUnit unit)
        {
            int teamId = unit.Team.Country == "C1" ? 1 : 2;
            BattleUnitSprite unitSprite = new BattleUnitSprite(unit, teamId);
            this.AddChild(unitSprite);
        }
    }
}

