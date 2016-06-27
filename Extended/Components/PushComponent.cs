using mapKnight.Core;
using mapKnight.Extended.Components.Communication;
using System;

namespace mapKnight.Extended.Components {
    public class PushComponent : Component {
        private int intervall;
        private Vector2 velocity;
        private int lastPush;
        private bool resetLastVelocity;

        public PushComponent (Entity owner, float intervall, Vector2 velocity, bool resetlastvelocity) : base(owner) {
            this.intervall = (int)(intervall * 1000); // to ms
            this.velocity = velocity;
            this.resetLastVelocity = resetlastvelocity;
        }

        public override void Prepare ( ) {
            this.lastPush = Environment.TickCount;
        }

        public override void Update (TimeSpan dt) {
            if (lastPush + intervall < Environment.TickCount) {
                lastPush += intervall;
                if (resetLastVelocity) {
                    Owner.SetComponentInfo(Identifier.Motion, Identifier.Push, Data.Velocity, -(Vector2)Owner.GetComponentState(Identifier.Motion) + velocity);
                } else {
                    Owner.SetComponentInfo(Identifier.Motion, Identifier.Push, Data.Velocity, velocity);
                }
            }
        }
    }
}