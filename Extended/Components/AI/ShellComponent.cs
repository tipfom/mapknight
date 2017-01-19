using System;
using System.Timers;
using mapKnight.Core;
using mapKnight.Extended.Components.AI.Basics;
using mapKnight.Extended.Components.Attributes;
using mapKnight.Extended.Components.Graphics;
using mapKnight.Extended.Components.Movement;
using mapKnight.Extended.Components.Stats;

namespace mapKnight.Extended.Components.AI {

    [ComponentRequirement(typeof(SpeedComponent))]
    [ComponentRequirement(typeof(TriggerComponent))]
    public class ShellComponent : Component {
        private float frenzySpeed;
        private float attackSpeed;
        private bool hasting;
        private float hastingDirection;
        private MotionComponent motionComponent;
        private SpeedComponent speedComponent;
        private bool stunned;
        private Entity target;
        private float walkpossibility = 1f;

        public ShellComponent (Entity owner, float frenzyspeed, float attackspeed) : base(owner) {
            owner.Domain = EntityDomain.Enemy;

            frenzySpeed = frenzyspeed;
            attackSpeed = attackspeed;

            Owner.SetComponentInfo(ComponentData.BoneTexture, "shell");
            Owner.SetComponentInfo(ComponentData.BoneOffset, Tuple.Create("shell", new Vector2(9.5f, 12.5f)));
            Owner.SetComponentInfo(ComponentData.VertexAnimation, "idle", true, (AnimationComponent.AnimationCallback)AnimationCallbackIdle);
        }

        public override void Prepare ( ) {
            Owner.GetComponent<TriggerComponent>( ).Triggered += Trigger_Triggered;
            speedComponent = Owner.GetComponent<SpeedComponent>( );
            motionComponent = Owner.GetComponent<MotionComponent>( );
            hasting = false;
            stunned = false;
        }

        public override void Update (DeltaTime dt) {
            if (motionComponent.IsAtWall && !stunned) {
                hasting = false;
                stunned = true;
                motionComponent.AimedVelocity.X = -hastingDirection * frenzySpeed * speedComponent.Speed.X;
                Owner.SetComponentInfo(ComponentData.VertexAnimation, "frenzy", true, (AnimationComponent.AnimationCallback)AnimationCallbackFrenzy);
            }
        }

        private void Trigger_Triggered (Entity entity) {
            if (!(hasting || stunned) && entity.Domain == EntityDomain.Player) {
                target = entity;
                Owner.SetComponentInfo(ComponentData.VertexAnimation, "prepare", true, (AnimationComponent.AnimationCallback)AnimationCallbackPrepare);
            }
        }

        private void AnimationCallbackPrepare (bool success) {
            hastingDirection = (target.Transform.BL.X > Owner.Transform.TR.X) ? 1 : -1;
            motionComponent.AimedVelocity.X = speedComponent.Speed.X * attackSpeed * hastingDirection;
            hasting = true;
            Owner.SetComponentInfo(ComponentData.VertexAnimation, "attack", true);
        }

        private void AnimationCallbackFrenzy (bool success) {
            stunned = false;
            motionComponent.AimedVelocity.X = 0;
            Owner.SetComponentInfo(ComponentData.VertexAnimation, "idle", true, (AnimationComponent.AnimationCallback)AnimationCallbackIdle);
        }

        private void AnimationCallbackIdle (bool success) {
            if (success) {
                if (Mathf.Random( ) > walkpossibility) {
                    walkpossibility = 1f;
                    float direction = Math.Sign(Mathf.Random( ) - 0.5f);
                    motionComponent.AimedVelocity.X = speedComponent.Speed.X * direction;
                    Owner.SetComponentInfo(ComponentData.VertexAnimation, "walk", true, (AnimationComponent.AnimationCallback)AnimationCallbackWalk);
                } else {
                    Owner.SetComponentInfo(ComponentData.VertexAnimation, "idle", true, (AnimationComponent.AnimationCallback)AnimationCallbackIdle);
                }
                walkpossibility *= 0.8f;
            }
        }

        private void AnimationCallbackWalk (bool success) {
            if (success) {
                motionComponent.AimedVelocity.X = 0f;
                Owner.SetComponentInfo(ComponentData.VertexAnimation, "idle", true, (AnimationComponent.AnimationCallback)AnimationCallbackIdle);
            }
        }

        public new class Configuration : Component.Configuration {
            public float FrenzySpeed;
            public float AttackSpeed;

            public override Component Create (Entity owner) {
                return new ShellComponent(owner, FrenzySpeed, AttackSpeed);
            }
        }
    }
}