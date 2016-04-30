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
            set {
                _Text = value;
                RequestUpdate ( );
            }
        }

        private fVector2D _Position; // links oben 0,0
        public fVector2D Position {
            get { return _Position; }
            set {
                // transform to global space
                _Position = value;
                RequestUpdate ( );
            }
        }
        readonly fVector2D charSize;

        public GUILabel (fVector2D position, float size, string text = "default") : base (new fRectangle (0f, 0f, 0f, 0f)) {
            // label needs no touch management
            this._Position = position;
            this._Text = text;
            this.charSize = new fVector2D (CHAR_WIDTH_PIXEL * size / CHAR_HEIGHT_PIXEL, size);
        }

        public override List<VertexData> GetVertexData () {
            return GetVertexData (this.Text, Screen.ToGlobal (this.Position), this.charSize);
        }

        public fVector2D MeasureText () {
            return MeasureText (Text, this.charSize);
        }


        public static List<VertexData> GetVertexData (string text, fVector2D position, fVector2D charSize) {
            List<VertexData> vertexData = new List<VertexData> ( );

            fVector2D currentPoint = position;
            foreach (char character in text) {
                if (character == '\n') {
                    currentPoint.X = position.X;
                    currentPoint.Y -= charSize.Y;
                } else if (character == ' ') {
                    currentPoint.X += charSize.X;
                } else {
                    vertexData.Add (new VertexData (
                        new float[ ] {
                            currentPoint.X ,currentPoint.Y,
                            currentPoint.X,currentPoint.Y - charSize.Y,
                            currentPoint.X+ charSize.X,currentPoint.Y - charSize.Y,
                            currentPoint.X+ charSize.X,currentPoint.Y},
                        character.ToString ( ).ToUpper ( ), Color.White
                        ));
                    currentPoint.X += charSize.X;
                }
            }

            return vertexData;
        }

        public static fVector2D MeasureText (string text, fVector2D charSize) {
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

            return new fVector2D (charSize.X * maxWidth, charSize.Y * height);
        }

    }
}