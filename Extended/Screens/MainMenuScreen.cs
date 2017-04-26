using System.Reflection;
using mapKnight.Extended.Graphics.UI;
using mapKnight.Extended.Graphics.UI.Layout;
using mapKnight.Extended.Graphics;
using mapKnight.Extended.Graphics.Buffer;
using OpenTK.Graphics.ES20;
using mapKnight.Core;

namespace mapKnight.Extended.Screens {
    public class MainMenuScreen : Screen {
        public override void Load ( ) {
            new UILabel(this, new UIRightMargin(0.05f), new UIBottomMargin(0.05f), UIDepths.FOREGROUND, 0.07f, "VERSION: " + Assembly.GetExecutingAssembly( ).GetName( ).Version.ToString(3)
#if DEBUG
                + " - DEBUG"
#endif                
                );
            UIMap map = new UIMap(this);

            UIButton playButton = new UIButton(this, new UILeftMargin(293f / 450f * Window.Ratio * 2f), new UIBottomMargin(176f / 450f * Window.Ratio * 2f), new AbsoluteSize(102f / 450f * Window.Ratio * 2f, 53f / 450f * Window.Ratio * 2f), "PLAY");
            playButton.Release += ( ) => {
                if (map.CurrentSelection != null) {
                    Screen.Gameplay = new GameplayScreen(map.CurrentSelection);
                    UIRenderer.Prepare(Screen.Gameplay);
                    Screen.Gameplay.Load( );
                    Screen.Active = Screen.Gameplay;
                }
            };

            base.Load( );
        }
        int x = 5;
        public override void Update (DeltaTime dt) {
            base.Update(dt);
            x--;
            if (x < 0) {
                UIWindow s = new UIWindow( );
                s.FillUIBuffer( );
                Screen.Active = s;
            }
        }

   }
}
