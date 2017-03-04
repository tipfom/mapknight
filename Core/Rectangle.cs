namespace mapKnight.Core {
    public struct Rectangle {
        public Vector2 Position;
        public Vector2 Size;
        public float Rotation;
        public bool Flipped;

        public Rectangle (float x, float y, float width, float height, float rotation = 0, bool flipped = false) {
            this.Position = new Vector2(x, y);
            this.Size = new Vector2(width, height);
            this.Rotation = rotation;
            this.Flipped = flipped;
        }

        public void Verticies ( ref float[] result) {
            Mathf.TransformAtOrigin(Size.ToQuad( ), ref result, Position.X, Position.Y, Rotation, Flipped);
        }
    }
}