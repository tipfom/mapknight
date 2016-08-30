using System;
using System.Collections.Generic;
using System.Text;
using mapKnight.Core;
using mapKnight.Extended.Components.Attributes;
using mapKnight.Extended.Components.Stats;
using mapKnight.Extended.Screens;

namespace mapKnight.Extended.Components.Movement {

    [ComponentRequirement(typeof(SpeedComponent))]
    [UpdateAfter(ComponentEnum.Stats_Speed)]
    [UpdateBefore(ComponentEnum.Motion)]
    public class PlayerComponent : Component {
        private IInputProvider inputProvider;
        private MotionComponent motionComponent;
        private SpeedComponent speedComponent;

        public PlayerComponent (Entity owner, IInputProvider inputprovider) : base(owner) {
            inputProvider = inputprovider;
        }

        public interface IInputProvider {
            bool Jump { get; }
            bool Left { get; }
            bool Right { get; }
        }

        public override void Destroy ( ) {
            GameOverScreen gameOverScreen = new GameOverScreen( );
            gameOverScreen.Load( );
            Screen.Active = gameOverScreen;
        }

        public override void Prepare ( ) {
            speedComponent = Owner.GetComponent<SpeedComponent>( );
            motionComponent = Owner.GetComponent<MotionComponent>( );
        }

        public override void Update (DeltaTime dt) {
            Vector2 speed = speedComponent.Speed;
            if (inputProvider.Jump && motionComponent.IsOnGround) {
                Owner.SetComponentInfo(ComponentData.Velocity, new Vector2(0, speed.Y));
            }

            if (inputProvider.Left) {
                Owner.SetComponentInfo(ComponentData.Velocity, new Vector2(-speed.X - motionComponent.Velocity.X, 0));
            } else if (inputProvider.Right) {
                Owner.SetComponentInfo(ComponentData.Velocity, new Vector2(speed.X - motionComponent.Velocity.X, 0));
            } else {
                Owner.SetComponentInfo(ComponentData.Velocity, new Vector2(-motionComponent.Velocity.X, 0));
            }
        }

        public new class Configuration : Component.Configuration {
            private PlayerComponent.IInputProvider inputProvider;

            public Configuration (PlayerComponent.IInputProvider inputprovider) {
                inputProvider = inputprovider;
            }

            public override Component Create (Entity owner) {
                return new PlayerComponent(owner, inputProvider);
            }
        }
    }
}