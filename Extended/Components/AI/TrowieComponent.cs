using System;
using mapKnight.Core;
using mapKnight.Extended.Components.Attributes;
using mapKnight.Extended.Components.Graphics;

namespace mapKnight.Extended.Components.AI {

    [ComponentRequirement(typeof(TriggerComponent))]
    public class TrowieComponent : Component {
        private BulletComponent.Configuration bulletComponentConfiguration;
        private Entity.Configuration bulletEntityConfiguration;
        private int nextThrow;
        private int timeBetweenThrows;
        private bool isThrowing;
        private Entity currentTarget;

        public TrowieComponent (Entity owner, Entity.Configuration bullet, int timebetweenthrows) : base(owner) {
            bulletEntityConfiguration = bullet;
            bulletComponentConfiguration = bulletEntityConfiguration.Components.GetConfiguration<BulletComponent.Configuration>( );
            timeBetweenThrows = timebetweenthrows;
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

        private void ThrowAnimationFinishedCallback(bool success) {
            isThrowing = false;
            bulletComponentConfiguration.Target = currentTarget;
            Vector2 spawnPoint = new Vector2(Owner.Transform.Center.X, Owner.Transform.TR.Y + bulletEntityConfiguration.Transform.HalfSize.Y);
            bulletEntityConfiguration.Create(spawnPoint, Owner.World);
       }

        public new class Configuration : Component.Configuration {
            public Entity.Configuration Bullet;
            public int TimeBetweenThrows;
            public float ThrowRate { get { return 1000f / TimeBetweenThrows; } set { TimeBetweenThrows = (int)(1f / value * 1000); } }

            public override Component Create (Entity owner) {
                return new TrowieComponent(owner, Bullet, TimeBetweenThrows);
            }
        }
    }
}