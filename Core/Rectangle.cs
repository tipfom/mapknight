using System;

namespace mapKnight.Core {
    public struct Rectangle {
        public Vector2 Position;

        private Vector2 halfSize;
        private Vector2 _Size;
        public Vector2 Size { get { return _Size; } set { _Size = value; halfSize = value / 2; } }

        public float Left { get { return Position.X; } }
        public float Right { get { return Position.X + Size.X; } }
        public float Top { get { return Position.Y; } }
        public float Bottom { get { return Position.Y - Size.Y; } }
        public float Width { get { return Size.X; } }
        public float Height { get { return Size.Y; } }

        public Rectangle (Vector2 position, Vector2 size) : this( ) {
            Position = position;
            Size = size;
        }

        public Rectangle (float x, float y, float width, float height) : this( ) {
            Position = new Vector2(x, y);
            Size = new Vector2(width, height);
        }

        public bool Collides (Vector2 point) {
            return !(
                point.X < Left ||
                point.X > Right ||
                point.Y > Top ||
                point.Y < Bottom
                );
        }

        public float[ ] Verticies ( ) {
            return Verticies(Anchor.Top | Anchor.Left);
        }

        public float[ ] Verticies (Anchor anchor) {
            float leftMultiplier = anchor.HasFlag(Anchor.Left) ? 0 : anchor.HasFlag(Anchor.Right) ? 1 : 0.5f;
            float rightMultiplier = anchor.HasFlag(Anchor.Left) ? 1 : anchor.HasFlag(Anchor.Right) ? 0 : 0.5f;
            float topMultiplier = anchor.HasFlag(Anchor.Top) ? 0 : anchor.HasFlag(Anchor.Bottom) ? 1 : 0.5f;
            float bottomMultiplier = anchor.HasFlag(Anchor.Top) ? 1 : anchor.HasFlag(Anchor.Bottom) ? 0 : 0.5f;

            return new float[ ] {
                Position.X - leftMultiplier * Size.X, Position.Y + topMultiplier * Size.Y,
                Position.X - leftMultiplier * Size.X, Position.Y - bottomMultiplier * Size.Y,
                Position.X + rightMultiplier * Size.X, Position.Y - bottomMultiplier * Size.Y,
                Position.X + rightMultiplier * Size.X, Position.Y + topMultiplier * Size.Y,
            };
        }
    }
}