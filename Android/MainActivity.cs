using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using mapKnight.Android.CGL;

namespace mapKnight.Android {
    [Activity (Label = "mapKnight", ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.KeyboardHidden, MainLauncher = true, ScreenOrientation = ScreenOrientation.SensorLandscape, Icon = "@drawable/icon")]
    public class MainActivity : Activity {
        CGLView view;

        protected override void OnCreate (Bundle bundle) {
            RequestWindowFeature (WindowFeatures.NoTitle);
            base.OnCreate (bundle);
            // Create our OpenGL view, and display it
            Content.Initialized += Content_Initialized;
            Content.PrepareInit (this);

            view = new CGLView (this);
            SetContentView (view);
            HideNavBar ( );
        }

        private void Content_Initialized (global::Android.Content.Context GameContext) {
            view.SetOnTouchListener (Content.GUI);
        }

        protected override void OnStop () {
            base.OnStop ( );
        }

        protected override void OnResume () {
            base.OnResume ( );
            HideNavBar ( );
        }

        public void HideNavBar () {
            //versteckt die Navigationsleiste
            View decorView = Window.DecorView;
            int newUiOptions = (int)decorView.WindowSystemUiVisibility;
            newUiOptions |= (int)SystemUiFlags.Fullscreen;
            newUiOptions |= (int)SystemUiFlags.HideNavigation;
            newUiOptions |= (int)SystemUiFlags.ImmersiveSticky;
            decorView.SystemUiVisibility = (StatusBarVisibility)newUiOptions;
        }
    }
}
