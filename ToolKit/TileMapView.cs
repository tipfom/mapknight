using System;
using System.ComponentModel;
using mapKnight.Core;
using mapKnight.ToolKit.Xna;
using Microsoft.Xna.Framework.Graphics;
using Color = Microsoft.Xna.Framework.Color;
using Point = Microsoft.Xna.Framework.Point;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace mapKnight.ToolKit {
    public class TileMapView : XnaControl {
        const int TILESIZE_MIN = 20;
        const int TILESIZE_MAX = 100;
        const int ZOOM_LEVELS = 15;

        private Map _CurrentMap;
        public Map CurrentMap {
            get { return _CurrentMap; }
            set { _CurrentMap = value; Update( ); }
        }

        private Point _Offset;
        public Point Offset {
            get { return _Offset; }
            set { _Offset = value; Update( ); }
        }


        private int _ZoomLevel = 6;
        public int ZoomLevel {
            get { return _ZoomLevel; }
            set {
                _ZoomLevel = (int)Mathf.Clamp(value, 0, ZOOM_LEVELS);
                _TileSize = (int)Mathf.Interpolate(TILESIZE_MIN, TILESIZE_MAX, (float)_ZoomLevel / ZOOM_LEVELS);
                Update( );
            }
        }

        private int _TileSize = 40;
        public int TileSize {
            get { return _TileSize; }
        }

        private Point _CurrentSelection = new Point(-1, -1);
        public Point CurrentSelection {
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

        private Texture2D selectionTexture;
        private Texture2D spawnpointTexture;

        public TileMapView ( ) {
            base.DeviceInitialized += ( ) => {
                CreateEmptyTexture( );
            };
            Loaded += (sender, e) => {
                this.Focus( );
            };
        }

        public bool IsLayerActive (int id) {
            return id < 0 || id > 3 || Layer[id];
        }

        private void CreateEmptyTexture ( ) {
            if (DesignerProperties.GetIsInDesignMode(this))
                return;

            selectionTexture = new Texture2D(GraphicsDevice, 1, 1);
            selectionTexture.SetData(new Color[ ] { new Color(255, 0, 0, 63) });

            spawnpointTexture = new Texture2D(GraphicsDevice, 1, 1);
            spawnpointTexture.SetData(new Color[ ] { new Color(0, 255, 0, 63) });
        }

        protected override void Render (SpriteBatch spriteBatch) {
            if (CurrentMap == null)
                return;

            int columns = (int)Math.Min(RenderSize.Width / TileSize + 1, CurrentMap.Width - Offset.X);
            int rows = (int)Math.Min(RenderSize.Height / TileSize + 1, CurrentMap.Height - Offset.Y);
            for (int x = 0; x < columns; x++) {
                for (int y = 0; y < rows; y++) {
                    int cx = x + Offset.X, cy = CurrentMap.Height - y - 1 - Offset.Y;
                    Rectangle drawingRectangle = new Rectangle((int)((x + 0.5f) * TileSize), (int)((y + 0.5f) * TileSize), TileSize, TileSize);
                    for (int l = 0; l < 3; l++) {
                        if (Layer[l]) {
                            float rotation = CurrentMap.GetRotation(cx, cy, l) * (float)Math.PI;
                            spriteBatch.Draw(App.Project.GetMapXNATextures(CurrentMap)[CurrentMap.GetTile(cx, cy, l).Name], drawingRectangle, null, Color.White, rotation, new Microsoft.Xna.Framework.Vector2(Map.TILE_PXL_SIZE / 2f, Map.TILE_PXL_SIZE / 2f), SpriteEffects.None, 0);
                        }
                    }
                }
            }

            // draw selected tile
            if (CurrentSelection.X > -1 && CurrentSelection.Y > -1)
                spriteBatch.Draw(selectionTexture, new Rectangle(CurrentSelection.X * TileSize, CurrentSelection.Y * TileSize, TileSize, TileSize), Color.White);

            // draw spawnpoint tile
            spriteBatch.Draw(spawnpointTexture, new Rectangle((int)((CurrentMap.SpawnPoint.X - Offset.X) * TileSize), (int)((CurrentMap.Height - CurrentMap.SpawnPoint.Y - 1 - Offset.Y) * TileSize), TileSize, TileSize), Color.White);
        }
    }
}
