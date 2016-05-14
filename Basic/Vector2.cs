using System;

namespace mapKnight.Basic {
    public struct Vector2 {
        public static Vector2 operator + (Vector2 vec1, Vector2 vec2) {
            return new Vector2 (vec1.X + vec2.X, vec1.Y + vec2.Y);
        }

        public static Vector2 operator * (Vector2 vec1, int multiplier) {
            return new Vector2 (vec1.X * (float)multiplier, vec1.Y * (float)multiplier);
        }

        public static Vector2 operator * (Vector2 vec1, float multiplier) {
            return new Vector2 (vec1.X * multiplier, vec1.Y * multiplier);
        }

        public static Vector2 operator / (Vector2 vec1, int divider) {
            return new Vector2 (vec1.X / divider, vec1.Y / divider);
        }

        public static Vector2 operator / (Vector2 vec1, Vector2 vec2) {
            return new Vector2 (vec1.X / vec2.X, vec1.Y / vec2.Y);
        }

        public static Vector2 operator - (Vector2 vec1, Vector2 vec2) {
            return new Vector2 (vec1.X - vec2.X, vec1.Y - vec2.Y);
        }

        public static bool operator != (Vector2 vec1, Vector2 vec2) {
            return (vec1.X != vec2.X && vec1.Y != vec2.Y);
        }

        public static bool operator == (Vector2 vec1, Vector2 vec2) {
            return (vec1.X == vec2.X && vec1.Y == vec2.Y);
        }

        public static bool operator < (Vector2 vec1, Vector2 vec2) {
            return (vec1.X < vec2.X) && (vec1.Y < vec2.Y);
        }

        public static bool operator > (Vector2 vec1, Vector2 vec2) {
            return (vec1.X > vec2.X) && (vec1.Y > vec2.Y);
        }

        public static Vector2 operator - (Vector2 vec) {
            return new Vector2 (-vec.X, -vec.Y);
        }

        public static explicit operator Vector2 (Size size) {
            return new Vector2 (size.Width, (float)size.Height);
        }

        public float X;
        public float Y;

        public Vector2 (float x, float y) {
            X = x;
            Y = y;
        }

        public override string ToString () {
            return string.Format ("X={0}; Y={1}", X.ToString ( ), Y.ToString ( ));
        }

        public static Vector2 Zero { get { return new Vector2 (0, (float)0); } }

        public Vector2 Abs () {
            return new Vector2 (Math.Abs (this.X), Math.Abs (this.Y));
        }

        public override bool Equals (object obj) {
            return (obj.GetType ( ) == typeof (Vector2)) ? (((Vector2)obj) == this) : false;
        }

        public override int GetHashCode () {
            return base.GetHashCode ( );
        }
    }
}

