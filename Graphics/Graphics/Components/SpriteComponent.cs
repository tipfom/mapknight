using mapKnight.Extended.Components.Communication;
using mapKnight.Extended.Graphics;
using System.Collections.Generic;
using System.Linq;

namespace mapKnight.Extended.Components {
    public class SpriteComponent : Component {
        private Dictionary<string, SpriteAnimation> sprites;

        public SpriteComponent (Entity owner, Dictionary<string, SpriteAnimation> sprites, string texture) : base (owner) {
            this.sprites = sprites;
            Owner.Owner.Renderer.AddTexture (Owner.ID, Assets.Load<SpriteBatch> (texture));
        }

        public override void Update (float dt) {
            foreach (string bone in sprites.Keys) {
                sprites[bone].Update (dt);
            }

            Owner.SetComponentInfo (Identifier.Draw, Identifier.Sprite, Data.Texture, sprites.ToDictionary (v => v.Key, v => v.Value.Current));
        }
    }
}