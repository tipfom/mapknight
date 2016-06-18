using System;
using System.Collections.Generic;
using System.Text;
using mapKnight.Core;
using System.IO;
using static mapKnight.Extended.Graphics.Programs.ColorProgram;
using OpenTK.Graphics.ES20;

namespace mapKnight.Extended.Graphics {
    public class Map : Core.Map {
        const float DRAW_WIDTH = 18;

        private ColorBufferBatch buffer;
        private Texture2D texture;
        private float[ ][ ][ ] layerBuffer;

        public Size DrawSize { get; private set; }
        public float VertexSize { get; private set; }

        public Map (Stream input) : base(input) {
            DrawSize = new Size((int)DRAW_WIDTH + 2, (int)(DRAW_WIDTH / Window.Ratio) + 2);
            VertexSize = 2 * Window.Ratio / DRAW_WIDTH;
            buffer = new ColorBufferBatch(DrawSize.Area * 3, 2);
            for (int i = 0; i < buffer.Color.Length; i++) {
                buffer.Color[i] = 1f;
            }
            texture = Assets.Load<Texture2D>(Texture);
            SetVertexCoords( );
            InitTextureCoords( );

            Window.Changed += Window_Changed;
        }

        private void Window_Changed ( ) {
            if ((int)(DRAW_WIDTH / Window.Ratio) + 2 != DrawSize.Height) {
                DrawSize = new Size((int)DRAW_WIDTH + 2, (int)(DRAW_WIDTH / Window.Ratio) + 2);
                buffer = new ColorBufferBatch(DrawSize.Area * 3, 2);
                for(int i = 0; i < buffer.Color.Length; i++) {
                    buffer.Color[i] = 1f;
                }
            }
            VertexSize = 2 * Window.Ratio / DRAW_WIDTH;
            SetVertexCoords( );
        }

        private void SetVertexCoords ( ) {
            int iTileCount = DrawSize.Width * DrawSize.Height;
            float ystart = -DrawSize.Height / 2f * VertexSize;

            for (int i = 0; i < 3; i++) { // PR tile and overlay vertex
                for (int y = 0; y < DrawSize.Height; y++) {
                    for (int x = 0; x < DrawSize.Width; x++) {

                        buffer.Verticies[x * 8 + y * DrawSize.Width * 8 + i * iTileCount * 8 + 0] = -Window.Ratio - VertexSize + (x * VertexSize);
                        buffer.Verticies[x * 8 + y * DrawSize.Width * 8 + i * iTileCount * 8 + 1] = ystart + (y * VertexSize);
                        buffer.Verticies[x * 8 + y * DrawSize.Width * 8 + i * iTileCount * 8 + 2] = -Window.Ratio - VertexSize + (x * VertexSize);
                        buffer.Verticies[x * 8 + y * DrawSize.Width * 8 + i * iTileCount * 8 + 3] = ystart + ((y + 1) * VertexSize);
                        buffer.Verticies[x * 8 + y * DrawSize.Width * 8 + i * iTileCount * 8 + 4] = -Window.Ratio - VertexSize + (x * VertexSize) + VertexSize;
                        buffer.Verticies[x * 8 + y * DrawSize.Width * 8 + i * iTileCount * 8 + 5] = ystart + ((y + 1) * VertexSize);
                        buffer.Verticies[x * 8 + y * DrawSize.Width * 8 + i * iTileCount * 8 + 6] = -Window.Ratio - VertexSize + (x * VertexSize) + VertexSize;
                        buffer.Verticies[x * 8 + y * DrawSize.Width * 8 + i * iTileCount * 8 + 7] = ystart + (y * VertexSize);
                    }
                }
            }
        }

        private void InitTextureCoords ( ) {
            // buffer tile coords
            layerBuffer = new float[3][ ][ ];
            for (int layer = 0; layer < 3; layer++) {
                layerBuffer[layer] = new float[(int)base.Size.Height][ ];
                for (int y = 0; y < this.Size.Height; y++) {
                    layerBuffer[layer][y] = new float[(int)this.Size.Width * 8];
                }
            }

            for (int layer = 0; layer < 3; layer++) {
                for (int y = 0; y < Size.Height; y++) {
                    for (int x = 0; x < Size.Width; x++) {
                        for (int c = 0; c < 8; c++) {
                            layerBuffer[layer][y][x * 8 + c] = this.GetTile(x, y, layer).Texture[c];
                        }
                    }
                }
            }
        }

        public void UpdateTextureBuffer (int x, int y) {
            // insert buffered tile coords to texturebuffer
            for (int layer = 0; layer < 3; layer++) {
                for (int ly = 0; ly < DrawSize.Height; ly++) {
                    Array.Copy(layerBuffer[layer][ly + y], x * 8, buffer.Texture, ly * DrawSize.Width * 8 + layer * DrawSize.Area * 8, DrawSize.Width * 8);
                }
            }
        }

        public void Draw ( ) {
            ErrorCode error = GL.GetErrorCode( );
            Program.Begin( );
            error = GL.GetErrorCode( );

            Program.Draw(buffer, texture, Matrix.Default.MVP, true);
            error = GL.GetErrorCode( );

            Program.End( );
            error = GL.GetErrorCode( );
        }

        public void Update (float dt, int focusEntityID) {
            UpdateTextureBuffer(0, Height - DrawSize.Height);
            // Camera.Update (entities.Find (entity => entity.ID == focusEntityID)?.Transform.Center ?? new Vector2 (0, 0), this);
        }
    }
}