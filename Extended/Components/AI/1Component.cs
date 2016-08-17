using System;
using System.Collections.Generic;
using System.Text;
using mapKnight.Core;
using mapKnight.Extended.Components.Attributes;
using mapKnight.Extended.Components.Stats;

namespace mapKnight.Extended.Components.AI {

    [ComponentRequirement(typeof(MotionComponent))]
    [ComponentRequirement(typeof(SpeedComponent))]
    [UpdateBefore(ComponentEnum.Motion)]
    public class _1Component : Component {
        public readonly bool IsScaredToFall;
        private DamageComponent damageComponent;
        private MotionComponent motionComponent;
        private SpeedComponent speedComponent;
        private int speedMult = 1;

        public _1Component (Entity owner, bool scaredtofall) : base(owner) {
            IsScaredToFall = scaredtofall;
        }

        public override void Collision (Entity collidingEntity) {
            if (!collidingEntity.Info.IsTemporary) {
                if (speedMult == 1 && Owner.Transform.Center.X < collidingEntity.Transform.BL.X) // walking right
                    speedMult = -1;
                else if (speedMult == -1 && Owner.Transform.Center.X > collidingEntity.Transform.TR.X)
                    speedMult = 1;
            }
        }

        public override void Prepare ( ) {
            motionComponent = Owner.GetComponent<MotionComponent>( );
            speedComponent = Owner.GetComponent<SpeedComponent>( );
            if (Owner.Info.HasDamage) damageComponent = Owner.GetComponent<DamageComponent>( );
        }

        public override void Update (DeltaTime dt) {
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
            motionComponent.Velocity.X = speedComponent.Speed.X * speedMult;
        }

        public new class Configuration : Component.Configuration {
            public bool IsScaredToFall;

            public override Component Create (Entity owner) {
                return new _1Component(owner, IsScaredToFall);
            }
        }
    }
}