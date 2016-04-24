using mapKnight.Basic;

namespace mapKnight.Android.CGL.GUI {
    public class GUILabel {
        public delegate void OnTextChanged (GUILabel sender);
        public event OnTextChanged TextChanged;
        public delegate void OnPositionChanged (GUILabel sender);
        public event OnPositionChanged PositionChanged;

        public int[] ParsedText { get; private set; }
        private string _Text;
        public string Text {
            get { return _Text; }
            set {
                _Text = value;
                ParsedText = GUITextRenderer.Parse (value);
                TextChanged?.Invoke (this);
            }
        }
        private fVector2D _Position; // links oben 0,0
        public fVector2D Position {
            get { return _Position; }
            set {
                // transform to global space
                _Position = new fVector2D ((value.X - 0.5f) * 2 * Content.ScreenRatio, (value.Y - 0.5f) * -2);
                PositionChanged?.Invoke (this);
            }
        }
        public readonly float Size;
        public readonly fVector2D CharSize;

        public GUILabel (fVector2D position, float size, string text = "default") {
            this.Position = position;
            this._Text = text;
            this.ParsedText = GUITextRenderer.Parse (_Text);
            this.Size = size;
            this.CharSize = new fVector2D (GUITextRenderer.CHAR_WIDTH_PXL * size / GUITextRenderer.CHAR_HEIGHT_PXL, size);
        }

        public fVector2D MeasureText (string text) {
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
            return new fVector2D (this.CharSize.X * maxWidth, this.CharSize.Y * height);
        }

        public fVector2D MeasureText () {
            return MeasureText (Text);
        }
    }
}