using System;
using System.Collections.Generic;

using Android.Opengl;
using GL = Android.Opengl.GLES20;

using Java.Nio;

using GC = mapKnight_Android.GlobalContent;

namespace mapKnight_Android
{
	namespace CGL
	{		
		public class CGLMap : Map
		{
			FloatBuffer VertexBuffer;
			IntBuffer IndexBuffer;
			FloatBuffer TextureBuffer;

			FloatBuffer fboVertexBuffer;
			ShortBuffer fboIndexBuffer;
			FloatBuffer fboTextureBuffer;

			// Shader
			private int fboVertexShader;
			private static string fboVertexShaderCode =
				"uniform mat4 uMVPMatrix;" +
				"attribute vec4 vPosition;" +
				"attribute vec2 a_TexCoordinate;" +
				"varying vec2 v_TexCoordinate;" +

				"void main()" +
				"{" +
				"  v_TexCoordinate = a_TexCoordinate;" +
				"  gl_Position = uMVPMatrix * vPosition;" +
				"}";
			
			private int VertexShader;
			private static string VertexShaderCode =
				"attribute vec4 vPosition;" +
				"attribute vec2 a_TexCoordinate;" +
				"varying vec2 v_TexCoordinate;" +

				"void main()" +
				"{" +
				"  v_TexCoordinate = a_TexCoordinate;" +
				"  gl_Position = vPosition;" +
				"}";

			private int FragmentShader;
			private static string FragmentShaderCode =
				"precision mediump float;" +
				"uniform sampler2D u_Texture;" +
				"varying vec2 v_TexCoordinate; " +
				""+
				"void main()" +
				"{" +
				"  gl_FragColor = texture2D(u_Texture, v_TexCoordinate);" +
				"}";
			
			private int fboRenderProgram;
			private int RenderProgram;

			private int fboVertexCount;
			private int VertexStride;

			private static int CoordsPerVertex = 3;
			private float[] VertexCoords;

			private static float[] TextureCoords = {
				0f, 1f,
				0f, 0f,
				1f, 0f,
				1f, 1f
			};

			private int[] VertexIndices = {
				0,
				1,
				2,
				0,
				2,
				3
			};

			private int framebufferID;
			private int framebufferTexture;
			private int renderbufferID;

			Dictionary<int, float[]> LineTileTextureCoords;
			Dictionary<int, float[]> LineOvrlTextureCoords;

