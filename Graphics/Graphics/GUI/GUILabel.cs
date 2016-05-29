using mapKnight.Core;
using System.Collections.Generic;

namespace mapKnight.Graphics.GUI {
    public class GUILabel : GUIItem {
        public const int CHAR_WIDTH_PIXEL = 7;
        public const int CHAR_HEIGHT_PIXEL = 9;
        public const int CHAR_SPACING_PIXEL = 1;
        public const int DEFAULT_TEXT_DEPTH = - 2;

        private string _Text;
        public string Text {
            get { return _Text; }
            set { _Text = value; RequestUpdate( ); }
        }
        private Color _Color;
        public Color Color {
            get { return _Color; }
            set { _Color = value; RequestUpdate( ); }
        }

        readonly Vector2 charSize;

        public GUILabel (Vector2 position, float size, string text) : this(position, DEFAULT_TEXT_DEPTH, size, text) {

        }

        public GUILabel (Vector2 position, int depth, float size, string text) : this(position, depth, size, Color.White, text) {

        }

        public GUILabel (Vector2 position, int depth, float size, Color color, string text) : base(new Rectangle(position, new Vector2(0f, 0f)), depth) {
            // label needs no touch management
            this._Text = text;
            this._Color = color;
            this.charSize = new Vector2(CHAR_WIDTH_PIXEL * size / CHAR_HEIGHT_PIXEL, size);
        }

        public override List<VertexData> GetVertexData ( ) {
            return GetVertexData(this.Text, this.Position, this.charSize, DepthOnScreen, this.Color);
        }

        public Vector2 MeasureText ( ) {
            return MeasureText(Text, this.charSize);
        }


        public static List<VertexData> GetVertexData (string text, Vector2 position, Vector2 charSize, int depth, Color color) {
            List<VertexData> vertexData = new List<VertexData>( );
            charSize *= 2; // scale to screen size

            Vector2 currentPoint = position;
            foreach (char character in text) {
                if (character == '\n') {
                    currentPoint.X = position.X;
                    currentPoint.Y -= charSize.Y;
                } else if (character == ' ') {
                    currentPoint.X += charSize.X;
                } else {
                    vertexData.Add(new VertexData(
                        new float[ ] {
                            currentPoint.X ,currentPoint.Y,
                            currentPoint.X,currentPoint.Y - charSize.Y,
                            currentPoint.X+ charSize.X,currentPoint.Y - charSize.Y,
                            currentPoint.X+ charSize.X,currentPoint.Y},
                        character.ToString( ).ToUpper( ),
                        depth,
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

            return new Vector2(charSize.X * maxWidth, charSize.Y * height);
        }

    }
}