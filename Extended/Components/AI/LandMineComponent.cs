using System;
using System.Timers;
using mapKnight.Core;
using mapKnight.Extended.Components.Attributes;
using mapKnight.Extended.Components.Graphics;
using mapKnight.Extended.Components.Stats;

namespace mapKnight.Extended.Components.AI {

    public class LandMineComponent : Component {
        public bool Exploding;
        private float damage;
        private float explosionRadius;
        private float throwBackSpeed;

        public LandMineComponent (Entity owner, float throwbackspeed, float explosionradius, float damage) : base(owner) {
            throwBackSpeed = throwbackspeed;
            explosionRadius = explosionradius;
            this.damage = damage;
        }

        public override void Collision (Entity collidingEntity) {
            if (!Exploding && collidingEntity.Info.IsPlayer) {
                Exploding = true;
                Owner.SetComponentInfo(ComponentData.SpriteAnimation, "explode", true, (SpriteComponent.AnimationCallback)((bool success) => { Explode(collidingEntity); }));
            }
        }

        private void Explode (Entity entity) {
            Vector2 closestDist = entity.Transform.Center - Owner.Transform.Center;
            float distpercent = closestDist.Magnitude( ) / explosionRadius;
            if (distpercent <= 1) {
                float influence = GetInfluence(distpercent);
                entity.SetComponentInfo(ComponentData.Velocity, closestDist.Normalize() * influence * throwBackSpeed);
                entity.SetComponentInfo(ComponentData.Damage, damage * influence);
            }
            Owner.Destroy( );
        }

        private float GetInfluence(float distpercent) {
            /* The Inluence of the Landmine scales by the cubic formula
             * f(x) = 1 - 0.8 * (x - 1) ^ 3
             */
            distpercent -= 1f;
            return 1f - 0.8f * distpercent * distpercent * distpercent;
        }

        public new class Configuration : Component.Configuration {
            public float ExplosionRadius;
            public float ThrowBackSpeed;
            public float Damage;

            public override Component Create (Entity owner) {
                return new LandMineComponent(owner, ThrowBackSpeed, ExplosionRadius, Damage);
            }
        }
    }
}