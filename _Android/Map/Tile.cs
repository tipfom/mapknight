using System;
using System.Collections.Generic;
using mapKnight.Basic;

namespace mapKnight.Android.Map {
    public struct Tile {
        public string Name;
        public float[ ] Texture;
        public TileMask Mask;
        public Dictionary<TileAttribute, string> Attributes;

        public Tile (TileManager.ExportTile tileData, fSize tvertexsize, int imagewidth, int imageheight) : this ( ) {
            this.Name = tileData.Name;
            if (!Enum.TryParse (String.Join (",", tileData.MaskFlag), out Mask))
                Mask = TileMask.NONE;

            this.Attributes = tileData.Attributes;

            float x = (float)tileData.X / imagewidth;
            float y = (float)tileData.Y / imageheight;

            this.Texture = new float[ ] {
                    x, y + tvertexsize.Height,
                    x, y,
                    x + tvertexsize.Width, y,
                    x + tvertexsize.Width, y + tvertexsize.Height
                };
        }

        [Flags]
        public enum TileMask {
            NONE = 0,
            COLLISION = 1
        }

        public enum TileAttribute {

        }
    }
}