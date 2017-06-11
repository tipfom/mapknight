using System;
using System.Collections.Generic;
using System.ComponentModel;
using mapKnight.Core;
using mapKnight.ToolKit.Controls.Xna;
using Microsoft.Xna.Framework.Graphics;
using Color = Microsoft.Xna.Framework.Color;
using Point = Microsoft.Xna.Framework.Point;
using Vector2 = Microsoft.Xna.Framework.Vector2;
using Rectangle = Microsoft.Xna.Framework.Rectangle;
using mapKnight.Core.World;
using System.IO;
using mapKnight.Core.World.Components;
using mapKnight.ToolKit.Data;

namespace mapKnight.ToolKit.Controls {
    public class TileMapView : XnaControl {
        const int TILESIZE_MIN = 20;
        const int TILESIZE_MAX = 100;
        const int ZOOM_LEVELS = 30;

        private Vector2 _Offset;
        public Vector2 Offset {
            get { return _Offset; }
            set { _Offset = value; Update( ); }
        }

        private int _ZoomLevel = 6;
        public int ZoomLevel {
            get { return _ZoomLevel; }
            set {
                _ZoomLevel = (int)Mathf.Clamp(value, 0, ZOOM_LEVELS);
                float tileSizeDelta = _TileSize;
                Vector2 prevOffset = _Offset;
                _TileSize = (int)Mathf.Interpolate(TILESIZE_MIN, TILESIZE_MAX, (float)_ZoomLevel / ZOOM_LEVELS);
                tileSizeDelta -= _TileSize;
                _Offset.X -= tileSizeDelta * CurrentSelection.X / _TileSize;
                _Offset.Y -= tileSizeDelta * CurrentSelection.Y / _TileSize;
                _CurrentSelection.X += prevOffset.X - _Offset.X;
                _CurrentSelection.Y += prevOffset.Y - _Offset.Y;
                Update( );
            }
        }

        private int _TileSize = 40;
        public int TileSize {
            get { return _TileSize; }
        }

        private Vector2 _CurrentSelection = new Vector2(-1, -1);
        public Vector2 CurrentSelection {
            get { return _CurrentSelection; }
            set { _CurrentSelection = value; }
        }

        private bool[ ] Layer = { true, true, true };
        public bool ShowForeground {
            get { return Layer[2]; }
            set { Layer[2] = value; Update( ); }
        }
        public bool ShowBackground {
            get { return Layer[0]; }
            set { Layer[0] = value; Update( ); }
        }
        public bool ShowMiddle {
            get { return Layer[1]; }
            set { Layer[1] = value; Update( ); }
        }
        public int Mode { get; set; }
        public EditorMap Map;

        public Action<SpriteBatch, float, float, int> AdditionalRenderCall { get; set; }

        private Texture2D selectionTexture;
        private Texture2D spawnpointTexture;
        private Texture2D emptyTexture;
        public Dictionary<string, Texture2D> EntityData;

        public TileMapView ( ) {
            DeviceInitialized += ( ) => {
                CreateEmptyTexture( );
            };
            Loaded += (sender, e) => {
                Focus( );
            };
        }
        
        public bool IsLayerActive (int id) {
            return !(id < 0) || !(id > 3) || Layer[id];
        }

        private void CreateEmptyTexture ( ) {
            if (DesignerProperties.GetIsInDesignMode(this))
                return;

            selectionTexture = new Texture2D(GraphicsDevice, 1, 1);
            selectionTexture.SetData(new Color[ ] { new Color(255, 0, 0, 63) });

            spawnpointTexture = new Texture2D(GraphicsDevice, 1, 1);
            spawnpointTexture.SetData(new Color[ ] { new Color(0, 255, 0, 63) });

            emptyTexture = new Texture2D(GraphicsDevice, 1, 1);
            emptyTexture.SetData(new Color[ ] { Color.White });
        }

