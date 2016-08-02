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
        private SpeedComponent speedComponent;
        private MotionComponent motionComponent;

        public UserControlComponent (Entity owner, IInputProvider inputprovider) : base(owner) {
            inputProvider = inputprovider;
        }

        public override void Prepare ( ) {
            speedComponent = Owner.GetComponent<SpeedComponent>( );
            motionComponent = Owner.GetComponent<MotionComponent>( );
        }

        public override void Update (TimeSpan dt) {
            Vector2 speed = speedComponent.Speed;
            if (inputProvider.Jump && motionComponent.IsOnGround) {
                Owner.SetComponentInfo(ComponentEnum.Motion, ComponentEnum.UserControl, ComponentData.Velocity, new Vector2(0, speed.Y));
            }

            if (inputProvider.Left) {
                Owner.SetComponentInfo(ComponentEnum.Motion, ComponentEnum.UserControl, ComponentData.Velocity, new Vector2(-speed.X - motionComponent.Velocity.X, 0));
            } else if (inputProvider.Right) {
                Owner.SetComponentInfo(ComponentEnum.Motion, ComponentEnum.UserControl, ComponentData.Velocity, new Vector2(speed.X - motionComponent.Velocity.X, 0));
            } else {
                Owner.SetComponentInfo(ComponentEnum.Motion, ComponentEnum.UserControl, ComponentData.Velocity, new Vector2(-motionComponent.Velocity.X, 0));
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
