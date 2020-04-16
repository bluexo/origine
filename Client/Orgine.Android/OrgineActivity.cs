using Android.App;
using Android.OS;
using Android.Content.PM;

using Xenko.Engine;
using Xenko.Starter;

namespace Orgine.Android
{
    [Activity(MainLauncher = true, 
              Icon = "@drawable/icon", 
              ScreenOrientation = ScreenOrientation.Landscape,
              ConfigurationChanges = ConfigChanges.UiMode | ConfigChanges.Orientation | ConfigChanges.KeyboardHidden | ConfigChanges.ScreenSize)]
    public class OrgineActivity : AndroidXenkoActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            Game = new Game();
            Game.Run(GameContext);
        }

        protected override void OnDestroy()
        {
            Game.Dispose();

            base.OnDestroy();
        }
    }
}
