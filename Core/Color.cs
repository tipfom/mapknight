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

        public static Color Navy { get { return new Color("#001F3F"); } }
        public static Color Blue { get { return new Color("#0074D9"); } }
        public static Color Aqua { get { return new Color("#7FDBFF"); } }
        public static Color Teal { get { return new Color("#39CCCC"); } }
        public static Color Olive { get { return new Color("#4D9970"); } }
        public static Color Green { get { return new Color("#2ECC40"); } }
        public static Color Lime { get { return new Color("#01FF70"); } }
        public static Color Yellow { get { return new Color("#FFDC00"); } }
        public static Color Orange { get { return new Color("#FF851B"); } }
        public static Color Red { get { return new Color("#FF4136"); } }
        public static Color Maroon { get { return new Color("#84144B"); } }
        public static Color Fuchsia { get { return new Color("#F012BE"); } }
        public static Color Purple { get { return new Color("#B10DC9"); } }
        public static Color Black { get { return new Color("#000000"); } }
        public static Color Gray { get { return new Color("#AAAAAA"); } }
        public static Color Silver { get { return new Color("#DDDDDD"); } }
        public static Color White { get { return new Color("#FFFFFF"); } }
        #endregion
    }
}