using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using mapKnight.Core;
using mapKnight.Extended.Graphics.UI;
using mapKnight.Extended.Graphics.UI.Layout;

namespace mapKnight.Extended.Screens {
    public class MainMenuScreen : Screen {
        public override void Load( ) {
            new UILabel(this, new UIHorizontalCenterMargin(0f), new UITopMargin(0.1f), 0.2f, "MAPKNIGHT");
            new UILabel(this, new UIRightMargin(0.05f), new UIBottomMargin(0.05f), 0.07f, "VERSION: " + Assembly.GetExecutingAssembly( ).GetName( ).Version.ToString(3)
#if DEBUG
                + " - DEBUG"
#endif                
                );
            UIMap map = new UIMap(this, new UILeftMargin(0.3f), new UIBottomMargin(0.15f), new AbsoluteSize(1.4f, 1.4f), UIDepths.MIDDLE);

            UIButton playButton = new UIButton(this, new UIRightMargin(0.3f), new UIVerticalCenterMargin(0.2f), new AbsoluteSize(1.3f, 0.3f), "PLAY");
            playButton.Release += ( ) => {
                if (map.CurrentSelection != null) {
                    Screen.Gameplay = new GameplayScreen(map.CurrentSelection);
                    UIRenderer.Prepare(Screen.Gameplay);
                    Screen.Gameplay.Load( );
                    Screen.Active = Screen.Gameplay;
                }
            };
            UIButton creditsButton = new UIButton(this, new UIRightMargin(0.3f), new UIVerticalCenterMargin(-0.2f), new AbsoluteSize(1.3f, 0.3f), "CREDITS");
            creditsButton.Release += ( ) => { };
            UIButton settingsButton = new UIButton(this, new UIRightMargin(0.3f), new UIVerticalCenterMargin(-0.6f), new AbsoluteSize(1.3f, 0.3f), "SETTINGS");
            settingsButton.Release += ( ) => { };

            base.Load( );


        }
    }
}
