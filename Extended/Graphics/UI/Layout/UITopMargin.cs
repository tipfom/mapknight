using System;
using System.Collections.Generic;
using System.Text;

namespace mapKnight.Extended.Graphics.UI.Layout {
    public class UITopMargin : UIMargin {
        public UITopMargin (float margin) : base(margin, false) {
        }

        protected override void CalculateScreenPosition ( ) {
            ScreenPosition = 1f - Margin;
        }
    }
}
