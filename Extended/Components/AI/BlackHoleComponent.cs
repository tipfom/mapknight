using mapKnight.Core.World;

namespace mapKnight.Extended.Components.AI {
    public class BlackHoleComponent : Component {
        public BlackHoleComponent (Entity owner) : base(owner) {
        }

        public new class Configuration : Component.Configuration {
            public override Component Create (Entity owner) {
                return new BlackHoleComponent(owner);
            }
        }
    }
}
