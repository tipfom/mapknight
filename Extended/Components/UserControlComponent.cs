using System;
using System.Collections.Generic;
using System.Text;
using mapKnight.Core;
using mapKnight.Extended.Components.Communication;

namespace mapKnight.Extended.Components {
    public class UserControlComponent : Component {
        private IInputProvider inputProvider;

        public UserControlComponent (Entity owner, IInputProvider inputprovider) : base(owner) {
            inputProvider = inputprovider;
        }

        public override void Update (float dt) {
            Vector2 speed = (Vector2)Owner.GetComponentState(Identifier.Speed);
            if (inputProvider.Jump && ((MotionComponent)Owner.GetComponent(Identifier.Motion)).IsOnGround) {
                Owner.SetComponentInfo(Identifier.Motion, Identifier.UserControl, Data.Velocity, new Vector2(0, speed.Y));
            }
            if (Owner.GetComponentState(Identifier.Motion) == null)
                return;

            Vector2 current = (Vector2)Owner.GetComponentState(Identifier.Motion);
            if (inputProvider.Left) {
                Owner.SetComponentInfo(Identifier.Motion, Identifier.UserControl, Data.Velocity, new Vector2(-speed.X - current.X, 0));
            } else if (inputProvider.Right) {
                Owner.SetComponentInfo(Identifier.Motion, Identifier.UserControl, Data.Velocity, new Vector2(speed.X - current.X, 0));
            } else {
                Owner.SetComponentInfo(Identifier.Motion, Identifier.UserControl, Data.Velocity, new Vector2(-current.X, 0));
            }
        }

        public interface IInputProvider {
            bool Jump { get; }
            bool Left { get; }
            bool Right { get; }
        }
    }
}
