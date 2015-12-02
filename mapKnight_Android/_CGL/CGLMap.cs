using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Android.Opengl;
using GL = Android.Opengl.GLES20;

using Java.Nio;

namespace mapKnight_Android.CGL
{
	public class CGLMap : Map
	{
		private enum UpdateType
		{
			Complete = 0,
			RemoveTop = -1,
			RemoveBottom = 1
		}

		FloatBuffer iVertexBuffer;
		ShortBuffer iIndexBuffer;
		FloatBuffer iTextureBuffer;

		FloatBuffer fboVertexBuffer;
		ShortBuffer fboIndexBuffer;
		FloatBuffer fboTextureBuffer;
			
		private int fboRenderProgram;
		private int RenderProgram;

		private static int CoordsPerVertex = 2;
		private static int VertexStride = CoordsPerVertex * sizeof(float);

		private float[] VertexCoords;

		private short[] VertexIndices = {
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
		// left top
		private int currentTileY;

		private Size screenBounds;

		public CGLMap (int MapDrawWidth, float screenRatio, XMLElemental MapXML, Size ScreenSize) : base (MapXML)
		{
			int tileSizeInPXL = Content.TileSize;
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
			fboRenderProgram = CGLTools.GetProgram (Content.FragmentShaderN, Content.VertexShaderM);
			RenderProgram = CGLTools.GetProgram (Content.FragmentShaderN, Content.VertexShaderN);

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
		}

		private void initVertexCoords ()
		{	
			int iTileCount = mapDrawWidth * mapDrawHeight;
			VertexCoords = new float[iTileCount * 8 * 2];
			VertexIndices = new short[iTileCount * 6 * 2];

			for (int i = 0; i < 2; i++) { // tile and overlay vertex
				for (int y = 0; y < mapDrawHeight; y++) {
					for (int x = 0; x < mapDrawWidth; x++) {

						VertexCoords [x * 8 + y * mapDrawWidth * 8 + i * iTileCount * 8 + 0] = -1f + (x * vertexSize);
						VertexCoords [x * 8 + y * mapDrawWidth * 8 + i * iTileCount * 8 + 1] = 1f - (y * vertexSize);
						VertexCoords [x * 8 + y * mapDrawWidth * 8 + i * iTileCount * 8 + 2] = -1f + (x * vertexSize);
						VertexCoords [x * 8 + y * mapDrawWidth * 8 + i * iTileCount * 8 + 3] = 1f - ((y + 1) * vertexSize);
						VertexCoords [x * 8 + y * mapDrawWidth * 8 + i * iTileCount * 8 + 4] = -1f + (x * vertexSize) + vertexSize;
						VertexCoords [x * 8 + y * mapDrawWidth * 8 + i * iTileCount * 8 + 5] = 1f - ((y + 1) * vertexSize);
						VertexCoords [x * 8 + y * mapDrawWidth * 8 + i * iTileCount * 8 + 6] = -1f + (x * vertexSize) + vertexSize;
						VertexCoords [x * 8 + y * mapDrawWidth * 8 + i * iTileCount * 8 + 7] = 1f - (y * vertexSize);

						VertexIndices [x * 6 + y * mapDrawWidth * 6 + i * iTileCount * 6 + 0] = (short)(x * 4 + y * mapDrawWidth * 4 + i * iTileCount * 4 + 0);
						VertexIndices [x * 6 + y * mapDrawWidth * 6 + i * iTileCount * 6 + 1] = (short)(x * 4 + y * mapDrawWidth * 4 + i * iTileCount * 4 + 1);
						VertexIndices [x * 6 + y * mapDrawWidth * 6 + i * iTileCount * 6 + 2] = (short)(x * 4 + y * mapDrawWidth * 4 + i * iTileCount * 4 + 2);
						VertexIndices [x * 6 + y * mapDrawWidth * 6 + i * iTileCount * 6 + 3] = (short)(x * 4 + y * mapDrawWidth * 4 + i * iTileCount * 4 + 0);
						VertexIndices [x * 6 + y * mapDrawWidth * 6 + i * iTileCount * 6 + 4] = (short)(x * 4 + y * mapDrawWidth * 4 + i * iTileCount * 4 + 2);
						VertexIndices [x * 6 + y * mapDrawWidth * 6 + i * iTileCount * 6 + 5] = (short)(x * 4 + y * mapDrawWidth * 4 + i * iTileCount * 4 + 3);

					}
				}
			}

			ByteBuffer bytebuffer = ByteBuffer.AllocateDirect (VertexCoords.Length * sizeof(float));
			bytebuffer.Order (ByteOrder.NativeOrder ());
			iVertexBuffer = bytebuffer.AsFloatBuffer ();
			iVertexBuffer.Put (VertexCoords);
			iVertexBuffer.Position (0);

			bytebuffer = ByteBuffer.AllocateDirect (VertexIndices.Length * sizeof(short));
			bytebuffer.Order (ByteOrder.NativeOrder ());
			iIndexBuffer = bytebuffer.AsShortBuffer ();
			iIndexBuffer.Put (VertexIndices);
			iIndexBuffer.Position (0);
		}

		private void initTextureCoords ()
		{
			//init Texture Buffer
			ByteBuffer bytebuffer = ByteBuffer.AllocateDirect (mapDrawWidth * mapDrawHeight * 8 * 2 * sizeof(float));
			bytebuffer.Order (ByteOrder.NativeOrder ());
			iTextureBuffer = bytebuffer.AsFloatBuffer ();
			iTextureBuffer.Position (0);

			LineTileTextureCoords = new Dictionary<int, float[]> ();
			LineOvrlTextureCoords = new Dictionary<int, float[]> ();

			for (uint y = 0; y < Height; y++) {
				float[] BufferedTileTexCoords = new float[Width * 8];
				float[] BufferedOvrlTexCoords = new float[Width * 8];

				for (uint x = 0; x < Width; x++) {
					for (uint c = 0; c < 8; c++) {
						BufferedTileTexCoords [x * 8 + c] = Content.TileTexCoordManager [(short)GetTile (x, y)] [c];
						BufferedOvrlTexCoords [x * 8 + c] = Content.OverlayTexCoordManager [(short)GetOverlay (x, y)] [c];
					}
				}

				LineTileTextureCoords.Add ((int)y, BufferedTileTexCoords);
				LineOvrlTextureCoords.Add ((int)y, BufferedOvrlTexCoords);
			}
		}

		private bool updateTextureBuffer (UpdateType updateType)
		{
			switch (updateType) {
			case UpdateType.RemoveBottom:	// Just add a new line at the top
				float[] copiedpart = new float[mapDrawWidth * (mapDrawHeight - 1) * 8];
				iTextureBuffer.Position (0);
				iTextureBuffer.Get (copiedpart, 0, mapDrawWidth * (mapDrawHeight - 1) * 8);

				iTextureBuffer.Position (0);
				iTextureBuffer.Put (AO.Cut (LineTileTextureCoords [currentTileY - mapDrawHeight / 2], (currentTileX - mapDrawWidth / 2) * 8, mapDrawWidth * 8));
				iTextureBuffer.Put (copiedpart);
				break;
			case UpdateType.RemoveTop:	// Just add a new line at the bottom
				copiedpart = new float[mapDrawWidth * (mapDrawHeight - 1) * 8];
				iTextureBuffer.Position (mapDrawWidth * 8);
				iTextureBuffer.Get (copiedpart, 0, mapDrawWidth * (mapDrawHeight - 1) * 8);

				iTextureBuffer.Position (0);
				iTextureBuffer.Put (copiedpart);
				iTextureBuffer.Put (AO.Cut (LineTileTextureCoords [currentTileY + mapDrawHeight / 2], (currentTileX - mapDrawWidth / 2) * 8, mapDrawWidth * 8));
				break;
			case UpdateType.Complete:
				iTextureBuffer.Position (0);
				for (int y = 0; y < mapDrawHeight; y++) { // tiles
					iTextureBuffer.Put (AO.Cut (LineTileTextureCoords [currentTileY - mapDrawHeight / 2 + y], (currentTileX - mapDrawWidth / 2) * 8, mapDrawWidth * 8));
				}

				for (int y = 0; y < mapDrawHeight; y++) {
					iTextureBuffer.Put (AO.Cut (LineOvrlTextureCoords [currentTileY - mapDrawHeight / 2 + y], (currentTileX - mapDrawWidth / 2) * 8, mapDrawWidth * 8));
				}
				break;
			}
			iTextureBuffer.Position (0);

			renderFrameBuffer ();
			return true;
		}

		private void renderFrameBuffer ()
		{
			GL.GlBindFramebuffer (GL.GlFramebuffer, framebuffer.FrameBuffer);
			GL.GlViewport (0, 0, Content.TileSize * mapDrawWidth, Content.TileSize * mapDrawHeight);
			GL.GlClearColor (1.0f, 1.0f, 1.0f, 1.0f);
			GL.GlClear (GL.GlColorBufferBit | GL.GlDepthBufferBit);

			GL.GlUseProgram (RenderProgram);

			GL.GlActiveTexture (GL.GlTexture0);
			GL.GlBindTexture (GL.GlTexture2d, Content.MainTexture);

			int PositionHandle = GL.GlGetAttribLocation (RenderProgram, "vPosition");
			GL.GlEnableVertexAttribArray (PositionHandle);
			GL.GlVertexAttribPointer (PositionHandle, CoordsPerVertex, GL.GlFloat, false, VertexStride, iVertexBuffer);

			GL.GlEnable (GL.GlBlend);
			GL.GlBlendFunc (GL.GlSrcAlpha, GL.GlOneMinusSrcAlpha);

			int mTextureUniformHandle = GL.GlGetUniformLocation (RenderProgram, "u_Texture");
			GL.GlUniform1i (mTextureUniformHandle, 0);

			int mTextureCoordinateHandle = GL.GlGetAttribLocation (RenderProgram, "a_TexCoordinate");
			GL.GlVertexAttribPointer (mTextureCoordinateHandle, 2, GL.GlFloat, false, 0, iTextureBuffer);
			GL.GlEnableVertexAttribArray (mTextureCoordinateHandle);

			GL.GlDrawElements (GL.GlTriangles, VertexIndices.Length, GL.GlUnsignedShort, iIndexBuffer);
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
				currentTileX = x;
				currentTileY = y;
				updateTextureBuffer (UpdateType.Complete);
				return;
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
