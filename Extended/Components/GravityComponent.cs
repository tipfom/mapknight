using System;

namespace mapKnight.Extended.Components {
    [ComponentRequirement(typeof(MotionComponent))]
    [ComponentOrder(ComponentEnum.Motion)]
    public class GravityComponent : Component {
        private float gravityInfluence;

        public GravityComponent (Entity owner, float gravityinfluence) : base(owner) {
            this.gravityInfluence = gravityinfluence;
        }

        public override void Update (TimeSpan dt) {
            // say to the collisioncomponent to use a part of the global gravity when calculating movement
            // that the motioncomponent is contained by the entity needs to be handled by dependencies
            Owner.SetComponentInfo(ComponentEnum.Motion, ComponentEnum.Gravity, ComponentData.Acceleration, Owner.Owner.Gravity * gravityInfluence);
        }

        public new class Configuration : Component.Configuration {
            public float Influence;

            public override Component Create (Entity owner) {
                return new GravityComponent(owner, Influence);
            }
        }
    }
}