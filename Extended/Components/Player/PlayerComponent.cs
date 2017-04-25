﻿using System;
using mapKnight.Core;
using mapKnight.Core.World;
using mapKnight.Core.World.Components;
using mapKnight.Extended.Components.AI;
using mapKnight.Extended.Components.Graphics;
using mapKnight.Extended.Components.Movement;
using mapKnight.Extended.Components.Stats;
using mapKnight.Extended.Graphics.UI;
using mapKnight.Extended.Screens;
using mapKnight.Extended.Warfare;
using mapKnight.Extended.Graphics.Animation;
using static mapKnight.Extended.Components.Player.PlayerAnimationComponent;

namespace mapKnight.Extended.Components.Player {
    [UpdateAfter(typeof(SpeedComponent))]
    [UpdateBefore(typeof(MotionComponent))]
    public class PlayerComponent : Component {
        public ActionMask Action;
        public BaseWeapon BaseWeapon;
        public HealthTracker Health;

        private bool currentlyTalking;
        private Entity nearbyNPC;
        private MotionComponent motionComponent;
        private SpeedComponent speedComponent;
        private AnimationState bodyAnimationState = AnimationState.None;
        private AnimationState weaponAnimationState = AnimationState.None;
        private VertexAnimationData bodyAnimationData; // TEMP, WILL SOON BE PART OF THE ARMOR

        public PlayerComponent (Entity owner, BaseWeapon baseWeapon, float health, VertexAnimationData bodyAnimationData) : base(owner) {
            this.bodyAnimationData = bodyAnimationData;

            owner.Domain = EntityDomain.Player;

            BaseWeapon = baseWeapon;
            Health = new HealthTracker(health);
        }

        public override void Destroy ( ) {
            GameOverScreen gameOverScreen = new GameOverScreen((Extended.Graphics.Map)Owner.World);
            gameOverScreen.Load( );
            Screen.Active = gameOverScreen;
#if DEBUG
            BaseWeapon.Destroy( );
#endif
        }

        public override void Prepare ( ) {
            speedComponent = Owner.GetComponent<SpeedComponent>( );
            motionComponent = Owner.GetComponent<MotionComponent>( );
            BaseWeapon.Prepare( );

            Owner.GetComponent<PlayerAnimationComponent>( ).LoadAnimations(bodyAnimationData, BaseWeapon.AnimationData, "player", BaseWeapon.Texture);
        }

        public override void Collision (Entity collidingEntity) {
            if (collidingEntity.Domain == EntityDomain.NPC) {
                nearbyNPC = collidingEntity;
            }
        }

        public override void Update (DeltaTime dt) {
            bool attemptJump = false;

            while (Owner.HasComponentInfo(ComponentData.InputInclude))
                Action |= (ActionMask)Owner.GetComponentInfo(ComponentData.InputInclude)[0];
            while (Owner.HasComponentInfo(ComponentData.InputExclude))
                Action &= ~(ActionMask)Owner.GetComponentInfo(ComponentData.InputExclude)[0];

            while (Owner.HasComponentInfo(ComponentData.Damage)) {
                float value = (float)Owner.GetComponentInfo(ComponentData.Damage)[0];
                if (nearbyNPC == null) { // npcs are an safezone
                    Health.Value -= value;
                    if (Health.Value < 0) {
                        Owner.Destroy( );
                    }
                }
            }

            if (nearbyNPC == null && weaponAnimationState != AnimationState.Attack && BaseWeapon.Update( )) {
                Owner.SetComponentInfo(ComponentData.VertexAnimation, WEAPON_ANIMATION, "attack", (AnimationCallback)AttackAnimationCallback); // TODO
                weaponAnimationState = AnimationState.Attack;
                BaseWeapon.Attack( );
            }

            while (Owner.HasComponentInfo(ComponentData.InputGesture)) {
                string data = (string)Owner.GetComponentInfo(ComponentData.InputGesture)[0];
                if (data == string.Empty) {
                    if (nearbyNPC == null) {
                        attemptJump = true;
                    } else if (!currentlyTalking) {
                        NPCComponent npccomponent = nearbyNPC.GetComponent<NPCComponent>( );
                        if (npccomponent.Available) {
                            currentlyTalking = true;
                            new UIDialog(Screen.Gameplay, npccomponent).DialogFinished += ( ) => currentlyTalking = false;
                        }
                    }
                } else {
                    // SPECIAL ATTACK, TODO
                }
            }

            if (!currentlyTalking) {
                if (bodyAnimationState == AnimationState.Jump) return;

                Vector2 speed = speedComponent.Speed;
                if (Action.HasFlag(ActionMask.Left)) {
                    motionComponent.AimedVelocity.X = -speed.X;
                } else if (Action.HasFlag(ActionMask.Right)) {
                    motionComponent.AimedVelocity.X = speed.X;
                } else {
                    motionComponent.AimedVelocity.X = 0;
                }

                if (motionComponent.IsOnGround || motionComponent.IsOnPlatform) {
                    if (attemptJump) {
                        SetAnimationIfUnset("jump",AnimationState.Jump, JumpAnimationCallback, null);
                        motionComponent.AimedVelocity.Y = speedComponent.Speed.Y;
                        return;
                    } else {
                        motionComponent.AimedVelocity.Y = 0;
                    }

                    if (motionComponent.AimedVelocity.X != 0) {
                        SetAnimationIfUnset("walk", AnimationState.Walk, null, null);
                    } else {
                        SetAnimationIfUnset("idle", AnimationState.Idle, null, null);
                    }
                } else {
                    SetAnimationIfUnset("fall", AnimationState.Fall, null, null);
                }
            } else if (motionComponent.IsOnGround || motionComponent.IsOnPlatform) {
                motionComponent.AimedVelocity.Y = 0;
            }
            nearbyNPC = null;
        }

