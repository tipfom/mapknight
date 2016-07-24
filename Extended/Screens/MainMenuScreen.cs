using System;
using System.Collections.Generic;
using System.Text;
using mapKnight.Core;
using mapKnight.Extended.Graphics.UI;
using mapKnight.Extended.Graphics.UI.Layout;

namespace mapKnight.Extended.Screens {
    public class MainMenuScreen : Screen {
        public override void Load ( ) {
            new UILabel(this, new UIRightMargin(0.3f), new UITopMargin(0.5f), 0.05f, "meinnameisttim");
            UIButton button = new UIButton(this, new UILeftMargin(1.3f), new UIBottomMargin(0f), new Vector2(1.3f, 0.3f), ".;;cool?!!!1");
            button.Release += ( ) => { Active = Gameplay; };
            base.Load( );
        }
    }
}
