using System;

namespace mapKnight.Basic {
    public struct fVector2D {
        public static fVector2D operator + (fVector2D vec1, fVector2D vec2) {
            return new fVector2D (vec1.X + vec2.X, vec1.Y + vec2.Y);
        }

        public static fVector2D operator * (fVector2D vec1, int multiplier) {
            return new fVector2D (vec1.X * (float)multiplier, vec1.Y * (float)multiplier);
        }

        public static fVector2D operator - (fVector2D vec1, fVector2D vec2) {
            return new fVector2D (vec1.X - vec2.X, vec1.Y - vec2.Y);
        }

        public float X;
        public float Y;

        public fVector2D (float x, float y) {
            X = x;
            Y = y;
        }

        public fVector2D GSub (fVector2D v2, float xMin, float yMin) {
            return new fVector2D (Math.Min (this.X - v2.X, xMin), Math.Min (this.Y - v2.Y, yMin));
        }

        public override string ToString () {
            return string.Format ("X={0}; Y={1}", X.ToString (), Y.ToString ());
        }
    }
}

