using System;
using mapKnight.Core;

namespace mapKnight.Extended.Components {
    [ComponentRequirement(typeof(MotionComponent))]
    [ComponentOrder(ComponentEnum.Motion)]
    public class PushComponent : Component {
        private int intervall;
        private Vector2 velocity;
        private int lastPush;
        private bool resetLastVelocity;
        private MotionComponent motionComponent;

        public PushComponent (Entity owner, int intervall, Vector2 velocity, bool resetlastvelocity) : base(owner) {
            this.intervall = intervall; // ms
            this.velocity = velocity;
            this.resetLastVelocity = resetlastvelocity;
        }

        public override void Prepare ( ) {
            this.lastPush = Environment.TickCount;
            this.motionComponent = Owner.GetComponent<MotionComponent>( );
        }

        public override void Update (DeltaTime dt) {
            if (Environment.TickCount > lastPush + intervall) {
                lastPush += intervall;
                if (resetLastVelocity) {
                    Owner.SetComponentInfo(ComponentEnum.Motion, ComponentEnum.Push, ComponentData.Velocity, -motionComponent.Velocity + velocity);
                } else {
                    Owner.SetComponentInfo(ComponentEnum.Motion, ComponentEnum.Push, ComponentData.Velocity, velocity);
                }
            }
        }

        public new class Configuration : Component.Configuration {
            public int Intervall;
            public Vector2 Velocity;
            public bool ResetVelocity;

            public override Component Create (Entity owner) {
                return new PushComponent(owner, Intervall, Velocity, ResetVelocity);
            }
        }
    }
}