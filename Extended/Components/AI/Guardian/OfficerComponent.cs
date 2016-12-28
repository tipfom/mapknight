using System;
using mapKnight.Core;
using mapKnight.Extended.Components.Attributes;
using mapKnight.Extended.Components.Movement;
using mapKnight.Extended.Components.Stats;

namespace mapKnight.Extended.Components.AI.Guardian {
    [ComponentRequirement(typeof(MotionComponent))]
    [ComponentRequirement(typeof(SpeedComponent))]
    public class OfficerComponent : Component {
        public Entity Target;
        private TentComponent tent;
        private MotionComponent motionComponent;
        private SpeedComponent speedComponent;

        public OfficerComponent (Entity owner, TentComponent tent) : base(owner) {
            this.tent = tent;
        }

        public override void Prepare ( ) {
            motionComponent = Owner.GetComponent<MotionComponent>( );
            speedComponent = Owner.GetComponent<SpeedComponent>( );
        }

        public override void Update (DeltaTime dt) {
            if(Owner.Transform.BL.X >= Target.Transform.TR.X) {
                // walk left
                motionComponent.AimedVelocity.X = -speedComponent.Speed.X;
            } else if (Owner.Transform.TR.X <= Target.Transform.BL.X) {
                // walk right
                motionComponent.AimedVelocity.X = speedComponent.Speed.X;
            } else {
                motionComponent.AimedVelocity.X = 0;
            }
        }

        public void ReturnHome ( ) {
            Target = tent.Owner;
        }

        public override void Destroy ( ) {
            tent.OfficerDied( );
        }

        public new class Configuration : Component.Configuration {
            public TentComponent Tent;

            public override Component Create (Entity owner) {
                return new OfficerComponent(owner, Tent);
            }
        }
    }
}
