using mapKnight.Android.CGL.GUI;
using mapKnight.Basic;
using System;

namespace mapKnight.Android.Scenes {
    public class MainMenuScene : IScene {
        public GUI GUI { get; private set; }
        private GUILabel versionLabel;
        private GUILabel headerLabel;
        private GUIButton playButton;

        public MainMenuScene () {
            string versionLabelText = $"Version : {Content.Version}";


            GUI = new GUI ();
            versionLabel = GUI.Add (new GUILabel (new Vector2 (0f, 0f), 0.05f, versionLabelText));
            headerLabel = GUI.Add (new GUILabel (new Vector2 (0f, 0.8f), 0.2f, "MAPKNIGHT"));
            playButton = GUI.Add (new GUIButton ("play", Color.White, new Rectangle (0.25f, 0.35f, 0.5f, 0.3f)));
            playButton.Click += () => {
                Content.SceneManager.Next (this, typeof (GameScene), null);
            };
        }

        public void Begin (Type caller, object[] data) {
            if (caller == null)
                return; // has been called from the initialization

            // handle caller specific tasks here
        }

        public void Draw () {
            GUI.Draw ();
        }

        public void Update (float dt) {
            GUI.Update (dt);
        }
    }
}