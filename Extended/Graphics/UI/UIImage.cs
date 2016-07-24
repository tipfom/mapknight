using System.Collections.Generic;
using mapKnight.Core;
using mapKnight.Extended.Graphics.UI.Layout;

namespace mapKnight.Extended.Graphics.UI {
    public class UIImage : UIItem {
        private string textureIdle;
        private string textureClick;
        private Color _ModificationColor;
        public Color ModificationColor { get { return _ModificationColor; } set { _ModificationColor = value; RequestUpdate( ); } }

        public UIImage (Screen owner, UIMargin hmargin, UIMargin vmargin, Vector2 size, string idletexture, string clicktexture, int depth, Color modificationcolor) : base(owner, hmargin, vmargin, size, depth) {
            textureIdle = idletexture;
            textureClick = clicktexture;

            this.Click += ( ) => { RequestUpdate( ); };
            this.Release += ( ) => { RequestUpdate( ); };
            vmargin.Bind(this); hmargin.Bind(this);
        }

        public override List<VertexData> GetVertexData ( ) {
            return new List<VertexData>( ) { new VertexData(Bounds.Verticies(DEFAULT_ANCHOR), this.Clicked ? textureClick : textureIdle, DepthOnScreen, ModificationColor) };
        }
    }
}