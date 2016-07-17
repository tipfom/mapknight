using System;
using System.Collections.Generic;
using System.Text;
using mapKnight.Core;


namespace mapKnight.Extended.Components {
    [ComponentRequirement(typeof(SpeedComponent))]
    [ComponentOrder(ComponentEnum.Speed, false)]
    [ComponentOrder(ComponentEnum.Motion)]
    public class UserControlComponent : Component {
        private IInputProvider inputProvider;

        public UserControlComponent (Entity owner, IInputProvider inputprovider) : base(owner) {
            inputProvider = inputprovider;
        }

        public override void Update (TimeSpan dt) {
            Vector2 speed = (Vector2)Owner.GetComponentState(ComponentEnum.Speed);
            if (inputProvider.Jump && ((MotionComponent)Owner.GetComponent(ComponentEnum.Motion)).IsOnGround) {
                Owner.SetComponentInfo(ComponentEnum.Motion, ComponentEnum.UserControl, ComponentData.Velocity, new Vector2(0, speed.Y));
            }
            if (Owner.GetComponentState(ComponentEnum.Motion) == null)
                return;

            Vector2 current = (Vector2)Owner.GetComponentState(ComponentEnum.Motion);
            if (inputProvider.Left) {
                Owner.SetComponentInfo(ComponentEnum.Motion, ComponentEnum.UserControl, ComponentData.Velocity, new Vector2(-speed.X - current.X, 0));
            } else if (inputProvider.Right) {
                Owner.SetComponentInfo(ComponentEnum.Motion, ComponentEnum.UserControl, ComponentData.Velocity, new Vector2(speed.X - current.X, 0));
            } else {
                Owner.SetComponentInfo(ComponentEnum.Motion, ComponentEnum.UserControl, ComponentData.Velocity, new Vector2(-current.X, 0));
            }
        }

        public interface IInputProvider {
            bool Jump { get; }
            bool Left { get; }
            bool Right { get; }
        }

        public new class Configuration : Component.Configuration {
            private UserControlComponent.IInputProvider inputProvider;

            public Configuration (UserControlComponent.IInputProvider inputprovider) {
                inputProvider = inputprovider;
            }

            public override Component Create (Entity owner) {
                return new UserControlComponent(owner, inputProvider);
            }
        }
    }
}
