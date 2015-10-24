using System;

using Android.Opengl;
using GL = Android.Opengl.GLES20;

using mapKnight_Android.Utils;

namespace mapKnight_Android
{
	namespace CGL
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

				Log.All (typeof(CGLTools), "Loaded new shader (id = " + program.ToString () + ")", MessageType.Info);
				Log.All (typeof(CGLTools), "Log = " + GL.GlGetProgramInfoLog (program), MessageType.Info);

				return program;
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
				return GenerateFramebuffer (GlobalContent.ScreenSize);
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
		}
	}
}

