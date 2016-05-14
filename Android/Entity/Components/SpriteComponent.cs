using mapKnight.Android.CGL;
using mapKnight.Basic.Components;
using System.Collections.Generic;
using System.Linq;

namespace mapKnight.Android.Entity.Components {
    public class SpriteComponent : Component {
        private Dictionary<string, Sprite> sprites;

        public SpriteComponent (Entity owner, Dictionary<string, Sprite> sprites, string texture) : base (owner) {
            this.sprites = sprites;
            Owner.Owner.Renderer.AddTexture (Owner.ID, Assets.Load<CGLSprite2D> (texture + ".png", texture + ".json"));
        }

        public override void Update (float dt) {
            foreach (string bone in sprites.Keys) {
                sprites[bone].Update (dt);
            }

            Owner.SetComponentInfo (ComponentType.Draw, ComponentType.Sprite, ComponentAction.TextureData, sprites.ToDictionary (v => v.Key, v => v.Value.Current));
        }
    }
}