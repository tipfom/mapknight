using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using mapKnight.Core;
using OpenTK.Graphics.ES20;
using static mapKnight.Extended.Graphics.Programs.MatrixProgram;

namespace mapKnight.Extended.Graphics {
    public class Map : Core.Map, IEntityContainer {
        const float DRAW_WIDTH = 18;

        private BufferBatch buffer;
        private Texture2D texture;
        private Matrix matrix = new Matrix( );
        private float[ ][ ][ ] layerBuffer;
        private float yOffsetRaw;
        private float yOffsetTile;
        private int lastSpecies = 0;
        private int lastID = 0;
        private int focusEntityIndex = -1;
        private Vector2 focusCenter;
        private Vector2 updateTile = new Vector2(-1, -1);

        public Size DrawSize { get; private set; }
        public float VertexSize { get; private set; }

        public IEntityRenderer Renderer { get; } = new EntityRenderer( );

        public Vector2 Gravity {
            get {
                return new Vector2(0, -10);
            }
        }

        public Vector2 Bounds { get; private set; }

        public Map (Stream input) : base(input) {
            Bounds = new Vector2(Width, Height);
            DrawSize = new Size((int)DRAW_WIDTH + 2, (int)Math.Ceiling(DRAW_WIDTH / Window.Ratio + 2));
            VertexSize = 2 * Window.Ratio / DRAW_WIDTH;
            buffer = new BufferBatch(DrawSize.Area * 3, 2);
            texture = Assets.Load<Texture2D>(Texture);
            SetVertexCoords( );
            InitTextureCoords( );

            Window.Changed += Window_Changed;
        }

        private void Window_Changed ( ) {
            if ((int)(DRAW_WIDTH / Window.Ratio) + 2 != DrawSize.Height) {
                DrawSize = new Size((int)DRAW_WIDTH + 2, (int)(DRAW_WIDTH / Window.Ratio + 2));
                buffer = new ColorBufferBatch(DrawSize.Area * 3, 2);
            }
            VertexSize = 2 * Window.Ratio / DRAW_WIDTH;
            SetVertexCoords( );
        }

        private void SetVertexCoords ( ) {
            int iTileCount = DrawSize.Width * DrawSize.Height;
            float ystart = -(DrawSize.Height / 2f * VertexSize);
            yOffsetRaw = (VertexSize - Math.Abs(ystart + 1));
            yOffsetTile = yOffsetRaw / VertexSize;

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

        private void UpdateTextureBuffer ( ) {
            // insert buffered tile coords to texturebuffer
            for (int layer = 0; layer < 3; layer++) {
                for (int ly = 0; ly < DrawSize.Height; ly++) {
                    Array.Copy(layerBuffer[layer][ly + (int)updateTile.Y], (int)updateTile.X * 8, buffer.Texture, ly * DrawSize.Width * 8 + layer * DrawSize.Area * 8, DrawSize.Width * 8);
                }
            }
        }

        public void Draw ( ) {
            Program.Begin( );
            Program.Draw(buffer, texture, matrix.MVP, true);
            Program.End( );
            ((EntityRenderer)Renderer).Draw( );
        }

        public void Update (float dt, int focusEntityID) {
            foreach (Entity entity in GetEntities( ))
                entity.Update(dt);
            UpdateFocus( );
        }

        public void Focus (int entityID) {
            focusEntityIndex = entities.FindIndex(entity => entity.ID == entityID);
        }

        private void UpdateFocus ( ) {
            if (focusEntityIndex > -1) {
                Vector2 focusPoint = entities[focusEntityIndex].Transform.Center;
                focusCenter = new Vector2(
                    Mathf.Clamp(focusPoint.X, DrawSize.Width / 2f - 1, Width - DrawSize.Width / 2f + 1),
                    Mathf.Clamp(focusPoint.Y, DrawSize.Height / 2f - 1 + yOffsetTile, Height - DrawSize.Height / 2f + 1 - yOffsetTile)
                    );
                int xClamp = Width - DrawSize.Width, yClamp = Height - DrawSize.Height;
                Vector2 nextTile = new Vector2(
                    Mathf.Clamp(focusPoint.X - DrawSize.Width / 2f, -1, xClamp + 1),
                    Mathf.Clamp(focusPoint.Y - DrawSize.Height / 2f, -1, yClamp + 1));
                matrix.ResetView( );
                float mapOffsetX;
                if (nextTile.X < 0) mapOffsetX = (-nextTile.X) * VertexSize;
                else if (nextTile.X > xClamp) mapOffsetX = -(nextTile.X - xClamp) * VertexSize;
                else mapOffsetX = -((nextTile.X) % 1) * VertexSize;

                float mapOffsetY;
                if (nextTile.Y < 0) mapOffsetY = -nextTile.Y * (VertexSize - yOffsetRaw);
                else if (nextTile.Y >= yClamp) mapOffsetY = -(nextTile.Y - yClamp) * (VertexSize - yOffsetRaw);
                else mapOffsetY = -((nextTile.Y) % 1) * VertexSize;

                matrix.TranslateView(mapOffsetX, mapOffsetY, 0);
                matrix.CalculateMVP( );

                Vector2 intNextTile = new Vector2((int)Mathf.Clamp(nextTile.X, 0, Width - DrawSize.Width), (int)Mathf.Clamp(nextTile.Y, 0, Height - DrawSize.Height));
                if (updateTile != intNextTile) {
                    updateTile = intNextTile;
                    UpdateTextureBuffer( );
                }
            }
        }

        public Vector2 GetPositionOnScreen (Entity entity) {
            return (entity.Transform.Center - focusCenter) * VertexSize;
        }
        List<Entity> entities = new List<Entity>( );
        public List<Entity> GetEntities ( ) {
            return entities;
        }

        public List<Entity> GetEntities (Predicate<Entity> predicate) {
            return entities.FindAll(predicate);
        }

        public void Add (Entity entity) {
            entities.Add(entity);
        }

        public bool HasCollider (int x, int y) {
            return GetTile(x, y).HasFlag(TileAttribute.Collision);
        }

        public bool IsOnScreen (Entity entity) {
            return true;
        }

        public int NewSpecies ( ) {
            return ++lastSpecies;
        }

        public int NewID ( ) {
            return ++lastID;
        }
    }
}