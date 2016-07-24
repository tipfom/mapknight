using System;
using System.Collections.Generic;
using System.Text;

namespace mapKnight.Extended.Graphics.UI.Layout {
    public class UILeftMargin : UIMargin {
        public UILeftMargin (float margin) : base(margin, true) {
        }

        protected override void CalculateScreenPosition ( ) {
            ScreenPosition = -Window.Ratio + Margin;
        }
    }
}
