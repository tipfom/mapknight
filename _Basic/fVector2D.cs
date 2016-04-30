namespace mapKnight.Basic {
    public struct fVector2D {
        public static fVector2D operator + (fVector2D vec1, fVector2D vec2) {
            return new fVector2D (vec1.X + vec2.X, vec1.Y + vec2.Y);
        }

        public static fVector2D operator * (fVector2D vec1, int multiplier) {
            return new fVector2D (vec1.X * (float)multiplier, vec1.Y * (float)multiplier);
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

        public static explicit operator fVector2D (Vector2D vec) {
            return new fVector2D (vec.X, vec.Y);
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
    }
}

