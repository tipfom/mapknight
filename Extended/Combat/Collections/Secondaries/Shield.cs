using System;
using mapKnight.Core;
using mapKnight.Core.World;
using mapKnight.Extended.Graphics.UI;
using mapKnight.Extended.Components.Movement;

namespace mapKnight.Extended.Combat.Collections.Secondaries {
    public class Shield : SecondaryWeapon {
        const int CHARGE_DURATION = 125;
        const float CHARGE_SPEED = 20f;

        private enum Action {
            None,
            Charge,
        }

        private Action currentAction;
        private MotionComponent motionComponent;
        private int chargeFinish;

        public Shield (Entity Owner) : base(Owner, "gestures") {
        }

        public override void Prepare ( ) {
            motionComponent = Owner.GetComponent<MotionComponent>( );
        }

        public override void OnGesture (string gesture) {
            switch (gesture) {
                case UIGesturePanel.SWIPE_LEFT:
                    Charge(-1);
                    break;
                case UIGesturePanel.SWIPE_RIGHT:
                    Charge(1);
                    break;
                case UIGesturePanel.SWIPE_DOWN:
                    break;
            }
        }

        private void Charge (int direction) {
            Lock = true;
            motionComponent.AimedVelocity.X = CHARGE_SPEED * direction;
            chargeFinish = Environment.TickCount + CHARGE_DURATION;
            currentAction = Action.Charge;
        }

        public override void Update (DeltaTime dt) {
            if (currentAction == Action.Charge) {
                motionComponent.AimedVelocity.Y -= motionComponent.Velocity.Y;
                if (chargeFinish < Environment.TickCount) {
                    Lock = false;
                    motionComponent.AimedVelocity.X = 0;
                    currentAction = Action.None;
                }
            }
        }
    }
}
