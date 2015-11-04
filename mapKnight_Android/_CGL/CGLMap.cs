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
			private enum UpdateType
			{
				Complete = 0,
				RemoveTop = -1,
				RemoveBottom = 1
			}

			FloatBuffer VertexBuffer;
			IntBuffer IndexBuffer;
			FloatBuffer TextureBuffer;
			private float[] lastTTextureCoors;

			FloatBuffer fboVertexBuffer;
			ShortBuffer fboIndexBuffer;
			FloatBuffer fboTextureBuffer;
			
			private int fboRenderProgram;
			private int RenderProgram;

			private int fboVertexCount;
			private int VertexStride;

			private static int CoordsPerVertex = 2;
			private float[] VertexCoords;

			private int[] VertexIndices = {
				0,
				1,
				2,
				0,
				2,
				3
			};

			private BufferData framebuffer;

			Dictionary<int, float[]> LineTileTextureCoords;
			Dictionary<int, float[]> LineOvrlTextureCoords;

			private readonly int mapDrawWidth;
			private readonly int mapDrawHeight;
			private readonly float vertexSize;

			private int currentTileX;
			private int currentTileY;

			private Size screenBounds;

			public CGLMap (int MapDrawWidth, float screenRatio, Utils.XMLElemental MapXML, Size ScreenSize) : base (MapXML)
			{
				int tileSizeInPXL = GlobalContent.TileSize;
				screenBounds = ScreenSize;

				mapDrawWidth = (int)MapDrawWidth;
				mapDrawHeight = (int)((float)mapDrawWidth / screenRatio) + 1;
				vertexSize = 2 / (float)(mapDrawWidth);
				currentTileX = mapDrawWidth / 2;
				currentTileY = mapDrawHeight / 2;

				initFBOBuffer (screenRatio);
			
				framebuffer = CGLTools.GenerateFramebuffer (tileSizeInPXL * mapDrawWidth, tileSizeInPXL * mapDrawHeight); 

				initVertexCoords ();
				initTextureCoords ();
				updateTextureBuffer (UpdateType.Complete);
			}

			private void initFBOBuffer (float screenRatio)
			{
				fboRenderProgram = CGLTools.GetProgram (GlobalContent.FragmentShaderN, GlobalContent.VertexShaderM);
				RenderProgram = CGLTools.GetProgram (GlobalContent.FragmentShaderN, GlobalContent.VertexShaderN);

				VertexCoords = new float[12];

				VertexCoords [0] = -screenRatio;
				VertexCoords [1] = 1f;

				VertexCoords [3] = -screenRatio;
				VertexCoords [4] = -1f;

				VertexCoords [6] = screenRatio;
				VertexCoords [7] = -1f;

				VertexCoords [9] = screenRatio;
				VertexCoords [10] = 1f;

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
				bytebuffer.Order (ByteOrder.NativeOrder ());
				fboVertexBuffer = bytebuffer.AsFloatBuffer ();
				fboVertexBuffer.Put (VertexCoords);
				fboVertexBuffer.Position (0);

				bytebuffer = ByteBuffer.AllocateDirect (VertexIndices.Length * sizeof(short));
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
			}

			private void initVertexCoords ()
			{	
				VertexCoords = new float[(mapDrawWidth) * (mapDrawHeight) * 8];
				VertexIndices = new int[mapDrawWidth * mapDrawHeight * 6];

				for (int y = 0; y < mapDrawHeight; y++) {
					for (int x = 0; x < mapDrawWidth; x++) {

						VertexCoords [x * 8 + y * mapDrawWidth * 8 + 0] = -1f + (x * vertexSize);
						VertexCoords [x * 8 + y * mapDrawWidth * 8 + 1] = 1f - (y * vertexSize);
						VertexCoords [x * 8 + y * mapDrawWidth * 8 + 2] = -1f + (x * vertexSize);
						VertexCoords [x * 8 + y * mapDrawWidth * 8 + 3] = 1f - ((y + 1) * vertexSize);
						VertexCoords [x * 8 + y * mapDrawWidth * 8 + 4] = -1f + (x * vertexSize) + vertexSize;
						VertexCoords [x * 8 + y * mapDrawWidth * 8 + 5] = 1f - ((y + 1) * vertexSize);
						VertexCoords [x * 8 + y * mapDrawWidth * 8 + 6] = -1f + (x * vertexSize) + vertexSize;
						VertexCoords [x * 8 + y * mapDrawWidth * 8 + 7] = 1f - (y * vertexSize);

						VertexIndices [x * 6 + y * mapDrawWidth * 6 + 0] = x * 4 + y * mapDrawWidth * 4 + 0;
						VertexIndices [x * 6 + y * mapDrawWidth * 6 + 1] = x * 4 + y * mapDrawWidth * 4 + 1;
						VertexIndices [x * 6 + y * mapDrawWidth * 6 + 2] = x * 4 + y * mapDrawWidth * 4 + 2;
						VertexIndices [x * 6 + y * mapDrawWidth * 6 + 3] = x * 4 + y * mapDrawWidth * 4 + 0;
						VertexIndices [x * 6 + y * mapDrawWidth * 6 + 4] = x * 4 + y * mapDrawWidth * 4 + 2;
						VertexIndices [x * 6 + y * mapDrawWidth * 6 + 5] = x * 4 + y * mapDrawWidth * 4 + 3;

					}
				}

				VertexStride = CoordsPerVertex * sizeof(float);

				ByteBuffer bytebuffer = ByteBuffer.AllocateDirect (VertexCoords.Length * sizeof(float));
				bytebuffer.Order (ByteOrder.NativeOrder ());
				VertexBuffer = bytebuffer.AsFloatBuffer ();
				VertexBuffer.Put (VertexCoords);
				VertexBuffer.Position (0);

				bytebuffer = ByteBuffer.AllocateDirect (VertexIndices.Length * sizeof(int));
				bytebuffer.Order (ByteOrder.NativeOrder ());
				IndexBuffer = bytebuffer.AsIntBuffer ();
				IndexBuffer.Put (VertexIndices);
				IndexBuffer.Position (0);
			}

			private void initTextureCoords ()
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

			private bool updateTextureBuffer (UpdateType updateType)
			{
				FloatBuffer TileTexBuffer;
				float[] texturecoords = new float[mapDrawWidth * mapDrawHeight * 8];

				switch (updateType) {
				case UpdateType.RemoveBottom:	// Just add a new line at the top
					float[] topline = AO.Cut (LineTileTextureCoords [currentTileY - mapDrawHeight / 2], (currentTileX - mapDrawWidth / 2) * 8, mapDrawWidth * 8);
					float[] bottomrest = AO.Cut (lastTTextureCoors, 0, mapDrawWidth * (mapDrawHeight - 1) * 8);
					Array.Copy (topline, texturecoords, topline.Length);
					Array.Copy (bottomrest, 0, texturecoords, topline.Length, bottomrest.Length);
					break;
				case UpdateType.RemoveTop:	// Just add a new line at the bottom
					float[] bottomline = AO.Cut (LineTileTextureCoords [currentTileY + mapDrawHeight / 2], (currentTileX - mapDrawWidth / 2) * 8, mapDrawWidth * 8);
					float[] toprest = AO.Cut (lastTTextureCoors, mapDrawWidth * 8, mapDrawWidth * (mapDrawHeight - 1) * 8);
					Array.Copy (toprest, texturecoords, toprest.Length);
					Array.Copy (bottomline, 0, texturecoords, toprest.Length, bottomline.Length);
					break;
				case UpdateType.Complete:
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

				renderFrameBuffer ();
				return true;
			}

			private void renderFrameBuffer ()
			{
				GL.GlBindFramebuffer (GL.GlFramebuffer, framebuffer.FrameBuffer);
				GL.GlViewport (0, 0, GlobalContent.TileSize * mapDrawWidth, GlobalContent.TileSize * mapDrawHeight);
				GL.GlClearColor (1.0f, 1.0f, 1.0f, 1.0f);
				GL.GlClear (GL.GlColorBufferBit | GL.GlDepthBufferBit);

				GL.GlUseProgram (RenderProgram);

				GL.GlActiveTexture (GL.GlTexture0);
				GL.GlBindTexture (GL.GlTexture2d, GlobalContent.MainTexture);

				int PositionHandle = GL.GlGetAttribLocation (RenderProgram, "vPosition");
				GL.GlEnableVertexAttribArray (PositionHandle);
				GL.GlVertexAttribPointer (PositionHandle, CoordsPerVertex, GL.GlFloat, false, VertexStride, VertexBuffer);

				GL.GlEnable (GL.GlBlend);
				GL.GlBlendFunc (GL.GlSrcAlpha, GL.GlOneMinusSrcAlpha);

				int mTextureUniformHandle = GL.GlGetUniformLocation (RenderProgram, "u_Texture");
				GL.GlUniform1i (mTextureUniformHandle, 0);

				int mTextureCoordinateHandle = GL.GlGetAttribLocation (RenderProgram, "a_TexCoordinate");
				GL.GlVertexAttribPointer (mTextureCoordinateHandle, 2, GL.GlFloat, false, 0, TextureBuffer);
				GL.GlEnableVertexAttribArray (mTextureCoordinateHandle);

				GL.GlDrawElements (GL.GlTriangles, VertexIndices.Length, GL.GlUnsignedInt, IndexBuffer);
				GL.GlDisableVertexAttribArray (PositionHandle);
				GL.GlDisableVertexAttribArray (mTextureCoordinateHandle);

				// end draw
				GL.GlBindFramebuffer (GL.GlFramebuffer, 0);

				GL.GlViewport (0, 0, screenBounds.Width, screenBounds.Height);
			}

			public void MoveTo (int x, int y)
			{
				currentTileX = x;
				currentTileY = y;
				updateTextureBuffer (UpdateType.Complete);
			}

			public void MoveToAdv (int x, int y)
			{
				Size difference = new Size (currentTileX - x, currentTileY - y);
				if (difference.Width == 0 && difference.Height == 0) {
					return;
				} else if (difference.Width == 0 && difference.Height == 1) {
					currentTileY -= 1;
					updateTextureBuffer (UpdateType.RemoveBottom);
					return;
				} else if (difference.Width == 0 && difference.Height == -1) {
					currentTileY += 1;
					updateTextureBuffer (UpdateType.RemoveTop);
					return;
				} else {
				
				}
			}

			public void MoveBy (int x, int y)
			{
				currentTileX += x;
				currentTileY += y;
				updateTextureBuffer (UpdateType.Complete);
			}

			public void MoveByAdv (int x, int y)
			{
				MoveToAdv (currentTileX + x, currentTileY + y);
			}

			public void Move (Orientation direction)
			{
				switch (direction) {
				case Orientation.Up:
					currentTileY -= 1;
					updateTextureBuffer (UpdateType.RemoveTop);
					break;
				case Orientation.Down:
					currentTileY += 1;
					updateTextureBuffer (UpdateType.RemoveBottom);
					break;
				case Orientation.West:
					currentTileX -= 1;
					updateTextureBuffer (UpdateType.Complete);
					break;
				case Orientation.East:
					currentTileX += 1;
					updateTextureBuffer (UpdateType.Complete);
					break;
				}
			}

			public void Draw (float[] _mvpMatrix)
			{
				GL.GlUseProgram (fboRenderProgram);

				// Set the active texture unit to texture unit 0.

				int PositionHandle = GL.GlGetAttribLocation (fboRenderProgram, "vPosition");
				GL.GlEnableVertexAttribArray (PositionHandle);
				GL.GlVertexAttribPointer (PositionHandle, 3, GL.GlFloat, false, 3 * sizeof(float), fboVertexBuffer);

				int MVPMatrixHandle = GL.GlGetUniformLocation (fboRenderProgram, "uMVPMatrix");
				GL.GlUniformMatrix4fv (MVPMatrixHandle, 1, false, _mvpMatrix, 0);

				GL.GlEnable (GL.GlBlend);
				GL.GlBlendFunc (GL.GlSrcAlpha, GL.GlOneMinusSrcAlpha);

				int mTextureUniformHandle = GL.GlGetUniformLocation (fboRenderProgram, "u_Texture");
				int mTextureCoordinateHandle = GL.GlGetAttribLocation (fboRenderProgram, "a_TexCoordinate");
				GL.GlVertexAttribPointer (mTextureCoordinateHandle, 2, GL.GlFloat, false, 0, fboTextureBuffer);
				GL.GlEnableVertexAttribArray (mTextureCoordinateHandle);

				GL.GlActiveTexture (GL.GlTexture2);
				GL.GlBindTexture (GL.GlTexture2d, framebuffer.FrameBufferTexture);
				GL.GlUniform1i (mTextureUniformHandle, 2);

				GL.GlDrawElements (GL.GlTriangles, 6, GL.GlUnsignedShort, fboIndexBuffer);
				GL.GlDisableVertexAttribArray (PositionHandle);
				GL.GlDisableVertexAttribArray (mTextureCoordinateHandle);
			}
		}
	}
}