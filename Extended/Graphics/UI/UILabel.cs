using System.Collections.Generic;
using System.Linq;
using mapKnight.Core;
using mapKnight.Extended.Graphics.UI.Layout;

namespace mapKnight.Extended.Graphics.UI {
    public class UILabel : UIItem {
        public const int CHAR_WIDTH_PIXEL = 7;
        public const int CHAR_HEIGHT_PIXEL = 9;
        public const int CHAR_SPACING_PIXEL = 1;

        private static Dictionary<char, float> charScales = new Dictionary<char, float>( ) { [' '] = 1f };

        static UILabel ( ) {
            foreach (string entry in UIRenderer.Texture.Sprites( )) {
                char entryCharacter;
                if (char.TryParse(entry, out entryCharacter)) {
                    float[ ] verticies = UIRenderer.Texture[entry];
                    float scale = ((verticies[4] - verticies[0]) * UIRenderer.Texture.Width) / ((verticies[3] - verticies[1]) * UIRenderer.Texture.Height);
                    charScales.Add(entryCharacter, scale);
                }
            }
        }

        private string _Text;
        public string Text {
            get { return _Text; }
            set { _Text = value; lines = Text.Split('\n'); Layout.AdjustSize(MeasureText(value, charSize)); } //size setting will call requupdate
        }
        private string[ ] lines;

        private UITextAlignment _Alignment;
        public UITextAlignment Alignment { get { return _Alignment; } set { _Alignment = value; IsDirty = true; } }

        private Color _Color;
        public Color Color {
            get { return _Color; }
            set { _Color = value; IsDirty = true; }
        }

        readonly float charSize;

        public UILabel (Screen owner, UILayout layout, float size, string text, UITextAlignment alignment = UITextAlignment.Left) : this(owner, layout, UIDepths.MIDDLE, size, text, alignment) {

        }

        public UILabel (Screen owner, UILayout layout, int depth, float size, string text, UITextAlignment alignment = UITextAlignment.Left) : this(owner, layout, depth, size, Color.White, text, alignment) {

        }

        public UILabel (Screen owner, UILayout layout, int depth, float size, Color color, string text, UITextAlignment alignment = UITextAlignment.Left) : base(owner, layout, depth) {
            charSize = size;
            Text = text;
            Color = color;
            Alignment = alignment;
            Layout.Refresh( );
        }

        public override IEnumerable<DepthVertexData> ConstructVertexData ( ) {
            switch (Alignment) {
                case UITextAlignment.Left:
                    return GetVertexData(lines, Alignment, Layout.Position, this.charSize, Depth, this.Color);
                case UITextAlignment.Right:
                    return GetVertexData(lines, Alignment, Layout.Position + new Vector2(Layout.Width, 0), this.charSize, Depth, this.Color);
                case UITextAlignment.Center:
                    return GetVertexData(lines, Alignment, Layout.Position + new Vector2(Layout.Width * 0.5f, 0), this.charSize, Depth, this.Color);
                default:
                    return null;
            }
        }

        public Vector2 MeasureText ( ) {
            return MeasureText(Text, this.charSize);
        }

        public static IEnumerable<DepthVertexData> GetVertexData (string[ ] lines, UITextAlignment alignment, Vector2 position, float charSize, int depth, Color color) {
            Vector2 currentPosition = position;
            Vector2[ ] sizes;

            switch (alignment) {
                case UITextAlignment.Left:
                    foreach (string line in lines) {
                        foreach (char character in line.ToUpper( )) {
                            if (character == ' ') {
                                currentPosition.X += charSize;
                            } else {
                                float characterWidth = charScales[character] * charSize;
                                yield return new DepthVertexData(
                                    new float[ ] {
                                        currentPosition.X, currentPosition.Y,
                                        currentPosition.X, currentPosition.Y - charSize,
                                        currentPosition.X + characterWidth, currentPosition.Y - charSize,
                                        currentPosition.X + characterWidth, currentPosition.Y },
                                    character.ToString( ),
                                    depth,
                                    color);
                                currentPosition.X += characterWidth;
                            }
                        }
                        currentPosition.X = position.X;
                        currentPosition.Y -= charSize;
                    }
                    break;
                case UITextAlignment.Right:
                    for (int i = 0; i < lines.Length; i++) {
                        currentPosition.X = position.X;
                        foreach (char character in lines[i].ToUpper( ).Reverse( )) {
                            if (character == ' ') {
                                currentPosition.X -= charSize;
                            } else {
                                float characterWidth = charScales[character] * charSize;
                                yield return new DepthVertexData(
                                    new float[ ] {
                                        currentPosition.X - characterWidth, currentPosition.Y,
                                        currentPosition.X - characterWidth, currentPosition.Y - charSize,
                                        currentPosition.X, currentPosition.Y - charSize,
                                        currentPosition.X, currentPosition.Y },
                                    character.ToString( ),
                                    depth,
                                    color);
                                currentPosition.X -= characterWidth;
                            }
                        }
                        currentPosition.Y -= charSize;
                    }
                    break;
                case UITextAlignment.Center:
                    sizes = MeasureLines(lines, charSize);

                    for (int i = 0; i < lines.Length; i++) {
                        currentPosition.X = position.X - sizes[i].X * 0.5f;
                        foreach (char character in lines[i].ToUpper( )) {
                            if (character == ' ') {
                                currentPosition.X += charSize;
                            } else {
                                float characterWidth = charScales[character] * charSize;
                                yield return new DepthVertexData(
                                    new float[ ] {
                                        currentPosition.X, currentPosition.Y,
                                        currentPosition.X, currentPosition.Y - charSize,
                                        currentPosition.X + characterWidth, currentPosition.Y - charSize,
                                        currentPosition.X + characterWidth, currentPosition.Y },
                                    character.ToString( ),
                                    depth,
                                    color);
                                currentPosition.X += characterWidth;
                            }
                        }
                        currentPosition.Y -= charSize;
                    }
                    break;
            }
        }

        public static Vector2[ ] MeasureLines (string[ ] lines, float charSize) {
            Vector2[ ] lineSizes = new Vector2[lines.Length];
            for (int i = 0; i < lineSizes.Length; i++) {
                lineSizes[i].Y = charSize;
                foreach (char character in lines[i].ToUpper( )) {
                    lineSizes[i].X += charScales[character] * charSize;
                }
            }
            return lineSizes;
        }

        public static Vector2 MeasureText (string text, float charSize) {
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