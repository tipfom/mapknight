using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Content.PM;

using mapKnight_Android.CGL;
using mapKnight_Android.Utils;

namespace mapKnight_Android
{

	[Activity (Label = "mapKnight",
		ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.KeyboardHidden,
		MainLauncher = true,
		ScreenOrientation = ScreenOrientation.SensorLandscape,
		Icon = "@drawable/icon")]
	
	public class MainActivity : Activity
	{
		CGLView view;

		protected override void OnCreate (Bundle bundle)
		{
			RequestWindowFeature (WindowFeatures.NoTitle);
			base.OnCreate (bundle);

			// Create our OpenGL view, and display it
			view = new CGLView (this);

			SetContentView (view);

			HideNavBar ();
		}

		protected override void OnPause ()
		{
			base.OnPause ();
		}

		protected override void OnResume ()
		{
			base.OnResume ();
			HideNavBar ();
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