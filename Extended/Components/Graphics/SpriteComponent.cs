using System;
using System.Collections.Generic;
using System.Linq;
using mapKnight.Core;
using mapKnight.Core.Graphics;
using mapKnight.Extended.Components.Attributes;
using mapKnight.Extended.Graphics;

namespace mapKnight.Extended.Components.Graphics {

    [ComponentRequirement(typeof(DrawComponent))]
    [UpdateBefore(ComponentEnum.Draw)]
    public class SpriteComponent : Component {
        private SpriteAnimation[ ] sprites;
        private string[ ] result;

        public SpriteComponent (Entity owner, SpriteAnimation[ ] sprites, string texture) : base(owner) {
            this.sprites = sprites;
            result = new string[sprites.Length];
            Owner.World.Renderer.AddTexture(Owner.Species, Assets.Load<SpriteBatch>(texture));
        }

        public override void Update (DeltaTime dt) {
            for (int i = 0; i < sprites.Length; i++) {
                sprites[i].Update(dt.Milliseconds);
                result[i] = sprites[i].Current;
            }

            Owner.SetComponentInfo(ComponentData.Texture, result);
        }

        public new class Configuration : Component.Configuration {
            private SpriteAnimation[ ] internalSortedAnimations;
            public Dictionary<string, SpriteAnimation> Sprites {
                set {
                    List<string> bones = new List<string>(value.Keys);
                    bones.Sort( );
                    internalSortedAnimations = bones.Select(item => value[item]).ToArray( );
                }
            }

            public string Texture;

            public override Component Create (Entity owner) {
                return new SpriteComponent(owner, internalSortedAnimations, Texture);
            }
        }
    }
}