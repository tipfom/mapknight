using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using mapKnight.Extended;

namespace mapKnight.Android {
    [Activity(
        Label = "@string/app_name", 
        ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.KeyboardHidden, 
        ScreenOrientation = ScreenOrientation.SensorLandscape, 
        Icon = "@drawable/icon", 
        Theme = "@style/thisTheme",
        HardwareAccelerated = true)]
    public class MainActivity : Activity {
        View view;

        protected override void OnCreate (Bundle bundle) {
            RequestWindowFeature(WindowFeatures.NoTitle);
            base.OnCreate(bundle);

            // Create our OpenGL view, and display it
            Extended.Assets.Context = this;

            view = new View(this);
            view.SetOnTouchListener(TouchHandler.Instance);
            SetContentView(view);
        }

        protected override void OnStop ( ) {
            base.OnStop( );
        }

        protected override void OnResume ( ) {
            base.OnResume( );
        }

        protected override void OnDestroy ( ) {
            Manager.Destroy( );
            base.OnDestroy( );
        }

        public override void OnWindowFocusChanged (bool hasFocus) {
            base.OnWindowFocusChanged(hasFocus);
            if (hasFocus) {
                base.Window.DecorView.SystemUiVisibility = (StatusBarVisibility)(
                    SystemUiFlags.LayoutFullscreen |
                    SystemUiFlags.Fullscreen | 
                    SystemUiFlags.LayoutHideNavigation|
                    SystemUiFlags.HideNavigation| 
                    SystemUiFlags.ImmersiveSticky);
            }
        }
    }
}
