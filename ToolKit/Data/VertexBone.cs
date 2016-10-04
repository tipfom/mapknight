using System;
using System.Windows.Media.Imaging;
using mapKnight.Core;
using Newtonsoft.Json;

namespace mapKnight.ToolKit.Data {
    public class VertexBone {
        public bool Mirrored { get; set; }
        public Vector2 Position { get; set; }
        public float Rotation { get; set; }
        public string Image { get; set; }
        public float Scale { get; set; }

        public VertexBone Clone ( ) {
            return new VertexBone( ) { Mirrored = Mirrored, Position = Position, Rotation = Rotation, Image = Image, Scale = Scale };
        }
    }
}
