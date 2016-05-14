using System;
using mapKnight.Android.CGL;
using mapKnight.Android.CGL.GUI;
using mapKnight.Android.Config;

namespace mapKnight.Android.Scenes {
    public class GameScene : IScene {
        public GUI GUI { get; private set; }
        public CGLCamera Camera;
        public CGLMap Map;

        public GameScene (GameConfig config) {
            GUI = new GUI ( );
            Camera = new CGLCamera (config.CharacterOffset);
            Map = new CGLMap (config.Map, Camera);
            Entity.Entity.Config potatoe_joe_config = Assets.Load<Entity.Entity.Config> ("potatoe_patrick");
            Entity.Entity potatoe_joe_example = potatoe_joe_config.Create (new Basic.Vector2 (5f, 7f), Map);
            //potatoe_joe_example.SetComponentInfo (Entity.Component.Type.Animation, Entity.Component.Type.Animation, Entity.Component.Action.Animation, "check_1234");
        }

        public void Begin (Type caller, object[ ] data) {

        }

        public void Draw () {
            Map.Draw (Camera);
            GUI.Draw ( );
            Map.Draw ( );
        }

        private float dtpassed;
        public void Update (float dt) {
            dtpassed += dt / 1000f;

            GUI.Update (dt);
            Map.Update (dt);

            Camera.Update (new Basic.Vector2 (0, 0), Map);
            Map.updateTextureBuffer (Camera);
        }
    }
}