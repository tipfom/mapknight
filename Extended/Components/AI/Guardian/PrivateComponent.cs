using System;
using mapKnight.Core;
using mapKnight.Extended.Components.AI.Basics;
using mapKnight.Extended.Components.Graphics;

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
                Owner.SetComponentInfo(ComponentData.SpriteAnimation, "def", true, (SpriteComponent.AnimationCallback)DefAnimationCallback);
                Owner.SetComponentInfo(ComponentData.SpriteAnimation, "walk", false);
                motionComponent.AimedVelocity.X = 0f;
            }
        }

        public override void Update (DeltaTime dt) {
            if (state == State.Walking) {
                if ((Owner.Transform.Center - tent.Owner.Transform.Center).MagnitudeSqr( ) > patrolDistanceSqr) {
                    Turn( );
                }
                base.Update(dt);
            }
        }

        public override void Destroy ( ) {
            tent.PrivateDied(Owner);
            base.Destroy( );
        }

        private void DefAnimationCallback (bool success) {
            if (success) {
                state = State.Walking;
            } 
        }

        public new class Configuration : Component.Configuration {
            public TentComponent Tent;

            public override Component Create (Entity owner) {
                return new PrivateComponent(owner, Tent);
            }
        }
    }
}
