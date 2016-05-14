using System.Collections.Generic;
using mapKnight.Basic;

namespace mapKnight.Android.CGL.GUI {
    public class GUIImage : GUIItem {
        private string textureIdle;
        private string textureClick;
        private Color _ModificationColor;
        public Color ModificationColor { get { return _ModificationColor; } set { _ModificationColor = value; RequestUpdate ( ); } }

        public GUIImage (string idletexture, string clicktexture, Color modificationcolor, Rectangle bounds) : base (bounds) {
            textureIdle = idletexture;
            textureClick = clicktexture;

            this.Click += () => { RequestUpdate ( ); };
            this.Release += () => { RequestUpdate ( ); };
        }

        public override List<CGLVertexData> GetVertexData () {
            return new List<CGLVertexData> ( ) { new CGLVertexData (Screen.ToGlobal (Bounds.GetVerticies ( )), this.Clicked ? textureClick : textureIdle, ModificationColor) };
        }
    }
}