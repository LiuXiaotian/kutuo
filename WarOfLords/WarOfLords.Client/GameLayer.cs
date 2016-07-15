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
    public class GameLayer : CCLayerColor
    {

        // Define a label variable
        CCLabel team1Label;
        CCLabel team2Label;
        SwordMan swordMan;
        BattleTeam battleTeam1;
        BattleTeam battleTeam2;
        BattleManager battleManager;
        CancellationTokenSource cancelSourceForMessageLoopUp = new CancellationTokenSource();

        public GameLayer() : base(CCColor4B.Blue)
        {
            var touchListener = new CCEventListenerTouchAllAtOnce(); 
            touchListener.OnTouchesEnded += this.OnTouchesEnded;
            AddEventListener(touchListener, this);

            this.Color = new CCColor3B(CCColor4B.White);
            this.Opacity = 0xFF;

           // //create and initialize a Label
           //label = new CCLabel("Hello CocosSharp", "Fonts/MarkerFelt", 22, CCLabelFormat.SpriteFont);

           // //add the label as a child to this Layer
           // AddChild(label);

        }

        protected override void AddedToScene()
        {
            base.AddedToScene();

            
            
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
            team1Label.Color = new CCColor3B(CCColor4B.Blue);
            team1Label.PositionX = 0;
            team1Label.PositionY = 0;
            team1Label.AnchorPoint = new CCPoint(0, 0);
            AddChild(team1Label);

            team2Label = new CCLabel("Team2Label", "Arial", 18, CCLabelFormat.SystemFont);
            team2Label.Color = new CCColor3B(CCColor4B.Red);
            team2Label.PositionX = 0;
            team2Label.PositionY = this.GameView.DesignResolution.Height;
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
                MapVertex attackPos = new MapVertex
                {
                    X = (int)touches[0].Location.X,
                    Y = (int)touches[0].Location.Y,
                    Z = 0
                };
                var task1 = battleTeam1.Attack(attackPos);
                var task2 = battleTeam2.Attack(attackPos);
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
            MapMaker mapMaker = new MapMaker();

            battleManager = new BattleManager();

            battleManager.CountryFederationMapDic.AddOrUpdate(country1, federation1, (key, oldvalue) => federation1);
            battleManager.CountryFederationMapDic.AddOrUpdate(country2, federation2, (key, oldvalue) => federation2);

            battleManager.CountryBattleTeamsMapDic.AddOrUpdate(country1, new List<BattleTeam>(), (key, oldValue) => oldValue);
            battleManager.CountryBattleTeamsMapDic.AddOrUpdate(country2, new List<BattleTeam>(), (key, oldValue) => oldValue);

            battleTeam1 = new BattleTeam(ArmyMaker.NewId(), "C1.T1", country1, federation1,  this.battleManager);
            battleTeam2 = new BattleTeam(ArmyMaker.NewId(), "C2.T2", country2, federation2, this.battleManager);

            battleManager.CountryBattleTeamsMapDic[country1].Add(battleTeam1);
            battleManager.CountryBattleTeamsMapDic[country2].Add(battleTeam2);

            battleTeam1.OnAddBattleUnitSucceeded += this.OnAddBattleUnitToBattleTeamSucceeded;
            battleTeam2.OnAddBattleUnitSucceeded += this.OnAddBattleUnitToBattleTeamSucceeded;

            ArmyMaker.MakeArmy(battleTeam1, 10, 10, 20, 2, 2);
            ArmyMaker.MakeArmy(battleTeam2, 10, 10, 20, 2, 2);
            battleTeam1.Setting.MaxLockPerEnemy = 5;
            battleTeam2.Setting.MaxLockPerEnemy = 2;

            battleManager.CountryBattleTeamsMapDic[country1].Add(battleTeam1);
            battleManager.CountryBattleTeamsMapDic[country2].Add(battleTeam2);

            var map = MapMaker.MakeRectMap(
                new MapVertex
                {
                    X = 0,
                    Y = 0,
                    Z = 0
                }, 
                this.GameView.DesignResolution.Width, 
                this.GameView.DesignResolution.Height
                );

            battleTeam1.EnterMap(map);
            battleTeam1.Position = new MapVertex
            {
                X = this.GameView.DesignResolution.Width / 2,
                Y = 30,
                Z = 0
            }; 

            battleTeam2.EnterMap(map);
            battleTeam2.Position = new MapVertex
            {
                X = this.GameView.DesignResolution.Width / 2,
                Y = this.GameView.DesignResolution.Height - 50,
                Z = 0
            };

            var center = new MapVertex
            {
                X = this.GameView.DesignResolution.Width / 2,
                Y = this.GameView.DesignResolution.Height / 2,
                Z = 0
            };

            battleTeam1.SelectAll();
            battleTeam2.SelectAll();

            battleTeam1.SetEnemy(battleTeam2.Country, battleTeam2.Federation);
            battleTeam2.SetEnemy(battleTeam1.Country, battleTeam1.Federation);

            var detectTask = battleTeam1.Detect(cancelSourceForMessageLoopUp.Token);
            var detectTask2 = battleTeam2.Detect(cancelSourceForMessageLoopUp.Token);
            var updateLoop = labelUpdateLoop();

            //battleTeam1.CreateSubTeam<BowMan>(1);
            //battleTeam1.CreateSubTeam(2, 500, 0, 0, 0, 0);
            //battleTeam1.CreateSubTeam(3, 0, 0, 2, 2, 2);
        }

        async Task labelUpdateLoop()
        {
            string team1 = battleTeam1.Name;
            string team2 = battleTeam2.Name;
            int team1Alive = battleTeam1.AllAliveUnits.Count();
            int team2Alive = battleTeam2.AllAliveUnits.Count();

            while (team1Alive > 0 && team2Alive > 0)
            {
                this.team1Label.Text = string.Format("{0}:{1}, {2}", team1, team1Alive, battleTeam1.TotalHealth);
                this.team2Label.Text = string.Format("{0}:{1},  {2}", team2, team2Alive, battleTeam2.TotalHealth);
                await Task.Delay(500);
                team1Alive = battleTeam1.AllAliveUnits.Count();
                team2Alive = battleTeam2.AllAliveUnits.Count();
            }
            this.team1Label.Text = string.Format("{0}:{1},  {2}:{3}", team1, team1Alive, team2, team2Alive);

            //await Task.Delay(1000);
            BattleResult result = new BattleResult
            {
                team1 = team1,
                team1Alive = team1Alive,
                team2 = team2,
                team2Alive = team2Alive
            };

            GameOverLayer overLayer = new GameOverLayer(result);
            CCScene scene = new CCScene(GameView);
            scene.AddLayer(overLayer);
            GameView.RunWithScene(scene);
        }

        void OnAddBattleUnitToBattleTeamSucceeded(BattleUnit unit)
        {
            int teamId = unit.Team.Country == "C1" ? 1 : 2;
            BattleUnitSprite unitSprite = new BattleUnitSprite(unit, teamId);
            this.AddChild(unitSprite);
        }
    }
}

