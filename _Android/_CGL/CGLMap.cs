using System;
using System.Collections.Generic;
using Java.Nio;
using mapKnight.Basic;
using GL = Android.Opengl.GLES20;

namespace mapKnight.Android.CGL {
    public class CGLMap : PhysX.PhysXMap {
        public const int DRAW_WIDTH = 18;

        public enum UpdateType {
            Complete = 0,
            RemoveTop = -1,
            RemoveBottom = 1
        }

        FloatBuffer vertexBuffer;
        ShortBuffer indexBuffer;
        FloatBuffer textureBuffer;

        private int renderProgram;
        private float[][][] layerBuffer; // buffers each texturecoordinate for every layer

        public readonly Size DrawSize;
        public readonly fSize RealDrawSize;
        public float vertexSize;

        [Obsolete ("only for debug purposes")]
        public List<Point> hitboxTiles = new List<Point> ();
        private static float[] hitboxTexCoords;

        public CGLMap (string name) : base (name) {
            RealDrawSize = new fSize (DRAW_WIDTH, DRAW_WIDTH / Content.ScreenRatio);
            DrawSize = new Size (DRAW_WIDTH + 2, (int)((float)DRAW_WIDTH / Content.ScreenRatio) + 2);
            vertexSize = 2 * Content.ScreenRatio / (float)(DRAW_WIDTH);


            renderProgram = CGLTools.GetProgram (Shader.VertexShaderM, Shader.FragmentShader);

            initVertexCoords ();
            initTextureCoords ();

            Content.OnUpdate += () => {
                vertexSize = 2 * Content.ScreenRatio / (float)(DRAW_WIDTH);
                initVertexCoords ();
                updateTextureBuffer ();
            };

            hitboxTexCoords = TileManager.GetTile (5).Texture;
        }

        private void initVertexCoords () {
            int iTileCount = DrawSize.Width * DrawSize.Height;
            float[] vertexCoords = new float[iTileCount * 8 * 3];
            short[] vertexIndices = new short[iTileCount * 6 * 3];

            for (int i = 0; i < 3; i++) { // PR tile and overlay vertex
                for (int y = 0; y < DrawSize.Height; y++) {
                    for (int x = 0; x < DrawSize.Width; x++) {

                        vertexCoords[x * 8 + y * DrawSize.Width * 8 + i * iTileCount * 8 + 0] = Content.ScreenRatio - (x * vertexSize);
                        vertexCoords[x * 8 + y * DrawSize.Width * 8 + i * iTileCount * 8 + 1] = -1f + (y * vertexSize);
                        vertexCoords[x * 8 + y * DrawSize.Width * 8 + i * iTileCount * 8 + 2] = Content.ScreenRatio - (x * vertexSize);
                        vertexCoords[x * 8 + y * DrawSize.Width * 8 + i * iTileCount * 8 + 3] = -1f + ((y + 1) * vertexSize);
                        vertexCoords[x * 8 + y * DrawSize.Width * 8 + i * iTileCount * 8 + 4] = Content.ScreenRatio - (x * vertexSize) - vertexSize;
                        vertexCoords[x * 8 + y * DrawSize.Width * 8 + i * iTileCount * 8 + 5] = -1f + ((y + 1) * vertexSize);
                        vertexCoords[x * 8 + y * DrawSize.Width * 8 + i * iTileCount * 8 + 6] = Content.ScreenRatio - (x * vertexSize) - vertexSize;
                        vertexCoords[x * 8 + y * DrawSize.Width * 8 + i * iTileCount * 8 + 7] = -1f + (y * vertexSize);

                        vertexIndices[x * 6 + y * DrawSize.Width * 6 + i * iTileCount * 6 + 0] = (short)(x * 4 + y * DrawSize.Width * 4 + i * iTileCount * 4 + 0);
                        vertexIndices[x * 6 + y * DrawSize.Width * 6 + i * iTileCount * 6 + 1] = (short)(x * 4 + y * DrawSize.Width * 4 + i * iTileCount * 4 + 1);
                        vertexIndices[x * 6 + y * DrawSize.Width * 6 + i * iTileCount * 6 + 2] = (short)(x * 4 + y * DrawSize.Width * 4 + i * iTileCount * 4 + 2);
                        vertexIndices[x * 6 + y * DrawSize.Width * 6 + i * iTileCount * 6 + 3] = (short)(x * 4 + y * DrawSize.Width * 4 + i * iTileCount * 4 + 0);
                        vertexIndices[x * 6 + y * DrawSize.Width * 6 + i * iTileCount * 6 + 4] = (short)(x * 4 + y * DrawSize.Width * 4 + i * iTileCount * 4 + 2);
                        vertexIndices[x * 6 + y * DrawSize.Width * 6 + i * iTileCount * 6 + 5] = (short)(x * 4 + y * DrawSize.Width * 4 + i * iTileCount * 4 + 3);

                    }
                }
            }

            vertexBuffer = CGLTools.CreateBuffer (vertexCoords);
            indexBuffer = CGLTools.CreateBuffer (vertexIndices);
        }

