using mapKnight.Core;
using mapKnight.Core.World;
using mapKnight.Core.World.Components;
using mapKnight.Extended.Components.Movement;

namespace mapKnight.Extended.Components.Stats {
    [UpdateBefore(typeof(MotionComponent), false)]
    public class SpeedComponent : Component {
        public Vector2 Speed;
        private Vector2 defaultSpeed;

        public SpeedComponent (Entity owner, Vector2 defaultspeed) : base(owner) {
            defaultSpeed = defaultspeed;
            Speed = defaultspeed;
        }

        public override void Update (DeltaTime dt) {
            if (Owner.HasComponentInfo(ComponentData.SlowDown)) {
                Vector2 slowDown = (Vector2)Owner.GetComponentInfo(ComponentData.SlowDown)[0];
                Speed = defaultSpeed * slowDown;
            } else {
                Speed = defaultSpeed;
            }
        }

        public new class Configuration : Component.Configuration {
            public float X;
            public float Y;

            public override Component Create (Entity owner) {
                return new SpeedComponent(owner, new Vector2(X, Y));
            }
        }
    }
}