using System;
using mapKnight.Core;
using mapKnight.Extended.Components.AI.Basics;
using mapKnight.Extended.Components.Graphics;
using mapKnight.Extended.Components.Movement;
using mapKnight.Extended.Components.Stats;
using mapKnight.Core.World.Components;
using mapKnight.Core.World;

namespace mapKnight.Extended.Components.AI {
    [ComponentRequirement(typeof(SpeedComponent))]
    [ComponentRequirement(typeof(HealthComponent))]
    [ComponentRequirement(typeof(TriggerComponent))]
    public class ShellComponent : Component {
        private float frenzySpeed;
        private float attackSpeed;
        private bool hasting;
        private float hastingDirection;
        private MotionComponent motionComponent;
        private SpeedComponent speedComponent;
        private HealthComponent healthComponent;
        private bool stunned;
        private Entity target;
        private float walkpossibility = 1f;

        public ShellComponent (Entity owner, float frenzyspeed, float attackspeed) : base(owner) {
            owner.Domain = EntityDomain.Enemy;

            frenzySpeed = frenzyspeed;
            attackSpeed = attackspeed;
        }

        public override void Prepare ( ) {
            Owner.GetComponent<TriggerComponent>( ).Triggered += Trigger_Triggered;
            speedComponent = Owner.GetComponent<SpeedComponent>( );
            motionComponent = Owner.GetComponent<MotionComponent>( );
            healthComponent = Owner.GetComponent<HealthComponent>( );
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
            if (Owner.HasComponentInfo(ComponentData.Damage) && healthComponent.Current > healthComponent.Initial * .75f) {
                object[ ] data = Owner.GetComponentInfo(ComponentData.Damage);
                float initialDamage = (float)data[1];
                Owner.SetComponentInfo(ComponentData.Damage, data[0], Math.Min(healthComponent.Current - healthComponent.Initial * .7f, initialDamage));
            }
        }

        private void Trigger_Triggered (Entity entity) {
            if (!(hasting || stunned) && entity.Domain == EntityDomain.Player && motionComponent.ScaleX == Math.Sign(entity.Transform.Center.X - Owner.Transform.Center.X) && Math.Abs(entity.Transform.Center.X - Owner.Transform.Center.X) > Owner.Transform.HalfSize.X / 2f) {
                target = entity;
                Owner.SetComponentInfo(ComponentData.VertexAnimation, "prepare", true, (AnimationComponent.AnimationCallback)AnimationCallbackPrepare);
            }
        }

        private void AnimationCallbackPrepare (bool success) {
            hastingDirection = (target.Transform.Center.X > Owner.Transform.Center.X) ? 1 : -1;
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
                    walkpossibility *= 0.8f;
                }
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