using System;

namespace mapKnight.Core {
    public struct Vector2 {
        #region operator

        #region +, -

        public static Vector2 operator + (Vector2 vec1, Vector2 vec2) {
            return new Vector2(vec1.X + vec2.X, vec1.Y + vec2.Y);
        }

        public static Vector2 operator - (Vector2 vec) {
            return new Vector2(-vec.X, -vec.Y);
        }

        public static Vector2 operator - (Vector2 vec1, Vector2 vec2) {
            return new Vector2(vec1.X - vec2.X, vec1.Y - vec2.Y);
        }

        #endregion

        #region *, /

        public static Vector2 operator * (Vector2 vec1, int multiplier) {
            return new Vector2(vec1.X * (float)multiplier, vec1.Y * (float)multiplier);
        }

        public static Vector2 operator * (Vector2 vec1, float multiplier) {
            return new Vector2(vec1.X * multiplier, vec1.Y * multiplier);
        }

        public static Vector2 operator / (Vector2 vec1, int divider) {
            return new Vector2(vec1.X / divider, vec1.Y / divider);
        }

        public static Vector2 operator / (Vector2 vec1, float divider) {
            return new Vector2(vec1.X / divider, vec1.Y / divider);
        }

        public static Vector2 operator / (Vector2 vec1, Vector2 vec2) {
            return new Vector2(vec1.X / vec2.X, vec1.Y / vec2.Y);
        }

        #endregion

        #region comparison

        public static bool operator != (Vector2 vec1, Vector2 vec2) {
            return (vec1.X != vec2.X || vec1.Y != vec2.Y);
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

        #endregion

        #region conversion

        public static explicit operator Vector2 (Size size) {
            return new Vector2(size.Width, (float)size.Height);
        }

        #endregion

        #endregion

        #region defaults

        public static Vector2 Zero {
            get { return new Vector2(0, 0); }
        }

        public static Vector2 One {
            get { return new Vector2(1, 1); }
        }

        #endregion

        public float X;
        public float Y;

        public Vector2 (float x, float y) {
            X = x;
            Y = y;
        }

        #region methods and functions

        public Vector2 Abs ( ) {
            return new Vector2(Math.Abs(this.X), Math.Abs(this.Y));
        }

        /// <returns>
        /// returns the distance discribed by the vector
        /// </returns>
        public float Magnitude ( ) {
            return (float)Math.Sqrt(Math.Pow(X, 2) + Math.Pow(Y, 2));
        }

        /// <returns>
        /// returns the squared distance of the vector (much faster)
        /// </returns>
        public float MagnitudeSqr ( ) {
            return (float)(Math.Pow(X, 2f) + Math.Pow(Y, 2f));
        }

        public float[ ] ToQuad ( ) {
            return new float[ ] {
                0, Y,
                0, 0,
                X, 0,
                X, Y
            };
        }

        #endregion

        #region overrides

        public override bool Equals (object obj) {
            return (obj.GetType( ) == typeof(Vector2)) ? (((Vector2)obj) == this) : false;
        }

        public override int GetHashCode ( ) {
            return base.GetHashCode( );
        }

        public override string ToString ( ) {
            return $"({X} {Y})";
        }

        #endregion
    }
}