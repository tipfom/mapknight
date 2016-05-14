using System.Collections.Generic;

namespace mapKnight.Android.ECS {
    public struct EntityVertexData {
        public List<string> SpriteNames;
        public float[] VertexCoords;
        public int QuadCount;
        public int Entity;
    }
}