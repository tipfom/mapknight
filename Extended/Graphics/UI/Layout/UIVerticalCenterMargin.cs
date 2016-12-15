using System;
using System.Collections.Generic;
using System.Text;

namespace mapKnight.Extended.Graphics.UI.Layout {
    public class UIVerticalCenterMargin : UIMargin {
        public UIVerticalCenterMargin (float margin) : base(margin, false) {
        }

        protected override void CalculateScreenPosition ( ) {
            ScreenPosition = Owner.Size.Y / 2f + Margin;
            base.CalculateScreenPosition( );
        }
    }
}
