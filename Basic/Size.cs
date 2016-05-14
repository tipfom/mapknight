using System;

namespace mapKnight.Basic {
    public struct Size {
        public static Size operator - (Size size) {
            return new Size (-size.Width, -size.Height);
        }

        public static Size operator / (Size size, int integer) {
            return new Size (size.Width / integer, size.Height / integer);
        }

        public static Size operator - (Size size1, Size size2) {
            return new Size (size1.Width - size2.Width, size1.Height - size2.Height);
        }

        public static explicit operator Size (Vector2 fvec) {
            return new Size ((int)fvec.X, (int)fvec.Y);
        }

        public int Width;
        public int Height;

        public Size (int width, int height) {
            Width = width;
            Height = height;
        }

        public Size (int sqrsidelength) {
            Width = sqrsidelength;
            Height = sqrsidelength;
        }

        public override string ToString () {
            return String.Format ("Width = {0}; Height = {1}", Width.ToString ( ), Height.ToString ( ));
        }
    }
}