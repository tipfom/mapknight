using mapKnight.Core;
using mapKnight.ToolKit.Xna;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Color = Microsoft.Xna.Framework.Color;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace mapKnight.ToolKit {
    public class TileMapView : XnaControl {
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

        private int _TileSize = 40;
        public int TileSize {
            get { return _TileSize; }
            set { _TileSize = value; Update( ); }
        }

        public TileMapView ( ) {

        }

        protected override void Render (SpriteBatch spriteBatch) {
            if (CurrentMap == null)
                return;

            int columns = (int)Math.Min(RenderSize.Width / TileSize + 1, CurrentMap.Width - Offset.X);
            int rows = (int)Math.Min(RenderSize.Height / TileSize + 1, CurrentMap.Height - Offset.Y);
            for (int x = 0; x < columns; x++) {
                for (int y = 0; y < rows; y++) {
                    int cx = x + Offset.X, cy = CurrentMap.Height - y-1 - Offset.Y;
                    Rectangle drawingRectangle = new Rectangle(x * TileSize, y * TileSize, TileSize, TileSize);
                    for (int l = 0; l < 3; l++) {
                        spriteBatch.Draw(App.Project.GetMapXNATextures(CurrentMap)[CurrentMap.GetTile(cx, cy, l).Name], drawingRectangle, Color.White);
                    }
                }
            }
        }
    }
}
