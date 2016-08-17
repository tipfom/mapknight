using System;
using mapKnight.Core;
using mapKnight.Extended.Components.Attributes;

namespace mapKnight.Extended.Components {

    [ComponentRequirement(typeof(MotionComponent))]
    [UpdateBefore(ComponentEnum.Motion)]
    public class PushComponent : Component {
        private int intervall;
        private MotionComponent motionComponent;
        private int nextPush;
        private bool resetLastVelocity;
        private Vector2 velocity;

        public PushComponent (Entity owner, int intervall, Vector2 velocity, bool resetlastvelocity) : base(owner) {
            this.intervall = intervall; // ms
            this.velocity = velocity;
            this.resetLastVelocity = resetlastvelocity;
        }

        public override void Prepare ( ) {
            this.nextPush = Environment.TickCount;
            this.motionComponent = Owner.GetComponent<MotionComponent>( );
        }

        public override void Update (DeltaTime dt) {
            if (Environment.TickCount > nextPush) {
                nextPush += intervall;
                if (resetLastVelocity) {
                    Owner.SetComponentInfo(ComponentEnum.Motion, new Tuple<ComponentData, Vector2>(ComponentData.Velocity, -motionComponent.Velocity + velocity));
                } else {
                    Owner.SetComponentInfo(ComponentEnum.Motion, new Tuple<ComponentData, Vector2>(ComponentData.Velocity, velocity));
                }
            }
        }

        public new class Configuration : Component.Configuration {
            public int Intervall;
            public bool ResetVelocity;
            public Vector2 Velocity;

            public override Component Create (Entity owner) {
                return new PushComponent(owner, Intervall, Velocity, ResetVelocity);
            }
        }
    }
}