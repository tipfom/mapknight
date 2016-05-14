using mapKnight.Basic.Components;
using System.Collections.Generic;

namespace mapKnight.Android.ECS.Components.Configs {
    public class SpriteComponentConfig : ComponentConfig {
        public override int Priority { get { return 1; } }

        public string Texture;
        public Dictionary<string, Sprite> Sprites;

        public override Component Create (Entity owner) {
            return new SpriteComponent (owner, Sprites, Texture);
        }
    }
}