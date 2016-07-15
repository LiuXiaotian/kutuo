using System;
using System.Collections.Generic;
using CocosSharp;
using CocosDenshion;

namespace WarOfLords.Client
{
    
    public class GameStartLayer : CCLayerColor
    {
        CCLabel startLabel;
        public GameStartLayer () : base ()
        {
            var touchListener = new CCEventListenerTouchAllAtOnce ();
            touchListener.OnTouchesEnded += (touches, ccevent) =>
            {
                CCScene scene = new CCScene(GameView);
                scene.AddLayer(new GameLayer());
                GameView.RunWithScene(scene);
            };
            AddEventListener (touchListener, this);

            Color = CCColor3B.Black;
            Opacity = 255;
        }

        protected override void AddedToScene ()
        {
            base.AddedToScene ();

            startLabel = new CCLabel("Tap Screen to Fight!", "arial", 22) {
                Position = VisibleBoundsWorldspace.Center,
                Color = CCColor3B.Green,
                HorizontalAlignment = CCTextAlignment.Center,
                VerticalAlignment = CCVerticalTextAlignment.Center,
                AnchorPoint = CCPoint.AnchorMiddle
            };

            

            AddChild (startLabel);
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