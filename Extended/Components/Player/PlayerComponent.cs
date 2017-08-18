using System;
using mapKnight.Core;
using mapKnight.Core.World;
using mapKnight.Core.World.Components;
using mapKnight.Extended.Components.AI;
using mapKnight.Extended.Components.Graphics;
using mapKnight.Extended.Components.Movement;
using mapKnight.Extended.Components.Stats;
using mapKnight.Extended.Graphics.UI;
using mapKnight.Extended.Screens;
using mapKnight.Extended.Combat;
using mapKnight.Extended.Graphics.Animation;
using static mapKnight.Extended.Components.Player.PlayerAnimationComponent;
using mapKnight.Core.World.Serialization;
using System.Collections.Generic;

namespace mapKnight.Extended.Components.Player {
    [UpdateAfter(typeof(SpeedComponent))]
    [UpdateBefore(typeof(MotionComponent))]
    public class PlayerComponent : Component {
        public ActionMask Action;
        public PrimaryWeapon PrimaryWeapon;
        public SecondaryWeapon SecondaryWeapon;
        public HealthTracker Health;

        private bool currentlyTalking;
        private bool attemptJump = false;
        private float jumpHeight;
        private Entity nearbyNPC;
        private MotionComponent motionComponent;
        private SpeedComponent speedComponent;
        private AnimationState bodyAnimationState = AnimationState.None;
        private AnimationState weaponAnimationState = AnimationState.None;
        private VertexAnimationData bodyAnimationData; // TEMP, WILL SOON BE PART OF THE ARMOR

        public PlayerComponent (Entity owner, float health, float jumpHeight, VertexAnimationData bodyAnimationData) : base(owner) {
            this.bodyAnimationData = bodyAnimationData;
            this.jumpHeight = jumpHeight;

            owner.Domain = EntityDomain.Player;

            Health = new HealthTracker(health);
        }

        public override void Destroy ( ) {
            GameOverScreen gameOverScreen = new GameOverScreen((Extended.Graphics.Map)Owner.World);
            gameOverScreen.Load( );
            Screen.Active = gameOverScreen;
#if DEBUG
            PrimaryWeapon.Destroy( );
#endif
        }

        public override void Prepare ( ) {
            speedComponent = Owner.GetComponent<SpeedComponent>( );
            motionComponent = Owner.GetComponent<MotionComponent>( );
            speedComponent.Default.Y = Mathf.Sqrt(2 * jumpHeight * -Owner.World.Gravity.Y);

            SecondaryWeapon = new Combat.Collections.Secondaries.Shield(Owner);
            SecondaryWeapon.Prepare( );

            PrimaryWeapon = Screen.MainMenu.SelectedWeapon(Owner);
            PrimaryWeapon.Prepare( );
            Owner.GetComponent<PlayerAnimationComponent>( ).LoadAnimations(bodyAnimationData, PrimaryWeapon.AnimationData, SecondaryWeapon.AnimationData, "player", PrimaryWeapon.Texture, SecondaryWeapon.Texture);
        }

        public override void Collision (Entity collidingEntity) {
            if (collidingEntity.Domain == EntityDomain.NPC) {
                nearbyNPC = collidingEntity;
            }
        }

        public void ActionRequested ( ) {
            if (nearbyNPC == null) {
                attemptJump = true;
            } else if (!currentlyTalking) {
                NPCComponent npccomponent = nearbyNPC.GetComponent<NPCComponent>( );
                if (npccomponent.Available) {
                    currentlyTalking = true;
                    new UIDialog(Screen.Gameplay, npccomponent).DialogFinished += ( ) => currentlyTalking = false;
                }
            }
        }

        public override void Update (DeltaTime dt) {
            SecondaryWeapon.Update(dt);

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

            while (Owner.HasComponentInfo(ComponentData.Heal)) {
                float value = (float)Owner.GetComponentInfo(ComponentData.Heal)[0];
                Health.Value += value;
            }

            if (nearbyNPC == null && weaponAnimationState != AnimationState.Attack && PrimaryWeapon.Update( )) {
                Owner.SetComponentInfo(ComponentData.VertexAnimation, WEAPON_ANIMATION, "attack" + Mathi.Random(0, 3), (AnimationCallback)AttackAnimationCallback); // TODO
                weaponAnimationState = AnimationState.Attack;
                PrimaryWeapon.Attack( );
            }

            motionComponent.Lock = SecondaryWeapon.Lock;
            if (SecondaryWeapon.Lock) return;

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
                        SetAnimationIfUnset("jump", AnimationState.Jump, JumpAnimationCallback, null);
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
            attemptJump = false;
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
            if (!completed) PrimaryWeapon.Abort( );

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
            PrimaryWeapon.Draw( );
        }
#endif  

        public new class Configuration : Component.Configuration {
            public float Health;
            public float JumpHeight;
            public VertexAnimationData BodyAnimationData; // TEMP

            public override Component Create (Entity owner) {
                return new PlayerComponent(owner, Health, JumpHeight, BodyAnimationData);
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