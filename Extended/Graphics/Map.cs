using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using mapKnight.Core;
using mapKnight.Extended.Graphics.Buffer;
using mapKnight.Extended.Graphics.Particles;
using OpenTK.Graphics.ES20;
using static mapKnight.Extended.Graphics.Programs.MatrixProgram;

namespace mapKnight.Extended.Graphics {
    public class Map : Core.Map, IEntityWorld {
        const int DRAW_WIDTH = 13;

        private BufferBatch mainBuffer, foregroundBuffer;
        private Texture2D texture;
        private Matrix matrix = new Matrix(new Vector2(Window.Ratio, 1));
        private float[ ][ ][ ] layerBuffer;
        private float yOffsetRaw;
        private float yOffsetTile;

        private float yVertexSize;
        private Entity focusEntity;
        private Vector2 focusCenter;
        private Vector2 updateTile = new Vector2(-1, -1);

        public Size DrawSize { get; private set; }
        public float VertexSize { get; private set; }

        public IEntityRenderer Renderer { get; } = new EntityRenderer( );

        private CachedGPUBuffer foregroundTextureBuffer { get { return (CachedGPUBuffer)foregroundBuffer.TextureBuffer; } }
        private CachedGPUBuffer mainTextureBuffer { get { return (CachedGPUBuffer)mainBuffer.TextureBuffer; } }

        public Map (Stream input) : base(input) {
            Emitter.Matrix = new Matrix(new Vector2(DRAW_WIDTH / 2f, DRAW_WIDTH / Window.Ratio / 2f));
            Window_Changed( );
            InitTextureCoords( );

            texture = Assets.Load<Texture2D>(Texture);

            Window.Changed += Window_Changed;
        }

        private void Window_Changed ( ) {
            matrix.UpdateProjection(Window.ProjectionSize);
            matrix.CalculateMVP( );

            if (2 * Window.Ratio / DRAW_WIDTH != VertexSize) {
                VertexSize = 2 * Window.Ratio / DRAW_WIDTH;
                DrawSize = new Size(DRAW_WIDTH + 2, Mathi.Ceil(DRAW_WIDTH / Window.Ratio + 2));
                mainBuffer?.Dispose( );
                foregroundBuffer?.Dispose( );
                mainBuffer = new BufferBatch(new IndexBuffer(DrawSize.Area * 2), new GPUBuffer(2, DrawSize.Area * 2, PrimitiveType.Quad, GenerateMainVerticies( )), new CachedGPUBuffer(2, DrawSize.Area * 2, PrimitiveType.Quad));
                foregroundBuffer = new BufferBatch(new IndexBuffer(DrawSize.Area), new GPUBuffer(2, DrawSize.Area, PrimitiveType.Quad, GenerateForegroundVerticies( )), new CachedGPUBuffer(2, DrawSize.Area, PrimitiveType.Quad));
            }
            UpdateEmitterMatrix( );
        }

        private void UpdateEmitterMatrix ( ) {
            Emitter.Matrix.UpdateProjection(new Vector2(DRAW_WIDTH / 2f, DRAW_WIDTH / Window.Ratio / 2f));
            Emitter.Matrix.CalculateMVP( );
        }

        private float[ ] GenerateMainVerticies ( ) {
            int tileCount = DrawSize.Area;

            float ystart = -(DrawSize.Height / 2f * VertexSize);
            yOffsetRaw = (VertexSize - Math.Abs(ystart + 1));
            yOffsetTile = yOffsetRaw / VertexSize;
            yVertexSize = VertexSize - yOffsetRaw;

            float[ ] verticies = new float[tileCount * 2 * 2 * 4];
            for (int i = 0; i < 2; i++) { // PR tile and overlay vertex
                for (int y = 0; y < DrawSize.Height; y++) {
                    for (int x = 0; x < DrawSize.Width; x++) {
                        verticies[x * 8 + y * DrawSize.Width * 8 + i * tileCount * 8 + 0] = -Window.Ratio - VertexSize + (x * VertexSize);
                        verticies[x * 8 + y * DrawSize.Width * 8 + i * tileCount * 8 + 1] = ystart + (y * VertexSize);
                        verticies[x * 8 + y * DrawSize.Width * 8 + i * tileCount * 8 + 2] = -Window.Ratio - VertexSize + (x * VertexSize);
                        verticies[x * 8 + y * DrawSize.Width * 8 + i * tileCount * 8 + 3] = ystart + ((y + 1) * VertexSize);
                        verticies[x * 8 + y * DrawSize.Width * 8 + i * tileCount * 8 + 4] = -Window.Ratio - VertexSize + (x * VertexSize) + VertexSize;
                        verticies[x * 8 + y * DrawSize.Width * 8 + i * tileCount * 8 + 5] = ystart + ((y + 1) * VertexSize);
                        verticies[x * 8 + y * DrawSize.Width * 8 + i * tileCount * 8 + 6] = -Window.Ratio - VertexSize + (x * VertexSize) + VertexSize;
                        verticies[x * 8 + y * DrawSize.Width * 8 + i * tileCount * 8 + 7] = ystart + (y * VertexSize);
                    }
                }
            }
            return verticies;
        }

