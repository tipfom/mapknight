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

        public static Vector2 operator * (Vector2 vec1, Vector2 vec2) {
            return new Vector2(vec1.X * vec2.X, vec1.Y * vec2.Y);
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

#if __ANDROID__
        public float X;
        public float Y;
#else
        public float X { get; set; }
        public float Y { get; set; }
#endif

        public Vector2 (float x, float y) {
            X = x;
            Y = y;
        }

        public Vector2 (Vector2 vector) {
            X = vector.X;
            Y = vector.Y;
        }

        #region methods and functions

        /// <returns>
        /// returns the absolute of both vector components
        /// </returns>
        public Vector2 Abs ( ) {
            return new Vector2(Math.Abs(this.X), Math.Abs(this.Y));
        }

        /// <returns>
        /// returns the normalized vector
        /// </returns>
        public Vector2 Normalize ( ) {
            return this / Magnitude( );
        }

        /// <returns>
        /// returns the distance discribed by the vector
        /// </returns>
        public float Magnitude ( ) {
            return Mathf.Sqrt(X * X + Y * Y);
        }

        /// <returns>
        /// returns the squared distance of the vector (much faster)
        /// </returns>
        public float MagnitudeSqr ( ) {
            return (X * X + Y * Y);
        }

        public float[ ] ToQuad ( ) {
            float xh = X / 2f, yh = Y / 2f;
            return new float[ ] {
                -xh, yh,
                -xh, -yh,
                xh, -yh,
                xh, yh
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