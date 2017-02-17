using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using mapKnight.Core;
using mapKnight.Extended.Graphics.UI;
using mapKnight.Extended.Graphics.UI.Layout;

namespace mapKnight.Extended.Screens {
    public class MainMenuScreen : Screen {
        public override void Load ( ) {
            new UILabel(this, new UIHorizontalCenterMargin(0f), new UITopMargin(0.2f), 0.2f, "MAPKNIGHT");
            new UILabel(this, new UIRightMargin(0.05f), new UIBottomMargin(0.05f), 0.07f, "VERSION: " + Assembly.GetExecutingAssembly( ).GetName( ).Version.ToString(3)
#if DEBUG
                + " - DEBUG"
#endif                
                );

            UIButton button = new UIButton(this, new UIHorizontalCenterMargin(0f), new UIVerticalCenterMargin(-0.2f), new AbsoluteSize(1.3f, 0.3f), "PLAY");
            button.Release += ( ) => { Screen.Active = Gameplay; };
            base.Load( );
        }
    }
}