			public CGLMap(int mapDrawWidth, float screenRatio, Utils.XMLElemental MapXML) : base(MapXML){
				int tileSizeInPXL = GlobalContent.TileSize;

				int mapDrawHeight = (int)((float)mapDrawWidth / screenRatio) + 1;
				float VertexSize = 2 / (float)(mapDrawWidth);

				fboVertexShader = CGLTools.LoadShader (GL.GlVertexShader, fboVertexShaderCode);
				VertexShader = CGLTools.LoadShader (GL.GlVertexShader, VertexShaderCode);
				FragmentShader = CGLTools.LoadShader (GL.GlFragmentShader, FragmentShaderCode);

				fboRenderProgram = GL.GlCreateProgram ();
				GL.GlAttachShader (fboRenderProgram, fboVertexShader);
				GL.GlAttachShader (fboRenderProgram, FragmentShader);
				GL.GlLinkProgram (fboRenderProgram);

				RenderProgram = GL.GlCreateProgram ();
				GL.GlAttachShader (RenderProgram, VertexShader);
				GL.GlAttachShader (RenderProgram, FragmentShader);
				GL.GlLinkProgram (RenderProgram);

				VertexCoords = new float[12];

				VertexCoords [0] = -screenRatio;
				VertexCoords [1] =  1f ;

				VertexCoords [3] = -screenRatio;
				VertexCoords [4] = -1f ;

				VertexCoords [6] = screenRatio;
				VertexCoords [7] = -1f ;

				VertexCoords [9] = screenRatio;
				VertexCoords [10] = 1f ;

				TextureCoords = new float[8];
				TextureCoords [0] = 0;
				TextureCoords [1] = 1 / screenRatio;
				TextureCoords [2] = 0;
				TextureCoords [3] = 0;
				TextureCoords [4] = 1;
				TextureCoords [5] = 0;
				TextureCoords [6] = 1;
				TextureCoords [7] = 1 / screenRatio;

				ByteBuffer bytebuffer = ByteBuffer.AllocateDirect (VertexCoords.Length * sizeof(float));
				bytebuffer.Order (ByteOrder.NativeOrder());
				fboVertexBuffer = bytebuffer.AsFloatBuffer ();
				fboVertexBuffer.Put (VertexCoords);
				fboVertexBuffer.Position (0);

				bytebuffer = ByteBuffer.AllocateDirect(VertexIndices.Length * sizeof(short));
				bytebuffer.Order (ByteOrder.NativeOrder ());
				fboIndexBuffer = bytebuffer.AsShortBuffer ();
				short[] fboVertexIndices = { 0, 1, 2, 0, 2, 3 };
				fboIndexBuffer.Put (fboVertexIndices);
				fboIndexBuffer.Position (0);

				bytebuffer = ByteBuffer.AllocateDirect (TextureCoords.Length * sizeof(float));
				bytebuffer.Order (ByteOrder.NativeOrder ());
				fboTextureBuffer = bytebuffer.AsFloatBuffer ();
				fboTextureBuffer.Put (TextureCoords);
				fboTextureBuffer.Position (0);

				fboVertexCount = VertexCoords.Length / CoordsPerVertex;

				// init FrameBuffer
				int[] temp = new int[1];
				GL.GlGenFramebuffers (1, temp , 0);
				framebufferID = temp [0];

				GL.GlGenTextures (1, temp, 0);
				framebufferTexture = temp [0];

				GL.GlGenRenderbuffers (1, temp, 0);
				renderbufferID = temp [0];

				GL.GlBindTexture (GL.GlTexture2d, framebufferTexture);
				GL.GlTexImage2D (GL.GlTexture2d, 0, GL.GlRgba, tileSizeInPXL * mapDrawWidth, tileSizeInPXL *mapDrawHeight , 0, GL.GlRgba, GL.GlUnsignedByte, null);

				GL.GlTexParameteri (GL.GlTexture2d, GL.GlTextureMinFilter, GL.GlNearest);
				GL.GlTexParameteri (GL.GlTexture2d, GL.GlTextureMagFilter, GL.GlNearest);
				GL.GlTexParameteri(GL.GlTexture2d, GL.GlTextureWrapS, GL.GlClampToEdge);
				GL.GlTexParameteri(GL.GlTexture2d, GL.GlTextureWrapT, GL.GlClampToEdge);

				GL.GlBindRenderbuffer (GL.GlRenderbuffer, renderbufferID);
				GL.GlRenderbufferStorage (GL.GlRenderbuffer, GL.GlDepthComponent16, tileSizeInPXL * mapDrawWidth , tileSizeInPXL *mapDrawHeight);

				GL.GlBindFramebuffer (GL.GlFramebuffer,framebufferID);

				GL.GlFramebufferTexture2D (GL.GlFramebuffer, GL.GlColorAttachment0, GL.GlTexture2d, framebufferTexture, 0);

				// reset
				GL.GlBindTexture(GL.GlTexture2d, 0);
				GL.GlBindRenderbuffer(GL.GlRenderbuffer, 0);
				GL.GlBindFramebuffer(GL.GlFramebuffer, 0);

				// draw to fbo
				VertexCoords = new float[mapDrawWidth * mapDrawHeight * 12];
				VertexIndices = new int[mapDrawWidth * mapDrawHeight * 6];
				TextureCoords = new float[mapDrawWidth * mapDrawHeight * 8];

				for (int x = 0; x < mapDrawWidth; x++) {
					for (int y = 0; y < mapDrawHeight; y++) {
						
						VertexCoords [x * mapDrawHeight * 12 + y * 12 +  0] = -1f + (x * VertexSize);
						VertexCoords [x * mapDrawHeight * 12 + y * 12 +  1] = -1f + y * VertexSize;
						VertexCoords [x * mapDrawHeight * 12 + y * 12 +  2] = 0f;
						VertexCoords [x * mapDrawHeight * 12 + y * 12 +  3] = -1f + (x * VertexSize);
						VertexCoords [x * mapDrawHeight * 12 + y * 12 +  4] = -1f + y * VertexSize + VertexSize;
						VertexCoords [x * mapDrawHeight * 12 + y * 12 +  5] = 0f;
						VertexCoords [x * mapDrawHeight * 12 + y * 12 +  6] = -1f + (x * VertexSize) + VertexSize;
						VertexCoords [x * mapDrawHeight * 12 + y * 12 +  7] = -1f + y * VertexSize + VertexSize;
						VertexCoords [x * mapDrawHeight * 12 + y * 12 +  8] = 0f;
						VertexCoords [x * mapDrawHeight * 12 + y * 12 +  9] = -1f + (x * VertexSize) + VertexSize;
						VertexCoords [x * mapDrawHeight * 12 + y * 12 + 10] = -1f + y * VertexSize;
						VertexCoords [x * mapDrawHeight * 12 + y * 12 + 11] = 0f;

						VertexIndices [x * mapDrawHeight * 6 + y * 6 + 0] = x * mapDrawHeight * 4 + y * 4 + 0;
						VertexIndices [x * mapDrawHeight * 6 + y * 6 + 1] = x * mapDrawHeight * 4 + y * 4 + 1;
						VertexIndices [x * mapDrawHeight * 6 + y * 6 + 2] = x * mapDrawHeight * 4 + y * 4 + 2;
						VertexIndices [x * mapDrawHeight * 6 + y * 6 + 3] = x * mapDrawHeight * 4 + y * 4 + 0;
						VertexIndices [x * mapDrawHeight * 6 + y * 6 + 4] = x * mapDrawHeight * 4 + y * 4 + 2;
						VertexIndices [x * mapDrawHeight * 6 + y * 6 + 5] = x * mapDrawHeight * 4 + y * 4 + 3;

						TextureCoords [x * mapDrawHeight * 8 + y * 8 + 0] = 0f;
						TextureCoords [x * mapDrawHeight * 8 + y * 8 + 1] = 1f;
						TextureCoords [x * mapDrawHeight * 8 + y * 8 + 2] = 0f;
						TextureCoords [x * mapDrawHeight * 8 + y * 8 + 3] = 0f;
						TextureCoords [x * mapDrawHeight * 8 + y * 8 + 4] = 1f;
						TextureCoords [x * mapDrawHeight * 8 + y * 8 + 5] = 0f;
						TextureCoords [x * mapDrawHeight * 8 + y * 8 + 6] = 1f;
						TextureCoords [x * mapDrawHeight * 8 + y * 8 + 7] = 1f;

					}
				}

				VertexStride = CoordsPerVertex * sizeof(float);

				bytebuffer = ByteBuffer.AllocateDirect (VertexCoords.Length * sizeof(float));
				bytebuffer.Order (ByteOrder.NativeOrder());
				VertexBuffer = bytebuffer.AsFloatBuffer ();
				VertexBuffer.Put (VertexCoords);
				VertexBuffer.Position (0);

				bytebuffer = ByteBuffer.AllocateDirect(VertexIndices.Length * sizeof(int));
				bytebuffer.Order (ByteOrder.NativeOrder ());
				IndexBuffer = bytebuffer.AsIntBuffer ();
				IndexBuffer.Put (VertexIndices);
				IndexBuffer.Position (0);

				bytebuffer = ByteBuffer.AllocateDirect (TextureCoords.Length * sizeof(float));
				bytebuffer.Order (ByteOrder.NativeOrder ());
				TextureBuffer = bytebuffer.AsFloatBuffer ();
				TextureBuffer.Put (TextureCoords);
				TextureBuffer.Position (0);

				GL.GlBindFramebuffer(GL.GlFramebuffer,framebufferID);
				GL.GlViewport (0, 0, tileSizeInPXL * mapDrawWidth, tileSizeInPXL * mapDrawHeight);
				GL.GlClearColor (1.0f, 1.0f, 1.0f, 1.0f);
				GL.GlClear (GL.GlColorBufferBit | GL.GlDepthBufferBit);

				GL.GlUseProgram (RenderProgram);

				GL.GlActiveTexture (GL.GlTexture0);
				GL.GlBindTexture (GL.GlTexture2d, GlobalContent.MainTexture);

				int PositionHandle = GL.GlGetAttribLocation (RenderProgram, "vPosition");
				GL.GlEnableVertexAttribArray (PositionHandle);
				GL.GlVertexAttribPointer (PositionHandle, CoordsPerVertex, GL.GlFloat, false, VertexStride, VertexBuffer);

				GL.GlEnable(GL.GlBlend);
				GL.GlBlendFunc(GL.GlSrcAlpha, GL.GlOneMinusSrcAlpha);

				int mTextureUniformHandle = GL.GlGetUniformLocation(RenderProgram, "u_Texture");
				GL.GlUniform1i(mTextureUniformHandle, 0);

				int mTextureCoordinateHandle = GL.GlGetAttribLocation(RenderProgram, "a_TexCoordinate");
				GL.GlVertexAttribPointer (mTextureCoordinateHandle, 2, GL.GlFloat, false,0, TextureBuffer);
				GL.GlEnableVertexAttribArray (mTextureCoordinateHandle);

				GL.GlDrawElements (GL.GlTriangles, VertexIndices.Length, GL.GlUnsignedInt, IndexBuffer);
				GL.GlDisableVertexAttribArray (PositionHandle);
				GL.GlDisableVertexAttribArray (mTextureCoordinateHandle);

				// end draw
				GL.GlBindFramebuffer(GL.GlFramebuffer, 0);
			}

