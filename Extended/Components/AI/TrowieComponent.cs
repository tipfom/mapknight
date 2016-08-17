using System;
using mapKnight.Core;
using mapKnight.Extended.Components.Attributes;

namespace mapKnight.Extended.Components.AI {

    [ComponentRequirement(typeof(TriggerComponent))]
    public class TrowieComponent : Component {
        private BulletComponent.Configuration bulletComponentConfiguration;
        private Entity.Configuration bulletEntityConfiguration;
        private int nextThrow;
        private int timeBetweenThrows;

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
            if (entity.Info.IsPlayer && Environment.TickCount > nextThrow) {
                nextThrow = Environment.TickCount + timeBetweenThrows;
                bulletComponentConfiguration.Target = entity;
                Vector2 spawnPoint = new Vector2(Owner.Transform.Center.X, Owner.Transform.TR.Y + bulletEntityConfiguration.Transform.BoundsHalf.Y);
                bulletEntityConfiguration.Create(spawnPoint, Owner.Owner);
            }
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