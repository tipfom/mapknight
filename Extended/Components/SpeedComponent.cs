using System;
using System.Collections.Generic;
using System.Text;
using mapKnight.Core;

namespace mapKnight.Extended.Components {
    [ComponentRequirement(typeof(MotionComponent))]
    [ComponentOrder(ComponentEnum.Motion)]
    public class SpeedComponent : Component {
        private Vector2 defaultSpeed;

        public SpeedComponent (Entity owner, Vector2 defaultspeed) : base(owner) {
            defaultSpeed = defaultspeed;
        }

        public override void Update (TimeSpan dt) {
            if (Owner.HasComponentInfo(ComponentEnum.Speed)) {
                Vector2 slowDown = (Vector2)Owner.GetComponentInfo(ComponentEnum.Speed).Data;
                this.State = defaultSpeed * slowDown;
            } else {
                this.State = defaultSpeed;
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
