using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Content.PM;
using mapKnight.Android.CGL;

namespace mapKnight.Android
{
	[Activity (Label = "mapKnight", ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.KeyboardHidden, MainLauncher = true, ScreenOrientation = ScreenOrientation.SensorLandscape, Icon = "@drawable/icon")]
	public class MainActivity : Activity
	{
		CGLView view;

		protected override void OnCreate (Bundle bundle)
		{
			Xamarin.Insights.Initialize (XamarinInsights.ApiKey, this);
			RequestWindowFeature (WindowFeatures.NoTitle);
			base.OnCreate (bundle);
			// Create our OpenGL view, and display it
			view = new CGLView (this);
			SetContentView (view);
			HideNavBar ();
			Content.OnInit += OnInit;
		}

		protected override void OnStop ()
		{
			Content.Terminal.Disconnect ();
			base.OnStop ();
		}

		protected override void OnResume ()
		{
			base.OnResume ();
			HideNavBar ();
		}

		private void OnInit (Context context)
		{
			view.SetOnTouchListener (Content.TouchManager);
			CGL.Entity.CGLEntityPreset preset = new mapKnight.Android.CGL.Entity.CGLEntityPreset (Utils.XMLElemental.Load (context.Assets.Open ("character/robot.character")), context);
			CGL.Entity.CGLEntity test = preset.Instantiate (4, "futuristic");
		}

		public void HideNavBar ()
		{
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
