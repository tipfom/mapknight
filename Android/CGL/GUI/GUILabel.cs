using System.Collections.Generic;
using mapKnight.Basic;

namespace mapKnight.Android.CGL.GUI {
    public class GUILabel : GUIItem {
        public const int CHAR_WIDTH_PIXEL = 7;
        public const int CHAR_HEIGHT_PIXEL = 9;
        public const int CHAR_SPACING_PIXEL = 1;

        private string _Text;
        public string Text {
            get { return _Text; }
            set { _Text = value; RequestUpdate ( ); }
        }
        private Color _Color;
        public Color Color {
            get { return _Color; }
            set { _Color = value; RequestUpdate ( ); }
        }

        readonly Vector2 charSize;

        public GUILabel (Vector2 position, float size, string text = "default") : this (position, size, Color.White, text) {

        }

        public GUILabel (Vector2 position, float size, Color color, string text = "default") : base (new Rectangle (position, new Vector2 (0f, 0f))) {
            // label needs no touch management
            this._Text = text;
            this._Color = color;
            this.charSize = new Vector2 (CHAR_WIDTH_PIXEL * size / CHAR_HEIGHT_PIXEL, size);
        }

        public override List<CGLVertexData> GetVertexData () {
            return GetVertexData (this.Text, Screen.ToGlobal (this.Position), this.charSize, this.Color);
        }

        public Vector2 MeasureText () {
            return MeasureText (Text, this.charSize);
        }


        public static List<CGLVertexData> GetVertexData (string text, Vector2 position, Vector2 charSize, Color color) {
            List<CGLVertexData> vertexData = new List<CGLVertexData> ( );
            charSize *= 2; // scale to screen size

            Vector2 currentPoint = position;
            foreach (char character in text) {
                if (character == '\n') {
                    currentPoint.X = position.X;
                    currentPoint.Y -= charSize.Y;
                } else if (character == ' ') {
                    currentPoint.X += charSize.X;
                } else {
                    vertexData.Add (new CGLVertexData (
                        new float[ ] {
                            currentPoint.X ,currentPoint.Y,
                            currentPoint.X,currentPoint.Y - charSize.Y,
                            currentPoint.X+ charSize.X,currentPoint.Y - charSize.Y,
                            currentPoint.X+ charSize.X,currentPoint.Y},
                        character.ToString ( ).ToUpper ( ),
                        color
                        ));
                    currentPoint.X += charSize.X;
                }
            }

            return vertexData;
        }

        public static Vector2 MeasureText (string text, Vector2 charSize) {
            charSize *= 2;

            int maxWidth = 0;
            int currentWidth = 0;
            int height = 1;
            foreach (char charachter in text) {
                if (charachter == '\n') {
                    height++;
                    if (currentWidth > maxWidth)
                        maxWidth = currentWidth;
                    currentWidth = 0;
                } else {
                    currentWidth++;
                }
            }
            if (currentWidth > maxWidth)
                maxWidth = currentWidth;

            return new Vector2 (charSize.X * maxWidth, charSize.Y * height);
        }

    }
}