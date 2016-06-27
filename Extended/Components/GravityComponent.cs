using System;
using mapKnight.Extended.Components.Communication;

namespace mapKnight.Extended.Components {
    public class GravityComponent : Component {
        private float gravityInfluence;

        public GravityComponent (Entity owner, float gravityinfluence) : base(owner) {
            this.gravityInfluence = gravityinfluence;
        }

        public override void Update (TimeSpan dt) {
            // say to the collisioncomponent to use a part of the global gravity when calculating movement
            // that the motioncomponent is contained by the entity needs to be handled by dependencies
            Owner.SetComponentInfo(Identifier.Motion, Identifier.Gravity, Data.Acceleration, Owner.Owner.Gravity * gravityInfluence);
        }
    }
}