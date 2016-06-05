using mapKnight.Core;
using System.Collections.Generic;

namespace mapKnight.Extended.Components.Configs {
    public class PlatformComponentConfig : ComponentConfig {
        public override int Priority { get { return 0; } }

        public List<Vector2> Waypoints;
        public float Speed;

        public override Component Create (Entity owner) {
            return new PlatformComponent (owner, Waypoints, Speed);
        }
    }
}