using System;
using mapKnight.Core;
using mapKnight.Core.World.Components;
using mapKnight.Core.World;
using mapKnight.Extended.Components.Graphics;
using mapKnight.Extended.Components.Movement;
using mapKnight.Extended.Components.Stats;
using mapKnight.Extended.Components.AI.Basics;
using AnimationCallback = mapKnight.Extended.Components.Graphics.SpriteComponent.AnimationCallback;

namespace mapKnight.Extended.Components.AI {
    [UpdateBefore(typeof(MotionComponent))]
    [UpdateBefore(typeof(SpriteComponent))]
    [UpdateAfter(typeof(TriggerComponent))]
    [ComponentRequirement(typeof(SpeedComponent))]
    public class SeplingComponent : Component {
        const float VELOCITY_RATIO = 167f / 65f;

        private float bulletSpeed;
        private int nextShootTime;
        private int shootCooldown;
        private int chillDistance;
        private float walkpossibility = .8f;
        private float forcedNextDirection;
        private bool shooting;
        private bool walking;
        private bool scared;
        private Entity enemy;
        private Entity.Configuration bullet;
        private MotionComponent motionComponent;
        private SpeedComponent speedComponent;
        private Vector2 bulletOffset;

        public SeplingComponent (Entity owner, Entity.Configuration bullet, int shootcooldown, int chilldistance, float bulletspeed, Vector2 bulletoffset) : base(owner) {
            owner.Domain = EntityDomain.Enemy;

            this.bullet = bullet;
            this.shootCooldown = shootcooldown;
            this.chillDistance = chilldistance;
            this.bulletSpeed = bulletspeed;
            this.bulletOffset = bulletoffset;
        }

        public override void Prepare ( ) {
            base.Prepare( );
            motionComponent = Owner.GetComponent<MotionComponent>( );
            speedComponent = Owner.GetComponent<SpeedComponent>( );
            Owner.GetComponent<TriggerComponent>( ).Triggered += TriggerComponent_Triggered;
            Owner.GetComponent<HealthComponent>( ).IsHit = HealthComponent_IsHit;

            Owner.SetComponentInfo(ComponentData.SpriteAnimation, "idle", true, (AnimationCallback)AnimationCallbackIdle);
        }

        private void TriggerComponent_Triggered (Entity entity) {
            if (scared && entity.Domain == EntityDomain.Player) {
                enemy = entity;
            }
        }

        private bool HealthComponent_IsHit (Entity entity) {
            if(entity.Domain == EntityDomain.Player) {
                scared = true;
                return true;
            }
            return false;
        }

        public override void Update (DeltaTime dt) {
            base.Update(dt);
            if (enemy != null) {
                if (nextShootTime < Environment.TickCount && !shooting) {
                    // shoot
                    float dir = Math.Sign(enemy.Transform.X - Owner.Transform.X);
                    if (dir != 0) motionComponent.ScaleX = dir;

                    motionComponent.AimedVelocity.X = 0;
                    shooting = true;
                    Owner.SetComponentInfo(ComponentData.SpriteAnimation, "shoot_prep", true, (AnimationCallback)AnimationCallbackShootPrepare);
                } else if (!shooting) {
                    // run away if the entity still is in range
                    if (Math.Abs(enemy.Transform.X - Owner.Transform.X) < chillDistance) {
                        if (motionComponent.IsAtWall) {
                            Owner.SetComponentInfo(ComponentData.SpriteAnimation, "def", true);
                        } else {
                            motionComponent.AimedVelocity.X = Math.Sign(Owner.Transform.X - enemy.Transform.X) * speedComponent.Speed.X;
                            Owner.SetComponentInfo(ComponentData.SpriteAnimation, "run", true);
                        }
                    } else {
                        enemy = null;
                        Owner.SetComponentInfo(ComponentData.SpriteAnimation, "idle", true, (AnimationCallback)AnimationCallbackIdle);
                        motionComponent.AimedVelocity.X = 0;
                    }
                }
            } else if(walking) {
                if (motionComponent.IsAtWall) {
                    forcedNextDirection = -Math.Sign(motionComponent.AimedVelocity.X);
                    motionComponent.AimedVelocity.X = 0f;
                    Owner.SetComponentInfo(ComponentData.SpriteAnimation, "idle", true, (AnimationCallback)AnimationCallbackIdle);
                }
            }
        }

        private void AnimationCallbackShootPrepare (bool success) {
            if (success) {
                float vy = bulletSpeed * VELOCITY_RATIO;
                bullet.Create(Owner.Transform.Center + bulletOffset * new Vector2(motionComponent.ScaleX, 1) * Owner.Transform.Size, Owner.World)
                    .GetComponent<MotionComponent>( ).AimedVelocity = new Vector2(motionComponent.ScaleX * bulletSpeed, vy);

                Owner.SetComponentInfo(ComponentData.SpriteAnimation, "shoot_end", true, (AnimationCallback)AnimationCallbackShootEnd);
            } else {
                shooting = false;
            }
        }

        private void AnimationCallbackShootEnd (bool success) {
            Owner.SetComponentInfo(ComponentData.SpriteAnimation, "grow", true, (AnimationCallback)AnimationCallbackGrowEnd);
            Owner.SetComponentInfo(ComponentData.SpriteAnimation, "idle", false, (AnimationCallback)AnimationCallbackIdle);
        }

        private void AnimationCallbackGrowEnd (bool success) {
            if (!success) {
                // try to regrow
                Owner.SetComponentInfo(ComponentData.SpriteAnimation, "grow", true, (AnimationCallback)AnimationCallbackGrowEnd);
            } else {
                shooting = false;
                nextShootTime = Environment.TickCount + shootCooldown;
            }
        }

        private void AnimationCallbackIdle(bool success) {
            if (success) {
                if (Mathf.Random( ) > walkpossibility) {
                    walkpossibility = .8f;
                    float direction = motionComponent.IsAtWall ? forcedNextDirection : Math.Sign(Mathf.Random( ) - 0.5f);
                    motionComponent.AimedVelocity.X = speedComponent.Speed.X * direction;
                    walking = true;
                    Owner.SetComponentInfo(ComponentData.SpriteAnimation, "walk", true, (AnimationCallback)AnimationCallbackCWalk);
                } else {
                    walkpossibility *= 0.7f;
                }
            }
        }

        private void AnimationCallbackCWalk(bool success) {
            walking = false;
            if (success) {
                motionComponent.AimedVelocity.X = 0f;
                Owner.SetComponentInfo(ComponentData.SpriteAnimation, "idle",true, (AnimationCallback)AnimationCallbackIdle);
            }
        }

        public new class Configuration : Component.Configuration {
            public Entity.Configuration Bullet;
            public Vector2 BulletOffset;
            public float BulletSpeed;
            public int ShootCooldown;
            public int ChillDistance;

            public override Component Create (Entity owner) {
                return new SeplingComponent(owner, Bullet, ShootCooldown, ChillDistance, BulletSpeed, BulletOffset);
            }
        }
    }
}
