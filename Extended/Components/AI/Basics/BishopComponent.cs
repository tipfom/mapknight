using mapKnight.Core;
using mapKnight.Core.World;
using mapKnight.Core.World.Components;
using mapKnight.Extended.Components.Movement;
using mapKnight.Extended.Components.Stats;

namespace mapKnight.Extended.Components.AI.Basics {
    [UpdateBefore(typeof(MotionComponent))]
    [UpdateAfter(typeof(SpeedComponent))]
    public class BishopComponent : Component {
        public readonly bool IsScaredToFall;
        protected MotionComponent motionComponent;
        private SpeedComponent speedComponent;
        private float speedMult = 1f;

        public BishopComponent (Entity owner, bool scaredtofall) : base(owner) {
            IsScaredToFall = scaredtofall;
        }

        public override void Collision (Entity collidingEntity) {
            if (collidingEntity.Domain != EntityDomain.Temporary) {
                if (speedMult == 1 && Owner.Transform.Center.X < collidingEntity.Transform.BL.X) // walking right
                    speedMult = -1;
                else if (speedMult == -1 && Owner.Transform.Center.X > collidingEntity.Transform.TR.X)
                    speedMult = 1;
            }
        }

        public override void Prepare ( ) {
            motionComponent = Owner.GetComponent<MotionComponent>( );
            speedComponent = Owner.GetComponent<SpeedComponent>( );
        }

        public override void Update (DeltaTime dt) {
            if (motionComponent.IsAtWall) {
                speedMult *= -1;
            } else if (IsScaredToFall && Owner.Transform.BL.Y >= 1) {
                if (speedMult == 1) {
                    // moves right
                    if (!Owner.World.HasCollider(Mathi.Floor(Owner.Transform.TR.X), Mathi.Floor(Owner.Transform.BL.Y) - 1))
                        speedMult *= -1;
                } else {
                    // moves left
                    if (!Owner.World.HasCollider(Mathi.Floor(Owner.Transform.BL.X), Mathi.Floor(Owner.Transform.BL.Y) - 1))
                        speedMult *= -1;
                }
            }
            motionComponent.AimedVelocity.X = speedComponent.Speed.X * speedMult;
        }

        protected void Turn ( ) {
            speedMult *= -1;
        }

        protected void Walk(bool right) {
            speedMult = right ? 1f : -1f;
        }

        public new class Configuration : Component.Configuration {
            public bool IsScaredToFall;

            public override Component Create (Entity owner) {
                return new BishopComponent(owner, IsScaredToFall);
            }
        }
    }
}