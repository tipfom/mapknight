using System.Collections.Generic;
using mapKnight.Basic;

namespace mapKnight.Android.CGL.GUI {
    public class GUIButton : GUIItem {
        const float DEFAULT_TEXT_SIZE = 0.1f;

        private string _Text;
        public string Text { get { return _Text; } set { _Text = value; RequestUpdate ( ); } }
        private Color _Color;
        public Color Color { get { return _Color; } set { _Color = value; RequestUpdate ( ); } }
        private fVector2D charSize;

        public GUIButton (string text, fRectangle bounds) : this (text, DEFAULT_TEXT_SIZE, Color.White, bounds) {

        }

        public GUIButton (string text, Color color, fRectangle bounds) : this (text, DEFAULT_TEXT_SIZE, color, bounds) {

        }

        public GUIButton (string text, float textsize, Color color, fRectangle bounds) : base (bounds, false) {
            Text = text;
            Color = color;
            charSize = new fVector2D (GUILabel.CHAR_WIDTH_PIXEL * textsize / GUILabel.CHAR_HEIGHT_PIXEL, textsize);
            base.Click += this_Click;
            base.Release += this_Release;
        }

        private void this_Release () {
            RequestUpdate ( );
        }

        private void this_Click () {
            RequestUpdate ( );
        }

        public override List<CGLVertexData> GetVertexData () {
            List<CGLVertexData> vertexData = new List<CGLVertexData> ( );
            vertexData.Add (new CGLVertexData (Screen.ToGlobal (Bounds.GetVerticies ( )), (this.Clicked ? "button_pressed" : "button_idle"), Color));

            fVector2D textSize = GUILabel.MeasureText (this.Text, charSize);
            fVector2D centeredTextPosition = Screen.ToGlobal (new fVector2D (this.Bounds.Left + this.Bounds.Width / 2, this.Bounds.Top + this.Bounds.Height / 2)) - (textSize / new fVector2D (2, -2));
            vertexData.AddRange (GUILabel.GetVertexData (this.Text, centeredTextPosition, charSize));
            return vertexData;
        }
    }
}