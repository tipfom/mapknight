using mapKnight.Android.CGL;
using System.Collections.Generic;
using System.Linq;

namespace mapKnight.Android.Entity.Components {
    public class TextureComponent : Component {
        private string textureName;
        private Dictionary<string, string> textures;

        public TextureComponent (Entity owner, string texturename) : base (owner) {
            CGLSprite2D texture = Assets.Load<CGLSprite2D> (new string[] { texturename + ".png", texturename + ".json" });
            this.textureName = texture.Name;
            this.textures = texture.Sprites.Keys.ToDictionary (spritename => spritename);

            Owner.Owner.Renderer.AddTexture (Owner.ID, texture);
        }

        public override void Update (float dt) {
            Owner.SetComponentInfo (ComponentType.Draw, ComponentType.Texture, ComponentAction.TextureData, textures);
        }
    }
}