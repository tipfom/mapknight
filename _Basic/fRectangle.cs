namespace mapKnight.Basic {
    public struct fRectangle {
        private fVector2D _Position;

        public fVector2D Position { get { return _Position; } set { _Position = value; } }

        private fVector2D _Size;

        public fVector2D Size { get { return _Size; } set { _Size = value; } }

        public float Left { get { return Position.X; } set { _Position.X = value; } }

        public float Right { get { return Position.X + Width; } set { _Position.X = value - Width; } }

        public float Top { get { return Position.Y; } set { _Position.Y = value; } }

        public float Bottom { get { return Position.Y + Height; } set { _Position.Y = value - Height; } }

        public float Width { get { return _Size.X; } set { _Size.X = value; } }

        public float Height { get { return _Size.Y; } set { _Size.Y = value; } }

        public float[ ] Verticies { get { return new float[ ] { Left, Top, Left, Bottom, Right, Bottom, Right, Top }; } }

        public fRectangle (fVector2D position, fVector2D size) : this ( ) {
            this.Position = position;
            this.Size = size;
        }

        public fRectangle (float x, float y, float width, float height) : this (new fVector2D (x, y), new fVector2D (width, height)) {

        }

        public float[ ] GetVerticies () {
            return new float[ ] { Left, Top, Left, Bottom, Right, Bottom, Right, Top };
        }
    }
}