        protected override void Render (SpriteBatch spriteBatch) {
            if (Map == null)
                return;

            int columns = (int)(RenderSize.Width / TileSize + 2);
            int rows = (int)(RenderSize.Height / TileSize + 2);
            DrawLayer(0, columns, rows, Offset.X, Offset.Y, spriteBatch);
            DrawLayer(1, columns, rows, Offset.X, Offset.Y, spriteBatch);
            foreach (Entity e in Map.Entities) {
                Texture2D texture = EntityData[e.Name];
                Color color = e.Domain == EntityDomain.Temporary ? new Color(64, 64, 64, 128) : Color.White;
                Rectangle entityRectangle = new Rectangle((int)((e.Transform.X - Offset.X) * TileSize), (int)((Map.Height - e.Transform.Y - Offset.Y) * TileSize), (int)(e.Transform.Size.X * TileSize), (int)(e.Transform.Size.Y * TileSize));
                SpriteEffects effect = SpriteEffects.None;
                float rotation = 0f;
                e.Draw( );
                if (e.HasComponentInfo(ComponentData.Texture)) {
                    object[ ] data = e.GetComponentInfo(ComponentData.Texture);
                    effect = (SpriteEffects)data[0];
                    rotation = (float)data[1];
                }
                switch (e.Domain) {
                    case EntityDomain.Obstacle:
                        spriteBatch.Draw(emptyTexture, entityRectangle, null, new Color(0, 0, 128, 128), rotation, new Vector2(.5f, .5f), effect, 0);
                        break;
                    case EntityDomain.NPC:
                        spriteBatch.Draw(emptyTexture, entityRectangle, null, new Color(128, 0, 0, 128), rotation, new Vector2(.5f, .5f), effect, 0);
                        break;
                }
                spriteBatch.Draw(texture, entityRectangle, null, color, rotation, new Vector2(texture.Width / 2f, texture.Height / 2f), effect, 0);
            }
            DrawLayer(2, columns, rows, Offset.X, Offset.Y, spriteBatch);

            // draw selected tile
            if (CurrentSelection.X > -1 && CurrentSelection.Y > -1) {
                if (Mode == 0 || Mode == 2) {
                    spriteBatch.Draw(selectionTexture, new Rectangle((int)((CurrentSelection.X + 0.5f) * TileSize), (int)((CurrentSelection.Y + 0.5f) * TileSize), TileSize, TileSize), null, Color.White, 0f, new Vector2(0.5f, 0.5f), SpriteEffects.None, 0);
                } else if (Mode == 1) {
                    spriteBatch.Draw(emptyTexture, new Rectangle((int)((CurrentSelection.X) * TileSize), (int)((CurrentSelection.Y) * TileSize), TileSize / 5, TileSize / 5), null, new Color(255, 0, 0, 63), Mathf.PI / 4f, new Vector2(0.5f, 0.5f), SpriteEffects.None, 0);
                }
            }

            // draw spawnpoint tile
            spriteBatch.Draw(spawnpointTexture, new Rectangle((int)((Map.SpawnPoint.X - Offset.X) * TileSize), (int)((Map.Height - Map.SpawnPoint.Y - 1 - Offset.Y) * TileSize), TileSize, TileSize), Color.White);

            AdditionalRenderCall?.Invoke(spriteBatch, Offset.X, Offset.Y, TileSize);
        }

        private void DrawLayer (int layer, int columns, int rows, float offsetx, float offsety, SpriteBatch spriteBatch) {
            if (!Layer[layer])
                return;
            for (int x = -1; x < columns; x++) {
                int tx = x + (int)Math.Floor(offsetx);
                if (tx < 0 || tx >= Map.Width)
                    continue;
                for (int y = 0; y < rows; y++) {
                    int ty = Map.Height - y - 1 - (int)Math.Floor(offsety);
                    if (ty < 0 || ty >= Map.Height)
                        continue;
                    Rectangle drawingRectangle = new Rectangle((int)((-offsetx + Math.Floor(offsetx) + x + .5f) * TileSize), (int)((-offsety + Math.Floor(offsety) + y + .5f) * TileSize), TileSize, TileSize);
                    float rotation = Map.Rotations[tx, ty, layer] * (float)Math.PI;
                    spriteBatch.Draw(Map.XnaTextures[Map.GetTile(tx, ty, layer).Name], drawingRectangle, null, Color.White, rotation, new Vector2(Core.Map.TILE_PXL_SIZE / 2f, Core.Map.TILE_PXL_SIZE / 2f), SpriteEffects.None, 0);
                }
            }
        }
    }
}
