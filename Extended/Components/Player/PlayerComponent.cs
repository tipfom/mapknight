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
using mapKnight.Extended.Warfare;
using System.Timers;

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
        private AnimationState animationState = AnimationState.None;

        public PlayerComponent (Entity owner, BaseWeapon baseWeapon, float health) : base(owner) {
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
        }

        public override void Collision (Entity collidingEntity) {
            if (collidingEntity.Domain == EntityDomain.NPC) {
                nearbyNPC = collidingEntity;
            }
        }

        public override void Update (DeltaTime dt) {
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

            if (nearbyNPC == null) {
                if (BaseWeapon.Update( )) {
                    Owner.SetComponentInfo(ComponentData.VertexAnimation, "hit", true, (AnimationComponent.AnimationCallback)AnimationCallbackAttack);
                    animationState = AnimationState.Attack;
                    BaseWeapon.Attack( );
                }
            }

            while (Owner.HasComponentInfo(ComponentData.InputGesture)) {
                string data = (string)Owner.GetComponentInfo(ComponentData.InputGesture)[0];
                if (data == string.Empty) {
                    if (nearbyNPC == null) {
                        Action |= ActionMask.Jump;
                    } else if (!currentlyTalking) {
                        NPCComponent npccomponent = nearbyNPC.GetComponent<NPCComponent>( );
                        if (npccomponent.Available) {
                            currentlyTalking = true;
                            new UIDialog(Screen.Gameplay, npccomponent).DialogFinished += ( ) => currentlyTalking = false;
                        }
                    }
                    // JUMP, TODO
                } else {
                    // SPECIAL ATTACK, TODO
                }
            }

            if (currentlyTalking) return;

            Vector2 speed = speedComponent.Speed;
            if (Action.HasFlag(ActionMask.Left)) {
                motionComponent.AimedVelocity.X = -speed.X;
            } else if (Action.HasFlag(ActionMask.Right)) {
                motionComponent.AimedVelocity.X = speed.X;
            } else {
                motionComponent.AimedVelocity.X = 0;
            }

            if (animationState != AnimationState.Attack) {
                if (animationState != AnimationState.Jump) {
                    if (motionComponent.IsOnGround || motionComponent.IsOnPlatform) {
                        if (Action.HasFlag(ActionMask.Jump)) {
                            Action &= ~ActionMask.Jump;
                            animationState = AnimationState.Jump;
                            Owner.SetComponentInfo(ComponentData.VertexAnimation, "jump", true, (AnimationComponent.AnimationCallback)AnimationCallbackFinishJumping);
                            motionComponent.AimedVelocity.Y = speedComponent.Speed.Y;
                            return;
                        } else {
                            motionComponent.AimedVelocity.Y = 0;
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
                    } else if (animationState != AnimationState.Fall) {
                        Owner.SetComponentInfo(ComponentData.VertexAnimation, "fall", true);
                        animationState = AnimationState.Fall;
                    }
                }
            } else if (motionComponent.IsOnGround || motionComponent.IsOnPlatform) {
                motionComponent.AimedVelocity.Y = 0;
            }
            nearbyNPC = null;
        }

        private void AnimationCallbackFinishJumping (bool success) {
            if (success) {
                Owner.SetComponentInfo(ComponentData.VertexAnimation, "fall", true);
                animationState = AnimationState.Fall;
            }
        }

        private void AnimationCallbackAttack (bool success) {
            Owner.SetComponentInfo(ComponentData.VertexAnimation, "idle", true);
            animationState = AnimationState.Idle;
        }

#if DEBUG
        public override void Draw ( ) {
            BaseWeapon.Draw( );
        }
#endif  

        public new class Configuration : Component.Configuration {
            public string Weapon;
            public float Health;

            public override Component Create (Entity owner) {
                return new PlayerComponent(owner, BaseWeaponCollection.DiamondSword(owner), Health);
            }
        }

        public class HealthTracker : UIBar.IValueBinder {
            private float _Value;
            public float Value { get { return _Value; } set { _Value = value; ValueChanged?.Invoke(value); } }
            public float Maximum { get; }
            public event Action<float> ValueChanged;

            public HealthTracker (float health) {
                Maximum = health;
                _Value = health;
            }
        }
    }
}