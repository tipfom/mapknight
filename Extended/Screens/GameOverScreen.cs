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
            UIDim dim = new UIDim(this, 0.15f, UIDepths.BACKGROUND);
            UIImage skull = new UIImage(this, new UILayout(new UIMargin(0.4f, 0.4f * 35f / 29f), UIMarginType.Absolute, UIPosition.Center, UIPosition.Center), "skull", "skull", UIDepths.FOREGROUND, Color.White);
            UILabel continueLabel = new UILabel(this, new UILayout(new UIMargin(0, 0, .3f, 0), UIMarginType.Absolute, UIPosition.Top | UIPosition.Center, UIPosition.Bottom | UIPosition.Center, skull), .1f, "Click to continue...", UITextAlignment.Center);

            dim.Release += OnScreenClicked;
            skull.Release += OnScreenClicked;
            continueLabel.Release += OnScreenClicked;
        }

        private void OnScreenClicked ( ) {
            Screen.Active = Screen.MainMenu;
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