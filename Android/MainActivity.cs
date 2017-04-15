using Android.App;
using Android.Content.PM;
using Android.OS;
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
        protected override void OnCreate (Bundle bundle) {
            base.Window.DecorView.SystemUiVisibility = Constants.STATUS_BAR_VISIBILITY;
            base.Window.DecorView.SystemUiVisibilityChange += (sender, e) => {
                if (Window.DecorView.SystemUiVisibility != Constants.STATUS_BAR_VISIBILITY) 
                    Window.DecorView.SystemUiVisibility = Constants.STATUS_BAR_VISIBILITY;
            };
            base.OnCreate(bundle);

            // Create our OpenGL view, and display it
            Extended.Assets.Context = this;

            View view = new View(this);
            view.SetOnTouchListener(TouchHandler.Instance);
            SetContentView(view);
        }

        protected override void OnDestroy ( ) {
            Manager.Destroy( );
            base.OnDestroy( );
        }
    }
}
