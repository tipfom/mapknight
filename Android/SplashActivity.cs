using System;
using System.Timers;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Content.PM;
using static Android.Views.ViewTreeObserver;

namespace mapKnight.Android {
    [Activity(
        MainLauncher = true,
        Label = "@string/app_name",
        Theme = "@android:style/Theme.NoTitleBar.Fullscreen",
        NoHistory = true,
        ScreenOrientation = ScreenOrientation.SensorLandscape)]
    public class SplashActivity : Activity {
        Timer timer = new Timer(30);
        bool allreadyStarting = false;

        protected override void OnCreate(Bundle savedInstanceState) {
            Window.DecorView.SystemUiVisibility = Constants.STATUS_BAR_VISIBILITY;
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.SplashScreen);

            timer.Elapsed += Timer_Elapsed;
            LayoutListener listener = new LayoutListener( );
            listener.LoadingFinished += ( ) => {
                if (!allreadyStarting) {
                    timer?.Stop( );
                    timer?.Start( );
                }
            };
            FindViewById<ImageView>(Resource.Id.iconimage).ViewTreeObserver.AddOnGlobalLayoutListener(listener);
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e) {
            if (!allreadyStarting) {
                allreadyStarting = true;
                timer.Stop( );

                Task task = new Task(( ) => {
                    Intent intent = new Intent(this, typeof(MainActivity));
                    intent.SetFlags(ActivityFlags.ClearTop);
                    StartActivity(intent);
                }, TaskCreationOptions.AttachedToParent);
                task.Start( );
            }
        }

        protected override void OnDestroy( ) {
            allreadyStarting = false;
            base.OnDestroy( );
        }

        public override void OnBackPressed( ) {
        }

        private class LayoutListener : Java.Lang.Object, IOnGlobalLayoutListener {
            public event Action LoadingFinished;

            public void OnGlobalLayout( ) {
                LoadingFinished?.Invoke( );
            }
        }
    }
}