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
	public class CGLMap : PhysX.PhysXMap
	{
		public enum UpdateType
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

		public readonly Size DrawSize;
		private float vertexSize;

		public CGLMap (int mapDrawWidth, string name) : base (name)
		{
			DrawSize = new Size (mapDrawWidth + 2, (int)((float)mapDrawWidth / Content.ScreenRatio) + 2);
			vertexSize = 2 * Content.ScreenRatio / (float)(DrawSize.Width - 2);


			renderProgram = CGLTools.GetProgram (Shader.VertexShaderM, Shader.FragmentShader); 

			initVertexCoords ();
			initTextureCoords ();

			Content.OnUpdate += () => {
				vertexSize = 2 * Content.ScreenRatio / (float)(DrawSize.Width - 2);
				initVertexCoords ();
				updateTextureBuffer (UpdateType.Complete);
			};
		}

		private void initVertexCoords ()
		{	
			int iTileCount = DrawSize.Width * DrawSize.Height;
			float[] vertexCoords = new float[iTileCount * 8 * 2];
			short[] vertexIndices = new short[iTileCount * 6 * 2];

			for (int i = 0; i < 2; i++) { // tile and overlay vertex
				for (int y = 0; y < DrawSize.Height; y++) {
					for (int x = 0; x < DrawSize.Width; x++) {

						vertexCoords [x * 8 + y * DrawSize.Width * 8 + i * iTileCount * 8 + 0] = Content.ScreenRatio - (x * vertexSize);
						vertexCoords [x * 8 + y * DrawSize.Width * 8 + i * iTileCount * 8 + 1] = -1f + (y * vertexSize);
						vertexCoords [x * 8 + y * DrawSize.Width * 8 + i * iTileCount * 8 + 2] = Content.ScreenRatio - (x * vertexSize);
						vertexCoords [x * 8 + y * DrawSize.Width * 8 + i * iTileCount * 8 + 3] = -1f + ((y + 1) * vertexSize);
						vertexCoords [x * 8 + y * DrawSize.Width * 8 + i * iTileCount * 8 + 4] = Content.ScreenRatio - (x * vertexSize) + vertexSize;
						vertexCoords [x * 8 + y * DrawSize.Width * 8 + i * iTileCount * 8 + 5] = -1f + ((y + 1) * vertexSize);
						vertexCoords [x * 8 + y * DrawSize.Width * 8 + i * iTileCount * 8 + 6] = Content.ScreenRatio - (x * vertexSize) + vertexSize;
						vertexCoords [x * 8 + y * DrawSize.Width * 8 + i * iTileCount * 8 + 7] = -1f + (y * vertexSize);

						vertexIndices [x * 6 + y * DrawSize.Width * 6 + i * iTileCount * 6 + 0] = (short)(x * 4 + y * DrawSize.Width * 4 + i * iTileCount * 4 + 0);
						vertexIndices [x * 6 + y * DrawSize.Width * 6 + i * iTileCount * 6 + 1] = (short)(x * 4 + y * DrawSize.Width * 4 + i * iTileCount * 4 + 1);
						vertexIndices [x * 6 + y * DrawSize.Width * 6 + i * iTileCount * 6 + 2] = (short)(x * 4 + y * DrawSize.Width * 4 + i * iTileCount * 4 + 2);
						vertexIndices [x * 6 + y * DrawSize.Width * 6 + i * iTileCount * 6 + 3] = (short)(x * 4 + y * DrawSize.Width * 4 + i * iTileCount * 4 + 0);
						vertexIndices [x * 6 + y * DrawSize.Width * 6 + i * iTileCount * 6 + 4] = (short)(x * 4 + y * DrawSize.Width * 4 + i * iTileCount * 4 + 2);
						vertexIndices [x * 6 + y * DrawSize.Width * 6 + i * iTileCount * 6 + 5] = (short)(x * 4 + y * DrawSize.Width * 4 + i * iTileCount * 4 + 3);

					}
				}
			}

			vertexBuffer = CGLTools.CreateBuffer (vertexCoords);
			indexBuffer = CGLTools.CreateBuffer (vertexIndices);
		}

		private void initTextureCoords ()
		{
			//init Texture Buffer
			textureBuffer = CGLTools.CreateBuffer ((float)(DrawSize.Width * DrawSize.Height * 8 * 2));

			LineTileTextureCoords = new Dictionary<int, float[]> ();
			LineOvrlTextureCoords = new Dictionary<int, float[]> ();

			for (int y = 0; y < Height; y++) {
				float[] BufferedTileTexCoords = new float[Width * 8];
				float[] BufferedOvrlTexCoords = new float[Width * 8];

				for (int x = 0; x < Width; x++) {
					for (uint c = 0; c < 8; c++) {
						BufferedTileTexCoords [x * 8 + c] = Content.TileTexCoordManager [(ushort)GetTile (x, y)] [c];
						BufferedOvrlTexCoords [x * 8 + c] = Content.OverlayTexCoordManager [(ushort)GetOverlay (x, y)] [c];
					}
				}

				LineTileTextureCoords.Add ((int)y, BufferedTileTexCoords);
				LineOvrlTextureCoords.Add ((int)y, BufferedOvrlTexCoords);
			}
		}

		public bool updateTextureBuffer (UpdateType updateType)
		{
			switch (updateType) {
			case UpdateType.RemoveBottom:	// Just add a new line at the top
				float[] copiedpart = new float[DrawSize.Width * (DrawSize.Height - 1) * 8];
				textureBuffer.Position (0);
				textureBuffer.Get (copiedpart, 0, DrawSize.Width * (DrawSize.Height - 1) * 8);

				textureBuffer.Position (0);
			
				textureBuffer.Put (LineTileTextureCoords [Content.Camera.CurrentMapTile.Y].Cut ((Content.Camera.CurrentMapTile.X) * 8, DrawSize.Width * 8));
				textureBuffer.Put (copiedpart);
				break;
			case UpdateType.RemoveTop:	// Just add a new line at the bottom
				copiedpart = new float[DrawSize.Width * (DrawSize.Height - 1) * 8];
				textureBuffer.Position (DrawSize.Width * 8);
				textureBuffer.Get (copiedpart, 0, DrawSize.Width * (DrawSize.Height - 1) * 8);

				textureBuffer.Position (0);
				textureBuffer.Put (copiedpart);
				textureBuffer.Put (LineTileTextureCoords [Content.Camera.CurrentMapTile.Y].Cut ((Content.Camera.CurrentMapTile.X) * 8, DrawSize.Width * 8));
				break;
			case UpdateType.Complete:
				for (int y = 0; y < DrawSize.Height; y++) { // tiles
					textureBuffer.Put (LineTileTextureCoords [Content.Camera.CurrentMapTile.Y + y].Cut ((Content.Camera.CurrentMapTile.X) * 8, DrawSize.Width * 8));
				}

				for (int y = 0; y < DrawSize.Height; y++) {
					textureBuffer.Put (LineOvrlTextureCoords [Content.Camera.CurrentMapTile.Y + y].Cut ((Content.Camera.CurrentMapTile.X) * 8, DrawSize.Width * 8));
				}
				break;
			}
			textureBuffer.Position (0);
			return true;
		}

		//		public void MoveTo (int x, int y)
		//		{
		//			Size difference = new Size (Content.Camera.CurrentMapCenter.X - x, Content.Camera.CurrentMapCenter.Y - y);
		//			if (difference.Width == 0 && difference.Height == 0) {
		//				return;
		//			} else if (difference.Width == 0 && difference.Height == 1) {
		//				Content.Camera.CurrentMapCenter.Y -= 1;
		//				updateTextureBuffer (UpdateType.RemoveBottom);
		//				return;
		//			} else if (difference.Width == 0 && difference.Height == -1) {
		//				Content.Camera.CurrentMapCenter.Y += 1;
		//				updateTextureBuffer (UpdateType.RemoveTop);
		//				return;
		//			} else {
		//				Content.Camera.CurrentMapCenter.X = x;
		//				Content.Camera.CurrentMapCenter.Y = y;
		//				updateTextureBuffer (UpdateType.Complete);
		//				return;
		//			}
		//		}
		//
		//		public void Move (Orientation direction)
		//		{
		//			switch (direction) {
		//			case Orientation.Up:
		//				Content.Camera.CurrentMapCenter.Y -= 1;
		//				updateTextureBuffer (UpdateType.RemoveTop);
		//				break;
		//			case Orientation.Down:
		//				Content.Camera.CurrentMapCenter.Y += 1;
		//				updateTextureBuffer (UpdateType.RemoveBottom);
		//				break;
		//			case Orientation.West:
		//				Content.Camera.CurrentMapCenter.X -= 1;
		//				updateTextureBuffer (UpdateType.Complete);
		//				break;
		//			case Orientation.East:
		//				Content.Camera.CurrentMapCenter.X += 1;
		//				updateTextureBuffer (UpdateType.Complete);
		//				break;
		//			}
		//		}

		public void Draw ()
		{
			GL.GlUseProgram (renderProgram);

			// Set the active texture unit to texture unit 0.

			int PositionHandle = GL.GlGetAttribLocation (renderProgram, "vPosition");
			GL.GlEnableVertexAttribArray (PositionHandle);
			GL.GlVertexAttribPointer (PositionHandle, 2, GL.GlFloat, false, 2 * sizeof(float), vertexBuffer);

			int MVPMatrixHandle = GL.GlGetUniformLocation (renderProgram, "uMVPMatrix");
			GL.GlUniformMatrix4fv (MVPMatrixHandle, 1, false, Content.Camera.MapMVPMatrix, 0);

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
