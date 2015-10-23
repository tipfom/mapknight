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
			public static int LoadShader(int type, string code){
				int shader = GL.GlCreateShader (type);

				GL.GlShaderSource (shader, code);
				GL.GlCompileShader (shader);

				Log.All (typeof(CGLTools), "Loaded new shader (id = " + shader.ToString () + ")", MessageType.Info);
				Log.All (typeof(CGLTools), "Log = " +  GL.GlGetShaderInfoLog (shader), MessageType.Info);

				return shader;
			}

			public static int LoadProgram(params int[] Shader){
				int program = GL.GlCreateProgram ();

				foreach (int shader in Shader) {
					GL.GlAttachShader (program, shader);
				}

				GL.GlLinkProgram (program);

				Log.All (typeof(CGLTools), "Loaded new shader (id = " + program.ToString () + ")", MessageType.Info);
				Log.All (typeof(CGLTools), "Log = " +  GL.GlGetProgramInfoLog (program), MessageType.Info);

				return program;
			}
		}
	}
}

