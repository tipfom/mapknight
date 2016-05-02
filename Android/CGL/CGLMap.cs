using Java.Nio;
using mapKnight.Basic;

namespace mapKnight.Android.CGL {
    public class CGLMap : Physics.Map {
        public const int DRAW_WIDTH = 18;

        public enum UpdateType {
            Complete = 0,
            RemoveTop = -1,
            RemoveBottom = 1
        }

        FloatBuffer vertexBuffer;
        ShortBuffer indexBuffer;
        FloatBuffer textureBuffer;

        private float[ ][ ][ ] layerBuffer; // buffers each texturecoordinate for every layer

        public readonly Size DrawSize;
        public readonly fSize RealDrawSize;
        public float VertexSize;

        public CGLMap (string name) : base (name) {
            RealDrawSize = new fSize (DRAW_WIDTH, DRAW_WIDTH / Screen.ScreenRatio);
            DrawSize = new Size (DRAW_WIDTH + 2, (int)((float)DRAW_WIDTH / Screen.ScreenRatio) + 2);
            VertexSize = 2 * Screen.ScreenRatio / (float)(DRAW_WIDTH);

            setVertexCoords ( );
            initTextureCoords ( );

            Screen.Changed += () => {
                VertexSize = 2 * Screen.ScreenRatio / (float)(DRAW_WIDTH);
                setVertexCoords ( );
            };
        }

        private void setVertexCoords () {
            int iTileCount = DrawSize.Width * DrawSize.Height;
            float[ ] vertexCoords = new float[iTileCount * 8 * 3];
            short[ ] vertexIndices = new short[iTileCount * 6 * 3];

            for (int i = 0; i < 3; i++) { // PR tile and overlay vertex
                for (int y = 0; y < DrawSize.Height; y++) {
                    for (int x = 0; x < DrawSize.Width; x++) {

                        vertexCoords[x * 8 + y * DrawSize.Width * 8 + i * iTileCount * 8 + 0] = -Screen.ScreenRatio - VertexSize + (x * VertexSize);
                        vertexCoords[x * 8 + y * DrawSize.Width * 8 + i * iTileCount * 8 + 1] = -1f - VertexSize + (y * VertexSize);
                        vertexCoords[x * 8 + y * DrawSize.Width * 8 + i * iTileCount * 8 + 2] = -Screen.ScreenRatio - VertexSize + (x * VertexSize);
                        vertexCoords[x * 8 + y * DrawSize.Width * 8 + i * iTileCount * 8 + 3] = -1f - VertexSize + ((y + 1) * VertexSize);
                        vertexCoords[x * 8 + y * DrawSize.Width * 8 + i * iTileCount * 8 + 4] = -Screen.ScreenRatio - VertexSize + (x * VertexSize) + VertexSize;
                        vertexCoords[x * 8 + y * DrawSize.Width * 8 + i * iTileCount * 8 + 5] = -1f - VertexSize + ((y + 1) * VertexSize);
                        vertexCoords[x * 8 + y * DrawSize.Width * 8 + i * iTileCount * 8 + 6] = -Screen.ScreenRatio - VertexSize + (x * VertexSize) + VertexSize;
                        vertexCoords[x * 8 + y * DrawSize.Width * 8 + i * iTileCount * 8 + 7] = -1f - VertexSize + (y * VertexSize);

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
            textureBuffer = CGLTools.CreateFloatBuffer (DrawSize.Width * DrawSize.Height * 8 * 3);

            // buffer tile coords
            layerBuffer = new float[3][ ][ ];
            for (int layer = 0; layer < 3; layer++) {
                layerBuffer[layer] = new float[this.Size.Height][ ];
                for (int y = 0; y < this.Size.Height; y++) {
                    layerBuffer[layer][y] = new float[this.Size.Width * 8];
                }
            }

            for (int layer = 0; layer < 3; layer++) {
                for (int y = 0; y < Size.Height; y++) {
                    for (int x = 0; x < Size.Width; x++) {
                        for (int c = 0; c < 8; c++) {
                            layerBuffer[layer][y][x * 8 + c] = this.GetTile (x, y, layer).Texture[c];
                        }
                    }
                }
            }
        }

        public void updateTextureBuffer (CGLCamera camera) {
            // insert buffered tile coords to texturebuffer
            for (int layer = 0; layer < 3; layer++) {
                for (int y = 0; y < DrawSize.Height; y++) {
                    textureBuffer.Put (layerBuffer[layer][camera.CurrentMapTile.Y + y].Cut (camera.CurrentMapTile.X * 8, DrawSize.Width * 8));
                }
            }
            textureBuffer.Position (0);
        }

        public void Draw (CGLCamera camera) {
            Content.MatrixProgram.Begin ( );

            Content.MatrixProgram.Draw (vertexBuffer, textureBuffer, indexBuffer, TileManager.Texture.Texture, camera.MapMatrix.MVP, true);

            Content.MatrixProgram.End ( );
        }
    }
}
