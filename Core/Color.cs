using System.Globalization;

namespace mapKnight.Core {
    public struct Color {
        public byte R;
        public byte G;
        public byte B;
        public byte A;

        public Color (byte red, byte green, byte blue, byte alpha) : this( ) {
            R = red;
            G = green;
            B = blue;
            A = alpha;
        }

        public Color (byte red, byte green, byte blue) : this(red, green, blue, 255) {

        }

        public Color (string rgb) : this( ) {
            int startIndex = rgb.Contains("#") ? 1 : 0;
            R = byte.Parse(rgb.Substring(startIndex + 0, 2), NumberStyles.HexNumber);
            G = byte.Parse(rgb.Substring(startIndex + 2, 2), NumberStyles.HexNumber);
            B = byte.Parse(rgb.Substring(startIndex + 4, 2), NumberStyles.HexNumber);
            A = 255;
        }

        public Color (Range<Color> color) : this( ) {
            R = (byte)Mathi.Random(color.Min.R, color.Max.R);
            G = (byte)Mathi.Random(color.Min.G, color.Max.G);
            B = (byte)Mathi.Random(color.Min.B, color.Max.B);
            A = (byte)Mathi.Random(color.Min.A, color.Max.A);
        }

        public Color(int rgba) {
            R = (byte)(rgba >> 24);
            G = (byte)(rgba >> 16);
            B = (byte)(rgba >> 8);
            A = (byte)(rgba);
        }

        public string ToRGB ( ) {
            return string.Format("#{0}{1}{2} {3}", R.ToString("X2"), G.ToString("X2"), B.ToString("X2"), A.ToString("X2"));
        }

        public override string ToString ( ) {
            return ToRGB( );
        }

        public float[ ] ToOpenGL ( ) {
            float a = A / 255f, b = B / 255f, g = G / 255f, r = R / 255f;
            return new float[ ] {
                r, g, b, a,
                r, g, b, a,
                r, g, b, a,
                r, g, b, a
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