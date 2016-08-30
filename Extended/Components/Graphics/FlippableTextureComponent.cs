using System;
using System.Collections.Generic;
using System.Linq;
using mapKnight.Core;
using mapKnight.Extended.Components.Attributes;
using mapKnight.Extended.Graphics;

namespace mapKnight.Extended.Components.Graphics {

    [ComponentRequirement(typeof(DrawComponent))]
    [UpdateBefore(ComponentEnum.Draw)]
    public class FlippableTextureComponent : Component {
        public bool Rotated = false;
        private Dictionary<string, string> normalTextures = new Dictionary<string, string>( );
        private Dictionary<string, string> rotatedTextures = new Dictionary<string, string>( );

        public FlippableTextureComponent (Entity owner, string texturename) : base(owner) {
            SpriteBatch texture = Assets.Load<SpriteBatch>(texturename);
            string[ ] texture_keys = texture.Sprites.Keys.ToArray( );
            for (int i = 0; i < texture_keys.Length; i++) {
                texture.Sprites.Add(texture_keys[i] + "*", FlipTexture(texture.Get(texture_keys[i])));
                normalTextures.Add(texture_keys[i], texture_keys[i]);
                rotatedTextures.Add(texture_keys[i], texture_keys[i] + "*");
            }

            Owner.World.Renderer.AddTexture(Owner.Species, texture);
        }

        public override void Prepare ( ) {
        }

        public override void Update (DeltaTime dt) {
            if (Rotated) Owner.SetComponentInfo(ComponentData.Texture, normalTextures);
            else Owner.SetComponentInfo(ComponentData.Texture, rotatedTextures);
        }

        private float[ ] FlipTexture (float[ ] coords) {
            return new float[ ] {
                coords[6], coords[7],
                coords[4], coords[5],
                coords[2], coords[3],
                coords[0], coords[1]
            };
        }

        public new class Configuration : Component.Configuration {
            public string Texture;

            public override Component Create (Entity owner) {
                return new FlippableTextureComponent(owner, Texture);
            }
        }
    }
}