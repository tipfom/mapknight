using System;
using mapKnight.Core;
using mapKnight.Extended.Components.Attributes;

namespace mapKnight.Extended.Components.Movement {

    [ComponentRequirement(typeof(MotionComponent))]
    [UpdateBefore(ComponentEnum.Motion)]
    public class GravityComponent : Component {
        public float Influence;

        public GravityComponent (Entity owner, float gravityinfluence) : base(owner) {
            this.Influence = gravityinfluence;
        }

        public override void Update (DeltaTime dt) {
            // say to the collisioncomponent to use a part of the global gravity when calculating
            // movement that the motioncomponent is contained by the entity needs to be handled by dependencies
            Owner.SetComponentInfo(ComponentEnum.Motion, new Tuple<ComponentData, Vector2>(ComponentData.Acceleration, Owner.World.Gravity * Influence));
        }

        public new class Configuration : Component.Configuration {
            public float Influence;

            public override Component Create (Entity owner) {
                return new GravityComponent(owner, Influence);
            }
        }
    }
}