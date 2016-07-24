using System.Collections.Generic;
using mapKnight.Core;
using mapKnight.Extended.Graphics.UI.Layout;

namespace mapKnight.Extended.Graphics.UI {
    public class UILabel : UIItem {
        public const int CHAR_WIDTH_PIXEL = 7;
        public const int CHAR_HEIGHT_PIXEL = 9;
        public const int CHAR_SPACING_PIXEL = 1;
        public const int DEFAULT_TEXT_DEPTH = -2;

        private static Dictionary<char, float> charScales = new Dictionary<char, float>( );

        static UILabel ( ) {
            foreach (KeyValuePair<string, float[ ]> entry in UIRenderer.Texture.Sprites) {
                char entryCharacter;
                if (char.TryParse(entry.Key, out entryCharacter)) {
                    float[ ] verticies = UIRenderer.Texture.Get(entry.Key);
                    float scale = ((verticies[4] - verticies[0]) * UIRenderer.Texture.Width) / ((verticies[3] - verticies[1]) * UIRenderer.Texture.Height);
                    charScales.Add(entryCharacter, scale);
                }
            }
        }

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

        readonly float charSize;

        public UILabel (Screen owner, UIMargin hmargin, UIMargin vmargin, float size, string text) : this(owner, hmargin, vmargin, DEFAULT_TEXT_DEPTH, size, text) {

        }

        public UILabel (Screen owner, UIMargin hmargin, UIMargin vmargin, int depth, float size, string text) : this(owner, hmargin, vmargin, depth, size, Color.White, text) {

        }

        public UILabel (Screen owner, UIMargin hmargin, UIMargin vmargin, int depth, float size, Color color, string text) : base(owner, hmargin, vmargin, new Vector2(0, 0), depth) {
            // label needs no touch management
            this._Text = text;
            this._Color = color;
            this.charSize = size;
            vmargin.Bind(this); hmargin.Bind(this);
        }

        public override List<VertexData> GetVertexData ( ) {
            return GetVertexData(this.Text, this.Position, this.charSize, DepthOnScreen, this.Color);
        }

        public Vector2 MeasureText ( ) {
            return MeasureText(Text, this.charSize);
        }


        public static List<VertexData> GetVertexData (string text, Vector2 position, float charSize, int depth, Color color) {
            List<VertexData> vertexData = new List<VertexData>( );
            charSize *= 2; // scale to screen size

            Vector2 currentPoint = position;
            foreach (char character in text.ToUpper( )) {
                float characterWidth = charScales[character] * charSize;
                if (character == '\n') {
                    currentPoint.X = position.X;
                    currentPoint.Y -= charSize;
                } else if (character == ' ') {
                    currentPoint.X += charSize;
                } else {
                    vertexData.Add(new VertexData(
                        new float[ ] {
                            currentPoint.X ,currentPoint.Y,
                            currentPoint.X,currentPoint.Y - charSize,
                            currentPoint.X+ characterWidth,currentPoint.Y - charSize,
                            currentPoint.X+ characterWidth,currentPoint.Y},
                        character.ToString( ),
                        depth,
                        color
                        ));
                    currentPoint.X += characterWidth;
                }
            }

            return vertexData;
        }

        public static Vector2 MeasureText (string text, float charSize) {
            charSize *= 2;

            float maxWidth = 0;
            float currentWidth = 0;
            int height = 1;
            foreach (char character in text.ToUpper( )) {
                if (character == '\n') {
                    height++;
                    if (currentWidth > maxWidth)
                        maxWidth = currentWidth;
                    currentWidth = 0;
                } else {
                    currentWidth += charScales[character] * charSize;
                }
            }
            if (currentWidth > maxWidth)
                maxWidth = currentWidth;

            return new Vector2(maxWidth, charSize * height);
        }
    }
}