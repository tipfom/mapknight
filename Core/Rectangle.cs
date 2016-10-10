using System;

namespace mapKnight.Core {
    public struct Rectangle {
        public Vector2 Position;
        public Vector2 Size;
        public float Rotation;
        public bool Flipped;

        public float[ ] Verticies ( ) {
            return Mathf.TransformAtOrigin(Size.ToQuad( ), Position.X, Position.Y, Rotation, Flipped);
        }
    }
}