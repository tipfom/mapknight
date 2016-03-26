using System;

using GL = Android.Opengl.GLES20;

namespace mapKnight.Android.CGL
{
	public static partial class CGLTools
	{
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

		private static string FragmentShaderCode =
			"precision mediump float;" +
			"uniform sampler2D u_Texture;" +
			"varying vec2 v_TexCoordinate; " +
			"" +
			"void main()" +
			"{" +
			"  gl_FragColor = texture2D(u_Texture, v_TexCoordinate);" +
			"}";

		private static string FragmentShaderLightningCode =
			"precision mediump float;" +
			"uniform sampler2D u_Texture;" +
			"attribute vec3 a_Ambient " +
			"varying vec2 v_TexCoordinate; " +
			"" +
			"void main()" +
			"{" +
			"  gl_FragColor = texture2D(u_Texture, v_TexCoordinate) * a_Ambient;" +
			"}";
	}
}
