using mapKnight.Extended.Graphics.UI;
using mapKnight.Extended.Graphics.UI.Layout;

namespace mapKnight.Extended.Screens {

    public class GameOverScreen : Screen {

        public GameOverScreen ( ) {
        }

        public override void Load ( ) {
            new UILabel(this, new UIHorizontalCenterMargin(0f), new UIVerticalCenterMargin(0f), 0.1f, "YOu Noob DIED!!11eleven");
        }
    }
}