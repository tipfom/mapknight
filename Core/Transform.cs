using System;

namespace mapKnight.Core {

    public class Transform {
        private Vector2 _Center;
        private Vector2 _BL;
        private Vector2 _TR;
        private Vector2 _Size;

        public Transform (Vector2 center, Vector2 size) {
            Center = center;
            Size = size;
        }

        public event Action SizeChanged;

        public Vector2 Center { get { return _Center; } set { _Center = value; _TR = _Center + HalfSize; _BL = _Center - HalfSize; } }
        public Vector2 BL { get { return _BL; } set { _BL = value; _Center = _BL + HalfSize; _TR = _BL + Size; } }
        public Vector2 TR { get { return _TR; } set { _TR = value; _Center = _TR - HalfSize; _BL = _TR - Size; } }
        public Vector2 HalfSize { get; private set; }
        public Vector2 Size { get { return _Size; } set { _Size = value; HalfSize = _Size / 2f; _TR = _Center + HalfSize; _BL = _Center - HalfSize; SizeChanged?.Invoke( ); } }

        public float Width { get { return Size.X; } set { Size = new Vector2(value, Size.Y); } }
        public float Height { get { return Size.Y; } set { Size = new Vector2(Size.X, value); } }
        public float X { get { return Center.X; } set { Center = new Vector2(value, Center.Y); } }
        public float Y { get { return Center.Y; } set { Center = new Vector2(Center.X, value); } }

        public void Align (Transform collider) {
            if (collider.BL.X < BL.X && collider.BL.X > TR.X) {
                // left side intersects
                X = collider.BL.X - HalfSize.X;
            } else if (collider.TR.X < TR.X && collider.TR.X > BL.X) {
                // right sided intersection
                X = collider.TR.X + HalfSize.X;
            } else if (collider.BL.Y < TR.Y && collider.BL.Y > BL.Y) {
                // intersection at the bottom
                Y = collider.BL.Y - HalfSize.Y;
            } else if (collider.TR.Y < TR.Y && collider.TR.Y > BL.Y) {
                // intersection at the top
                Y = collider.TR.Y + HalfSize.Y;
            }
        }

        public Transform Clone ( ) {
            return new Transform(Center, Size);
        }

        public bool Intersects (Transform transform) {
            return !(
                this.BL.X >= transform.TR.X ||    // ot. right of the other transform
                this.TR.X <= transform.BL.X ||    // ot. left of the other transform
                this.TR.Y <= transform.BL.Y ||    // below the other transform
                this.BL.Y >= transform.TR.Y);      // on top of the other transform
        }

        public bool Intersects (Vector2 point) {
            return
                this.BL.X <= point.X ||
                this.TR.X >= point.X ||
                this.TR.Y >= point.Y ||
                this.BL.Y <= point.Y;
        }

        public bool Touches (Transform transform) {
            return !(
                this.BL.X > transform.TR.X ||   // ot. right of the other transform
                this.TR.X < transform.BL.X ||   // ot. left of the other transform
                this.TR.Y < transform.BL.Y ||   // below the other transform
                this.BL.Y > transform.TR.Y);     // on top of the other transform
        }

        public void Translate (Vector2 delta) {
            Center += delta;
        }

        public void Translate (float dx, float dy) {
            Translate(new Vector2(dx, dy));
        }
    }
}