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

            CCLabel lbTeam1SwordNumber = new CCLabel("Team 1 Sword:", "arial", 30);
            lbTeam1SwordNumber.AnchorPoint = CCPoint.Zero;
            lbTeam1SwordNumber.Position = new CCPoint(10, 100);
            CCTextField txTeam1SwordNumber = new CCTextField("Team 1 Sword", "arial", 30, new CCSize(100, 40), CCTextAlignment.Left, CCLabelFormat.SystemFont);
            txTeam1SwordNumber.AnchorPoint = CCPoint.Zero;
            txTeam1SwordNumber.Position = new CCPoint(300, 100);
            txTeam1SwordNumber.AutoEdit = true;

            CCLabel lbTeam1BowNumber = new CCLabel("Team 1 Bow:", "arial", 30);
            lbTeam1BowNumber.AnchorPoint = CCPoint.Zero;
            lbTeam1BowNumber.Position = new CCPoint(10, 200);
            CCTextField txTeam1BowNumber = new CCTextField("Team 1 Bow", "arial", 30, new CCSize(100, 40), CCTextAlignment.Left, CCLabelFormat.SystemFont);
            txTeam1BowNumber.AnchorPoint = CCPoint.Zero;
            txTeam1BowNumber.Position = new CCPoint(300, 200);
            txTeam1BowNumber.AutoEdit = true;

            CCLabel lbTeam1MedicalNumber = new CCLabel("Team 1 Medical:", "arial", 30);
            lbTeam1MedicalNumber.AnchorPoint = CCPoint.Zero;
            lbTeam1MedicalNumber.Position = new CCPoint(10, 300);
            CCTextField txTeam1MedicalNumber = new CCTextField("Team 1 Medical", "arial", 30, new CCSize(100, 40), CCTextAlignment.Left, CCLabelFormat.SystemFont);
            txTeam1MedicalNumber.AnchorPoint = CCPoint.Zero;
            txTeam1MedicalNumber.Position = new CCPoint(300, 300);
            txTeam1MedicalNumber.AutoEdit = true;

            CCLabel lbTeam2SwordNumber = new CCLabel("Team 2 Sword:", "arial", 30);
            lbTeam2SwordNumber.AnchorPoint = CCPoint.Zero;
            lbTeam2SwordNumber.Position = new CCPoint(10, 900);
            CCTextField txTeam2SwordNumber = new CCTextField("Team 2 Sword", "arial", 30, new CCSize(100, 40), CCTextAlignment.Left, CCLabelFormat.SystemFont);
            txTeam2SwordNumber.AnchorPoint = CCPoint.Zero;
            txTeam2SwordNumber.Position = new CCPoint(300, 900);
            txTeam2SwordNumber.AutoEdit = true;

            CCLabel lbTeam2BowNumber = new CCLabel("Team 2 Bow:", "arial", 30);
            lbTeam2BowNumber.AnchorPoint = CCPoint.Zero;
            lbTeam2BowNumber.Position = new CCPoint(10, 800);
            CCTextField txTeam2BowNumber = new CCTextField("Team 2 Bow", "arial", 30, new CCSize(100, 40), CCTextAlignment.Left, CCLabelFormat.SystemFont);
            txTeam2BowNumber.AnchorPoint = CCPoint.Zero;
            txTeam2BowNumber.Position = new CCPoint(300, 800);
            txTeam2BowNumber.AutoEdit = true;

            CCLabel lbTeam2MedicalNumber = new CCLabel("Team 2 Medical:", "arial", 30);
            lbTeam2MedicalNumber.AnchorPoint = CCPoint.Zero;
            lbTeam2MedicalNumber.Position = new CCPoint(10, 700);
            CCTextField txTeam2MedicalNumber = new CCTextField("Team 2 Medical", "arial", 30, new CCSize(100, 40), CCTextAlignment.Left, CCLabelFormat.SystemFont);
            txTeam2MedicalNumber.AnchorPoint = CCPoint.Zero;
            txTeam2MedicalNumber.Position = new CCPoint(300, 700);
            txTeam2MedicalNumber.AutoEdit = true;


            AddChild(txTeam1BowNumber);
            AddChild(txTeam1SwordNumber);
            AddChild(txTeam2BowNumber);
            AddChild(txTeam2SwordNumber);
            AddChild(txTeam1MedicalNumber);
            AddChild(txTeam2MedicalNumber);


            txTeam1SwordNumber.Text = "15";
            txTeam1BowNumber.Text = "5";
            txTeam1MedicalNumber.Text = "5";
            txTeam2SwordNumber.Text = "5";
            txTeam2BowNumber.Text = "15";
            txTeam2MedicalNumber.Text = "5";

            AddChild(lbTeam1BowNumber);
            AddChild(lbTeam1SwordNumber);
            AddChild(lbTeam2BowNumber);
            AddChild(lbTeam2SwordNumber);
            AddChild(lbTeam1MedicalNumber);
            AddChild(lbTeam2MedicalNumber);

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
                    Team2BowNumber = int.Parse(txTeam2BowNumber.Text),
                    Team1MedicalNumber = int.Parse(txTeam1MedicalNumber.Text),
                    Team2MedicalNumber = int.Parse(txTeam2MedicalNumber.Text)
                };
                //CCScene scene = new CCScene(this.Window);
                //var tileMap = new CCTileMap("ClassicMap.tmx");
                ////tileMap.AnchorPoint = new CCPoint(0.5f, 0.5f);
                ////tileMap.Position = scene.VisibleBoundsWorldspace.Center;
                //scene.AddChild(tileMap);

                //scene.AddChild(new GameLayer(info));
                BattleScene battleScene = new BattleScene(this.Window, "ClassicMap.tmx", info);
                this.Window.RunWithScene(battleScene);
            });
            fightMenuItem.AnchorPoint = CCPoint.Zero;

            CCMenuItemLabel exitMenuItem = new CCMenuItemLabel(exitLabel, (obj) =>
            {
                //this.Application.
            });
            exitMenuItem.AnchorPoint = CCPoint.Zero;

            CCMenu menu = new CCMenu();
            menu.AnchorPoint = new CCPoint(0.5f, 0.5f);
            menu.Position = new CCPoint(400, 500);
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
                if (txTeam1MedicalNumber.BoundingBoxTransformedToWorld.ContainsPoint(touch.Location))
                {
                    tractingTextField = txTeam1MedicalNumber;
                    txTeam1MedicalNumber.Edit();
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
                if (txTeam2MedicalNumber.BoundingBoxTransformedToWorld.ContainsPoint(touch.Location))
                {
                    tractingTextField = txTeam2MedicalNumber;
                    txTeam2MedicalNumber.Edit();
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