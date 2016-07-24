using System;
using System.Collections.Generic;
using System.Text;

namespace mapKnight.Extended.Graphics.UI.Layout {
    public class UIHorizontalCenterMargin : UIMargin {
        public UIHorizontalCenterMargin (float margin) : base(margin, false) {
        }

        protected override void CalculateScreenPosition ( ) {
            ScreenPosition = -Owner.Size.X / 2f + Margin;
        }
    }
}
