using System;
using mapKnight.Basic;

namespace mapKnight.Android.Entity.Components {
    public class MotionComponent : Component {
        const int MAX_DELTA_TIME = 1000; // 1 sec

        public Vector2 Velocity;

        public MotionComponent (Entity owner) : base (owner) {
            Velocity = new Vector2 ( );
        }

        public override void Update (float dt) {
            if (Math.Abs (dt) > MAX_DELTA_TIME)
                return;
            dt /= 1000f;

            Vector2 appliedAcceleration = new Vector2 ( ); // reset acceleration
            Vector2 appliedVelocity = new Vector2 ( );

            while (Owner.HasComponentInfo (Type.Motion)) {
                Info componentInfo = Owner.GetComponentInfo (Type.Motion);
                if (componentInfo.Sender == Type.Collision) {
                    bool[ ] collisionData = (bool[ ])componentInfo.Data;
                    if (collisionData[0] == true) {
                        // collision on x
                        Velocity.X = 0;
                    }
                    if (collisionData[1] == true) {
                        Velocity.Y = 0;
                    }
                    break;
                } else {
                    switch (componentInfo.Action) {
                    case Action.Velocity:
                        appliedVelocity += (Vector2)componentInfo.Data;
                        break;
                    case Action.Acceleration:
                        appliedAcceleration += (Vector2)componentInfo.Data;
                        break;
                    }
                }
            }

            Velocity += appliedAcceleration * dt + appliedVelocity;

            if (Owner.HasComponent (Type.Collision)) {
                // transformation handled by CollisionComponent
                Transform targetTransform = new Transform (Owner.Transform.Center + Velocity * dt, Owner.Transform.Bounds);
                Owner.SetComponentInfo (Type.Collision, Type.Motion, Action.None, targetTransform);
            } else {
                Owner.Transform.Translate (Owner.Transform.Center + Velocity * dt);
            }

            this.State = Velocity;
        }
    }
}