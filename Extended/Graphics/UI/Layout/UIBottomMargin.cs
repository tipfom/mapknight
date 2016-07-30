using System;
using System.Collections.Generic;
using System.Text;

namespace mapKnight.Extended.Graphics.UI.Layout {
    public class UIBottomMargin : UIMargin {
        public UIBottomMargin (float margin) : base(margin, false) {
        }

        protected override void CalculateScreenPosition ( ) {
            ScreenPosition = -1f + Margin + Owner.Size.Y;
        }
    }
}
