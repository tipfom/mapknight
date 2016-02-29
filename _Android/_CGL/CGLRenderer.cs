using System;
using System.Diagnostics;

using Android.Opengl;
using Android.Graphics;
using Android.Content;
using GL = Android.Opengl.GLES20;

using Javax.Microedition.Khronos.Opengles;

using mapKnight.Basic;

namespace mapKnight.Android.CGL
{
	public class CGLRenderer : Java.Lang.Object, GLSurfaceView.IRenderer
	{
		float ratio;
		int screenHeight;
		CGLInterface gameInterface;
		Context context;
		CGLText infoText;
		int frameTime;
		int measueredFPS;

		public CGLRenderer (Context Context)
		{
			context = Context;
		}

#region IRenderer implementation

		public void OnDrawFrame (IGL10 gl)
		{
			GL.GlClear (GL.GlColorBufferBit | GL.GlDepthBufferBit);

			Content.Map.Draw ();
			gameInterface.Draw (Content.Camera.DefaultMVPMatrix);
			CGLEntity.Draw (frameTime);
			CGLText.CGLTextContainer.Draw (Content.Camera.DefaultMVPMatrix);

			Content.Map.Step (frameTime);
			Content.Camera.Update ();

			if (gameInterface.LeftButton.Clicked) {
				Content.Character.Move (Direction.Left);
			} else if (gameInterface.RightButton.Clicked) {
				Content.Character.Move (Direction.Right);
			} else {
				Content.Character.ResetMovement ();	
			}

			CalculateFrameRate ();
		}


		public void OnSurfaceChanged (IGL10 gl, int width, int height)
		{
			ratio = (float)width / height;
			screenHeight = height;

			GL.GlViewport (0, 0, width, height);
			GL.GlClearColor (0f, 0f, 0f, 1.0f);

			Content.Update (new Size (width, height));
		}

		public void OnSurfaceCreated (Javax.Microedition.Khronos.Opengles.IGL10 gl, Javax.Microedition.Khronos.Egl.EGLConfig config)
		{
			XMLElemental configfile = XMLElemental.Load (context.Assets.Open ("main.xml"));
			Content.PreInit (configfile, context);
			Content.Init (configfile);
			Content.AfterInit ();

			GL.GlClearColor (1f, 0f, 1f, 1.0f);

			gameInterface = new CGLInterface ();
			gameInterface.JumpButton.OnClick += () => {
				Content.Character.Jump ();
			};
			infoText = new CGLText ("fps", 30, Font.BitOperator, new Basic.Point (0, 20), Basic.Color.White, FontStyle.Bold);
		}

#endregion

		private int lastTick;
		private int lastSecond;
		private int ticks;

		private void CalculateFrameRate ()
		{
			frameTime = System.Environment.TickCount - lastTick;
			ticks++;
			if (System.Environment.TickCount - lastSecond > 1000) {
				measueredFPS = ticks;
				ticks = 0;
				lastSecond = System.Environment.TickCount;

				infoText.Text = " frametime = " + frameTime.ToString () + " ms ( " + (1000 / frameTime).ToString () + " fps , " + measueredFPS.ToString () + " fps counted )" +
				"\n version = " + Content.Version.ToString (false) +
				"\n terminal connected = " + Content.Terminal.Connected.ToString ().ToLower ();

			}
			lastTick = System.Environment.TickCount;
		}
	}
}