using System;
using mapKnight.Core;
using mapKnight.Extended.Components.AI.Basics;
using mapKnight.Extended.Components.Attributes;
using mapKnight.Extended.Components.Graphics;
using mapKnight.Extended.Components.Movement;

namespace mapKnight.Extended.Components.AI {

    [ComponentRequirement(typeof(TriggerComponent))]
    public class PluggerComponent : Component {
        private Entity.Configuration bulletEntityConfiguration;
        private float bulletSpeed;
        private int nextThrow;
        private int timeBetweenThrows;
        private bool isThrowing;
        private Entity currentTarget;

        public PluggerComponent (Entity owner, Entity.Configuration bullet, int timebetweenthrows, float bulletspeed) : base(owner) {
            bulletEntityConfiguration = bullet;
            timeBetweenThrows = timebetweenthrows;
            bulletSpeed = bulletspeed;
        }

        public override void Prepare ( ) {
            Owner.GetComponent<TriggerComponent>( ).Triggered += Trigger_Triggered;
            nextThrow = Environment.TickCount;
        }

        public override void Update (DeltaTime dt) {
        }

        private void Trigger_Triggered (Entity entity) {
            if (entity.Info.IsPlayer && Environment.TickCount > nextThrow && !isThrowing) {
                nextThrow = Environment.TickCount + timeBetweenThrows;
                isThrowing = true;
                currentTarget = entity;
                Owner.SetComponentInfo(ComponentData.SpriteAnimation, "shot", true, (SpriteComponent.AnimationCallback)ThrowAnimationFinishedCallback);
                Owner.SetComponentInfo(ComponentData.SpriteAnimation, "walk", false);
            }
        }

        private void ThrowAnimationFinishedCallback (bool success) {
            isThrowing = false;

            // calc velocity of the bullet to hit the player
            float c = currentTarget.Transform.Center.Y - Owner.Transform.Center.Y; // distance y axis
            float d = currentTarget.Transform.Center.X - Owner.Transform.Center.X; // distance x axis
            if (float.IsNaN(c) || float.IsNaN(d)) {
                return;
            }

            Vector2 spawnPoint = new Vector2(Owner.Transform.Center.X, Owner.Transform.TR.Y + bulletEntityConfiguration.Transform.HalfSize.Y);
            MotionComponent motionComponent = bulletEntityConfiguration.Create(spawnPoint, Owner.World).GetComponent<MotionComponent>( );

            float t = Math.Abs(d) / bulletSpeed; // time
            float vx = bulletSpeed * Math.Sign(d);
            float vy = (c - 0.5f * Owner.World.Gravity.Y * motionComponent.GravityInfluence * t * t) / t;
            motionComponent.AimedVelocity = new Vector2(vx, vy);
        }

        public new class Configuration : Component.Configuration {
            public Entity.Configuration Bullet;
            public float BulletSpeed;
            public int TimeBetweenThrows;

            public override Component Create (Entity owner) {
                return new PluggerComponent(owner, Bullet, TimeBetweenThrows, BulletSpeed);
            }
        }
    }
}