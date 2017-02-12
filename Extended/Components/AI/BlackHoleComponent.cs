using mapKnight.Core;
using System;

namespace mapKnight.Extended.Components.AI {
    public class BlackHoleComponent : Component {
        public BlackHoleComponent (Entity owner) : base(owner) {
            Owner.SetComponentInfo(ComponentData.BoneTexture, "blackhole");
            Owner.SetComponentInfo(ComponentData.BoneOffset, "o", new Vector2(12, 12));
            Owner.SetComponentInfo(ComponentData.BoneOffset, "i", new Vector2(12, 12));
        }

        public new class Configuration : Component.Configuration {
            public override Component Create (Entity owner) {
                return new BlackHoleComponent(owner);
            }
        }
    }
}
