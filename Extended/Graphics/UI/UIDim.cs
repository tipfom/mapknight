using System.Collections.Generic;
using mapKnight.Core;
using mapKnight.Extended.Graphics.UI.Layout;

namespace mapKnight.Extended.Graphics.UI {
    public class UIDim : UIItem {
        public float Opacity { get { return color.A / 255f; } set { color.A = (byte)((1f - value) * 255); IsDirty = true; } }
        private Color color;

        public UIDim(Screen owner, float opacity, int depth, bool multiclick = false) : base(owner, new UILeftMargin(0), new UITopMargin(0), new AbsoluteSize( ), depth, multiclick) {
            color = new Color(2, 2, 2);
            Opacity = opacity;
        }

        public override IEnumerable<DepthVertexData> ConstructVertexData( ) {
            yield return new DepthVertexData(new float[8] { -Window.Ratio, 1, -Window.Ratio, -1, Window.Ratio, -1, Window.Ratio, 1 }, "blank", Depth, color);
        }
    }
}
