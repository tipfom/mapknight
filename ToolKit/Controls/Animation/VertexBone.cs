using mapKnight.Core;
using Newtonsoft.Json;

namespace mapKnight.ToolKit.Controls.Components.Animation {
    public class VertexBone {
        public bool Mirrored { get; set; }
        public Vector2 Position { get; set; }
        public float Rotation { get; set; }
        public Vector2 Size { get; private set; }
        private Vector2 _AbsoluteSize;
        [JsonIgnore]
        public Vector2 AbsoluteSize { get { return _AbsoluteSize; } set { _AbsoluteSize = value; Size = AbsoluteSize / TextureSize; } }

        [JsonIgnore]
        public Vector2 TextureSize { get; set; } = Vector2.One;
    }
}
