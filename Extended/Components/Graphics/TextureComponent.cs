using System;
using System.Collections.Generic;
using System.Linq;
using mapKnight.Core;
using mapKnight.Extended.Components.Attributes;
using mapKnight.Extended.Graphics;

namespace mapKnight.Extended.Components.Graphics {

    [ComponentRequirement(typeof(DrawComponent))]
    [UpdateBefore(ComponentEnum.Draw)]
    public class TextureComponent : Component {
        private string[] textures;

        public TextureComponent (Entity owner, string texturename) : base(owner) {
            SpriteBatch texture = Assets.Load<SpriteBatch>(texturename);

            List<string> sortedKeys = texture.Sprites.Keys.ToList( );
            sortedKeys.Sort( );
            textures = sortedKeys.ToArray( );

            Owner.World.Renderer.AddTexture(Owner.Species, texture);
        }

        public override void PostUpdate () {
            Owner.SetComponentInfo(ComponentData.Texture, textures);
        }

        public new class Configuration : Component.Configuration {
            public string Texture;

            public override Component Create (Entity owner) {
                return new TextureComponent(owner, Texture);
            }
        }
    }
}