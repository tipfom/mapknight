using System.Collections.Generic;

namespace mapKnight.Core {
    public struct Tile {
        public string Name;
        public float[] Texture;
        public TileAttribute Mask;
        public Dictionary<TileAttribute, string> Attributes;
    }
}