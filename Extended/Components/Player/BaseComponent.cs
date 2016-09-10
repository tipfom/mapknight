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
    public class BaseComponent : Component {
        [Flags]
        public enum ActionMask {
            None = 0,
            Jump = 1 << 1,
            Left = 1 << 2,
            Right = 1 << 3,
        }

        public ActionMask Action;
        private MotionComponent motionComponent;
        private SpeedComponent speedComponent;

        public BaseComponent (Entity owner) : base(owner) {
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
            if (Action.HasFlag(ActionMask.Jump) && motionComponent.IsOnGround) {
                Owner.SetComponentInfo(ComponentData.Velocity, new Vector2(0, speed.Y));
                Action &= ~ActionMask.Jump;
            }

            if (Action.HasFlag(ActionMask.Left)) {
                Owner.SetComponentInfo(ComponentData.Velocity, new Vector2(-speed.X - motionComponent.Velocity.X, 0));
            } else if (Action.HasFlag(ActionMask.Right)) {
                Owner.SetComponentInfo(ComponentData.Velocity, new Vector2(speed.X - motionComponent.Velocity.X, 0));
            } else {
                Owner.SetComponentInfo(ComponentData.Velocity, new Vector2(-motionComponent.Velocity.X, 0));
            }
        }

        public new class Configuration : Component.Configuration {
            public override Component Create (Entity owner) {
                return new BaseComponent(owner);
            }
        }
    }
}