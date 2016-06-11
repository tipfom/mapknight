using mapKnight.Core;

namespace mapKnight.Extended.Graphics.Screens {
    public class MainMenuScreen : Screen {
        public MainMenuScreen ( ) {
            Interface.Add(new GUI.GUILabel(new Vector2(0, 0), 0.05f, "meinnameisttim"));
            Interface.Add(new GUI.GUIButton("meinnameistcooleralstim", new Rectangle(-1, 0, 1, 0.3f)));
        }
    }
}
