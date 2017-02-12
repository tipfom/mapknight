using System;
using mapKnight.Core;
using mapKnight.Extended.Components.AI.Basics;
using mapKnight.Extended.Components.Graphics;

namespace mapKnight.Extended.Components.AI.Guardian {
    public class PrivateComponent : BishopComponent {
        private TentComponent tent;
        private float patrolDistanceSqr;
        private float damage;
        private int attackCooldown;
        private int nextAttackTime;
        private bool inAttackRange;
        private Entity attackingEntity;

        public PrivateComponent (Entity owner, TentComponent tent, float damage, int attackCooldown) : base(owner, true) {
            owner.Domain = EntityDomain.Enemy;

            this.tent = tent;
            this.patrolDistanceSqr = tent.PatrolRangeSqr;
            this.damage = damage;
            this.attackCooldown = attackCooldown;
        }

        public override void Collision (Entity collidingEntity) {
            if (collidingEntity.Domain == EntityDomain.Player) {
                motionComponent.AimedVelocity.X = 0f;
                inAttackRange = true;
                if (Environment.TickCount > nextAttackTime) {
                    if (nextAttackTime != -1) {
                        attackingEntity = collidingEntity;
                        Owner.SetComponentInfo(ComponentData.SpriteAnimation, "atk", true, (SpriteComponent.AnimationCallback)AttackAnimationCallback);
                        Owner.SetComponentInfo(ComponentData.SpriteAnimation, "def", false, (SpriteComponent.AnimationCallback)DefAnimationCallback);
                        nextAttackTime = -1;
                    }
                } else {
                    Owner.SetComponentInfo(ComponentData.SpriteAnimation, "def", true, (SpriteComponent.AnimationCallback)DefAnimationCallback);
                    Owner.SetComponentInfo(ComponentData.SpriteAnimation, "walk", false);
                }
            }
        }

        public override void Update (DeltaTime dt) {
            if (!inAttackRange) {
                if ((Owner.Transform.Center - tent.Owner.Transform.Center).MagnitudeSqr( ) > patrolDistanceSqr) {
                    Turn( );
                }
                base.Update(dt);
            }
        }

        public override void Destroy ( ) {
            tent.PrivateDied(Owner);
            base.Destroy( );
        }

        private void DefAnimationCallback (bool success) {
            inAttackRange = !success;
        }

        private void AttackAnimationCallback (bool success) {
            if (success) {
                attackingEntity.SetComponentInfo(ComponentData.Damage, damage);
                nextAttackTime = Environment.TickCount + attackCooldown;
            }
        }

        public new class Configuration : Component.Configuration {
            public TentComponent Tent;
            public float Damage;
            public int AttackCooldown;

            public override Component Create (Entity owner) {
                return new PrivateComponent(owner, Tent, Damage, AttackCooldown);
            }
        }
    }
}
