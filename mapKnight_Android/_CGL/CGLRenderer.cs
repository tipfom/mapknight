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

			private float[] mMVPMatrix = new float[16];
			private float[] mProjectionMatrix = new float[16];
			private float[] mViewMatrix = new float[16];
			float ratio;
			int screenHeight;
			XMLElemental mapElemental;
			CGLInterface hallo;
			Android.Content.Context context;
			CGLText versionText;

			public CGLRenderer (Android.Content.Context Context)
			{
				context = Context;
			}

			#region IRenderer implementation

			public void OnDrawFrame (IGL10 gl)
			{
				GL.GlClear (GL.GlColorBufferBit | GL.GlDepthBufferBit);

				Android.Opengl.Matrix.SetLookAtM (mViewMatrix, 0, 0, 0, -3, 0f, 0f, 0f, 0f, 1f, 0f);
				Android.Opengl.Matrix.TranslateM (mViewMatrix, 0, 0f, 0f, 0f);
				Android.Opengl.Matrix.MultiplyMM (mMVPMatrix, 0, mProjectionMatrix, 0, mViewMatrix, 0);

				testsquaremap.Draw (mMVPMatrix);
				hallo.Draw (mMVPMatrix);
				CGLText.CGLTextContainer.Draw (mMVPMatrix);
				CalculateFrameRate ();
			}

			public void OnSurfaceChanged (IGL10 gl, int width, int height)
			{
				ratio = (float)width / height;
				screenHeight = height;
				Android.Opengl.Matrix.FrustumM (mProjectionMatrix, 0, -ratio, ratio, -1, 1, 3, 7);
				GL.GlViewport (0, 0, width, height);
				testsquaremap = new CGLMap (22, ratio, mapElemental, new Size (width, height));
				GL.GlViewport (0, 0, width, height);
				GL.GlClearColor (0f, 0f, 0f, 1.0f);

				GlobalContent.Update (new Size (width, height));
			}

			public void OnSurfaceCreated (Javax.Microedition.Khronos.Opengles.IGL10 gl, Javax.Microedition.Khronos.Egl.EGLConfig config)
			{
				GL.GlClearColor (1f, 0f, 1f, 1.0f);

				GlobalContent.OnInitCompleted += (Android.Content.Context GameContext) => {
					mapElemental = XMLElemental.Load (GameContext.Assets.Open ("maps/testMap.xml"));
					CGLText test = new CGLText ("hallo", 50, Font.Tahoma);//, new Point (1920, 1080), new Color ("#1053FF", 1.0f));
					test.Position = new Point (1920 - test.Width, 1080);
					test.Color = new Color ("#1053FF", 1.0f);
					test.FontStyle = FontStyle.Italic;
					CGLText test2 = new CGLText ("hallo welt, wie gehts?", 120, Font.ArcadeClassic, new Point (200, 200), Color.White);
					CGLText newtext = new CGLText ("mein popo kann schreiben", 90, Font.ArcadeDotted);
					newtext.Position = new Point (1920 - newtext.Width, GlobalContent.ScreenSize.Height);
					newtext.Color = Color.White;
					hallo = new CGLInterface ();
					versionText = new CGLText ("Version : " + GlobalContent.Version.ToString (), 40, Font.Tahoma, new Point (0, 40), new Color ("#00CCCC", 1.0f));
				};
				GlobalContent.Init (Utils.XMLElemental.Load (context.Assets.Open ("main.xml"), false), context);
			}

			#endregion

			private static int lastTick;
			private static int lastFrameRate;
			private static int frameRate;

			public static int CalculateFrameRate ()
			{
				if (System.Environment.TickCount - lastTick >= 1000) {
					lastFrameRate = frameRate;
					Android.Util.Log.Debug ("fps", lastFrameRate.ToString ());
					frameRate = 0;
					lastTick = System.Environment.TickCount;
				}
				frameRate++;
				return lastFrameRate;
			}

		}
	}
}