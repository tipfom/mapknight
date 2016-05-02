using System;
using mapKnight.Android.CGL;
using mapKnight.Android.CGL.GUI;
using mapKnight.Android.Config;

namespace mapKnight.Android.Scenes {
    public class GameScene : IScene {
        public GUI GUI { get; private set; }
        public CGLMap Map;
        public CGLCamera Camera;

        public GameScene (GameConfig config) {
            this.Map = new CGLMap (config.Map);
            GUI = new GUI ( );
            Camera = new CGLCamera (config.CharacterOffset);
        }

        public void Begin (Type caller, object[ ] data) {

        }

        public void Draw () {
            GUI.Draw ( );
            Map.Draw (Camera);
            // Entity.CGLEntity.Draw ();
        }

        public void Update (float dt) {
            Map.updateTextureBuffer (Camera);
            Map.Step (dt);

            // Entity.CGLEntity.Update (dt);
            GUI.Update (dt);

            Camera.Update (new Basic.fVector2D (0f, 0f), Map);
        }
    }
}