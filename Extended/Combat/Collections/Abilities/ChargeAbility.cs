using mapKnight.Core;
using mapKnight.Extended.Components.Movement;

namespace mapKnight.Extended.Combat.Collections.Abilities {
    public class ChargeAbility : Ability {
        const float CHARGE_DURATION = 125;
        const float CHARGE_SPEED = 20f;

        private bool charging = false;
        private float chargeLeft;
        private MotionComponent motionComponent;

        public ChargeAbility (SecondaryWeapon Weapon) : base(Weapon, "Charge", 400, "abil_1") {
        }

        public override void Prepare ( ) {
            motionComponent = Weapon.Owner.GetComponent<MotionComponent>( );
        }

        public override void OnCast ( ) {
            charging = true;
            Weapon.Lock = true;
            chargeLeft = CHARGE_DURATION;
            motionComponent.AimedVelocity.X = CHARGE_SPEED * motionComponent.ScaleX;
        }

        public override void Update (DeltaTime dt) {
            if (charging) {
                if (!motionComponent.IsOnGround)
                    motionComponent.AimedVelocity.Y -= motionComponent.Velocity.Y;
                else
                    motionComponent.AimedVelocity.Y = 0;

                Availability = 0f;
                chargeLeft -= dt.TotalMilliseconds;
                if (chargeLeft < 0f || motionComponent.IsAtWall) {
                    charging = false;
                    Weapon.Lock = false;
                    motionComponent.AimedVelocity.X = 0;
                    Availability = 0f;
                }
            }
            base.Update(dt);
        }
    }
}
