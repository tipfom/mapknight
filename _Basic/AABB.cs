using System;

namespace mapKnight.Basic {
    public class AABB {
        /// <summary>
        /// bottom left corner of the boundingbox
        /// </summary>
        public fVector2D A { get; private set; }
        /// <summary>
        /// top right corner of the boundingbox
        /// </summary>
        public fVector2D B { get; private set; }
        /// <summary>
        /// centre of the boundingbox
        /// </summary>
        public fVector2D Centre { get; private set; }

        public fSize Bounds;

        public AABB (fVector2D v) {
            A = new fVector2D (0, 0);
            B = new fVector2D (v.X, v.Y);
            Bounds = new fSize (v.X, v.Y);
            Centre = new fVector2D (A.X + (B.X - A.X) / 2, A.Y + (B.Y - A.Y) / 2);
        }

        public AABB (float x1, float y1)
            : this (new fVector2D (x1, y1)) {
        }

        public AABB (fVector2D v1, fVector2D v2) {
            A = new fVector2D (Math.Min (v1.X, v2.X), Math.Min (v1.Y, v2.Y));
            B = new fVector2D (Math.Max (v1.X, v2.X), Math.Max (v1.Y, v2.Y));
            Bounds = new fSize (B.X - A.X, B.Y - A.Y);
            Centre = new fVector2D (A.X + (B.X - A.X) / 2, A.Y + (B.Y - A.Y) / 2);
        }

        public AABB (float x1, float y1, float x2, float y2) : this (new fVector2D (x1, y1), new fVector2D (x2, y2)) {
        }

        public void Translate (fVector2D newCentre) {
            fVector2D offset = newCentre.GSub (Centre, Bounds.Width / 2, Bounds.Height / 2);
            Centre += offset;
            A += offset;
            B += offset;
        }

        public void Translate (float newX, float newY) {
            Translate (new fVector2D (newX, newY));
        }
    }
}
