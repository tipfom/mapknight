using mapKnight.Basic;
using System;

namespace mapKnight.Android.Entity.Components {
    public class MotionComponent : Component {
        const int MAX_DELTA_TIME = 1000; // 1 sec

        public Vector2 Velocity;

        public MotionComponent (Entity owner) : base (owner) {
            Velocity = new Vector2 ();
        }

        public override void Update (float dt) {
            if (Math.Abs (dt) > MAX_DELTA_TIME)
                return;
            dt /= 1000f;

            Vector2 appliedAcceleration = new Vector2 (); // reset acceleration
            Vector2 appliedVelocity = new Vector2 ();

            while (Owner.HasComponentInfo (ComponentType.Motion)) {
                ComponentInfo ComponentInfo = Owner.GetComponentInfo (ComponentType.Motion);
                if (ComponentInfo.Sender == ComponentType.Collision) {
                    bool[] collisionData = (bool[])ComponentInfo.Data;
                    if (collisionData[0] == true) {
                        // collision on x
                        Velocity.X = 0;
                    }
                    if (collisionData[1] == true) {
                        Velocity.Y = 0;
                    }
                    break;
                } else {
                    switch (ComponentInfo.Action) {
                        case ComponentAction.Velocity:
                            appliedVelocity += (Vector2)ComponentInfo.Data;
                            break;
                        case ComponentAction.Acceleration:
                            appliedAcceleration += (Vector2)ComponentInfo.Data;
                            break;
                    }
                }
            }

            Velocity += appliedAcceleration * dt + appliedVelocity;

            if (Owner.HasComponent (ComponentType.Collision)) {
                // transformation handled by CollisionComponent
                Transform targetTransform = new Transform (Owner.Transform.Center + Velocity * dt, Owner.Transform.Bounds);
                Owner.SetComponentInfo (ComponentType.Collision, ComponentType.Motion, ComponentAction.None, targetTransform);
            } else {
                Owner.Transform.Translate (Owner.Transform.Center + Velocity * dt);
            }

            this.State = Velocity;
        }
    }
}