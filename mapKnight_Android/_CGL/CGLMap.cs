using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Android.Opengl;
using GL = Android.Opengl.GLES20;

using Java.Nio;

using mapKnight.Values;
using mapKnight.Utils;

namespace mapKnight.Android.CGL
{
	public class CGLMap : Map
	{
		private enum UpdateType
		{
			Complete = 0,
			RemoveTop = -1,
			RemoveBottom = 1
		}

		FloatBuffer vertexBuffer;
		ShortBuffer indexBuffer;
		FloatBuffer textureBuffer;
			
		private int renderProgram;

		Dictionary<int, float[]> LineTileTextureCoords;
		Dictionary<int, float[]> LineOvrlTextureCoords;

		private readonly int mapDrawWidth;
		private readonly int mapDrawHeight;
		private float vertexSize;

		private int currentTileX;
		// left top
		private int currentTileY;

		public CGLMap (int mapDrawWidth, string name) : base (name)
		{
			this.mapDrawWidth = mapDrawWidth;
			mapDrawHeight = (int)((float)mapDrawWidth / Content.ScreenRatio) + 1;
			vertexSize = 2 * Content.ScreenRatio / (float)(mapDrawWidth);
			currentTileX = mapDrawWidth / 2;
			currentTileY = mapDrawHeight / 2;

			renderProgram = CGLTools.GetProgram (Shader.VertexShaderM, Shader.FragmentShader); 

			initVertexCoords ();
			initTextureCoords ();
			updateTextureBuffer (UpdateType.Complete);

			Content.OnUpdate += () => {
				vertexSize = 2 * Content.ScreenRatio / (float)(mapDrawWidth);
				initVertexCoords ();
			};
		}

		private void initVertexCoords ()
		{	
			int iTileCount = mapDrawWidth * mapDrawHeight;
			float[] vertexCoords = new float[iTileCount * 8 * 2];
			short[] vertexIndices = new short[iTileCount * 6 * 2];

			for (int i = 0; i < 2; i++) { // tile and overlay vertex
				for (int y = 0; y < mapDrawHeight; y++) {
					for (int x = 0; x < mapDrawWidth; x++) {

						vertexCoords [x * 8 + y * mapDrawWidth * 8 + i * iTileCount * 8 + 0] = Content.ScreenRatio - (x * vertexSize);
						vertexCoords [x * 8 + y * mapDrawWidth * 8 + i * iTileCount * 8 + 1] = 1f - (y * vertexSize);
						vertexCoords [x * 8 + y * mapDrawWidth * 8 + i * iTileCount * 8 + 2] = Content.ScreenRatio - (x * vertexSize);
						vertexCoords [x * 8 + y * mapDrawWidth * 8 + i * iTileCount * 8 + 3] = 1f - ((y + 1) * vertexSize);
						vertexCoords [x * 8 + y * mapDrawWidth * 8 + i * iTileCount * 8 + 4] = Content.ScreenRatio - (x * vertexSize) + vertexSize;
						vertexCoords [x * 8 + y * mapDrawWidth * 8 + i * iTileCount * 8 + 5] = 1f - ((y + 1) * vertexSize);
						vertexCoords [x * 8 + y * mapDrawWidth * 8 + i * iTileCount * 8 + 6] = Content.ScreenRatio - (x * vertexSize) + vertexSize;
						vertexCoords [x * 8 + y * mapDrawWidth * 8 + i * iTileCount * 8 + 7] = 1f - (y * vertexSize);

						vertexIndices [x * 6 + y * mapDrawWidth * 6 + i * iTileCount * 6 + 0] = (short)(x * 4 + y * mapDrawWidth * 4 + i * iTileCount * 4 + 0);
						vertexIndices [x * 6 + y * mapDrawWidth * 6 + i * iTileCount * 6 + 1] = (short)(x * 4 + y * mapDrawWidth * 4 + i * iTileCount * 4 + 1);
						vertexIndices [x * 6 + y * mapDrawWidth * 6 + i * iTileCount * 6 + 2] = (short)(x * 4 + y * mapDrawWidth * 4 + i * iTileCount * 4 + 2);
						vertexIndices [x * 6 + y * mapDrawWidth * 6 + i * iTileCount * 6 + 3] = (short)(x * 4 + y * mapDrawWidth * 4 + i * iTileCount * 4 + 0);
						vertexIndices [x * 6 + y * mapDrawWidth * 6 + i * iTileCount * 6 + 4] = (short)(x * 4 + y * mapDrawWidth * 4 + i * iTileCount * 4 + 2);
						vertexIndices [x * 6 + y * mapDrawWidth * 6 + i * iTileCount * 6 + 5] = (short)(x * 4 + y * mapDrawWidth * 4 + i * iTileCount * 4 + 3);

					}
				}
			}

			vertexBuffer = CGLTools.CreateBuffer (vertexCoords);
			indexBuffer = CGLTools.CreateBuffer (vertexIndices);
		}

