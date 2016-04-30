using System;

namespace mapKnight.Basic {
    public struct Vector2D {
        public static Vector2D operator + (Vector2D vec1, Vector2D vec2) {
            return new Vector2D (vec1.X + vec2.X, vec1.Y + vec2.Y);
        }

        public static Vector2D operator + (Vector2D vec1, fVector2D vec2) {
            return new Vector2D (vec1.X + (int)vec2.X, vec1.Y + (int)vec2.Y);
        }

        public static Vector2D operator * (Vector2D vec1, int multiplier) {
            return new Vector2D (vec1.X * multiplier, vec1.Y * multiplier);
        }

        public static explicit operator Vector2D (fVector2D vec) {
            return new Vector2D ((int)vec.X, (int)vec.Y);
        }

        /// <summary>
        /// Creates a vector with the minimum x and the minimum y of the to vectors
        /// </summary>
        /// <param name="vec1">Vector 1</param>
        /// <param name="vec2">Vector 2</param>
        public static Vector2D Min (Vector2D vec1, Vector2D vec2) {
            return new Vector2D (Math.Min (vec1.X, vec2.X), Math.Min (vec1.Y, vec2.Y));
        }

        /// <summary>
        /// Creates a vector with the maximum x and the minimum y of the to vectors
        /// </summary>
        /// <param name="vec1">Vector 1</param>
        /// <param name="vec2">Vector 2</param>
        public static Vector2D Max (Vector2D vec1, Vector2D vec2) {
            return new Vector2D (Math.Max (vec1.X, vec2.X), Math.Max (vec1.Y, vec2.Y));
        }

        public int X;
        public int Y;

        public Vector2D (int x, int y) {
            X = x;
            Y = y;
        }
    }
}