			private void initTextureCoords()
			{
				LineTileTextureCoords = new Dictionary<int, float[]> ();
				LineOvrlTextureCoords = new Dictionary<int, float[]> ();

				for (int y = 0; y < Height; y++) {
					float[] BufferedTileTexCoords = new float[Width * 8];
					float[] BufferedOvrlTexCoords = new float[Width * 8];

					for (int x = 0; x < Width; x++) {
						for (int c = 0; c < 8; c++) {
							BufferedTileTexCoords [x * 8 + c] = GC.TileTexCoordManager [(short)GC.Map.GetTile (x, y)] [c];
							BufferedOvrlTexCoords [x * 8 + c] = GC.OverlayTexCoordManager [(short)GC.Map.GetOverlay (x, y)] [c];
						}
					}

					LineTileTextureCoords.Add (y, BufferedTileTexCoords);
					LineOvrlTextureCoords.Add (y, BufferedOvrlTexCoords);
				}
			}

			public void Render(float[] _mvpMatrix){
				GL.GlUseProgram (fboRenderProgram);

				// Set the active texture unit to texture unit 0.

				int PositionHandle = GL.GlGetAttribLocation (fboRenderProgram, "vPosition");
				GL.GlEnableVertexAttribArray (PositionHandle);
				GL.GlVertexAttribPointer (PositionHandle, CoordsPerVertex, GL.GlFloat, false, VertexStride, fboVertexBuffer);

				int MVPMatrixHandle = GL.GlGetUniformLocation (fboRenderProgram, "uMVPMatrix");
				GL.GlUniformMatrix4fv (MVPMatrixHandle, 1, false, _mvpMatrix, 0);

				GL.GlEnable(GL.GlBlend);
				GL.GlBlendFunc(GL.GlSrcAlpha, GL.GlOneMinusSrcAlpha);

				int mTextureUniformHandle = GL.GlGetUniformLocation(fboRenderProgram, "u_Texture");
				int mTextureCoordinateHandle = GL.GlGetAttribLocation(fboRenderProgram, "a_TexCoordinate");
				GL.GlVertexAttribPointer (mTextureCoordinateHandle, 2, GL.GlFloat, false, 0, fboTextureBuffer);
				GL.GlEnableVertexAttribArray (mTextureCoordinateHandle);

				GL.GlActiveTexture(GL.GlTexture0);
				GL.GlBindTexture(GL.GlTexture2d, framebufferTexture);
				GL.GlUniform1i(mTextureUniformHandle, 0);

				GL.GlDrawElements (GL.GlTriangles, 6, GL.GlUnsignedShort, fboIndexBuffer);
				GL.GlDisableVertexAttribArray (PositionHandle);
				GL.GlDisableVertexAttribArray (mTextureCoordinateHandle);
			}
		}
	}
}