		private void initTextureCoords ()
		{
			//init Texture Buffer
			textureBuffer = CGLTools.CreateBuffer ((float)(mapDrawWidth * mapDrawHeight * 8 * 2));

			LineTileTextureCoords = new Dictionary<int, float[]> ();
			LineOvrlTextureCoords = new Dictionary<int, float[]> ();

			for (uint y = 0; y < Height; y++) {
				float[] BufferedTileTexCoords = new float[Width * 8];
				float[] BufferedOvrlTexCoords = new float[Width * 8];

				for (uint x = 0; x < Width; x++) {
					for (uint c = 0; c < 8; c++) {
						BufferedTileTexCoords [x * 8 + c] = Content.TileTexCoordManager [(ushort)GetTile (x, y)] [c];
						BufferedOvrlTexCoords [x * 8 + c] = Content.OverlayTexCoordManager [(ushort)GetOverlay (x, y)] [c];
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
				textureBuffer.Position (0);
				textureBuffer.Get (copiedpart, 0, mapDrawWidth * (mapDrawHeight - 1) * 8);

				textureBuffer.Position (0);
				textureBuffer.Put (AO.Cut (LineTileTextureCoords [currentTileY - mapDrawHeight / 2], (currentTileX - mapDrawWidth / 2) * 8, mapDrawWidth * 8));
				textureBuffer.Put (copiedpart);
				break;
			case UpdateType.RemoveTop:	// Just add a new line at the bottom
				copiedpart = new float[mapDrawWidth * (mapDrawHeight - 1) * 8];
				textureBuffer.Position (mapDrawWidth * 8);
				textureBuffer.Get (copiedpart, 0, mapDrawWidth * (mapDrawHeight - 1) * 8);

				textureBuffer.Position (0);
				textureBuffer.Put (copiedpart);
				textureBuffer.Put (AO.Cut (LineTileTextureCoords [currentTileY + mapDrawHeight / 2], (currentTileX - mapDrawWidth / 2) * 8, mapDrawWidth * 8));
				break;
			case UpdateType.Complete:
				textureBuffer.Position (0);
				for (int y = 0; y < mapDrawHeight; y++) { // tiles
					textureBuffer.Put (AO.Cut (LineTileTextureCoords [currentTileY - mapDrawHeight / 2 + y], (currentTileX - mapDrawWidth / 2) * 8, mapDrawWidth * 8));
				}

				for (int y = 0; y < mapDrawHeight; y++) {
					textureBuffer.Put (AO.Cut (LineOvrlTextureCoords [currentTileY - mapDrawHeight / 2 + y], (currentTileX - mapDrawWidth / 2) * 8, mapDrawWidth * 8));
				}
				break;
			}
			textureBuffer.Position (0);

			return true;
		}

		public void MoveTo (int x, int y)
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

		public void Draw ()
		{
			GL.GlUseProgram (renderProgram);

			// Set the active texture unit to texture unit 0.

			int PositionHandle = GL.GlGetAttribLocation (renderProgram, "vPosition");
			GL.GlEnableVertexAttribArray (PositionHandle);
			GL.GlVertexAttribPointer (PositionHandle, 2, GL.GlFloat, false, 2 * sizeof(float), vertexBuffer);

			int MVPMatrixHandle = GL.GlGetUniformLocation (renderProgram, "uMVPMatrix");
			GL.GlUniformMatrix4fv (MVPMatrixHandle, 1, false, Content.MVPMatrix, 0);

			GL.GlEnable (GL.GlBlend);
			GL.GlBlendFunc (GL.GlSrcAlpha, GL.GlOneMinusSrcAlpha);

			int mTextureUniformHandle = GL.GlGetUniformLocation (renderProgram, "u_Texture");
			int mTextureCoordinateHandle = GL.GlGetAttribLocation (renderProgram, "a_TexCoordinate");
			GL.GlVertexAttribPointer (mTextureCoordinateHandle, 2, GL.GlFloat, false, 0, textureBuffer);
			GL.GlEnableVertexAttribArray (mTextureCoordinateHandle);

			GL.GlActiveTexture (GL.GlTexture2);
			GL.GlBindTexture (GL.GlTexture2d, Content.MainTexture);
			GL.GlUniform1i (mTextureUniformHandle, 2);

			GL.GlDrawElements (GL.GlTriangles, indexBuffer.Limit (), GL.GlUnsignedShort, indexBuffer);
			GL.GlDisableVertexAttribArray (PositionHandle);
			GL.GlDisableVertexAttribArray (mTextureCoordinateHandle);
		}
	}
}
