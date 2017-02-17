using System;
using System.Collections.Generic;
using System.Linq;
using mapKnight.Core;
using mapKnight.Extended.Components.Attributes;
using mapKnight.Extended.Graphics;

namespace mapKnight.Extended.Components.Graphics {
    [UpdateBefore(typeof(DrawComponent))]
    public class TextureComponent : Component {
        private string[ ] textures;

        public TextureComponent(Entity owner, string texturename, string[ ] sprites) : base(owner) {
            SpriteBatch texture = Assets.Load<SpriteBatch>(texturename);

            if (sprites == null) {
                List<string> sortedKeys = texture.Sprites.Keys.ToList( );
                sortedKeys.Sort( );
                textures = sortedKeys.ToArray( );
            } else {
                textures = sprites;
            }

            if (!Owner.World.Renderer.HasTexture(Owner.Species))
                Owner.World.Renderer.AddTexture(Owner.Species, texture);
        }

        public override void PostUpdate( ) {
            if (!Owner.IsOnScreen)
                return;

            Owner.SetComponentInfo(ComponentData.Texture, textures);
        }

        public new class Configuration : Component.Configuration {
            public string Texture;
            public string[ ] Sprites;

            public override Component Create(Entity owner) {
                return new TextureComponent(owner, Texture, Sprites);
            }
        }
    }
}