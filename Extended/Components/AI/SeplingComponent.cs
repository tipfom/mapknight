using System;
using mapKnight.Core;
using mapKnight.Extended.Components.Attributes;
using mapKnight.Extended.Components.Graphics;
using mapKnight.Extended.Components.Movement;
using mapKnight.Extended.Components.Stats;
using mapKnight.Extended.Components.AI.Basics;

namespace mapKnight.Extended.Components.AI {
    [UpdateBefore(typeof(MotionComponent))]
    [UpdateBefore(typeof(SpriteComponent))]
    [UpdateAfter(typeof(TriggerComponent))]
    [ComponentRequirement(typeof(SpeedComponent))]
    public class SeplingComponent : Component {
        private int nextShootTime;
        private int shootCooldown;
        private int chillDistance;
        private bool shooting;
        private Entity enemy;
        private Entity.Configuration bullet;
        private MotionComponent motionComponent;
        private SpeedComponent speedComponent;

        public SeplingComponent (Entity owner, Entity.Configuration bullet, int shootcooldown, int chilldistance) : base(owner) {
            this.bullet = bullet;
            this.shootCooldown = shootcooldown;
            this.chillDistance = chilldistance;
        }

        public override void Prepare ( ) {
            base.Prepare( );
            motionComponent = Owner.GetComponent<MotionComponent>( );
            speedComponent = Owner.GetComponent<SpeedComponent>( );
            Owner.GetComponent<TriggerComponent>( ).Triggered += TriggerComponent_Triggered;
        }

        private void TriggerComponent_Triggered (Entity entity) {
            if (entity.Info.IsPlayer) {
                enemy = entity;
            }
        }

        public override void Update (DeltaTime dt) {
            base.Update(dt);
            if (enemy != null) {
                if (nextShootTime < Environment.TickCount && !shooting) {
                    // shoot
                    motionComponent.ScaleX = Math.Sign(Owner.Transform.X - enemy.Transform.X);
                    motionComponent.AimedVelocity.X = 0;
                    shooting = true;
                    Owner.SetComponentInfo(ComponentData.SpriteAnimation, "shoot_prep", true, (SpriteComponent.AnimationCallback)AnimationCallbackShootPrepare);
                } else if (!shooting) {
                    // run away if the entity still is in range
                    if (Math.Abs(enemy.Transform.X - Owner.Transform.X) < chillDistance) {
                        motionComponent.AimedVelocity.X = Math.Sign(Owner.Transform.X - enemy.Transform.X) * speedComponent.Speed.X;
                        Owner.SetComponentInfo(ComponentData.SpriteAnimation, "walk", false);
                    } else {
                        enemy = null;
                        Owner.SetComponentInfo(ComponentData.SpriteAnimation, "idle", true);
                        Owner.SetComponentInfo(ComponentData.SpriteAnimation, "idle", false);
                    }
                }
            }
        }

        private void AnimationCallbackShootPrepare (bool success) {
            if (success) {
                bullet.Create(Owner.Transform.Center, Owner.World);
                Owner.SetComponentInfo(ComponentData.SpriteAnimation, "shoot_end", true, (SpriteComponent.AnimationCallback)AnimationCallbackShootEnd);
            } else {
                shooting = false;
            }
        }

        private void AnimationCallbackShootEnd (bool success) {
            Owner.SetComponentInfo(ComponentData.SpriteAnimation, "grow", true, (SpriteComponent.AnimationCallback)AnimationCallbackGrowEnd);
        }

        private void AnimationCallbackGrowEnd(bool success) {
            if (!success) {
                // try to regrow
                Owner.SetComponentInfo(ComponentData.SpriteAnimation, "grow", true, (SpriteComponent.AnimationCallback)AnimationCallbackGrowEnd);
            } else {
                shooting = false;
                nextShootTime = Environment.TickCount + shootCooldown;
            }
        }

        public new class Configuration : Component.Configuration {
            public Entity.Configuration Bullet;
            public int ShootCooldown;
            public int ChillDistance;

            public override Component Create (Entity owner) {
                return new SeplingComponent(owner, Bullet, ShootCooldown, ChillDistance);
            }
        }
    }
}
