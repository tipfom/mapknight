namespace mapKnight.Core {
    public class Transform {
        public Vector2 Center { get; set; }
        private Vector2 _Bounds;
        public Vector2 Bounds { get { return _Bounds; } set { _Bounds = value; BoundsHalf = _Bounds / 2; } }
        public Vector2 BoundsHalf { get; private set; }
        public Vector2 TR { get { return Center + BoundsHalf; } } // top right
        public Vector2 BL { get { return Center - BoundsHalf; } } // bottom left

        public Transform (Vector2 center, Vector2 bounds) {
            Center = center;
            Bounds = bounds;
        }

        public void Translate (Vector2 newPosition) {
            Center = newPosition;
        }

        public void TranslateX (float newX) {
            Center = new Vector2 (newX, Center.Y);
        }

        public void TranslateY (float newY) {
            Center = new Vector2 (Center.X, newY);
        }

        public Transform Clone () {
            return new Transform (Center, Bounds);
        }

        public bool Touches (Transform transform) {
            return !(
                this.BL.X > transform.TR.X ||   // ot. right of the other transform
                this.TR.X < transform.BL.X ||   // ot. left of the other transform
                this.TR.Y < transform.BL.Y ||   // below the other transform
                this.BL.Y > transform.TR.Y);     // on top of the other transform
        }

        public bool Intersects (Transform transform) {
            return !(
                this.BL.X >= transform.TR.X ||    // ot. right of the other transform
                this.TR.X <= transform.BL.X ||    // ot. left of the other transform
                this.TR.Y <= transform.BL.Y ||    // below the other transform
                this.BL.Y >= transform.TR.Y);      // on top of the other transform
        }

        public void Align (Transform collider) {
            if (collider.BL.X < this.BL.X && collider.BL.X > this.TR.X) {
                // left side intersects
                this.TranslateX (collider.BL.X - this.BoundsHalf.X);
            } else if (collider.TR.X < this.TR.X && collider.TR.X > this.BL.X) {
                // right sided intersection
                this.TranslateX (collider.TR.X + this.BoundsHalf.X);
            } else if (collider.BL.Y < this.TR.Y && collider.BL.Y > this.BL.Y) {
                // intersection at the bottom
                this.TranslateY (collider.BL.Y - this.BoundsHalf.Y);
            } else if (collider.TR.Y < this.TR.Y && collider.TR.Y > this.BL.Y) {
                // intersection at the top
                this.TranslateY (collider.TR.Y + this.BoundsHalf.Y);
            }
        }
    }
}
