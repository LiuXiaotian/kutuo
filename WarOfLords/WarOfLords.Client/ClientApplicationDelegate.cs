using CocosDenshion;
using CocosSharp;
using System;
using System.Collections.Generic;

namespace WarOfLords.Client
{
    public class ClientApplicationDelegate
    {

        //public static void LoadGame(object sender, EventArgs e)
        //{
        //    CCGameView gameView = sender as CCGameView;

        //    if (gameView != null)
        //    {
        //        var contentSearchPaths = new List<string>() { "Fonts", "Sounds", "Images" };
        //        CCSizeI viewSize = gameView.ViewSize;
        //        // Set world dimensions
        //        gameView.DesignResolution = new CCSizeI(768, 1024);
        //        gameView.ResolutionPolicy = CCViewResolutionPolicy.ExactFit;

        //        // Determine whether to use the high or low def versions of our images
        //        // Make sure the default texel to content size ratio is set correctly
        //        // Of course you're free to have a finer set of image resolutions e.g (ld, hd, super-hd)
        //        //if (width < viewSize.Width)
        //        //{
        //        //    contentSearchPaths.Add("Images/Hd");
        //        //    CCSprite.DefaultTexelToContentSizeRatio = 2.0f;
        //        //}
        //        //else
        //        //{
        //        //    contentSearchPaths.Add("Images/Ld");
        //        //    CCSprite.DefaultTexelToContentSizeRatio = 1.0f;
        //        //}

        //        gameView.ContentManager.SearchPaths = contentSearchPaths;

        //        CCScene gameScene = new CCScene(gameView);
        //        gameScene.AddChild(new GameStartLayer());
        //        gameView.RunWithScene(gameScene);
        //    }
        //}

        //public override void ApplicationDidFinishLaunching (CCApplication application, CCWindow mainWindow)
        //{
        //    application.PreferMultiSampling = false;
        //    application.ContentRootDirectory = "Content";

        //    application.ContentSearchPaths.Add("Images");
        //    application.ContentSearchPaths.Add("Fonts");
        //    application.ContentSearchPaths.Add("Sounds");

        //    //CCSimpleAudioEngine.SharedEngine.PreloadEffect ("Sounds/tap");
        //    //CCSimpleAudioEngine.SharedEngine.PreloadBackgroundMusic ("Sounds/backgroundMusic");
        //    CCSize winSize = mainWindow.WindowSizeInPixels;
        //    CCScene.SetDefaultDesignResolution(winSize.Width, winSize.Height, CCSceneResolutionPolicy.ExactFit);

        //    CCScene scene = GameStartLayer.GameStartLayerScene(mainWindow);
        //    mainWindow.RunWithScene (scene);
        //}

        //public override void ApplicationDidEnterBackground (CCApplication application)
        //{
        //    // stop all of the animation actions that are running.
        //    application.Paused = true;

        //    // if you use SimpleAudioEngine, your music must be paused
        //    CCSimpleAudioEngine.SharedEngine.PauseBackgroundMusic ();
        //}

        //public override void ApplicationWillEnterForeground (CCApplication application)
        //{
        //    application.Paused = false;

        //    // if you use SimpleAudioEngine, your background music track must resume here. 
        //    CCSimpleAudioEngine.SharedEngine.ResumeBackgroundMusic ();
        //}
    }
}