using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
			private float[] lastTTextureCoors;

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

			private readonly int mapDrawWidth;
			private readonly int mapDrawHeight;
			private readonly float vertexSize;

			private int currentTileX;
			private int currentTileY;

			public CGLMap(int MapDrawWidth, float screenRatio, Utils.XMLElemental MapXML) : base(MapXML){
				int tileSizeInPXL = GlobalContent.TileSize;

				mapDrawWidth = (int)MapDrawWidth;
				mapDrawHeight = (int)((float)mapDrawWidth / screenRatio) + 1;
				vertexSize = 2 / (float)(mapDrawWidth);
				currentTileX = mapDrawWidth / 2;
				currentTileY = mapDrawHeight / 2;

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

				float[] TextureCoords = new float[8];
				TextureCoords [0] = 0;
				TextureCoords [1] = 1f;
				TextureCoords [2] = 0;
				TextureCoords [3] = 1f - mapDrawHeight * vertexSize * .5f;
				TextureCoords [4] = 1;
				TextureCoords [5] = 1f - mapDrawHeight * vertexSize * .5f;
				TextureCoords [6] = 1;
				TextureCoords [7] = 1f;

				ByteBuffer bytebuffer = ByteBuffer.AllocateDirect (VertexCoords.Length * sizeof(float));
				bytebuffer.Order (ByteOrder.NativeOrder());
				fboVertexBuffer = bytebuffer.AsFloatBuffer ();
				fboVertexBuffer.Put (VertexCoords);
				fboVertexBuffer.Position (0);

				bytebuffer = ByteBuffer.AllocateDirect(VertexIndices.Length * sizeof(short));
				bytebuffer.Order (ByteOrder.NativeOrder ());
				fboIndexBuffer = bytebuffer.AsShortBuffer ();
				fboIndexBuffer.Put (new short[] { 0, 1, 2, 0, 2, 3 });
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

				int multiplier = mapDrawWidth;

				for (int y = 0; y < mapDrawHeight; y++) {
					for (int x = 0; x < mapDrawWidth; x++) {
						
						VertexCoords [x * 12 + y * multiplier * 12 +  0] = -1f + (x * vertexSize);
						VertexCoords [x * 12 + y * multiplier * 12 +  1] = 1f - (y * vertexSize);//(.25f * mapDrawHeight * vertexSize / screenRatio) - (y * vertexSize) + vertexSize / 2;
						VertexCoords [x * 12 + y * multiplier * 12 +  2] = 0f;
						VertexCoords [x * 12 + y * multiplier * 12 +  3] = -1f + (x * vertexSize);
						VertexCoords [x * 12 + y * multiplier * 12 +  4] = 1f - ((y + 1) * vertexSize);//(.25f * mapDrawHeight * vertexSize / screenRatio) - (y * vertexSize) - vertexSize / 2;
						VertexCoords [x * 12 + y * multiplier * 12 +  5] = 0f;
						VertexCoords [x * 12 + y * multiplier * 12 +  6] = -1f + (x * vertexSize) + vertexSize;
						VertexCoords [x * 12 + y * multiplier * 12 +  7] = 1f - ((y + 1) * vertexSize);//(.25f * mapDrawHeight * vertexSize / screenRatio) - (y * vertexSize) - vertexSize / 2;
						VertexCoords [x * 12 + y * multiplier * 12 +  8] = 0f;
						VertexCoords [x * 12 + y * multiplier * 12 +  9] = -1f + (x * vertexSize) + vertexSize;
						VertexCoords [x * 12 + y * multiplier * 12 + 10] = 1f - (y * vertexSize);//(.25f * mapDrawHeight * vertexSize / screenRatio) - (y * vertexSize) + vertexSize / 2;
						VertexCoords [x * 12 + y * multiplier * 12 + 11] = 0f;

						VertexIndices [x * 6 + y * multiplier * 6 + 0] = x * 4 + y * multiplier * 4 + 0;
						VertexIndices [x * 6 + y * multiplier * 6 + 1] = x * 4 + y * multiplier * 4 + 1;
						VertexIndices [x * 6 + y * multiplier * 6 + 2] = x * 4 + y * multiplier * 4 + 2;
						VertexIndices [x * 6 + y * multiplier * 6 + 3] = x * 4 + y * multiplier * 4 + 0;
						VertexIndices [x * 6 + y * multiplier * 6 + 4] = x * 4 + y * multiplier * 4 + 2;
						VertexIndices [x * 6 + y * multiplier * 6 + 5] = x * 4 + y * multiplier * 4 + 3;

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

				initTextureCoords ();
				updateTextureBuffer (Orientation.East);

				GL.GlBindFramebuffer(GL.GlFramebuffer,framebufferID);
				GL.GlViewport (0, 0, tileSizeInPXL * mapDrawWidth, tileSizeInPXL * mapDrawHeight);
				GL.GlClearColor (1.0f, 1.0f, .0f, 1.0f);
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

				for (uint y = 0; y < Height; y++) {
					float[] BufferedTileTexCoords = new float[Width * 8];
					float[] BufferedOvrlTexCoords = new float[Width * 8];

					for (uint x = 0; x < Width; x++) {
						for (uint c = 0; c < 8; c++) {
							BufferedTileTexCoords [x * 8 + c] = GC.TileTexCoordManager [(short)GetTile (x, y)] [c];
							BufferedOvrlTexCoords [x * 8 + c] = GC.OverlayTexCoordManager [(short)GetOverlay (x, y)] [c];
						}
					}

					LineTileTextureCoords.Add ((int)y, BufferedTileTexCoords);
					LineOvrlTextureCoords.Add ((int)y, BufferedOvrlTexCoords);
				}
			}

			private bool updateTextureBuffer(Orientation updatedOrientation)
			{
				FloatBuffer TileTexBuffer;
				float[] texturecoords = new float[mapDrawWidth * mapDrawHeight * 8];
				switch (updatedOrientation) {
				case Orientation.Up:	// Just add a new line at the top
					float[] topline = AO.Cut (LineTileTextureCoords [currentTileY - mapDrawHeight / 2], (currentTileX - mapDrawWidth / 2) * 8, mapDrawWidth * 8);
					Array.Copy (topline, texturecoords, 0);
					Array.Copy (AO.Cut (lastTTextureCoors, mapDrawWidth * 8, mapDrawWidth * (mapDrawHeight - 1) * 8), 0, texturecoords, topline.Length, mapDrawWidth * (mapDrawHeight - 1) * 8 - topline.Length);
					break;
				case Orientation.Down:	// Just add a new line at the bottom
					float[] bottomline = AO.Cut (LineTileTextureCoords [currentTileY + mapDrawHeight / 2], (currentTileX - mapDrawWidth / 2) * 8, mapDrawWidth * 8);
					Array.Copy (AO.Cut (lastTTextureCoors, mapDrawWidth * 8, mapDrawWidth * (mapDrawHeight - 1) * 8), texturecoords, 0);
					Array.Copy (bottomline, 0, texturecoords, texturecoords.Length - bottomline.Length, bottomline.Length);
					break;
				case Orientation.West:	// Complete full rework
					for (int y = 0; y < mapDrawHeight; y++) {
						Array.Copy (AO.Cut (LineTileTextureCoords [currentTileY - mapDrawHeight / 2 + y], (currentTileX - mapDrawWidth / 2) * 8, mapDrawWidth * 8), 0, texturecoords, y * mapDrawWidth * 8, mapDrawWidth * 8);
					}
					break;
				case Orientation.East:
					for (int y = 0; y < mapDrawHeight; y++) {
						Array.Copy (AO.Cut (LineTileTextureCoords [currentTileY - mapDrawHeight / 2 + y], (currentTileX - mapDrawWidth / 2) * 8, mapDrawWidth * 8), 0, texturecoords, y * mapDrawWidth * 8, mapDrawWidth * 8);
					}
					break;
				}

				ByteBuffer bytebuffer = ByteBuffer.AllocateDirect (texturecoords.Length * sizeof(float));
				bytebuffer.Order (ByteOrder.NativeOrder ());
				TileTexBuffer = bytebuffer.AsFloatBuffer ();
				TileTexBuffer.Put (texturecoords);
				TileTexBuffer.Position (0);

				lastTTextureCoors = texturecoords;

				TextureBuffer = TileTexBuffer;

				return true;
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