namespace mapKnight.Basic {
    public struct Rectangle {
        private Vector2 _Position;

        public Vector2 Position { get { return _Position; } set { _Position = value; } }

        private Vector2 _Size;

        public Vector2 Size { get { return _Size; } set { _Size = value; } }

        public float Left { get { return Position.X; } set { _Position.X = value; } }

        public float Right { get { return Position.X + Width; } set { _Position.X = value - Width; } }

        public float Top { get { return Position.Y; } set { _Position.Y = value; } }

        public float Bottom { get { return Position.Y + Height; } set { _Position.Y = value - Height; } }

        public float Width { get { return _Size.X; } set { _Size.X = value; } }

        public float Height { get { return _Size.Y; } set { _Size.Y = value; } }

        public float X { get { return _Position.X; } set { _Position.X = value; } }

        public float Y { get { return _Position.Y; } set { _Position.Y = value; } }

        public float[ ] Verticies { get { return new float[ ] { Left, Top, Left, Bottom, Right, Bottom, Right, Top }; } }

        public Rectangle (Vector2 position, Vector2 size) : this ( ) {
            this.Position = position;
            this.Size = size;
        }

        public Rectangle (float x, float y, float width, float height) : this (new Vector2 (x, y), new Vector2 (width, height)) {

        }

        public float[ ] GetVerticies () {
            return new float[ ] { Left, Top, Left, Bottom, Right, Bottom, Right, Top };
        }
    }
}

