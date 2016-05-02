using System;

namespace mapKnight.Basic {
    public struct fVector2D {
        public static fVector2D operator + (fVector2D vec1, fVector2D vec2) {
            return new fVector2D (vec1.X + vec2.X, vec1.Y + vec2.Y);
        }

        public static fVector2D operator * (fVector2D vec1, int multiplier) {
            return new fVector2D (vec1.X * (float)multiplier, vec1.Y * (float)multiplier);
        }

        public static fVector2D operator / (fVector2D vec1, int divider) {
            return new fVector2D (vec1.X / divider, vec1.Y / divider);
        }

        public static fVector2D operator / (fVector2D vec1, fVector2D vec2) {
            return new fVector2D (vec1.X / vec2.X, vec1.Y / vec2.Y);
        }

        public static fVector2D operator - (fVector2D vec1, fVector2D vec2) {
            return new fVector2D (vec1.X - vec2.X, vec1.Y - vec2.Y);
        }

        public static bool operator != (fVector2D vec1, fVector2D vec2) {
            return (vec1.X != vec2.X && vec1.Y != vec2.Y);
        }

        public static bool operator == (fVector2D vec1, fVector2D vec2) {
            return (vec1.X == vec2.X && vec1.Y == vec2.Y);
        }

        public static bool operator < (fVector2D vec1, fVector2D vec2) {
            return (vec1.X < vec2.X) && (vec1.Y < vec2.Y);
        }

        public static bool operator > (fVector2D vec1, fVector2D vec2) {
            return (vec1.X > vec2.X) && (vec1.Y > vec2.Y);
        }

        public static explicit operator fVector2D (Vector2D vec) {
            return new fVector2D (vec.X, vec.Y);
        }

        public static explicit operator fVector2D (Size size) {
            return new fVector2D (size.Width, size.Height);
        }

        public float X;
        public float Y;

        public fVector2D (float x, float y) {
            X = x;
            Y = y;
        }

        public override string ToString () {
            return string.Format ("X={0}; Y={1}", X.ToString ( ), Y.ToString ( ));
        }

        public static fVector2D Zero { get { return new fVector2D (0, 0); } }

        public fVector2D Abs () {
            return new fVector2D (Math.Abs (this.X), Math.Abs (this.Y));
        }

        public override bool Equals (object obj) {
            return (obj.GetType ( ) == typeof (fVector2D)) ? (((fVector2D)obj) == this) : false;
        }

        public override int GetHashCode () {
            return base.GetHashCode ( );
        }
    }
}

