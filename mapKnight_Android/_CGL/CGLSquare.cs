using System;

using Android.Opengl;
using GL = Android.Opengl.GLES20;

using Java.Nio;

namespace mapKnight_Android{
	namespace CGL
	{
		public struct CGLSquare
		{
			private readonly ShortBuffer IndexBuffer;
			private readonly FloatBuffer VertexBuffer;
			private readonly FloatBuffer TextureBuffer;

			// Shader
			private int VertexShader;
			private static string VertexShaderCode =
				"uniform mat4 uMVPMatrix;" +
				"attribute vec4 vPosition;" +
				"attribute vec2 a_TexCoordinate;" +
				"varying vec2 v_TexCoordinate;" +

				"void main()" +
				"{" +
				"  v_TexCoordinate = a_TexCoordinate;" +
				"  gl_Position = uMVPMatrix * vPosition;" +
				"}";

			private int FragmentShader;
			private static string FragmentShaderCode =
				"precision mediump float;" +
				"uniform vec4 vColor;" +
				"uniform sampler2D u_Texture;" +
				"varying vec2 v_TexCoordinate; " +
				""+
				"void main()" +
				"{" +
				"  gl_FragColor = texture2D(u_Texture, v_TexCoordinate);" +
				"}";

			private int RenderProgram;

			private int VertexCount;
			private int VertexStride;

			public float[] RotationMatrix;

			private static int CoordsPerVertex = 3;
			private static float[] VertexCoords = { 	
										// entgegen den Uhrzeigersinn
				-0.09f,  0.09f, 0.0f,   	// oben links
				-0.09f, -0.09f, 0.0f,   	// unten linsks
				 0.09f, -0.09f, 0.0f,   	// unten rechts
				 0.09f,  0.09f, 0.0f  	// oben rechts
			};

			private static float[] TextureCoords = {
				0f, 1f,
				0f, 0f,
				1f, 0f,
				1f, 1f
			};

			private static short[] VertexIndices = {
				0,
				1,
				2,
				0,
				2,
				3
			};
			private static float[] Color = {
				0.0f, // rot
				1.0f, // grün
				1.0f, // blau
				1.0f  // opacity 
			};

			public CGLSquare(int texture){
				//VertexBuffer init
				ByteBuffer bytebuffer = ByteBuffer.AllocateDirect(VertexCoords.Length * sizeof(float));
				bytebuffer.Order (ByteOrder.NativeOrder ());
				VertexBuffer = bytebuffer.AsFloatBuffer ();
				VertexBuffer.Put (VertexCoords);
				VertexBuffer.Position (0);

				//IndexBuffer init
				bytebuffer = ByteBuffer.AllocateDirect(VertexIndices.Length * sizeof(short));
				bytebuffer.Order (ByteOrder.NativeOrder ());
				IndexBuffer = bytebuffer.AsShortBuffer ();
				IndexBuffer.Put (VertexIndices);
				IndexBuffer.Position (0);

				VertexShader = LoadShader (GL.GlVertexShader, VertexShaderCode);
				FragmentShader = LoadShader (GL.GlFragmentShader, FragmentShaderCode);

				RenderProgram = GL.GlCreateProgram ();
				GL.GlAttachShader (RenderProgram, VertexShader);
				GL.GlAttachShader (RenderProgram, FragmentShader);
				GL.GlLinkProgram (RenderProgram);
				string log = GL.GlGetProgramInfoLog (RenderProgram);

				VertexCount = VertexCoords.Length / CoordsPerVertex;
				VertexStride = CoordsPerVertex * sizeof(float);

				bytebuffer = ByteBuffer.AllocateDirect (TextureCoords.Length * sizeof(float));
				bytebuffer.Order (ByteOrder.NativeOrder ());
				TextureBuffer = bytebuffer.AsFloatBuffer ();
				TextureBuffer.Put (TextureCoords);
				TextureBuffer.Position (0);

				RotationMatrix = new float[16];
				Rotation = 0;
			}

			public void Render(float[] mvpMatrix, int texture){
				GL.GlUseProgram (RenderProgram);

				// Set the active texture unit to texture unit 0.
				GL.GlActiveTexture(GL.GlTexture0);
				// Bind the texture to this unit.
				GL.GlBindTexture(GL.GlTexture2d, texture);

				int PositionHandle = GL.GlGetAttribLocation (RenderProgram, "vPosition");
				GL.GlEnableVertexAttribArray (PositionHandle);
				GL.GlVertexAttribPointer (PositionHandle, CoordsPerVertex, GL.GlFloat, false, VertexStride, VertexBuffer);

	//			int mColorHandle = GL.GlGetUniformLocation(RenderProgram, "vColor");
	//			GL.GlUniform4fv (mColorHandle, 1, Color, 0);

				int MVPMatrixHandle = GL.GlGetUniformLocation (RenderProgram, "uMVPMatrix");
				float[] CombinedMatrix = new float[16];
				Matrix.MultiplyMM (CombinedMatrix, 0, mvpMatrix, 0, RotationMatrix, 0);
				GL.GlUniformMatrix4fv (MVPMatrixHandle, 1, false, CombinedMatrix, 0);
				
				GL.GlEnable(GL.GlBlend);
				GL.GlBlendFunc(GL.GlSrcAlpha, GL.GlOneMinusSrcAlpha);

				int mTextureUniformHandle = GL.GlGetUniformLocation(RenderProgram, "u_Texture");
				int mTextureCoordinateHandle = GL.GlGetAttribLocation(RenderProgram, "a_TexCoordinate");
				GL.GlVertexAttribPointer (mTextureCoordinateHandle, 2, GL.GlFloat, false, 0, TextureBuffer);
				GL.GlEnableVertexAttribArray (mTextureCoordinateHandle);
				//GL.GlEnableVertexAttribArray (mTextureCoordinateHandle);
				// Tell the texture uniform sampler to use this texture in the shader by binding to texture unit 0.
				GL.GlUniform1i(mTextureUniformHandle, 0);

				GL.GlDrawElements (GL.GlTriangles, VertexIndices.Length, GL.GlUnsignedShort, IndexBuffer);
				GL.GlDisableVertexAttribArray (PositionHandle);
				GL.GlDisableVertexAttribArray (mTextureCoordinateHandle);
			}

			public int Rotation { 
				get { return 0; } 
				set { Matrix.SetRotateM (RotationMatrix, 0, value, 0, 0, -1.0f); }
			}

			private static int LoadShader(int type, string code){
				int shader = GL.GlCreateShader (type);

				GL.GlShaderSource (shader, code);
				GL.GlCompileShader (shader);

				string log = GLES30.GlGetShaderInfoLog (shader);

				return shader;
			}
		}
	}
}