        private void initTextureCoords () {
            //init Texture Buffer
            textureBuffer = CGLTools.CreateBuffer ((float)(DrawSize.Width * DrawSize.Height * 8 * 3));

            // buffer tile coords
            layerBuffer = new float[3][][];
            for (int layer = 0; layer < 3; layer++) {
                layerBuffer[layer] = new float[this.Size.Height][];
                for (int y = 0; y < this.Size.Height; y++) {
                    layerBuffer[layer][y] = new float[this.Size.Width * 8];
                }
            }

            for (int y = 0; y < Size.Height; y++) {
                for (int x = 0; x < Size.Width; x++) {
                    for (int layer = 0; layer < 3; layer++) {
                        for (uint c = 0; c < 8; c++) {
                            layerBuffer[layer][y][x * 8 + c] = this.GetTile (x, y, layer).Texture[c];
                        }
                    }
                }
            }
        }

        //public bool updateTextureBuffer (UpdateType updateType) {
        //    switch (updateType) {
        //    case UpdateType.RemoveBottom:   // Just add a new line at the top
        //        float[] copiedpart = new float[DrawSize.Width * (DrawSize.Height - 1) * 8];
        //        textureBuffer.Position (0);
        //        textureBuffer.Get (copiedpart, 0, DrawSize.Width * (DrawSize.Height - 1) * 8);

        //        textureBuffer.Position (0);

        //        textureBuffer.Put (LineTileTextureCoords[Content.Camera.CurrentMapTile.Y].Cut ((Content.Camera.CurrentMapTile.X) * 8, DrawSize.Width * 8));
        //        textureBuffer.Put (copiedpart);
        //        break;
        //    case UpdateType.RemoveTop:  // Just add a new line at the bottom
        //        copiedpart = new float[DrawSize.Width * (DrawSize.Height - 1) * 8];
        //        textureBuffer.Position (DrawSize.Width * 8);
        //        textureBuffer.Get (copiedpart, 0, DrawSize.Width * (DrawSize.Height - 1) * 8);

        //        textureBuffer.Position (0);
        //        textureBuffer.Put (copiedpart);
        //        textureBuffer.Put (LineTileTextureCoords[Content.Camera.CurrentMapTile.Y].Cut ((Content.Camera.CurrentMapTile.X) * 8, DrawSize.Width * 8));
        //        break;
        //    case UpdateType.Complete:
        //        for(int layer = 0; layer < 3; layer++) {

        //        }
        //        for (int y = 0; y < DrawSize.Height; y++) { // tiles
        //            textureBuffer.Put (LineTileTextureCoords[Content.Camera.CurrentMapTile.Y + y].Cut ((Content.Camera.CurrentMapTile.X) * 8, DrawSize.Width * 8));
        //        }

        //        for (int y = 0; y < DrawSize.Height; y++) {
        //            textureBuffer.Put (LineOvrlTextureCoords[Content.Camera.CurrentMapTile.Y + y].Cut ((Content.Camera.CurrentMapTile.X) * 8, DrawSize.Width * 8));
        //        }
        //        break;
        //    }
        //    textureBuffer.Position (0);
        //    return true;
        //}

        public void updateTextureBuffer () {
            // insert buffered tile coords to texturebuffer
            for (int layer = 0; layer < 3; layer++) {
                for (int y = 0; y < DrawSize.Height; y++) {
                    textureBuffer.Put (layerBuffer[layer][Content.Camera.CurrentMapTile.Y + y].Cut (Content.Camera.CurrentMapTile.X * 8, DrawSize.Width * 8));
                }
            }
            foreach (Point tile in hitboxTiles) {
                textureBuffer.Position (((tile.Y - Content.Camera.CurrentMapTile.Y) * this.DrawSize.Width + (tile.X - Content.Camera.CurrentMapTile.X)) * 8);
                textureBuffer.Put (hitboxTexCoords);
            }
            textureBuffer.Position (0);
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

        public void Draw () {
            GL.GlUseProgram (renderProgram);

            // Set the active texture unit to texture unit 0.

            int PositionHandle = GL.GlGetAttribLocation (renderProgram, "vPosition");
            GL.GlEnableVertexAttribArray (PositionHandle);
            GL.GlVertexAttribPointer (PositionHandle, 2, GL.GlFloat, false, 2 * sizeof (float), vertexBuffer);

            int MVPMatrixHandle = GL.GlGetUniformLocation (renderProgram, "uMVPMatrix");
            GL.GlUniformMatrix4fv (MVPMatrixHandle, 1, false, Content.Camera.MapMVPMatrix, 0);

            GL.GlEnable (GL.GlBlend);
            GL.GlBlendFunc (GL.GlSrcAlpha, GL.GlOneMinusSrcAlpha);

            int mTextureUniformHandle = GL.GlGetUniformLocation (renderProgram, "u_Texture");
            int mTextureCoordinateHandle = GL.GlGetAttribLocation (renderProgram, "a_TexCoordinate");
            GL.GlVertexAttribPointer (mTextureCoordinateHandle, 2, GL.GlFloat, false, 0, textureBuffer);
            GL.GlEnableVertexAttribArray (mTextureCoordinateHandle);

            GL.GlActiveTexture (GL.GlTexture2);
            GL.GlBindTexture (GL.GlTexture2d, this.TileManager.Texture);
            GL.GlUniform1i (mTextureUniformHandle, 2);

            GL.GlDrawElements (GL.GlTriangles, indexBuffer.Limit (), GL.GlUnsignedShort, indexBuffer);
            GL.GlDisableVertexAttribArray (PositionHandle);
            GL.GlDisableVertexAttribArray (mTextureCoordinateHandle);
        }
    }
}
