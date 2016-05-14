namespace mapKnight.Basic {
    public class Transform {
        public Vector2 Center { get; set; }
        public Vector2 Bounds { get; set; }
        public Vector2 TR { get { return Center + Bounds / 2; } } // top right
        public Vector2 BL { get { return Center - Bounds / 2; } } // bottom left

        public Transform (Vector2 center, Vector2 bounds) {
            Center = center;
            Bounds = bounds;
        }

        public void Translate (Vector2 newPosition) {
            Center = newPosition;
        }

        public void TranslateHorizontally (float newX) {
            Center = new Vector2 (newX, Center.Y);
        }

        public void TranslateVertically (float newY) {
            Center = new Vector2 (Center.X, newY);
        }

        public Transform Clone () {
            return new Transform (Center, Bounds);
        }
    }
}
