using System;

namespace mapKnight.Core {
    public struct Rectangle {
        public Vector2 Position;
        public Vector2 Size;
        public float Rotation;
        public bool Flipped;

        public void Verticies ( ref float[] result) {
            Mathf.TransformAtOrigin(Size.ToQuad( ), ref result, Position.X, Position.Y, Rotation, Flipped);
        }
    }
}