        private float[ ] GenerateForegroundVerticies ( ) {
            int tileCount = DrawSize.Area;
            float ystart = -(DrawSize.Height / 2f * VertexSize);

            float[ ] verticies = new float[tileCount * 1 * 2 * 4];
            for (int y = 0; y < DrawSize.Height; y++) {
                for (int x = 0; x < DrawSize.Width; x++) {
                    verticies[x * 8 + y * DrawSize.Width * 8 + 0] = -Window.Ratio - VertexSize + (x * VertexSize);
                    verticies[x * 8 + y * DrawSize.Width * 8 + 1] = ystart + (y * VertexSize);
                    verticies[x * 8 + y * DrawSize.Width * 8 + 2] = -Window.Ratio - VertexSize + (x * VertexSize);
                    verticies[x * 8 + y * DrawSize.Width * 8 + 3] = ystart + ((y + 1) * VertexSize);
                    verticies[x * 8 + y * DrawSize.Width * 8 + 4] = -Window.Ratio - VertexSize + (x * VertexSize) + VertexSize;
                    verticies[x * 8 + y * DrawSize.Width * 8 + 5] = ystart + ((y + 1) * VertexSize);
                    verticies[x * 8 + y * DrawSize.Width * 8 + 6] = -Window.Ratio - VertexSize + (x * VertexSize) + VertexSize;
                    verticies[x * 8 + y * DrawSize.Width * 8 + 7] = ystart + (y * VertexSize);
                }
            }
            return verticies;
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
                for (int y = 0; y < base.Size.Height; y++) {
                    for (int x = 0; x < base.Size.Width; x++) {
                        for (int c = 0; c < 8; c++) {
                            layerBuffer[layer][y][x * 8 + c] = this.GetTile(x, y, layer).Texture[c];
                        }
                    }
                }
            }
        }

        private void UpdateTextureBuffer ( ) {
            for (int layer = 0; layer < 2; layer++) {
                for (int ly = 0; ly < DrawSize.Height; ly++) {
                    Array.Copy(layerBuffer[layer][ly + (int)updateTile.Y], (int)updateTile.X * 8, mainTextureBuffer.Cache, ly * DrawSize.Width * 8 + layer * DrawSize.Area * 8, DrawSize.Width * 8);
                }
            }
            for (int ly = 0; ly < DrawSize.Height; ly++) {
                Array.Copy(layerBuffer[2][ly + (int)updateTile.Y], (int)updateTile.X * 8, foregroundTextureBuffer.Cache, ly * DrawSize.Width * 8, DrawSize.Width * 8);
            }

            mainTextureBuffer.Apply( );
            foregroundTextureBuffer.Apply( );
        }

        public void Draw ( ) {
            Program.Begin( );
            Program.Draw(mainBuffer, texture, matrix, true);
            Program.End( );
            ((EntityRenderer)Renderer).Draw( );
            Program.Begin( );
            Program.Draw(foregroundBuffer, texture, matrix, true);
            Program.End( );
        }

        public void Update (DeltaTime dt) {
            Entity.UpdateAll(dt);
            UpdateFocus( );
            Entity.PostUpdateAll( );
        }

        public void Focus (int entityID) {
            focusEntity = Entity.Entities.Find(entity => entity.ID == entityID);
            focusEntity.Destroyed += ( ) => { focusEntity = null; };
        }

        private void UpdateFocus ( ) {
            if (focusEntity != null) {
                Vector2 focusPoint = focusEntity.Transform.Center;
                focusCenter = new Vector2(
                    Mathf.Clamp(focusPoint.X, DrawSize.Width / 2f - 1, Width - DrawSize.Width / 2f + 1),
                    Mathf.Clamp(focusPoint.Y, DrawSize.Height / 2f - 1 + yOffsetTile, Height - DrawSize.Height / 2f + 1)
                    );
                int xClamp = Width - DrawSize.Width, yClamp = Height - DrawSize.Height;
                Vector2 nextTile = new Vector2(
                    Mathf.Clamp(focusPoint.X - DrawSize.Width / 2f, -1, xClamp + 1),
                    Mathf.Clamp(focusPoint.Y - DrawSize.Height / 2f, -1, yClamp + 1));

                float mapOffsetX;
                if (nextTile.X == -1) mapOffsetX = VertexSize;
                else if (nextTile.X > xClamp) mapOffsetX = (xClamp - nextTile.X) * VertexSize;
                else mapOffsetX = -((nextTile.X) % 1) * VertexSize;

                float mapOffsetY;
                if (nextTile.Y < yOffsetTile) mapOffsetY = -nextTile.Y * yVertexSize;
                else if (nextTile.Y >= yClamp) mapOffsetY = (yClamp - nextTile.Y) * yVertexSize;
                else mapOffsetY = -((nextTile.Y) % 1) * VertexSize;

                matrix.ResetView( );
                matrix.TranslateView(mapOffsetX, mapOffsetY, 0);
                matrix.CalculateMVP( );

                Emitter.Matrix.ResetView( );
                Emitter.Matrix.TranslateView(-nextTile.X - DrawSize.Width / 2f, -nextTile.Y - DrawSize.Height / 2f, 0);
                Emitter.Matrix.CalculateMVP( );

                Vector2 intNextTile = new Vector2(Mathi.Clamp((int)nextTile.X, 0, Width - DrawSize.Width), Mathi.Clamp((int)nextTile.Y, 0, Height - DrawSize.Height));
                if (updateTile != intNextTile) {
                    updateTile = intNextTile;
                    UpdateTextureBuffer( );
                }
            }
        }

        public Vector2 GetPositionOnScreen (Entity entity) {
            return (entity.Transform.Center - focusCenter) * VertexSize;
        }

        public bool HasCollider (int x, int y) {
            return GetTile(x, y).HasFlag(TileAttribute.Collision);
        }

        public bool IsOnScreen (Entity entity) {
            return true;
        }
    }
}