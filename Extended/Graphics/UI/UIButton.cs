using System.Collections.Generic;
using mapKnight.Core;
using mapKnight.Extended.Graphics.UI.Layout;

namespace mapKnight.Extended.Graphics.UI {

    public class UIButton : UIItem {
        private const float DEFAULT_TEXT_SIZE = 0.1f;
        private const float EDGE_WIDTH_HEIGHT_RATIO = 3f / 20f;

        private Color _Color;
        private string _Text;
        private float charSize;
        private string[ ] lines;

        public UIButton (Screen owner, UIMargin hmargin, UIMargin vmargin, Vector2 size, string text) : this(owner, hmargin, vmargin, size, text, DEFAULT_TEXT_SIZE, 0, Color.White) {
        }

        public UIButton (Screen owner, UIMargin hmargin, UIMargin vmargin, Vector2 size, string text, int depth, Color color) : this(owner, hmargin, vmargin, size, text, DEFAULT_TEXT_SIZE, depth, color) {
        }

        public UIButton (Screen owner, UIMargin hmargin, UIMargin vmargin, Vector2 size, string text, float textsize, int depth, Color color) : base(owner, hmargin, vmargin, size, depth, false) {
            _Text = text;
            lines = text.Split('\n');
            charSize = textsize;
            Color = color;
            base.Click += this_Click;
            base.Release += this_Release;
        }

        public Color Color { get { return _Color; } set { _Color = value; IsDirty = true; ; } }
        public string Text { get { return _Text; } set { _Text = value; lines = _Text.Split('\n'); IsDirty = true; } }

        public override IEnumerable<DepthVertexData> ConstructVertexData ( ) {
            string textureDomain = "btn_" + (Clicked ? "p" : "i");
            float w = EDGE_WIDTH_HEIGHT_RATIO * Size.Y;
            yield return new DepthVertexData(new UIRectangle(Position.X, Position.Y, w, Size.Y).Verticies, textureDomain + "l", Depth, Color);
            yield return new DepthVertexData(new UIRectangle(Position.X+w, Position.Y, Size.X - 2 * w, Size.Y).Verticies, textureDomain + "c", Depth, Color);
            yield return new DepthVertexData(new UIRectangle(Position.X + Size.X - w, Position.Y, w, Size.Y).Verticies, textureDomain + "r", Depth, Color);

            Vector2 textPosition = new Vector2(Position.X + Size.X * 0.5f, Position.Y - (Size.Y - lines.Length * charSize) * 0.5f);

            foreach (DepthVertexData d in UILabel.GetVertexData(new string[ ] { Text }, UITextAlignment.Center, textPosition, charSize, Depth, Color.White)) {
                yield return d;
            }
        }

        private void this_Click ( ) {
            IsDirty = true;
        }

        private void this_Release ( ) {
            IsDirty = true; 
        }
    }
}