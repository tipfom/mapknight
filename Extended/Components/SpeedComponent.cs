using System;
using System.Collections.Generic;
using System.Text;
using mapKnight.Core;
using mapKnight.Extended.Components.Communication;

namespace mapKnight.Extended.Components {
    public class SpeedComponent : Component {
        private Vector2 defaultSpeed;

        public SpeedComponent (Entity owner, Vector2 defaultspeed) : base(owner) {
            defaultSpeed = defaultspeed;
        }

        public override void Update (float dt) {
            if (Owner.HasComponentInfo(Identifier.Speed)) {
                Vector2 slowDown = (Vector2)Owner.GetComponentInfo(Identifier.Speed).Data;
                this.State = defaultSpeed * slowDown;
            } else {
                this.State = defaultSpeed;
            }
        }
    }
}
