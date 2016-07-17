using System;
using System.Collections.Generic;
using System.Linq;
using mapKnight.Extended.Graphics;

namespace mapKnight.Extended.Components {
    [ComponentRequirement(typeof(DrawComponent))]
    [ComponentOrder(ComponentEnum.Draw)]
    public class SpriteComponent : Component {
        private Dictionary<string, SpriteAnimation> sprites;

        public SpriteComponent (Entity owner, Dictionary<string, SpriteAnimation> sprites, string texture) : base(owner) {
            this.sprites = sprites;
            Owner.Owner.Renderer.AddTexture(Owner.Species, Assets.Load<SpriteBatch>(texture));
        }

        public override void Update (TimeSpan dt) {
            foreach (string bone in sprites.Keys) {
                sprites[bone].Update(dt.Milliseconds);
            }

            Owner.SetComponentInfo(ComponentEnum.Draw, ComponentEnum.Sprite, ComponentData.Texture, sprites.ToDictionary(v => v.Key, v => v.Value.Current));
        }

        public new class Configuration : Component.Configuration {
            public string Texture;
            public Dictionary<string, SpriteAnimation> Sprites;

            public override Component Create (Entity owner) {
                return new SpriteComponent(owner, Sprites, Texture);
            }
        }
    }
}