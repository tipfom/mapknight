using System;

using Android.Opengl;
using GL = Android.Opengl.GLES20;

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

				string log = GL.GlGetShaderInfoLog (shader);

				return shader;
			}
		}
	}
}

