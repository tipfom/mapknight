using System;
using System.Collections.Generic;
using System.Text;
using mapKnight.Core;
using mapKnight.Extended.Graphics.UI;
using mapKnight.Extended.Graphics.UI.Layout;

namespace mapKnight.Extended.Screens {
    public class MainMenuScreen : Screen {
        public override void Load ( ) {
            new UILabel(this, new UIRightMargin(0.3f), new UIBottomMargin(0.25f), 0.1f, "meinnameisttim") { Depth = 1 };
            new UILabel(this, new UIRightMargin(0.3f), new UIBottomMargin(0.25f), 0.1f, "131") { Depth = 2 };
            UIButton button = new UIButton(this, new UILeftMargin(1.3f), new UIBottomMargin(0f), new Vector2(1.3f, 0.3f), ".;;cool?!!!1") { Depth = 0 };
            button.Release += ( ) => { Active = Gameplay; };
            base.Load( );
        }
    }
}
