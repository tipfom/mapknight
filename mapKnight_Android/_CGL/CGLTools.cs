using System;
using System.Collections.Generic;
using System.IO;

using Android.Opengl;
using Android.Content;
using Android.Graphics;
using GL = Android.Opengl.GLES20;

using mapKnight.Values;

namespace mapKnight.Android.CGL
{
	public static class CGLTools
	{
		public static int LoadShader (int type, string code)
		{
			int shader = GL.GlCreateShader (type);

			GL.GlShaderSource (shader, code);
			GL.GlCompileShader (shader);

			Log.All (typeof(CGLTools), "Loaded new shader (id = " + shader.ToString () + ")", MessageType.Info);
			Log.All (typeof(CGLTools), "Log = " + GL.GlGetShaderInfoLog (shader), MessageType.Info);

			return shader;
		}

		public static int LoadProgram (params int[] Shader)
		{
			int program = GL.GlCreateProgram ();

			foreach (int shader in Shader) {
				GL.GlAttachShader (program, shader);
			}

			GL.GlLinkProgram (program);

			Log.All (typeof(CGLTools), "Loaded new program (id = " + program.ToString () + ")", MessageType.Info);
			Log.All (typeof(CGLTools), "Log = " + GL.GlGetProgramInfoLog (program), MessageType.Info);

			return program;
		}

		private static Dictionary<int[], int> loadedPrograms = new Dictionary<int[], int> (new IntArrayComparer ());

		public static int GetProgram (params int[] shader)
		{
			Array.Sort (shader);
			if (loadedPrograms.ContainsKey (shader)) {
				return loadedPrograms [shader];
			} else {
				loadedPrograms.Add (shader, LoadProgram (shader));
				return loadedPrograms [shader];
			}
		}

		public static BufferData GenerateFramebuffer (int width, int height)
		{
			BufferData bufferdata = new BufferData (){ Width = width, Height = height };

			int[] temp = new int[1];
			GL.GlGenFramebuffers (1, temp, 0);
			bufferdata.FrameBuffer = temp [0];

			GL.GlGenTextures (1, temp, 0);
			bufferdata.FrameBufferTexture = temp [0];

			GL.GlGenRenderbuffers (1, temp, 0);
			bufferdata.RenderBuffer = temp [0];

			GL.GlBindTexture (GL.GlTexture2d, bufferdata.FrameBufferTexture);
			GL.GlTexImage2D (GL.GlTexture2d, 0, GL.GlRgba, width, height, 0, GL.GlRgba, GL.GlUnsignedByte, null);

			GL.GlTexParameteri (GL.GlTexture2d, GL.GlTextureMinFilter, GL.GlNearest);
			GL.GlTexParameteri (GL.GlTexture2d, GL.GlTextureMagFilter, GL.GlNearest);
			GL.GlTexParameteri (GL.GlTexture2d, GL.GlTextureWrapS, GL.GlClampToEdge);
			GL.GlTexParameteri (GL.GlTexture2d, GL.GlTextureWrapT, GL.GlClampToEdge);

			GL.GlBindRenderbuffer (GL.GlRenderbuffer, bufferdata.RenderBuffer);
			GL.GlRenderbufferStorage (GL.GlRenderbuffer, GL.GlDepthComponent16, width, height);

			GL.GlBindFramebuffer (GL.GlFramebuffer, bufferdata.FrameBuffer);

			GL.GlFramebufferTexture2D (GL.GlFramebuffer, GL.GlColorAttachment0, GL.GlTexture2d, bufferdata.FrameBufferTexture, 0);

			// reset
			GL.GlBindTexture (GL.GlTexture2d, 0);
			GL.GlBindRenderbuffer (GL.GlRenderbuffer, 0);
			GL.GlBindFramebuffer (GL.GlFramebuffer, 0);

			return bufferdata;
		}

		public static BufferData GenerateFramebuffer ()
		{
			return GenerateFramebuffer (Content.ScreenSize);
		}

		public static BufferData GenerateFramebuffer (Size size)
		{
			return GenerateFramebuffer (size.Width, size.Height);
		}

		public static void DeleteTexture (int texture)
		{
			GL.GlDeleteTextures (1, new int[]{ texture }, 0);
		}

		public static void DeleteFrameBuffer (int framebuffer)
		{
			GL.GlDeleteFramebuffers (1, new int[]{ framebuffer }, 0);
		}

		public static void DeleteRenderBuffer (int renderbuffer)
		{
			GL.GlDeleteRenderbuffers (1, new int[]{ renderbuffer }, 0);
		}

		public static void DeleteBufferData (BufferData bufferdata)
		{
			DeleteFrameBuffer (bufferdata.FrameBuffer);
			DeleteRenderBuffer (bufferdata.RenderBuffer);
			DeleteTexture (bufferdata.FrameBufferTexture);
		}

		public static BufferData UpdateFramebuffer (BufferData oldbufferdata)
		{
			DeleteBufferData (oldbufferdata);
			return GenerateFramebuffer (oldbufferdata.Width, oldbufferdata.Height);
		}

		public static LoadedImage LoadImage (Context context, string assetpath)
		{
			int[] loadedtexture = new int[1];
			GL.GlGenTextures (1, loadedtexture, 0);

			BitmapFactory.Options bfoptions = new BitmapFactory.Options ();
			bfoptions.InScaled = false;
			Bitmap bitmap = BitmapFactory.DecodeStream (context.Assets.Open (assetpath), null, bfoptions);

			GL.GlBindTexture (GL.GlTexture2d, loadedtexture [0]);

			GL.GlTexParameteri (GL.GlTexture2d, GL.GlTextureMinFilter, GL.GlNearest);
			GL.GlTexParameteri (GL.GlTexture2d, GL.GlTextureMagFilter, GL.GlNearest);
			GL.GlTexParameteri (GL.GlTexture2d, GL.GlTextureWrapS, GL.GlClampToEdge);
			GL.GlTexParameteri (GL.GlTexture2d, GL.GlTextureWrapT, GL.GlClampToEdge);

			GLUtils.TexImage2D (GL.GlTexture2d, 0, bitmap, 0);

			LoadedImage limage = new LoadedImage (loadedtexture [0], bitmap.Width, bitmap.Height);

			bitmap.Recycle ();
			GL.GlBindTexture (GL.GlTexture2d, 0);

			// Error Check
			int error = GL.GlGetError ();
			if (error != 0) {
				Log.All (typeof(Content), "error while loading mainimage (errorcode => " + error.ToString () + ")", MessageType.Debug);
				throw new FileLoadException ("error while loading mainimage (errorcode => " + error.ToString () + ")");
			}
			if (loadedtexture [0] == 0) {
				Log.All (typeof(Content), "loaded mainimage is zero", MessageType.Debug);
				throw new FileLoadException ("loaded mainimage is zero");
			}

			return limage;
		}

		public struct LoadedImage
		{
			public int Texture;
			public int Width;
			public int Height;

			public LoadedImage (int texture, int width, int height) : this ()
			{
				this.Texture = texture;
				this.Width = width;
				this.Height = height;
			}
		}

		public static fRectangle ParseCoordinates (Dictionary<string,string> config, Size imageSize)
		{
			// konvertiert die geradezahligen koordination von sprites in opengl koordinaten
			return (new Rectangle (new fPoint (Convert.ToInt32 (config ["x"]), Convert.ToInt32 (config ["y"])), new fSize (Convert.ToInt32 (config ["width"]), Convert.ToInt32 (config ["height"]))) / imageSize);
		}
	}
}
