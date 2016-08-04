using System;
using System.Collections.Generic;
using System.Text;
using mapKnight.Core;

namespace mapKnight.Extended.Components.AI {
    [ComponentRequirement(typeof(MotionComponent))]
    [ComponentRequirement(typeof(SpeedComponent))]
    [ComponentOrder(ComponentEnum.Motion)]
    public class _1Component : Component {
        private MotionComponent motionComponent;
        private SpeedComponent speed;
        private int speedMult = 1;

        public readonly bool IsScaredToFall;

        public _1Component (Entity owner, bool scaredtofall) : base(owner) {
            IsScaredToFall = scaredtofall;
        }

        public override void Prepare ( ) {
            motionComponent = Owner.GetComponent<MotionComponent>( );
            speed = Owner.GetComponent<SpeedComponent>( );
        }

        public override void Update (TimeSpan dt) {
            if (motionComponent.IsAtWall) {
                speedMult *= -1;
            } else if (IsScaredToFall && Owner.Transform.BL.Y >= 1) {
                if (speedMult == 1) {
                    // moves right 
                    if (!Owner.Owner.HasCollider(Mathi.Floor(Owner.Transform.TR.X), Mathi.Floor(Owner.Transform.BL.Y) - 1))
                        speedMult *= -1;

                } else {
                    // moves left
                    if (!Owner.Owner.HasCollider(Mathi.Floor(Owner.Transform.BL.X), Mathi.Floor(Owner.Transform.BL.Y) - 1))
                        speedMult *= -1;
                }
            }
            motionComponent.Velocity.X = speed.Speed.X * speedMult;
        }

        public override void Collision (Entity collidingEntity) {
            if (collidingEntity.IsPlayer) {
                if (collidingEntity.Transform.BL.Y > Owner.Transform.Center.Y) {
                    Owner.Destroy( );
                } else {
                    // let player take damage
                }
            } else {
                speedMult *= -1;
            }
        }

        public new class Configuration : Component.Configuration {
            public bool ScaredToFall;

            public override Component Create (Entity owner) {
                return new _1Component(owner, ScaredToFall);
            }
        }
    }
}
