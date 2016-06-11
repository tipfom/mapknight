using mapKnight.Core;
using System.Collections.Generic;

namespace mapKnight.Extended.Graphics.GUI {
    public class GUIImage : GUIItem {
        private string textureIdle;
        private string textureClick;
        private Color _ModificationColor;
        public Color ModificationColor { get { return _ModificationColor; } set { _ModificationColor = value; RequestUpdate( ); } }

        public GUIImage (string idletexture, string clicktexture, int depth, Color modificationcolor, Rectangle bounds) : base(bounds, depth) {
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