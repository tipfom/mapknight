using System;
using System.Collections.Generic;
using System.Text;
using mapKnight.Core;
using mapKnight.Extended.Graphics.UI.Layout;

namespace mapKnight.Extended.Graphics.UI {
    public class UIPanel : UIItem {
        public UIPanel (Screen owner, UIMargin hmargin, UIMargin vmargin, Vector2 size, bool multiclick = false) : base(owner, hmargin, vmargin, size, 0, multiclick) {
        }

        public override IEnumerable<DepthVertexData> ConstructVertexData ( ) {
            yield break;
        }
    }
}
