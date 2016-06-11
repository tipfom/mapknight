using System.Collections.Generic;

namespace mapKnight.Core {
    public struct Tile {
        public string Name;
        public float[] Texture;
        public Dictionary<TileAttribute, string> Attributes;

        public bool HasFlag(TileAttribute flag) {
            return Attributes.ContainsKey(flag);
        }
    }
}