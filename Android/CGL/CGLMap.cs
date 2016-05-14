using Java.Nio;
using mapKnight.Android.Config;
using mapKnight.Android.ECS;
using mapKnight.Android.Map;
using mapKnight.Basic;
using System;
using System.Collections.Generic;

namespace mapKnight.Android.CGL {
    public class CGLMap : Map.Map, IEntityContainer {
        public const int DRAW_WIDTH = 18;

        public enum UpdateType {
            Complete = 0,
            RemoveTop = -1,
            RemoveBottom = 1
        }

        FloatBuffer vertexBuffer;
        ShortBuffer indexBuffer;
        FloatBuffer textureBuffer;

        private float[][][] layerBuffer; // buffers each texturecoordinate for every layer

        public Size DrawSize { get; private set; }

        public Vector2 Gravity { get; set; }

        public Vector2 Bounds { get; private set; }

        private CGLEntityRenderer entityRenderer = new CGLEntityRenderer ();
        public IEntityRenderer Renderer { get { return entityRenderer; } }

        public CGLCamera Camera { get; private set; }

        private List<Entity> entities = new List<Entity> ();

        public CGLMap (GameConfig config) : this (config.Map, new CGLCamera (config.CharacterOffset)) {

        }

        public CGLMap (string name, CGLCamera camera) : base (name) {
            Bounds = new Vector2 (base.Size.Width - 1, base.Size.Height - 1);
            Gravity = new Vector2 (0, -10);
            Camera = camera;

            DrawSize = new Size (DRAW_WIDTH + 2, (int)((float)DRAW_WIDTH / Screen.ScreenRatio) + 2);
            VertexSize = 2 * Screen.ScreenRatio / (float)(DRAW_WIDTH);

            setVertexCoords ();
            initTextureBuffer ();
            initTextureCoords ();

            Screen.Changed += () => {
                DrawSize = new Size (DRAW_WIDTH + 2, (int)Math.Ceiling (DRAW_WIDTH / Screen.ScreenRatio) + 2);
                VertexSize = 2 * Screen.ScreenRatio / (float)(DRAW_WIDTH);
                initTextureBuffer ();
                setVertexCoords ();
            };
        }

        private void setVertexCoords () {
            int iTileCount = DrawSize.Width * DrawSize.Height;
            float[] vertexCoords = new float[iTileCount * 8 * 3];
            short[] vertexIndices = new short[iTileCount * 6 * 3];

            float ystart = -DrawSize.Height / 2f * VertexSize;

            for (int i = 0; i < 3; i++) { // PR tile and overlay vertex
                for (int y = 0; y < DrawSize.Height; y++) {
                    for (int x = 0; x < DrawSize.Width; x++) {

                        vertexCoords[x * 8 + y * DrawSize.Width * 8 + i * iTileCount * 8 + 0] = -Screen.ScreenRatio - VertexSize + (x * VertexSize);
                        vertexCoords[x * 8 + y * DrawSize.Width * 8 + i * iTileCount * 8 + 1] = ystart + (y * VertexSize);
                        vertexCoords[x * 8 + y * DrawSize.Width * 8 + i * iTileCount * 8 + 2] = -Screen.ScreenRatio - VertexSize + (x * VertexSize);
                        vertexCoords[x * 8 + y * DrawSize.Width * 8 + i * iTileCount * 8 + 3] = ystart + ((y + 1) * VertexSize);
                        vertexCoords[x * 8 + y * DrawSize.Width * 8 + i * iTileCount * 8 + 4] = -Screen.ScreenRatio - VertexSize + (x * VertexSize) + VertexSize;
                        vertexCoords[x * 8 + y * DrawSize.Width * 8 + i * iTileCount * 8 + 5] = ystart + ((y + 1) * VertexSize);
                        vertexCoords[x * 8 + y * DrawSize.Width * 8 + i * iTileCount * 8 + 6] = -Screen.ScreenRatio - VertexSize + (x * VertexSize) + VertexSize;
                        vertexCoords[x * 8 + y * DrawSize.Width * 8 + i * iTileCount * 8 + 7] = ystart + (y * VertexSize);

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

        private void initTextureBuffer () {
            //init Texture Buffer
            textureBuffer = CGLTools.CreateFloatBuffer (DrawSize.Width * DrawSize.Height * 8 * 3);
        }

        private void initTextureCoords () {
            // buffer tile coords
            layerBuffer = new float[3][][];
            for (int layer = 0; layer < 3; layer++) {
                layerBuffer[layer] = new float[(int)base.Size.Height][];
                for (int y = 0; y < this.Size.Height; y++) {
                    layerBuffer[layer][y] = new float[(int)this.Size.Width * 8];
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

        public void UpdateTextureBuffer () {
            // insert buffered tile coords to texturebuffer
            for (int layer = 0; layer < 3; layer++) {
                for (int y = 0; y < DrawSize.Height; y++) {
                    textureBuffer.Put (layerBuffer[layer][(int)Camera.CurrentMapTile.Y + y].Cut ((int)Camera.CurrentMapTile.X * 8, DrawSize.Width * 8));
                }
            }
            textureBuffer.Position (0);
        }

        public void Draw () {
            Content.ProgramCollection.Matrix.Begin ();

            Content.ProgramCollection.Matrix.Draw (vertexBuffer, textureBuffer, indexBuffer, TileManager.Texture.Texture, Camera.MapMatrix.MVP, true);

            Content.ProgramCollection.Matrix.End ();

            entityRenderer.Draw ();
        }

        public void Update (float dt, int focusEntityID) {
            foreach (Entity entity in entities) {
                entity.Update (dt);
            }
            entityRenderer.Update ();

            Camera.Update (entities.Find (entity => entity.ID == focusEntityID)?.Transform.Center ?? new Vector2 (0, 0), this);
        }

        public bool HasCollider (int x, int y) {
            return base.GetTile (x, y).Mask.HasFlag (TileMask.Collision);
        }

        private int currentID = 0;
        public int CreateID () {
            return ++currentID;
        }

        public void Add (Entity entity) {
            entities.Add (entity);
        }

        public bool IsOnScreen (Entity entity) {
            return (entity.Transform.Center - Camera.ScreenCentre).Abs () < Camera.DrawRange;
        }

        public Vector2 GetPositionOnScreen (Entity entity) {
            return (entity.Transform.Center - Camera.ScreenCentre) * this.VertexSize;
        }

        public void Prepare () {
            foreach (Entity entity in entities) {
                entity.Prepare ();
            }
        }
    }
}