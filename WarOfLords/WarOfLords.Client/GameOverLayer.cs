using System;
using System.Collections.Generic;
using CocosSharp;

namespace WarOfLords.Client
{
    public class GameOverLayer : CCLayerColor
    {

        string scoreMessage = string.Empty;

        public GameOverLayer (BattleResult battleResult)
        {

            var touchListener = new CCEventListenerTouchAllAtOnce ();
            touchListener.OnTouchesEnded += (touches, ccevent) =>
            {
                CCScene scene = new CCScene(this.Window);
                scene.AddChild(new GameStartLayer());
                Window.RunWithScene(scene);
            };

            AddEventListener (touchListener, this);

            scoreMessage = string.Format("{0}:{1},  {2}:{3}", battleResult.team1, battleResult.team1Alive, battleResult.team2, battleResult.team2Alive);

            Color = new CCColor3B (CCColor4B.Black);

            Opacity = 255;
        }

        protected override void AddedToScene ()
        {
            base.AddedToScene ();

            //Scene.SceneResolutionPolicy = CCSceneResolutionPolicy.ShowAll;

            var scoreLabel = new CCLabel (scoreMessage, "arial", 18) {
                Position = new CCPoint (VisibleBoundsWorldspace.Size.Center.X, VisibleBoundsWorldspace.Size.Center.Y + 50),
                Color = new CCColor3B (CCColor4B.Yellow),
                HorizontalAlignment = CCTextAlignment.Center,
                VerticalAlignment = CCVerticalTextAlignment.Center,
                AnchorPoint = CCPoint.AnchorMiddle
            };

            AddChild (scoreLabel);

            var playAgainLabel = new CCLabel ("Tap to Play Again", "arial", 18) {
                Position = VisibleBoundsWorldspace.Size.Center,
                Color = new CCColor3B (CCColor4B.Green),
                HorizontalAlignment = CCTextAlignment.Center,
                VerticalAlignment = CCVerticalTextAlignment.Center,
                AnchorPoint = CCPoint.AnchorMiddle,
            };

            AddChild (playAgainLabel);
        }
    }
}