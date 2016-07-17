using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using mapKnight.Extended;

namespace mapKnight.Android {
    [Activity(Label = "mapKnight", ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.KeyboardHidden, MainLauncher = true, ScreenOrientation = ScreenOrientation.SensorLandscape, Icon = "@drawable/icon", Theme = "@style/thisTheme")]
    public class MainActivity : Activity {
        View view;

        protected override void OnCreate (Bundle bundle) {
            RequestWindowFeature(WindowFeatures.NoTitle);
            base.OnCreate(bundle);

            // Create our OpenGL view, and display it
            AssetProvider.Context = this;
            Extended.Assets.AssetProvider = new AssetProvider( );

            view = new View(this);
            view.SetOnTouchListener(TouchHandler.Instance);
            SetContentView(view);
            HideNavBar( );
        }

        protected override void OnStop ( ) {
            base.OnStop( );
        }

        protected override void OnResume ( ) {
            base.OnResume( );
            HideNavBar( );
        }

        protected override void OnDestroy ( ) {
            Manager.Destroy( );
            base.OnDestroy( );
        }

        public void HideNavBar ( ) {
            //versteckt die Navigationsleiste
            global::Android.Views.View decorView = base.Window.DecorView;
            int newUiOptions = (int)decorView.WindowSystemUiVisibility;
            newUiOptions |= (int)SystemUiFlags.Fullscreen;
            newUiOptions |= (int)SystemUiFlags.HideNavigation;
            newUiOptions |= (int)SystemUiFlags.ImmersiveSticky;
            decorView.SystemUiVisibility = (StatusBarVisibility)newUiOptions;
        }
    }
}
