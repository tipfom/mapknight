using System;
using System.Collections.Generic;
using System.Text;
using mapKnight.Core;
using mapKnight.Extended.Components.Attributes;
using mapKnight.Extended.Components.Graphics;
using mapKnight.Extended.Components.Movement;
using mapKnight.Extended.Components.Stats;
using mapKnight.Extended.Screens;
using mapKnight.Extended.Warfare;

namespace mapKnight.Extended.Components.Player {

    [UpdateAfter(typeof(SpeedComponent))]
    [UpdateBefore(typeof(MotionComponent))]
    public class PlayerComponent : Component {
        public ActionMask Action;
        public IWeapon Weapon;

        private MotionComponent motionComponent;
        private SpeedComponent speedComponent;
        private AnimationState animationState = AnimationState.None;
        private bool startedJumping = false;

        public PlayerComponent (Entity owner, IWeapon weapon) : base(owner) {
            Weapon = weapon;

            // hardcoded, im sorry god :(
            Owner.SetComponentInfo(ComponentData.BoneTexture, "player");
            Owner.SetComponentInfo(ComponentData.BoneTexture, "Sword(2)");

            Owner.SetComponentInfo(ComponentData.BoneOffset, Tuple.Create("body", new Vector2(9, 5)));
            Owner.SetComponentInfo(ComponentData.BoneOffset, Tuple.Create("feet1", new Vector2(3, 2.5f)));
            Owner.SetComponentInfo(ComponentData.BoneOffset, Tuple.Create("feet2", new Vector2(3, 2.5f)));
            Owner.SetComponentInfo(ComponentData.BoneOffset, Tuple.Create("hand1", new Vector2(2, 2)));
            Owner.SetComponentInfo(ComponentData.BoneOffset, Tuple.Create("hand2", new Vector2(2, 2)));
            Owner.SetComponentInfo(ComponentData.BoneOffset, Tuple.Create("upper_arm1", new Vector2(2, 1.5f)));
            Owner.SetComponentInfo(ComponentData.BoneOffset, Tuple.Create("upper_arm2", new Vector2(2, 1.5f)));
            Owner.SetComponentInfo(ComponentData.BoneOffset, Tuple.Create("head", new Vector2(9, 9)));
            Owner.SetComponentInfo(ComponentData.BoneOffset, Tuple.Create("weapon", new Vector2(3, 25)));
            Owner.SetComponentInfo(ComponentData.BoneOffset, Tuple.Create("sword", new Vector2(3, 25)));
        }

        public override void Destroy ( ) {
            GameOverScreen gameOverScreen = new GameOverScreen( );
            gameOverScreen.Load( );
            Screen.Active = gameOverScreen;
        }

        public override void Prepare ( ) {
            speedComponent = Owner.GetComponent<SpeedComponent>( );
            motionComponent = Owner.GetComponent<MotionComponent>( );
            Weapon.Prepare( );
        }

        public override void Update (DeltaTime dt) {
            while (Owner.HasComponentInfo(ComponentData.InputInclude))
                Action |= (ActionMask)Owner.GetComponentInfo(ComponentData.InputInclude)[0];
            while (Owner.HasComponentInfo(ComponentData.InputExclude))
                Action &= ~(ActionMask)Owner.GetComponentInfo(ComponentData.InputExclude)[0];

            while (Owner.HasComponentInfo(ComponentData.InputGesture)) {
                string data = (string)Owner.GetComponentInfo(ComponentData.InputGesture)[0];
                if (data == string.Empty) {
                    Owner.SetComponentInfo(ComponentData.VertexAnimation, "hit_prepare", true, (AnimationComponent.AnimationCallback)AnimationCallbackContinueAttack);
                } else
                    Weapon.Special(data);
            }

            Vector2 speed = speedComponent.Speed;
            if (Action.HasFlag(ActionMask.Left)) {
                motionComponent.AimedVelocity.X = -speed.X;
            } else if (Action.HasFlag(ActionMask.Right)) {
                motionComponent.AimedVelocity.X = speed.X;
            } else {
                motionComponent.AimedVelocity.X = 0;
            }

            if (animationState != AnimationState.Jump && (motionComponent.IsOnGround || motionComponent.IsOnPlatform)) {
                if (Action.HasFlag(ActionMask.Jump)) {
                    animationState = AnimationState.Jump;
                    Owner.SetComponentInfo(ComponentData.VertexAnimation, "jump", true, (AnimationComponent.AnimationCallback)AnimationCallbackFinishJumping);
                    return;
                } else {
                    if (!startedJumping)
                        motionComponent.AimedVelocity.Y = 0;
                    startedJumping = false;
                }
                if (motionComponent.AimedVelocity.X != 0) {
                    if (animationState != AnimationState.Walk) {
                        Owner.SetComponentInfo(ComponentData.VertexAnimation, "walk", true);
                        animationState = AnimationState.Walk;
                    }
                } else {
                    if (animationState != AnimationState.Idle) {
                        Owner.SetComponentInfo(ComponentData.VertexAnimation, "idle", true);
                        animationState = AnimationState.Idle;
                    }
                }
            }
        }

        private void AnimationCallbackFinishJumping (bool success) {
            if (success) {
                motionComponent.AimedVelocity.Y = speedComponent.Speed.Y;
                Action &= ~ActionMask.Jump;
                animationState = AnimationState.Fall;
                Owner.SetComponentInfo(ComponentData.VertexAnimation, "fall", true);
                startedJumping = true;
            }
        }

        private void AnimationCallbackContinueAttack (bool success) {
            if (success) {
                Weapon.Attack( );
                Owner.SetComponentInfo(ComponentData.VertexAnimation, "hit_finish", true);
            }
        }

        public new class Configuration : Component.Configuration {
            public string Weapon;

            public override Component Create (Entity owner) {
                return new PlayerComponent(owner, (IWeapon)Activator.CreateInstance(Type.GetType("mapKnight.Extended.Warfare." + Weapon), owner));
            }
        }
    }
}