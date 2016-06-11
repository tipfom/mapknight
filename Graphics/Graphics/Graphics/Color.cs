using System.Globalization;

namespace mapKnight.Core {
    public struct Color {
        public int R;
        public int G;
        public int B;
        public int A;

        public Color (int red, int green, int blue, int alpha) : this ( ) {
            R = red;
            G = green;
            B = blue;
            A = alpha;
        }

        public Color (int red, int green, int blue) : this (red, green, blue, 255) {

        }

        public Color (string rgb) : this ( ) {
            int startIndex = rgb.Contains ("#") ? 1 : 0;
            R = int.Parse (rgb.Substring (startIndex + 0, 2), NumberStyles.HexNumber);
            G = int.Parse (rgb.Substring (startIndex + 2, 2), NumberStyles.HexNumber);
            B = int.Parse (rgb.Substring (startIndex + 4, 2), NumberStyles.HexNumber);
            A = 255;
        }

        public string ToRGB () {
            return string.Format ("#{0}{1}{2} {3}", R.ToString ("X2"), G.ToString ("X2"), B.ToString ("X2"), A.ToString ("X2"));
        }

        public override string ToString () {
            return ToRGB ( );
        }

        public float[ ] ToOpenGL () {
            return new float[ ] {
                R / 255f, G / 255f, B / 255f, A / 255f,
                R / 255f, G / 255f, B / 255f, A / 255f,
                R / 255f, G / 255f, B / 255f, A / 255f,
                R / 255f, G / 255f, B / 255f, A / 255f
            };
        }

        #region default colors
        // source : http://clrs.cc/

        public static Color Navy { get { return new Color ("#001F3F"); } }
        public static Color Blue { get { return new Color ("#0074D9"); } }
        public static Color Aqua { get { return new Color ("#7FDBFF"); } }
        public static Color Teal { get { return new Color ("#39CCCC"); } }
        public static Color Olive { get { return new Color ("#4D9970"); } }
        public static Color Green { get { return new Color ("#2ECC40"); } }
        public static Color Lime { get { return new Color ("#01FF70"); } }
        public static Color Yellow { get { return new Color ("#FFDC00"); } }
        public static Color Orange { get { return new Color ("#FF851B"); } }
        public static Color Red { get { return new Color ("#FF4136"); } }
        public static Color Maroon { get { return new Color ("#84144B"); } }
        public static Color Fuchsia { get { return new Color ("#F012BE"); } }
        public static Color Purple { get { return new Color ("#B10DC9"); } }
        public static Color Black { get { return new Color ("#111111"); } }
        public static Color Gray { get { return new Color ("#AAAAAA"); } }
        public static Color Silver { get { return new Color ("#DDDDDD"); } }
        public static Color White { get { return new Color ("#FFFFFF"); } }
        #endregion
    }
}