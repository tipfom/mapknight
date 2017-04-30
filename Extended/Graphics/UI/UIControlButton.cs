using System.Collections.Generic;
using mapKnight.Extended.Graphics.UI.Layout;
using mapKnight.Core;

namespace mapKnight.Extended.Graphics.UI
{
    public class UIControlButton : UIItem
    {
        private string direction;

        public UIControlButton(Screen owner, string direction, UILayout layout) : base(owner, layout, UIDepths.MIDDLE, true) {
            this.direction = direction;
            Click += ( ) => IsDirty = true;
            Release += ( ) => IsDirty = true;
            Leave += ( ) => IsDirty = true;
        }

        public override IEnumerable<DepthVertexData> ConstructVertexData ( ) {
            yield return new DepthVertexData(Layout, "btn_" + direction + "ctl" + (Clicked ? "a" : "i"), Depth, Color.White);
        }
    }
}
