using mapKnight.Core.World;

namespace mapKnight.Extended.Components.Stats {
    public class ArmorComponent : Component {
        public float PhysicalMultiplier;

        public ArmorComponent (Entity owner, int physicalArmorValue) : base(owner) {
            PhysicalMultiplier = ApplyScaleFunction(physicalArmorValue);
        }

        float ApplyScaleFunction (int value) {
            return 1f / (0.5f * value + 1);
        }

        public new class Configuration : Component.Configuration {
            public int Physical;

            public override Component Create (Entity owner) {
                return new ArmorComponent(owner, Physical);
            }
        }
    }
}
