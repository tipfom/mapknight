using mapKnight.Extended.Graphics;
using System.Collections.Generic;

namespace mapKnight.Extended.Components.Configs {
    public class SpriteComponentConfig : ComponentConfig {
        public override int Priority { get { return 1; } }

        public string Texture;
        public Dictionary<string, SpriteAnimation> Sprites;

        public override Component Create (Entity owner) {
            return new SpriteComponent (owner, Sprites, Texture);
        }
    }
}