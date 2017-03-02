using mapKnight.Core;
using mapKnight.Extended.Graphics;
using mapKnight.Extended.Graphics.UI;
using mapKnight.Extended.Graphics.UI.Layout;
using Map = mapKnight.Extended.Graphics.Map;
using static mapKnight.Extended.Graphics.Programs.MatrixProgram;
using mapKnight.Extended.Graphics.Buffer;
using System;
using System.Linq;

namespace mapKnight.Extended.Screens {

    public class GameOverScreen : Screen {
        private Map map;

        public GameOverScreen (Map map) {
            this.map = map;
        }

        public override void Load ( ) {
            new UIDim(this, 0.15f, UIDepths.BACKGROUND).Release += ( ) => {
                Screen.Active = Screen.MainMenu;
            };
            new UIImage(this, new UIHorizontalCenterMargin(0f), new UIVerticalCenterMargin(0f), new AbsoluteSize(1.2f / 35f * 29f, 1.2f), "skull", "skull", UIDepths.FOREGROUND, Color.White);
        }

        public override void Update (DeltaTime dt) {
            map.Update(dt);
            base.Update(dt);
        }

        public override void Draw ( ) {
            map.Draw( );
            base.Draw( );
        }
    }
}