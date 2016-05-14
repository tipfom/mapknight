using mapKnight.Basic;
using System;
using System.Collections.Generic;

namespace mapKnight.Android.Map {
    public struct Tile {
        public string Name;
        public float[] Texture;
        public TileMask Mask;
        public Dictionary<TileAttribute, string> Attributes;

        public Tile (TileManager.ExportTile tileData, Vector2 tvertexsize, int imagewidth, int imageheight) : this () {
            this.Name = tileData.Name;
            if (!Enum.TryParse (String.Join (",", tileData.MaskFlag), out Mask))
                Mask = TileMask.None;

            this.Attributes = tileData.Attributes;

            float x = (float)tileData.X / imagewidth;
            float y = (float)tileData.Y / imageheight;

            this.Texture = new float[] {
                    x, y + tvertexsize.Y,
                    x, y,
                    x + tvertexsize.X, y,
                    x + tvertexsize.X, y + tvertexsize.Y
                };
        }
    }
}