using System;
using mapKnight.Core;
using mapKnight.Extended.Components.Attributes;
using mapKnight.Extended.Components.Graphics;

namespace mapKnight.Extended.Components.AI {
    
    [ComponentRequirement(typeof(TriggerComponent))]
    public class TurretComponent : Component {
        public bool IsFacingLeft = true;
        private BulletComponent.Configuration bulletComponentConfig;
        private Entity.Configuration bulletEntityConfig;
        private float bulletSpawnpointYPercent;
        private int lockTime;
        private int nextShot;
        private int nextTurn;
        private int timeBetweenShots;
        private int timeBetweenTurns;

        public TurretComponent (Entity owner, Entity.Configuration bullet, int timebetweenshots, int timebetweenturns, int locktime, float bulletspawnpointypercent) : base(owner) {
            bulletEntityConfig = bullet;
            bulletComponentConfig = bullet.Components.GetConfiguration<BulletComponent.Configuration>( );
            timeBetweenShots = timebetweenshots;
            timeBetweenTurns = timebetweenturns;
            lockTime = locktime;
            bulletSpawnpointYPercent = bulletspawnpointypercent;
        }

        public override void Prepare ( ) {
            Owner.GetComponent<TriggerComponent>( ).Triggered += Trigger_Triggered;
        }

        public override void Update (DeltaTime dt) {
            if (Environment.TickCount > nextTurn) {
                IsFacingLeft = !IsFacingLeft;
                nextTurn += timeBetweenTurns;
            }
            Owner.SetComponentInfo(ComponentData.ScaleX, IsFacingLeft ? 1f : -1f);
        }

        private void Trigger_Triggered (Entity entity) {
            if (entity.Info.IsPlayer && Environment.TickCount > nextShot && ((IsFacingLeft && entity.Transform.BL.X > Owner.Transform.TR.X) || (!IsFacingLeft && entity.Transform.TR.X < Owner.Transform.BL.X))) {
                // shot
                nextShot = Environment.TickCount + timeBetweenShots;
                nextTurn = Environment.TickCount + lockTime;
                bulletComponentConfig.Target = entity;
                Vector2 spawnPoint = new Vector2(Owner.Transform.Center.X + (Owner.Transform.HalfSize.X + bulletEntityConfig.Transform.HalfSize.X) * (IsFacingLeft ? 1 : -1), Owner.Transform.BL.Y + Owner.Transform.Size.Y * bulletSpawnpointYPercent);
                bulletEntityConfig.Create(spawnPoint, Owner.World);
            }
        }

        public new class Configuration : Component.Configuration {
            public Entity.Configuration Bullet;
            public float BulletSpawnpointYPercent;
            public int LockOnTargetTime;
            public int TimeBetweenShots;
            public int TimeBetweenTurns;
            public float FireRate { get { return 1000f / TimeBetweenShots; } set { TimeBetweenShots = (int)(1f / value * 1000); } }
            public float TurnRate { get { return 1000f / TimeBetweenTurns; } set { TimeBetweenTurns = (int)(1f / value * 1000); } }

            public override Component Create (Entity owner) {
                return new TurretComponent(owner, Bullet, TimeBetweenShots, TimeBetweenTurns, LockOnTargetTime, BulletSpawnpointYPercent);
            }
        }
    }
}