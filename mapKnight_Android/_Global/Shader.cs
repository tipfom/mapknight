using System;

using GL = Android.Opengl.GLES20;

namespace mapKnight.Android
{
	public static partial class Content
	{
		public static int VertexShaderM { get; private set; }

		public static int VertexShaderN { get; private set; }

		public static int FragmentShaderN { get; private set; }

		private static string VertexShaderMCode =
			"uniform mat4 uMVPMatrix;" +
			"attribute vec4 vPosition;" +
			"attribute vec2 a_TexCoordinate;" +
			"varying vec2 v_TexCoordinate;" +

			"void main()" +
			"{" +
			"  v_TexCoordinate = a_TexCoordinate;" +
			"  gl_Position = uMVPMatrix * vPosition;" +
			"}";

		private static string VertexShaderNCode =
			"attribute vec4 vPosition;" +
			"attribute vec2 a_TexCoordinate;" +
			"varying vec2 v_TexCoordinate;" +

			"void main()" +
			"{" +
			"  v_TexCoordinate = a_TexCoordinate;" +
			"  gl_Position = vPosition;" +
			"}";

		private static string FragmentShaderNCode =
			"precision mediump float;" +
			"uniform sampler2D u_Texture;" +
			"varying vec2 v_TexCoordinate; " +
			"" +
			"void main()" +
			"{" +
			"  gl_FragColor = texture2D(u_Texture, v_TexCoordinate);" +
			"}";

		private static void LoadShader ()
		{
			VertexShaderN = CGL.CGLTools.LoadShader (GL.GlVertexShader, VertexShaderNCode);
			VertexShaderM = CGL.CGLTools.LoadShader (GL.GlVertexShader, VertexShaderMCode);

			FragmentShaderN = CGL.CGLTools.LoadShader (GL.GlFragmentShader, FragmentShaderNCode);
		}
	}
}
