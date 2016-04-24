using Android.App;
using Android.Content;
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
            view = new CGLView (this);
            SetContentView (view);
            HideNavBar ();
            Content.OnInit += OnInit;
        }

        protected override void OnStop () {
            base.OnStop ();
        }

        protected override void OnResume () {
            base.OnResume ();
            HideNavBar ();
        }

        private void OnInit (Context context) {
            view.SetOnTouchListener (new mapKnight.Android.CGL.GUI.GUI ());
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
