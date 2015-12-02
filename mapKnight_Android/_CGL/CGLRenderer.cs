using System;

using Android.Opengl;
using Android.Graphics;
using GL = Android.Opengl.GLES20;

using Javax.Microedition.Khronos.Opengles;

namespace mapKnight_Android.CGL
{
	public class CGLRenderer : Java.Lang.Object, GLSurfaceView.IRenderer
	{
		CGLMap testsquaremap;

		float ratio;
		int screenHeight;
		XMLElemental mapElemental;
		CGLInterface gameInterface;
		Android.Content.Context context;
		CGLText infoText;

		public CGLRenderer (Android.Content.Context Context)
		{
			context = Context;
		}

		#region IRenderer implementation

		public void OnDrawFrame (IGL10 gl)
		{
			GL.GlClear (GL.GlColorBufferBit | GL.GlDepthBufferBit);

			testsquaremap.Draw (Content.MVPMatrix);
			gameInterface.Draw (Content.MVPMatrix);
			CGLText.CGLTextContainer.Draw (Content.MVPMatrix);
			CalculateFrameRate ();
		}


		public void OnSurfaceChanged (IGL10 gl, int width, int height)
		{
			ratio = (float)width / height;
			screenHeight = height;

			GL.GlViewport (0, 0, width, height);
			testsquaremap = new CGLMap (22, ratio, mapElemental, new Size (width, height));
			GL.GlViewport (0, 0, width, height);
			GL.GlClearColor (0f, 0f, 0f, 1.0f);

			Content.Update (new Size (width, height));
		}

		public void OnSurfaceCreated (Javax.Microedition.Khronos.Opengles.IGL10 gl, Javax.Microedition.Khronos.Egl.EGLConfig config)
		{
			Content.Init (XMLElemental.Load (context.Assets.Open ("main.xml"), false), context);

			GL.GlClearColor (1f, 0f, 1f, 1.0f);

			mapElemental = XMLElemental.Load (context.Assets.Open ("maps/testMap.xml"), false, Compression.Uncompressed);
			gameInterface = new CGLInterface ();
			infoText = new CGLText ("fps", 20, Font.Tahoma, new Point (0, 20), Color.Black);
		}

		#endregion

		private int lastTick;

		private void CalculateFrameRate ()
		{
			infoText.Text = " frametime = " + (System.Environment.TickCount - lastTick).ToString () + " ms ( " + (1000 / (System.Environment.TickCount - lastTick)).ToString () + " fps ) version = " + Content.Version.ToString (false);
			lastTick = System.Environment.TickCount;
		}

	}
}