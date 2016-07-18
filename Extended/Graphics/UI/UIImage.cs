using System.Collections.Generic;
using mapKnight.Core;

namespace mapKnight.Extended.Graphics.UI {
    public class UIImage : UIItem {
        private string textureIdle;
        private string textureClick;
        private Color _ModificationColor;
        public Color ModificationColor { get { return _ModificationColor; } set { _ModificationColor = value; RequestUpdate( ); } }

        public UIImage (Screen owner, string idletexture, string clicktexture, int depth, Color modificationcolor, Rectangle bounds) : base(owner, bounds, depth) {
            textureIdle = idletexture;
            textureClick = clicktexture;

            this.Click += ( ) => { RequestUpdate( ); };
            this.Release += ( ) => { RequestUpdate( ); };
        }

        public override List<VertexData> GetVertexData ( ) {
            return new List<VertexData>( ) { new VertexData(Bounds.Verticies(DEFAULT_ANCHOR), this.Clicked ? textureClick : textureIdle, DepthOnScreen, ModificationColor) };
        }
    }
}