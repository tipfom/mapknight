using System;
using System.Collections.Generic;
using System.Linq;
using mapKnight.Extended.Components.Communication;
using mapKnight.Extended.Graphics;

namespace mapKnight.Extended.Components {
    public class TextureComponent : Component {
        private string textureName;
        private Dictionary<string, string> textures;

        public TextureComponent (Entity owner, string texturename) : base(owner) {
            SpriteBatch texture = Assets.Load<SpriteBatch>(texturename);
            this.textureName = texture.Name;
            this.textures = texture.Sprites.Keys.ToDictionary(spritename => spritename);

            Owner.Owner.Renderer.AddTexture(Owner.Species, texture);
        }

        public override void Update (TimeSpan dt) {
            Owner.SetComponentInfo(Identifier.Draw, Identifier.Texture, Data.Texture, textures);
        }
    }
}