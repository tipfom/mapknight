using mapKnight.Android.CGL;
using mapKnight.Android.CGL.GUI;
using mapKnight.Android.Config;
using mapKnight.Android.ECS;
using mapKnight.Android.ECS.Components.Configs;
using System;

namespace mapKnight.Android.Scenes {
    public class GameScene : IScene {
        public GUI GUI { get; private set; }
        public CGLMap Map;

        public GameScene (GameConfig config) {
            GUI = new GUI ();
            Map = new CGLMap (config);
            EntityConfig potatoe_joe_config = Assets.Load<EntityConfig> ("potatoe_patrick");
            Entity potatoe_joe_example = potatoe_joe_config.Create (new Basic.Vector2 (5f, 7f), Map);
            //potatoe_joe_example.SetComponentInfo (Entity.Component.Type.Animation, Entity.Component.Type.Animation, Entity.Component.Action.Animation, "check_1234");
            EntityConfig platform_config = Assets.Load<EntityConfig> ("test_platform");
            PlatformComponentConfig platformConfig = new PlatformComponentConfig ();
            platformConfig.Speed = 3;
            platformConfig.Waypoints = new System.Collections.Generic.List<Basic.Vector2> () { new Basic.Vector2 (0, 0), new Basic.Vector2 (5, 0), new Basic.Vector2 (5, 8) };
            platform_config.Components.Add (platformConfig);
            Entity platform1 = platform_config.Create (new Basic.Vector2 (0, 0), Map);
        }

        public void Begin (Type caller, object[] data) {
            Map.Prepare ();
        }

        public void Draw () {
            Map.Draw ();
            GUI.Draw ();
        }

        private float dtpassed;
        public void Update (float dt) {
            dtpassed += dt / 1000f;

            GUI.Update (dt);
            Map.Update (dt, 0);
        }
    }
}