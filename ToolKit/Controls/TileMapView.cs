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

namespace mapKnight.ToolKit.Controls
{
    public class TileMapView : XnaControl
    {
        const int TILESIZE_MIN = 20;
        const int TILESIZE_MAX = 100;
        const int ZOOM_LEVELS = 15;

        private Map _CurrentMap;
        public Map CurrentMap {
            get { return _CurrentMap; }
            set { _CurrentMap = value; Update(); }
        }

        private Vector2 _Offset;
        public Vector2 Offset {
            get { return _Offset; }
            set { _Offset = value; Update(); }
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
                _Offset.X = Math.Max(_Offset.X, 0);
                _Offset.Y = Math.Max(_Offset.Y, 0);
                _CurrentSelection.X += (int)prevOffset.X - (int)_Offset.X;
                _CurrentSelection.Y += (int)prevOffset.Y - (int)_Offset.Y;
                Update();
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

        private bool[] Layer = { true, true, true };
        public bool ShowForeground {
            get { return Layer[2]; }
            set { Layer[2] = value; Update(); }
        }
        public bool ShowBackground {
            get { return Layer[0]; }
            set { Layer[0] = value; Update(); }
        }
        public bool ShowMiddle {
            get { return Layer[1]; }
            set { Layer[1] = value; Update(); }
        }

        private Texture2D selectionTexture;
        private Texture2D spawnpointTexture;
        private Func<Map, Dictionary<string, Texture2D>> GetXNATextures;
        private Func<Map, int, int, int, float> GetRotation;

        public TileMapView()
        {
            base.DeviceInitialized += () =>
            {
                CreateEmptyTexture();
            };
            Loaded += (sender, e) =>
            {
                this.Focus();
            };
        }

        public void SetReceiveFuncs(Func<Map, Dictionary<string, Texture2D>> texfunc, Func<Map, int, int, int, float> rotfunc)
        {
            GetXNATextures = texfunc;
            GetRotation = rotfunc;
        }

        public bool IsLayerActive(int id)
        {
            return id < 0 || id > 3 || Layer[id];
        }

        private void CreateEmptyTexture()
        {
            if (DesignerProperties.GetIsInDesignMode(this))
                return;

            selectionTexture = new Texture2D(GraphicsDevice, 1, 1);
            selectionTexture.SetData(new Color[] { new Color(255, 0, 0, 63) });

            spawnpointTexture = new Texture2D(GraphicsDevice, 1, 1);
            spawnpointTexture.SetData(new Color[] { new Color(0, 255, 0, 63) });
        }

        protected override void Render(SpriteBatch spriteBatch)
        {
            if (CurrentMap == null)
                return;

            int columns = (int)Math.Min(RenderSize.Width / TileSize + 1, CurrentMap.Width - Offset.X);
            int rows = (int)Math.Min(RenderSize.Height / TileSize + 1, CurrentMap.Height - Offset.Y);
            for (int x = 0; x < columns; x++)
            {
                for (int y = 0; y < rows; y++)
                {
                    int cx = x + (int)Offset.X, cy = CurrentMap.Height - y - 1 - (int)Offset.Y;
                    Rectangle drawingRectangle = new Rectangle((int)((x + 0.5f) * TileSize), (int)((y + 0.5f) * TileSize), TileSize, TileSize);
                    for (int l = 0; l < 3; l++)
                    {
                        if (Layer[l])
                        {
                            float rotation = GetRotation(CurrentMap, cx, cy, l) * (float)Math.PI;
                            spriteBatch.Draw(GetXNATextures(CurrentMap)[CurrentMap.GetTile(cx, cy, l).Name], drawingRectangle, null, Color.White, rotation, new Microsoft.Xna.Framework.Vector2(Map.TILE_PXL_SIZE / 2f, Map.TILE_PXL_SIZE / 2f), SpriteEffects.None, 0);
                        }
                    }
                }
            }

            // draw selected tile
            if (CurrentSelection.X > -1 && CurrentSelection.Y > -1)
                spriteBatch.Draw(selectionTexture, new Rectangle(CurrentSelection.X * TileSize, CurrentSelection.Y * TileSize, TileSize, TileSize), Color.White);

            // draw spawnpoint tile
            spriteBatch.Draw(spawnpointTexture, new Rectangle((int)((CurrentMap.SpawnPoint.X - Math.Floor(Offset.X)) * TileSize), (int)((CurrentMap.Height - CurrentMap.SpawnPoint.Y - 1 - Math.Floor(Offset.Y)) * TileSize), TileSize, TileSize), Color.White);
        }
    }
}
