using mapKnight.Core;
using mapKnight.Extended.Components.Attributes;
using mapKnight.Extended.Components.Graphics;
using mapKnight.Extended.Components.Movement;
using System;

namespace mapKnight.Extended.Components.AI {

    [ComponentRequirement(typeof(MotionComponent))]
    public class MoonballComponent : Component {
        private float boostVelocity;
        private MotionComponent motionComponent;

        public MoonballComponent (Entity owner, float boostVelocity) : base(owner) {
            this.boostVelocity = boostVelocity;
        }

        public override void Prepare ( ) {
            motionComponent = Owner.GetComponent<MotionComponent>( );
            motionComponent.AimedVelocity.X = -boostVelocity;
        }

        public override void Update (DeltaTime dt) {
            if (motionComponent.IsAtWall)
                Owner.Destroy( );
        }

        public new class Configuration : Component.Configuration {
            public float BoostVelocity;

            public override Component Create (Entity owner) {
                return new MoonballComponent(owner, BoostVelocity);
            }
        }
    }
}
