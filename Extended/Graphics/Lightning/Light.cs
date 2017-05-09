using mapKnight.Core;

namespace mapKnight.Extended.Graphics.Lightning {
    public class Light {
        public float Radius;
        public Vector2 Position;
        public Color Color;

        public Light (float Radius, Vector2 Position, Color Color) {
            this.Radius = Radius;
            this.Position = Position;
            this.Color = Color;
        }

        public Light (float Radius, Vector2 Position, Color Color, float Intensity) {
            this.Radius = Radius;
            this.Position = Position;
            this.Color = Color;
            this.Color.A = (byte)(255 * Mathf.Clamp01(Intensity));
        }
    }
}
