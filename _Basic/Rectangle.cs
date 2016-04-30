namespace mapKnight.Basic {
    public struct Rectangle {
        private Point _Position;

        public Point Position { get { return _Position; } set { _Position = value; } }

        private Size _Size;

        public Size Size { get { return _Size; } set { _Size = value; } }

        public int Left { get { return Position.X; } set { _Position.X = value; } }

        public int Right { get { return Position.X + Width; } set { _Position.X = value - Width; } }

        public int Top { get { return Position.Y; } set { _Position.Y = value; } }

        public int Bottom { get { return Position.Y + Height; } set { _Position.Y = value - Height; } }

        public int Width { get { return Size.Width; } set { _Size.Width = value; } }

        public int Height { get { return Size.Height; } set { _Size.Height = value; } }

        public Rectangle (Point position, Size size) : this () {
            this.Position = position;
            this.Size = size;
        }

        public Rectangle (int x, int y, int width, int height) : this (new Point (x, y), new Size (width, height)) {

        }

        public bool Collides (Point point) {
            if (Right > point.X && Left < point.X && Top > point.Y && Bottom < point.Y)
                return true;
            return false;
        }
    }
}