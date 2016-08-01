using System.Collections.Generic;
using mapKnight.Core;
using mapKnight.Extended.Graphics.UI.Layout;

namespace mapKnight.Extended.Graphics.UI {
    public class UIButton : UIItem {
        const float DEFAULT_TEXT_SIZE = 0.1f;

        private string[ ] lines;
        private string _Text;
        public string Text { get { return _Text; } set { _Text = value; lines = _Text.Split('\n'); RequestUpdate( ); } }
        private Color _Color;
        public Color Color { get { return _Color; } set { _Color = value; RequestUpdate( ); } }
        private float charSize;

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

        private void this_Release ( ) {
            RequestUpdate( );
        }

        private void this_Click ( ) {
            RequestUpdate( );
        }

        public override List<DepthVertexData> GetVertexData ( ) {
            List<DepthVertexData> vertexData = new List<DepthVertexData>( );
            vertexData.Add(new DepthVertexData(Bounds.Verticies(Anchor.Left | Anchor.Top), (this.Clicked ? "button_pressed" : "button_idle"), Depth, Color));

            Vector2 textPosition = new Vector2(Position.X + Size.X * 0.5f, Position.Y - (Size.Y - lines.Length * charSize) * 0.5f);
            vertexData.AddRange(UILabel.GetVertexData(new string[ ] { Text }, UITextAlignment.Center, textPosition, charSize, Depth + 1, Color.White));
            return vertexData;
        }
    }
}