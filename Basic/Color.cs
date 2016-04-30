namespace mapKnight.Basic {
    public struct Color {
        public int Red;
        public int Green;
        public int Blue;
        public int Alpha;

        public Color (int red, int green, int blue, int alpha) : this ( ) {
            Red = red;
            Green = green;
            Blue = blue;
            Alpha = alpha;
        }

        public string ToRGB () {
            return string.Format ("#{0}{1}{2} {3}", Red.ToString ("X2"), Green.ToString ("X2"), Blue.ToString ("X2"), Alpha.ToString ("X2"));
        }

        public override string ToString () {
            return ToRGB ( );
        }

        public float[ ] ToOpenGL () {
            return new float[ ] {
                Red / 255f, Green / 255f, Blue / 255f, Alpha / 255f,
                Red / 255f, Green / 255f, Blue / 255f, Alpha / 255f,
                Red / 255f, Green / 255f, Blue / 255f, Alpha / 255f,
                Red / 255f, Green / 255f, Blue / 255f, Alpha / 255f
            };
        }

        public static Color White { get { return new Color (255, 255, 255, 255); } }

        public static Color Black { get { return new Color (0, 0, 0, 255); } }
    }
}