        private void SetAnimationIfUnset (string name, AnimationState state, AnimationCallback bodyCallback, AnimationCallback weaponCallback) {
            if (bodyAnimationState != state) {
                bodyAnimationState = state;
                if (SetAnimation(name, bodyCallback, weaponCallback))
                    weaponAnimationState = state;
            } else if (weaponAnimationState != AnimationState.Attack && weaponAnimationState != state) {
                weaponAnimationState = state;
                Owner.SetComponentInfo(ComponentData.VertexAnimation, WEAPON_ANIMATION, "walk", weaponCallback);
            }
        }

        private bool SetAnimation (string name, AnimationCallback bodyCallback, AnimationCallback weaponCallback) {
            Owner.SetComponentInfo(ComponentData.VertexAnimation, BODY_ANIMATION, name, bodyCallback);
            if (weaponAnimationState != AnimationState.Attack) {
                Owner.SetComponentInfo(ComponentData.VertexAnimation, WEAPON_ANIMATION, name, weaponCallback);
                return true;
            }
            return false;
        }

        private string JumpAnimationCallback (bool completed) {
            if (completed) {
                bodyAnimationState = AnimationState.Fall;
                if (SetAnimation("fall", null, null))
                    weaponAnimationState = AnimationState.Fall;
            }
            return null;
        }

        private string AttackAnimationCallback (bool completed) {
            if (!completed) BaseWeapon.Abort( );

            switch (bodyAnimationState) {
                case AnimationState.Fall:
                    weaponAnimationState = AnimationState.Fall;
                    return "fall";
                case AnimationState.Idle:
                    weaponAnimationState = AnimationState.Idle;
                    return "idle";
                case AnimationState.Jump:
                    weaponAnimationState = AnimationState.Jump;
                    return "jump";
                case AnimationState.Walk:
                    weaponAnimationState = AnimationState.Walk;
                    return "walk";
            }
            return null;
        }

#if DEBUG
        public override void Draw ( ) {
            BaseWeapon.Draw( );
        }
#endif  

        public new class Configuration : Component.Configuration {
            public float Health;
            public VertexAnimationData BodyAnimationData; // TEMP

            public override Component Create (Entity owner) {
                return new PlayerComponent(owner, BaseWeaponCollection.DiamondSword(owner), Health, BodyAnimationData);
            }
        }

        public class HealthTracker : UIBar.IValueBinder {
            private float _Value;
            public float Value { get { return _Value; } set { _Value = value; ValueChanged?.Invoke(value); } }
            public float Maximum { get; }
            public event Action<float> ValueChanged;
            public VertexAnimationData Animations; // TEMP

            public HealthTracker (float health) {
                Maximum = health;
                _Value = health;
            }
        }
    }
}