using System;
using System.Timers;
using mapKnight.Core;
using mapKnight.Extended.Components.Attributes;
using mapKnight.Extended.Components.Graphics;
using mapKnight.Extended.Components.Stats;

namespace mapKnight.Extended.Components.AI {

    [ComponentRequirement(typeof(DamageComponent))]
    public class LandMineComponent : Component {
        public bool Exploding;
        private DamageComponent damageComponent;
        private float sqrExplosionRadius;
        private float throwBackSpeed;

        public LandMineComponent (Entity owner, float throwbackspeed, float explosionradius) : base(owner) {
            throwBackSpeed = throwbackspeed;
            sqrExplosionRadius = explosionradius * explosionradius;
        }

        public override void Collision (Entity collidingEntity) {
            if (!Exploding && collidingEntity.Info.IsPlayer) {
                Exploding = true;
                Owner.SetComponentInfo(ComponentData.SpriteAnimation, "explode", true, (SpriteComponent.AnimationSuccessCallback)((bool success) => { Explode(collidingEntity); }));
            }
        }

        public override void Prepare ( ) {
            damageComponent = Owner.GetComponent<DamageComponent>( );
        }

        private void Explode (Entity entity) {
            Vector2 closestDist = GetClosestDistanceVectorTo(entity.Transform);
            Vector2 impulsDir = entity.Transform.Center - Owner.Transform.Center;
            impulsDir /= Math.Max(impulsDir.X, impulsDir.Y);
            if (float.IsNaN(impulsDir.X)) impulsDir.X = 0;
            if (float.IsNaN(impulsDir.Y)) impulsDir.Y = 1;
            float distpercent = closestDist.MagnitudeSqr( ) / sqrExplosionRadius;
            if (distpercent <= 1) {
                Vector2 appliedVel = impulsDir * (1.25f - distpercent) * throwBackSpeed;
                entity.SetComponentInfo(ComponentData.Velocity, appliedVel);
                entity.SetComponentInfo(ComponentData.Damage, damageComponent.OnTouch);
            }
            Owner.Destroy( );
        }

        private Vector2 GetClosestDistanceVectorTo (Transform transform) {
            float x = 0, y = 0;
            if (transform.BL.X > Owner.Transform.TR.X) {
                // right the bomb
                x = transform.BL.X - Owner.Transform.TR.X;
                if (transform.BL.Y > Owner.Transform.TR.Y) y = transform.BL.Y - Owner.Transform.TR.Y; // up right
                else if (transform.TR.Y < Owner.Transform.BL.Y) y = transform.TR.Y - Owner.Transform.BL.Y; // bottom right
            } else if (transform.TR.X < Owner.Transform.BL.X) {
                // left the bomb
                x = Owner.Transform.BL.X - transform.TR.X;
                if (transform.BL.Y > Owner.Transform.TR.Y) y = transform.BL.Y - Owner.Transform.TR.Y; // up left
                else if (transform.TR.Y < Owner.Transform.BL.Y) y = transform.TR.Y - Owner.Transform.BL.Y; // bottom left
            } else {
                // up or bellow the bomb
                if (transform.BL.Y > Owner.Transform.TR.Y) y = transform.BL.Y - Owner.Transform.TR.Y; // up
                else if (transform.TR.Y < Owner.Transform.BL.Y) y = transform.TR.Y - Owner.Transform.BL.Y; // bottom
            }
            return new Vector2(x, y);
        }

        public new class Configuration : Component.Configuration {
            public float ExplosionRadius;
            public float ThrowBackSpeed;

            public override Component Create (Entity owner) {
                return new LandMineComponent(owner, ThrowBackSpeed, ExplosionRadius);
            }
        }
    }
}