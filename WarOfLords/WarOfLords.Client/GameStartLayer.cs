using System;
using System.Collections.Generic;
using CocosSharp;
using CocosDenshion;

namespace WarOfLords.Client
{
    
    public class GameStartLayer : CCLayerColor
    {
        CCLabel startLabel;
        CCTextField tractingTextField;
        public GameStartLayer () : base(CCColor4B.Blue)
        {
            
        }

        protected override void AddedToScene ()
        {
            base.AddedToScene ();

            CCTextField txTeam1SwordNumber = new CCTextField("Team 1 Sword", "arial", 30, CCLabelFormat.SystemFont);
            txTeam1SwordNumber.AnchorPoint = CCPoint.Zero;
            txTeam1SwordNumber.Position = new CCPoint(100, 100);
            txTeam1SwordNumber.AutoEdit = true;

            CCTextField txTeam1BowNumber = new CCTextField("Team 1 Bow", "arial", 30, CCLabelFormat.SystemFont);
            txTeam1BowNumber.AnchorPoint = CCPoint.Zero;
            txTeam1BowNumber.Position = new CCPoint(100, 150);
            txTeam1BowNumber.AutoEdit = true;

            CCTextField txTeam2SwordNumber = new CCTextField("Team 2 Sword", "arial", 30, CCLabelFormat.SystemFont);
            txTeam2SwordNumber.AnchorPoint = CCPoint.Zero;
            txTeam2SwordNumber.Position = new CCPoint(100, 200);
            txTeam2SwordNumber.AutoEdit = true;

            CCTextField txTeam2BowNumber = new CCTextField("Team 2 Bow", "arial", 30, CCLabelFormat.SystemFont);
            txTeam2BowNumber.AnchorPoint = CCPoint.Zero;
            txTeam2BowNumber.Position = new CCPoint(100, 250);
            txTeam2SwordNumber.AutoEdit = true;

            AddChild(txTeam1BowNumber);
            AddChild(txTeam1SwordNumber);
            AddChild(txTeam2BowNumber);
            AddChild(txTeam2SwordNumber);
            txTeam1SwordNumber.Text = "50";
            txTeam1BowNumber.Text = "50";
            txTeam2SwordNumber.Text = "50";
            txTeam2BowNumber.Text = "50";

            startLabel = new CCLabel("Start to Fight", "arial", 50)
            {
                Color = CCColor3B.Green,
               //AnchorPoint = CCPoint.Zero,
               // Position = new CCPoint(0, 0), 
            };

            var exitLabel = new CCLabel("Exit", "arial", 50)
            {
                Color = CCColor3B.Green,
                //AnchorPoint = CCPoint.Zero,
                //Position = new CCPoint(0, 100)
            };

            CCMenuItemLabel fightMenuItem = new CCMenuItemLabel(startLabel, (obj) =>
            { 
                BattleInfo info = new BattleInfo
                {
                    Team1SwordNumber = int.Parse(txTeam1SwordNumber.Text),
                    Team1BowNumber = int.Parse(txTeam1BowNumber.Text),
                    Team2SwordNumber = int.Parse(txTeam2SwordNumber.Text),
                    Team2BowNumber = int.Parse(txTeam2BowNumber.Text)
                };
                CCScene scene = new CCScene(this.Window);
                var tileMap = new CCTileMap("BattleTiledMapBg.tmx");
                //tileMap.AnchorPoint = new CCPoint(0.5f, 0.5f);
                //tileMap.Position = scene.VisibleBoundsWorldspace.Center;
                scene.AddChild(tileMap);

                scene.AddChild(new GameLayer(info));
                this.Window.RunWithScene(scene);
            });
            fightMenuItem.AnchorPoint = CCPoint.Zero;

            CCMenuItemLabel exitMenuItem = new CCMenuItemLabel(exitLabel, (obj) =>
            {
                BattleInfo info = new BattleInfo
                {
                    Team1SwordNumber = int.Parse(txTeam1SwordNumber.Text),
                    Team1BowNumber = int.Parse(txTeam1BowNumber.Text),
                    Team2SwordNumber = int.Parse(txTeam2SwordNumber.Text),
                    Team2BowNumber = int.Parse(txTeam2BowNumber.Text)
                };
                CCScene scene = new CCScene(Window);
                scene.AddChild(new GameLayer(info));
                Window.RunWithScene(scene);
            });
            exitMenuItem.AnchorPoint = CCPoint.Zero;

            CCMenu menu = new CCMenu();
            menu.AnchorPoint = new CCPoint(0.5f, 0.5f);
            menu.Position = new CCPoint(300, 500);
            menu.AddChild(fightMenuItem);
            menu.AddChild(exitMenuItem);
            menu.AlignItemsVertically();

            AddChild (menu);

            var touchListener = new CCEventListenerTouchOneByOne();
            touchListener.IsSwallowTouches = true;

            touchListener.OnTouchBegan += (touch, ccevent) =>
            {
                //tractingTextField = txTeam1BowNumber;
                //txTeam1BowNumber.Edit();
                return true;
            };
            touchListener.OnTouchEnded += (touch, ccevent) =>
            {
                
                if (txTeam1BowNumber.BoundingBoxTransformedToWorld.ContainsPoint(touch.Location))
                {
                    tractingTextField = txTeam1BowNumber;
                    txTeam1BowNumber.Edit();
                }
                if (txTeam1SwordNumber.BoundingBoxTransformedToWorld.ContainsPoint(touch.Location))
                {
                    tractingTextField = txTeam1SwordNumber;
                    txTeam1SwordNumber.Edit();
                }
                if (txTeam2BowNumber.BoundingBoxTransformedToWorld.ContainsPoint(touch.Location))
                {
                    tractingTextField = txTeam2BowNumber;
                    txTeam2BowNumber.Edit();
                }
                if (txTeam2SwordNumber.BoundingBoxTransformedToWorld.ContainsPoint(touch.Location))
                {
                    tractingTextField = txTeam2SwordNumber;
                    txTeam2SwordNumber.Edit();
                }
                //if(tractingTextField != null)
                //{
                //    //tractingTextField.EndEdit();
                //    tractingTextField = null;
                //}
            };
            
            AddEventListener(touchListener, this);
            Opacity = 255;
        }

        //public static CCScene GameStartLayerScene (CCWindow mainWindow)
        //{
        //    var scene = new CCScene (mainWindow);
        //    var layer = new GameStartLayer ();

        //    scene.AddChild (layer);

        //    return scene;
        //}
    }
}