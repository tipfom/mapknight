using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Android.Opengl;
using Android.Content;
using Android.Graphics;
using GL = Android.Opengl.GLES20;

using Java.Nio;

using mapKnight.Basic;

namespace mapKnight.Android.CGL
{
	public static partial class CGLTools
	{
		private static int LoadShader (int type, string code)
		{
			int shader = GL.GlCreateShader (type);

			GL.GlShaderSource (shader, code);
			GL.GlCompileShader (shader);

			Log.All (typeof(CGLTools), "Loaded new shader (id = " + shader.ToString () + ")", MessageType.Info);
			Log.All (typeof(CGLTools), "Log = " + GL.GlGetShaderInfoLog (shader), MessageType.Info);

			return shader;
		}

		[Obsolete ("Using this will increase loadtime very much. Use CGLTools.GetProgram instead. :)")]
		public static int LoadProgram (params Shader[] Shader)
		{
			int program = GL.GlCreateProgram ();

			foreach (Shader shader in Shader) {
				GL.GlAttachShader (program, loadedShader [shader]);
			}

			GL.GlLinkProgram (program);

			Log.All (typeof(CGLTools), "Loaded new program (id = " + program.ToString () + ")", MessageType.Info);
			Log.All (typeof(CGLTools), "Log = " + GL.GlGetProgramInfoLog (program), MessageType.Info);

			return program;
		}

		private static Dictionary<Shader[], int> loadedPrograms = new Dictionary<Shader[], int> (new ShaderArrayComparer ());

		public static int GetProgram (params Shader[] shader)
		{
			Array.Sort (shader);
			if (loadedPrograms.ContainsKey (shader)) {
				return loadedPrograms [shader];
			} else {
				#pragma warning disable 612, 618
				// disables obsolte warning
				loadedPrograms.Add (shader, LoadProgram (shader));
				return loadedPrograms [shader];
			}
		}

		private static Dictionary<Shader,int> loadedShader = new Dictionary<Shader, int> ();

		public static void LoadShader ()
		{
			loadedShader.Add (Shader.VertexShaderN, LoadShader (GL.GlVertexShader, VertexShaderNCode));
			loadedShader.Add (Shader.VertexShaderM, LoadShader (GL.GlVertexShader, VertexShaderMCode));

			loadedShader.Add (Shader.FragmentShader, LoadShader (GL.GlFragmentShader, FragmentShaderCode));
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
			return (new Rectangle (new mapKnight.Basic.Point (Convert.ToInt32 (config ["x"]), Convert.ToInt32 (config ["y"])), new Size (Convert.ToInt32 (config ["width"]), Convert.ToInt32 (config ["height"]))) / imageSize);
		}

		public static FloatBuffer CreateBuffer (float[] source)
		{
			return CreateBuffer (source, source.Length);
		}

		public static FloatBuffer CreateBuffer (float size)
		{
			ByteBuffer byteBuffer = ByteBuffer.AllocateDirect ((int)size * sizeof(float));
			byteBuffer.Order (ByteOrder.NativeOrder ());
			FloatBuffer floatBuffer = byteBuffer.AsFloatBuffer ();
			floatBuffer.Position (0);
			return floatBuffer;
		}

		public static FloatBuffer CreateBuffer (float[] source, int size)
		{
			ByteBuffer byteBuffer = ByteBuffer.AllocateDirect (size * sizeof(float));
			byteBuffer.Order (ByteOrder.NativeOrder ());
			FloatBuffer floatBuffer = byteBuffer.AsFloatBuffer ();
			floatBuffer.Put (source);
			floatBuffer.Position (0);
			return floatBuffer;
		}

		public static ShortBuffer CreateBuffer (short[] source)
		{
			return CreateBuffer (source, source.Length);
		}

		public static ShortBuffer CreateBuffer (short[] source, int size)
		{
			ByteBuffer byteBuffer = ByteBuffer.AllocateDirect (size * sizeof(short));
			byteBuffer.Order (ByteOrder.NativeOrder ());
			ShortBuffer shortBuffer = byteBuffer.AsShortBuffer ();
			shortBuffer.Put (source);
			shortBuffer.Position (0);
			return shortBuffer;
		}

