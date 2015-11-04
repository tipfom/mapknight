using System;

using Android.Opengl;
using Android.Graphics;
using GL = Android.Opengl.GLES20;

using Javax.Microedition.Khronos.Opengles;

using mapKnight_Android.Utils;

namespace mapKnight_Android
{
	namespace CGL
	{
		public class CGLRenderer : Java.Lang.Object, GLSurfaceView.IRenderer
		{
			CGLMap testsquaremap;

			float ratio;
			int screenHeight;
			XMLElemental mapElemental;
			CGLInterface gameInterface;
			Android.Content.Context context;
			CGLText versionText;
			CGLText fpsText;

			public CGLRenderer (Android.Content.Context Context)
			{
				context = Context;
			}

			#region IRenderer implementation

			public void OnDrawFrame (IGL10 gl)
			{
				GL.GlClear (GL.GlColorBufferBit | GL.GlDepthBufferBit);

				testsquaremap.Draw (GlobalContent.MVPMatrix);
				gameInterface.Draw (GlobalContent.MVPMatrix);
				CGLText.CGLTextContainer.Draw (GlobalContent.MVPMatrix);
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

				GlobalContent.Update (new Size (width, height));
			}

			public void OnSurfaceCreated (Javax.Microedition.Khronos.Opengles.IGL10 gl, Javax.Microedition.Khronos.Egl.EGLConfig config)
			{
				GlobalContent.Init (Utils.XMLElemental.Load (context.Assets.Open ("main.xml"), false), context);

				GL.GlClearColor (1f, 0f, 1f, 1.0f);

				mapElemental = XMLElemental.Load (context.Assets.Open ("maps/testMap.xml"), false, Compression.Uncompressed);
				gameInterface = new CGLInterface ();
				versionText = new CGLText ("Version : " + GlobalContent.Version.ToString (), 60, Font.Tahoma, new Point (0, 120), new Color ("#00CCCC", 1.0f));
				fpsText = new CGLText ("fps", 20, Font.Tahoma, new Point (0, 20), Color.Black);
				CGLText.CGLTextContainer.RequestForeground (versionText);
			}

			#endregion

			private static int lastTick;
			private static int lastFrameRate;
			private static int frameRate;

			public int CalculateFrameRate ()
			{
				if (System.Environment.TickCount - lastTick >= 1000) {
					lastFrameRate = frameRate;
					fpsText.Text = "fps = " + lastFrameRate.ToString ();
					frameRate = 0;
					lastTick = System.Environment.TickCount;
				}
				frameRate++;
				return lastFrameRate;
			}

		}
	}
}