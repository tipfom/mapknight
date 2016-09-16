using System;
using mapKnight.Core;
using mapKnight.Extended.Components.Movement;

namespace mapKnight.Extended.Components.Player {

    public class KneightComponent : BaseComponent {
        private float dashDirection;
        private int dashStartTime;
        private int dashEndTime;
        private bool isDashing;
        private MotionComponent motionComponent;
        private static ActionMask dashMasks = ActionMask.DashLeft | ActionMask.DashRight;

        private DashData dashData;

        public KneightComponent (Entity owner, DashData dashdata) : base(owner) {
            dashData = dashdata;
        }

        public override void Prepare ( ) {
            motionComponent = Owner.GetComponent<MotionComponent>( );
            base.Prepare( );
        }

        public override void Update (DeltaTime dt) {
            if((Action & dashMasks) > 0) {
                if (!motionComponent.IsOnGround) {
                    isDashing = true;
                    dashStartTime = Environment.TickCount;
                    dashEndTime = dashStartTime + dashData.Time;
                    dashDirection = ((Action & ActionMask.DashLeft) == ActionMask.DashLeft) ? -1f : 1f;
                }
                Action &= ~dashMasks;
            }

            if (isDashing) {
                if (Environment.TickCount > dashEndTime || motionComponent.IsAtWall) {
                    isDashing = false;
                    motionComponent.AimedVelocity.X = 0;
                } else {
                    motionComponent.AimedVelocity.X = dashDirection * dashData.Velocity;
                    motionComponent.AimedVelocity.Y = motionComponent.AimedVelocity.Y  - motionComponent.TotalVelocity.Y;
                }
                Debug.Print(this, motionComponent.AimedVelocity.X);
            } else {
                // accept no input when the player is dashing
                base.Update(dt);
            }
        }

        public struct DashData {
            public int Time;
            public float Velocity;

            public DashData (int time, float dist) {
                // s = 0.5f * a * t * t + v0 * t
                Velocity = dist / time * 1000f;
                Time = time;
            }
        }

        public new class Configuration : Component.Configuration {
            public float DashDistance;
            public int DashTime;

            private DashData data = default(DashData);

            public override Component Create (Entity owner) {
                if (data.Equals(default(DashData))) data = new DashData(DashTime, DashDistance);

                return new KneightComponent(owner, data);
            }
        }
    }
}