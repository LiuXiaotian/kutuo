using System;
using System.Collections.Generic;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

using CocosSharp;
using Microsoft.Xna.Framework;

namespace WarOfLords.Client.Android
{
    [Activity(Label = "WarOfLords.Client.Android", MainLauncher = true, Icon = "@drawable/icon",
        AlwaysRetainTaskState = true,
        Theme = "@android:style/Theme.NoTitleBar",
        LaunchMode = LaunchMode.SingleInstance,
        ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize | ConfigChanges.Keyboard | ConfigChanges.KeyboardHidden)]
    public class MainActivity : Activity
    { 
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            // Get our game view from the layout resource,
            // and attach the view created event to it
            //CCGameView gameView = (CCGameView)FindViewById(Resource.Id.GameView);
            //gameView.ViewCreated += ClientApplicationDelegate.LoadGame;

            var application = new CCApplication();
            application.ApplicationDelegate = new GameAppDelegate();
            SetContentView(application.AndroidContentView);
            application.StartGame();
            application.AndroidContentView.RequestFocus();
            application.AndroidContentView.KeyPress += HandleKeyPress;
        }
    }
}

