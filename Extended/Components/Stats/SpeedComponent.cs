using System;
using System.Collections.Generic;
using System.Text;
using mapKnight.Core;
using mapKnight.Extended.Components.Attributes;

namespace mapKnight.Extended.Components.Stats {

    [ComponentRequirement(typeof(MotionComponent))]
    [UpdateBefore(ComponentEnum.Motion)]
    public class SpeedComponent : Component {
        public Vector2 Speed;
        private Vector2 defaultSpeed;

        public SpeedComponent (Entity owner, Vector2 defaultspeed) : base(owner) {
            defaultSpeed = defaultspeed;
            Speed = defaultspeed;
        }

        public override void Update (DeltaTime dt) {
            if (Owner.HasComponentInfo(ComponentEnum.Stats_Speed)) {
                Vector2 slowDown = (Vector2)Owner.GetComponentInfo(ComponentEnum.Stats_Speed);
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