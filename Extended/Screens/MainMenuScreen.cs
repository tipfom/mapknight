using System;
using System.Collections.Generic;
using System.Text;
using mapKnight.Core;
using mapKnight.Extended.Graphics.GUI;

namespace mapKnight.Extended.Screens {
    public class MainMenuScreen : Screen {
        public override void Load ( ) {
            new GUILabel(this, new Vector2(0, 0), 0.05f, "meinnameisttim");
            GUIButton button = new GUIButton(this, ".;;cool?!!!1", new Rectangle(-1, 1, 1, 0.3f));
            button.Click += ( ) => { Active = Gameplay; };
            base.Load( );
        }
    }
}
