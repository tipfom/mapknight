using System;
using System.Collections.Generic;
using System.Text;
using mapKnight.Core;

namespace mapKnight.Extended.Graphics.UI {
    public class UIRectangle {
        public Vector2 Size { get { return _Size; } set { _Size = value; UpdateVerticies( ); } }
        public Vector2 Position { get { return _Position; } set { _Position = value; UpdateVerticies( ); } }

        public float Left { get { return Position.X; } }
        public float Right { get { return Position.X + Size.X; } }
        public float Top { get { return Position.Y; } }
        public float Bottom { get { return Position.Y - Size.Y; } }

        public float Width { get { return Size.X; } }
        public float Height { get { return Size.Y; } }

        public float[ ] Verticies;

        private Vector2 _Size;
        private Vector2 _Position;

        public UIRectangle (float x, float y, float width, float height) : this(new Vector2(x, y), new Vector2(width, height)) {
        }

        public UIRectangle (Vector2 position, Vector2 size) {
            Verticies = new float[8];
            _Position = position;
            _Size = size;
            UpdateVerticies( );
        }

        public bool Collides (Vector2 point) {
            return !(
                point.X < Left ||
                point.X > Right ||
                point.Y > Top ||
                point.Y < Bottom
                );
        }

        private void UpdateVerticies ( ) {
            Verticies[0] = Left;
            Verticies[1] = Top;
            Verticies[2] = Left;
            Verticies[3] = Bottom;
            Verticies[4] = Right;
            Verticies[5] = Verticies[3]; // Bottom
            Verticies[6] = Verticies[4]; // Right
            Verticies[7] = Top;
        }

        public static float[] GetVerticies(float x, float y, float width, float height) {
            return new float[8] {
                x, y,
                x, y - height, 
                x + width, y - height, 
                x + width, y
            };
        }

        public static float[] GetVerticies(Vector2 position, Vector2 size) {
            return new float[8] {
                position.X, position.Y,
                position.X, position.Y - size.Y,
                position.X + size.X, position.Y - size.Y,
                position.X + size.X, position.Y
            };
        }
    }
}
