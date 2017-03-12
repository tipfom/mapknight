using System;
using System.ComponentModel;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Content.PM;
using static Android.Views.ViewTreeObserver;
using Android.Animation;
using Android.Views.Animations;

namespace mapKnight.Android {
    [Activity(
        MainLauncher = true,
        Label = "@string/app_name",
        Theme = "@android:style/Theme.NoTitleBar.Fullscreen",
        NoHistory = true,
        ScreenOrientation = ScreenOrientation.SensorLandscape)]
    public class SplashActivity : Activity {
        protected override void OnCreate (Bundle savedInstanceState) {
            base.OnCreate(savedInstanceState);
            Window.DecorView.SystemUiVisibility = (StatusBarVisibility)(
                    SystemUiFlags.LayoutFullscreen |
                    SystemUiFlags.Fullscreen |
                    SystemUiFlags.LayoutHideNavigation |
                    SystemUiFlags.HideNavigation |
                    SystemUiFlags.ImmersiveSticky);
            SetContentView(Resource.Layout.SplashScreen);


            ImageView iconImage = FindViewById<ImageView>(Resource.Id.iconimage);

            LayoutListener listener = new LayoutListener( );
            listener.LoadingFinished += ( ) => {
                BackgroundWorker worker = new BackgroundWorker( );
                worker.DoWork += (sender, e) => {
                    Intent intent = new Intent(this, typeof(MainActivity));
                    intent.SetFlags(ActivityFlags.ClearTop);
                    StartActivity(new Intent(this, typeof(MainActivity)));
                };
                worker.RunWorkerCompleted += (sender, e) => {
                    Finish( );
                };
                worker.RunWorkerAsync( );
            };
            iconImage.ViewTreeObserver.AddOnGlobalLayoutListener(listener);
        }

        private class LayoutListener : Java.Lang.Object, IOnGlobalLayoutListener {
            public event Action LoadingFinished;

            public void OnGlobalLayout( ) {
                LoadingFinished?.Invoke( );
            }
        }
    }
}