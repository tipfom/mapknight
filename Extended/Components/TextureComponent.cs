using System;
using System.Collections.Generic;
using System.Linq;
using mapKnight.Core;
using mapKnight.Extended.Components.Attributes;
using mapKnight.Extended.Graphics;

namespace mapKnight.Extended.Components {

    [ComponentRequirement(typeof(DrawComponent))]
    [UpdateBefore(ComponentEnum.Draw)]
    public class TextureComponent : Component {
        private Tuple<ComponentData, object> data;
        private string textureName;
        private Dictionary<string, string> textures;

        public TextureComponent (Entity owner, string texturename) : base(owner) {
            SpriteBatch texture = Assets.Load<SpriteBatch>(texturename);
            textureName = texture.Name;
            textures = texture.Sprites.Keys.ToDictionary(spritename => spritename);
            data = new Tuple<ComponentData, object>(ComponentData.Texture, textures);

            Owner.World.Renderer.AddTexture(Owner.Species, texture);
        }

        public override void Update (DeltaTime dt) {
            Owner.SetComponentInfo(ComponentEnum.Draw, data);
        }

        public new class Configuration : Component.Configuration {
            public string Texture;

            public override Component Create (Entity owner) {
                return new TextureComponent(owner, Texture);
            }
        }
    }
}