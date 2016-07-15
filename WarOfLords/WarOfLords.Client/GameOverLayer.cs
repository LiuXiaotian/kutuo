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
                CCScene scene = new CCScene(GameView);
                scene.AddLayer(new GameLayer());
                GameView.RunWithScene(scene);
            };

            AddEventListener (touchListener, this);

            scoreMessage = string.Format("{0}:{1},  {2}:{3}", battleResult.team1, battleResult.team1Alive, battleResult.team2, battleResult.team2Alive);

            Color = new CCColor3B (CCColor4B.Black);

            Opacity = 255;
        }

        public void AddMonkey ()
        {
            //var spriteSheet = new CCSpriteSheet ("animations/monkey.plist");
            //var frame = spriteSheet.Frames.Find ((x) => x.TextureFilename.StartsWith ("frame"));
           
            //var monkey = new CCSprite (frame) {
            //    Position = new CCPoint (VisibleBoundsWorldspace.Size.Center.X + 20, VisibleBoundsWorldspace.Size.Center.Y + 300),
            //    Scale = 0.5f
            //};

            //AddChild (monkey);
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

            AddMonkey ();
        }

        //public static CCScene SceneWithScore (CCWindow mainWindow, int score)
        //{
        //    var scene = new CCScene (mainWindow);
        //    var layer = new GameOverLayer (score);

        //    scene.AddChild (layer);

        //    return scene;
        //}
    }
}