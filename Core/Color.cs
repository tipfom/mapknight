using System.Globalization;

namespace mapKnight.Core {
    public struct Color {
        public float R;
        public float G;
        public float B;
        public float A;

        public Color (float red, float green, float blue, float alpha) : this( ) {
            R = red;
            G = green;
            B = blue;
            A = alpha;
        }

        public Color (byte red, byte green, byte blue, byte alpha) : this(red / 255f, green / 255f, blue / 255f, alpha / 255f) {
        }

        public Color (byte red, byte green, byte blue) : this(red, green, blue, 255) {
        }

        public Color (int rgba) : this((byte)(rgba >> 24) / 255f, (byte)(rgba >> 16) / 255f, (byte)(rgba >> 8) / 255f, (byte)(rgba) / 255f) {
        }

        public Color (string rgb) : this( ) {
            int startIndex = rgb.Contains("#") ? 1 : 0;
            R = byte.Parse(rgb.Substring(startIndex + 0, 2), NumberStyles.HexNumber) / 255f;
            G = byte.Parse(rgb.Substring(startIndex + 2, 2), NumberStyles.HexNumber) / 255f;
            B = byte.Parse(rgb.Substring(startIndex + 4, 2), NumberStyles.HexNumber) / 255f;
            A = 1.0f;
        }

        public Color (Range<Color> color) : this( ) {
            R = Mathf.Random(color.Min.R, color.Max.R);
            G = Mathf.Random(color.Min.G, color.Max.G);
            B = Mathf.Random(color.Min.B, color.Max.B);
            A = Mathf.Random(color.Min.A, color.Max.A);
        }

        public override string ToString ( ) {
            return string.Format("#{0}{1}{2} {3}", ((int)(R * 255)).ToString("X2"), ((int)(G * 255)).ToString("X2"), ((int)(B * 255)).ToString("X2"), ((int)(A * 255)).ToString("X2"));
        }

        public float[ ] ToArray ( ) {
            return new float[ ] { R, G, B, A };
        }

        public float[ ] ToArray4 ( ) {
            return new float[ ] {
                R, G, B, A,
                R, G, B, A,
                R, G, B, A,
                R, G, B, A
            };
        }

        #region default colors
        // source : http://clrs.cc/

        public static Color Blue { get; } = new Color(0, 116, 217);
        public static Color Green { get; } = new Color(46, 204, 64);
        public static Color Yellow { get; } = new Color(255, 220, 0);
        public static Color Orange { get; } = new Color(255, 133, 27);
        public static Color Red { get; } = new Color(255, 65, 54);
        public static Color Purple { get; } = new Color(177, 13, 201);
        public static Color Black { get; } = new Color(0, 0, 0);
        public static Color White { get; } = new Color(255, 255, 255);
        #endregion
    }
}