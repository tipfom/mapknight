using System;
using mapKnight.Core;
using mapKnight.Extended.Components.AI.Basics;

namespace mapKnight.Extended.Components.AI.Guardian {
    public class PrivateComponent : BishopComponent {
        private enum State {
            Attacking,
            Defending,
            Walking
        }

        private TentComponent tent;
        private float patrolDistanceSqr;
        private State state = State.Walking;

        public PrivateComponent (Entity owner, TentComponent tent) : base(owner, true) {
            owner.Domain = EntityDomain.Enemy;

            this.tent = tent;
            this.patrolDistanceSqr = tent.PatrolRangeSqr;
        }

        public override void Collision (Entity collidingEntity) {
            if (collidingEntity.Domain == EntityDomain.Player) {
                state = State.Defending;
                Owner.SetComponentInfo(ComponentData.SpriteAnimation, "def", true);
                motionComponent.AimedVelocity.X = 0f;
            }
        }

        public override void Update (DeltaTime dt) {
            switch (state) {
                case State.Walking:
                    if ((Owner.Transform.Center - tent.Owner.Transform.Center).MagnitudeSqr( ) > patrolDistanceSqr) {
                        Turn( );
                    }
                    base.Update(dt);
                    break;
                case State.Defending:
                    state = State.Walking;
                    Owner.SetComponentInfo(ComponentData.SpriteAnimation, "walk", true);
                    break;
            }
        }

        public override void Destroy ( ) {
            tent.PrivateDied(Owner);
            base.Destroy( );
        }

        public new class Configuration : Component.Configuration {
            public TentComponent Tent;

            public override Component Create (Entity owner) {
                return new PrivateComponent(owner, Tent);
            }
        }
    }
}
