using System;
using mapKnight.Core;
using mapKnight.Extended.Components.Attributes;

namespace mapKnight.Extended.Components.Movement {

    [ComponentRequirement(typeof(MotionComponent))]
    [UpdateBefore(ComponentEnum.Motion)]
    public class PushComponent : Component {
        private int intervall;
        private MotionComponent motionComponent;
        private int nextPush;
        private Vector2 velocity;

        public PushComponent (Entity owner, int intervall, Vector2 velocity) : base(owner) {
            this.intervall = intervall; // ms
            this.velocity = velocity;
        }

        public override void Prepare ( ) {
            this.nextPush = Environment.TickCount;
            this.motionComponent = Owner.GetComponent<MotionComponent>( );
        }

        public override void Update (DeltaTime dt) {
            if (Environment.TickCount > nextPush) {
                nextPush += intervall;
                motionComponent.AimedVelocity = velocity;
            }
        }

        public new class Configuration : Component.Configuration {
            public int Intervall;
            public Vector2 Velocity;

            public override Component Create (Entity owner) {
                return new PushComponent(owner, Intervall, Velocity);
            }
        }
    }
}