		public static float[] Rotate (float[] verticies, float centerX, float centerY, float angle)
		{
			if (verticies.Length % 2 != 0)
				return null; // no verticies with format 0 = x, 1 = y available
			
			angle *= (float)Math.PI / 180f; // convert to radians

			float[] rotatedVerticies = new float[verticies.Length];
			for (int i = 0; i < verticies.Length / 2; i++) {
				rotatedVerticies [i * 2 + 0] = centerX + (verticies [i * 2 + 0] - centerX) * (float)Math.Cos (angle) - (verticies [i * 2 + 1] - centerY) * (float)Math.Sin (angle);
				rotatedVerticies [i * 2 + 1] = centerX + (verticies [i * 2 + 0] - centerX) * (float)Math.Sin (angle) + (verticies [i * 2 + 1] - centerY) * (float)Math.Cos (angle);
			}
			return rotatedVerticies;
		}

		public static float[] Translate (float[] verticies, float oldCenterX, float oldCenterY, float newCenterX, float newCenterY)
		{
			if (verticies.Length % 2 != 0)
				return null;

			float[] transformedVerticies = new float[verticies.Length];
			float shiftingX = newCenterX - oldCenterX;
			float shiftingY = newCenterY - oldCenterY;
			for (int i = 0; i < verticies.Length / 2; i++) {
				transformedVerticies [i * 2 + 0] = verticies [i * 2 + 0] - shiftingX;
				transformedVerticies [i * 2 + 1] = verticies [i * 2 + 1] - shiftingY;
			}
			return transformedVerticies;
		}

		public static float[] TranslateRotate (float[] verticies, float oldCenterX, float oldCenterY, float newCenterX, float newCenterY, float angle)
		{
			if (verticies.Length % 2 != 0)
				return null;

			angle *= (float)Math.PI / 180f; // convert to radians

			float[] transformedRotatedVerticies = new float[verticies.Length];
			for (int i = 0; i < verticies.Length / 2; i++) {
				transformedRotatedVerticies [i * 2 + 0] = newCenterX + (verticies [i * 2 + 0] - oldCenterX) * (float)Math.Cos (angle) - (verticies [i * 2 + 1] - oldCenterY) * (float)Math.Sin (angle);
				transformedRotatedVerticies [i * 2 + 1] = newCenterY + (verticies [i * 2 + 0] - oldCenterX) * (float)Math.Sin (angle) + (verticies [i * 2 + 1] - oldCenterY) * (float)Math.Cos (angle);
			}
			return transformedRotatedVerticies;
		}

		public static float[] TranslateRotateMirror (float[] verticies, float oldCenterX, float oldCenterY, float newCenterX, float newCenterY, float angle, bool mirrored)
		{
			if (verticies.Length % 2 != 0)
				return null;

			angle *= (float)Math.PI / 180f; // convert to radians

			float[] transformedRotatedVerticies = new float[verticies.Length];
			for (int i = 0; i < verticies.Length / 2; i++) {
				if (mirrored) {
					transformedRotatedVerticies [i * 2 + 0] = newCenterX - (verticies [i * 2 + 0] - oldCenterX) * (float)Math.Cos (angle) + (verticies [i * 2 + 1] - oldCenterY) * (float)Math.Sin (angle);
					transformedRotatedVerticies [i * 2 + 1] = newCenterY + (verticies [i * 2 + 0] - oldCenterX) * (float)Math.Sin (angle) + (verticies [i * 2 + 1] - oldCenterY) * (float)Math.Cos (angle);
				} else {
					transformedRotatedVerticies [i * 2 + 0] = newCenterX + (verticies [i * 2 + 0] - oldCenterX) * (float)Math.Cos (angle) - (verticies [i * 2 + 1] - oldCenterY) * (float)Math.Sin (angle);
					transformedRotatedVerticies [i * 2 + 1] = newCenterY + (verticies [i * 2 + 0] - oldCenterX) * (float)Math.Sin (angle) + (verticies [i * 2 + 1] - oldCenterY) * (float)Math.Cos (angle);
				}
			}
			return transformedRotatedVerticies;
		}

		public static float[] GetVerticies (fSize size)
		{
			return new float[] {
				-size.Width / 2f,
				size.Height / 2f,
				-size.Width / 2f,
				-size.Height / 2f,
				size.Width / 2f,
				-size.Height / 2f,
				size.Width / 2f,
				size.Height / 2f
			};
		}

		public static float Lerp (float value1, float value2, float percent)
		{
			return value1 + (value2 - value1) * percent;
		}
	}
}