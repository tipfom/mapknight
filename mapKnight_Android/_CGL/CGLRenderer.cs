using System;

using Android.Opengl;
using GL = Android.Opengl.GLES20;

using Android.Graphics;

using Javax.Microedition.Khronos.Opengles;

namespace mapKnight_Android{
	namespace CGL
	{
		public class CGLRenderer : Java.Lang.Object, GLSurfaceView.IRenderer 
		{
			CGLMap testsquaremap;

			private float[] mMVPMatrix = new float[16];
			private float[] mProjectionMatrix = new float[16];
			private float[] mViewMatrix = new float[16];
			float ratio;
			int[] textures;
			Android.Content.Context context;
			int testtexid;
			int screenHeight;

			public CGLRenderer (Android.Content.Context texturecontent, int testtextureid)
			{
				context = texturecontent;
				testtexid = testtextureid;
			}

			#region IRenderer implementation

			public void OnDrawFrame (IGL10 gl)
			{
				GL.GlClear (GL.GlColorBufferBit | GL.GlDepthBufferBit);

				Android.Opengl.Matrix.SetLookAtM (mViewMatrix, 0, 0, 0, -3, 0f, 0f, 0f, 0f, 1f, 0f);
				Android.Opengl.Matrix.MultiplyMM (mMVPMatrix, 0, mProjectionMatrix, 0, mViewMatrix, 0);

				testsquaremap.Render(mMVPMatrix);
				CalculateFrameRate ();
			}

			public void OnSurfaceChanged (IGL10 gl, int width, int height)
			{

				ratio = (float) width / height;
				screenHeight = height;
				Android.Opengl.Matrix.FrustumM(mProjectionMatrix, 0, -ratio, ratio, -1, 1, 3, 7);
				testsquaremap = new CGLMap (22, ratio, Utils.XMLElemental.Load(context.Assets.Open("Maps/testMap.xml")));
				GL.GlViewport (0, 0, width, height);
				GL.GlClearColor (0f, 0f, 0f, 1.0f);

			}

			public void OnSurfaceCreated (Javax.Microedition.Khronos.Opengles.IGL10 gl, Javax.Microedition.Khronos.Egl.EGLConfig config)
			{
				textures = new int[1];
				GL.GlGenTextures (1, textures, 0);
				BitmapFactory.Options bo = new BitmapFactory.Options ();
				bo.InScaled = false;
				Bitmap bitmap = BitmapFactory.DecodeResource (context.Resources, testtexid, bo);
				GL.GlBindTexture (GL.GlTexture2d, textures [0]);
				GL.GlTexParameteri (GL.GlTexture2d, GL.GlTextureMinFilter, GL.GlNearest);
				GL.GlTexParameteri (GL.GlTexture2d, GL.GlTextureMagFilter, GL.GlNearest);
				//GL.GlGenerateMipmap (GL.GlTexture2d);
				GL.GlTexParameteri(GL.GlTexture2d, GL.GlTextureWrapS, GL.GlClampToEdge);
				GL.GlTexParameteri(GL.GlTexture2d, GL.GlTextureWrapT, GL.GlClampToEdge);

				GLUtils.TexImage2D (GL.GlTexture2d, 0, bitmap, 0);
				int error = GL.GlGetError ();
				bitmap.Recycle ();
				GL.GlBindTexture (GL.GlTexture2d, 0);
				if (textures [0] == 0)
					throw new ArgumentNullException ("loaded texture is 0");

				GL.GlClearColor (1f, 0f, 1f, 1.0f);
			}

			#endregion

			private static int lastTick;
			private static int lastFrameRate;
			private static int frameRate;

			public static int CalculateFrameRate()
			{
				if (System.Environment.TickCount - lastTick >= 1000)
				{
					lastFrameRate = frameRate;
					Android.Util.Log.Debug ("fps", lastFrameRate.ToString());
					frameRate = 0;
					lastTick = System.Environment.TickCount;
				}
				frameRate++;
				return lastFrameRate;
			}

		}
	}
}