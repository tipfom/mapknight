using System;
using System.Collections.Generic;
using System.Text;
using mapKnight.Core;

namespace mapKnight.Extended.Graphics {
    public class DepthVertexData : VertexData {
        public int Depth { get; set; }

        public DepthVertexData (float[ ] verticies, string texture, int depth) : this(verticies, texture, depth, Color.White) {
        }

        public DepthVertexData (float[ ] verticies, string texture, int depth, Color color) : base(verticies, texture, color) {
            Depth = depth;
        }
    }
}
