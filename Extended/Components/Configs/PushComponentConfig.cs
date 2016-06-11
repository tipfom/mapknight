using mapKnight.Core;

namespace mapKnight.Extended.Components.Configs {
    public class PushComponentConfig : ComponentConfig {
        public override int Priority { get { return 2; } }

        public float Intervall;
        public Vector2 Velocity;
        public bool ResetVelocity;

        public override Component Create (Entity owner) {
            return new PushComponent (owner, Intervall, Velocity, ResetVelocity);
        }
    }
}