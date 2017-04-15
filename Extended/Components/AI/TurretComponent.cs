using System;
using mapKnight.Core;
using mapKnight.Core.World;
using mapKnight.Core.World.Components;
using mapKnight.Extended.Components.AI.Basics;

namespace mapKnight.Extended.Components.AI {
    [ComponentRequirement(typeof(TriggerComponent))]
    public class TurretComponent : Component {
        public bool IsFacingLeft = true;
        private Entity.Configuration bulletEntityConfig;
        private float bulletSpawnpointYPercent;
        private float bulletSpeed;
        private int lockTime;
        private int nextShot;
        private int nextTurn;
        private int lockedTill;
        private int timeBetweenShots;
        private int timeBetweenTurns;
        private Entity currentTarget;

        public TurretComponent (Entity owner, Entity.Configuration bullet, int timebetweenshots, int timebetweenturns, int locktime, float bulletspawnpointypercent, float bulletspeed) : base(owner) {
            owner.Domain = EntityDomain.Enemy;

            bulletEntityConfig = bullet;
            timeBetweenShots = timebetweenshots;
            timeBetweenTurns = timebetweenturns;
            lockTime = locktime;
            bulletSpawnpointYPercent = bulletspawnpointypercent;
            bulletSpeed = bulletspeed;
        }

        public override void Prepare ( ) {
            Owner.GetComponent<TriggerComponent>( ).Triggered += Trigger_Triggered;
            nextTurn = Environment.TickCount;
        }

        public override void Update (DeltaTime dt) {
            if (Environment.TickCount < lockedTill) {
                IsFacingLeft = currentTarget.Transform.BL.X > Owner.Transform.TR.X; 
            } else if (Environment.TickCount > nextTurn) {
                IsFacingLeft = !IsFacingLeft;
                nextTurn += timeBetweenTurns;
            }
            Owner.SetComponentInfo(ComponentData.ScaleX, IsFacingLeft ? 1f : -1f);
        }

        private void Trigger_Triggered (Entity entity) {
            if (entity.Domain == EntityDomain.Player && Environment.TickCount > nextShot && ((IsFacingLeft && entity.Transform.BL.X > Owner.Transform.TR.X) || (!IsFacingLeft && entity.Transform.TR.X < Owner.Transform.BL.X))) {
                // shot
                currentTarget = entity;
                nextShot = Environment.TickCount + timeBetweenShots;
                lockedTill = Environment.TickCount + lockTime;
                nextTurn = lockedTill + timeBetweenTurns;
                Vector2 spawnPoint = new Vector2(Owner.Transform.Center.X + (Owner.Transform.HalfSize.X + bulletEntityConfig.Transform.HalfSize.X) * (IsFacingLeft ? 1 : -1), Owner.Transform.BL.Y + Owner.Transform.Size.Y * bulletSpawnpointYPercent);
                bulletEntityConfig.Create(spawnPoint, Owner.World).GetComponent< Movement.MotionComponent>().AimedVelocity.X = (IsFacingLeft ? 1f : -1f) * bulletSpeed;
            }
        }

        public new class Configuration : Component.Configuration {
            public Entity.Configuration Bullet;
            public float BulletSpawnpointYPercent;
            public int LockOnTargetTime;
            public int TimeBetweenShots;
            public int TimeBetweenTurns;
            public float BulletSpeed;

            public override Component Create (Entity owner) {
                return new TurretComponent(owner, Bullet, TimeBetweenShots, TimeBetweenTurns, LockOnTargetTime, BulletSpawnpointYPercent, BulletSpeed);
            }
        }
    }
}