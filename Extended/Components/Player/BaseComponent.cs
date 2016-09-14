﻿using System;
using System.Collections.Generic;
using System.Text;
using mapKnight.Core;
using mapKnight.Extended.Components.Attributes;
using mapKnight.Extended.Components.Movement;
using mapKnight.Extended.Components.Stats;
using mapKnight.Extended.Screens;

namespace mapKnight.Extended.Components.Player {

    [UpdateAfter(typeof(SpeedComponent))]
    [UpdateBefore(typeof(MotionComponent))]
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
            while (Owner.HasComponentInfo(ComponentData.InputInclude))
                Action |= (ActionMask)Owner.GetComponentInfo(ComponentData.InputInclude)[0];
            while (Owner.HasComponentInfo(ComponentData.InputExclude))
                Action &= ~(ActionMask)Owner.GetComponentInfo(ComponentData.InputExclude)[0];

            Vector2 speed = speedComponent.Speed;
            if (Action.HasFlag(ActionMask.Jump) && motionComponent.IsOnGround) {
                motionComponent.AimedVelocity.Y = speed.Y;
                Action &= ~ActionMask.Jump;
            }

            if (Action.HasFlag(ActionMask.Left)) {
                motionComponent.AimedVelocity.X = -speed.X;
            } else if (Action.HasFlag(ActionMask.Right)) {
                motionComponent.AimedVelocity.X = speed.X;
            } else {
                motionComponent.AimedVelocity.X = 0;
            }
        }

        public new class Configuration : Component.Configuration {
            public override Component Create (Entity owner) {
                return new BaseComponent(owner);
            }
        }
    }
}