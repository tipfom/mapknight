using System;
using System.Collections.Generic;
using System.Linq;
using mapKnight.Core;
using mapKnight.Extended.Components.Attributes;
using mapKnight.Extended.Graphics;

namespace mapKnight.Extended.Components {

    [ComponentRequirement(typeof(DrawComponent))]
    [UpdateBefore(ComponentEnum.Draw)]
    public class SpriteComponent : Component {
        private Dictionary<string, string> cachedResult = new Dictionary<string, string>( );
        private Dictionary<string, SpriteAnimation> sprites;

        public SpriteComponent (Entity owner, Dictionary<string, SpriteAnimation> sprites, string texture) : base(owner) {
            this.sprites = sprites;
            foreach (string key in sprites.Keys)
                cachedResult.Add(key, "");
            Owner.World.Renderer.AddTexture(Owner.Species, Assets.Load<SpriteBatch>(texture));
        }

        public override void Update (DeltaTime dt) {
            foreach (string bone in sprites.Keys) {
                sprites[bone].Update(dt.Milliseconds);
                cachedResult[bone] = sprites[bone].Current;
            }

            Owner.SetComponentInfo(ComponentEnum.Draw, new Tuple<ComponentData, object>(ComponentData.Texture, cachedResult));
        }

        public new class Configuration : Component.Configuration {
            public Dictionary<string, SpriteAnimation> Sprites;
            public string Texture;

            public override Component Create (Entity owner) {
                return new SpriteComponent(owner, Sprites, Texture);
            }
        }
    }
}