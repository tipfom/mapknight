using System;
using mapKnight.Basic;

namespace mapKnight.Android.Entity.Components {
    public class PushComponent : Component {
        private int intervall;
        private Vector2 velocity;
        private int lastPush;
        private bool resetLastVelocity;

        public PushComponent (Entity owner, float intervall, Vector2 velocity, bool resetlastvelocity) : base (owner) {
            this.intervall = (int)(intervall * 1000); // to ms
            this.velocity = velocity;
            this.lastPush = Environment.TickCount;
            this.resetLastVelocity = resetlastvelocity;
        }

        public override void Update (float dt) {
            if (lastPush + intervall < Environment.TickCount) {
                lastPush += intervall;
                if (resetLastVelocity) {
                    Owner.SetComponentInfo (ComponentType.Motion, ComponentType.Push, ComponentAction.Velocity, -(Vector2)Owner.GetComponentState (ComponentType.Motion) + velocity);
                } else {
                    Owner.SetComponentInfo (ComponentType.Motion, ComponentType.Push, ComponentAction.Velocity, velocity);
                }
            }
        }
    }
}