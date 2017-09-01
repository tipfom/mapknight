using System;
using System.Collections.Generic;
using mapKnight.Core;
using mapKnight.Core.World;
using mapKnight.Core.World.Components;
using mapKnight.Extended.Components.AI.Basics;
using mapKnight.Extended.Components.Graphics;
using static mapKnight.Extended.Components.Graphics.SpriteComponent;
using mapKnight.Extended.Graphics.Particles;
using mapKnight.Extended.Graphics;
using mapKnight.Extended.Combat;

namespace mapKnight.Extended.Components.AI {
    [ComponentRequirement(typeof(TriggerComponent))]
    public class DrillComponent : Component {
        private float speed;
        private float progress;
        private float totalTime;
        private Vector2 lastTarget;
        private Vector2 currentTarget;

        private bool exploding = false;
        private float explosionRadiusSqr;
        private float damage;
        private Entity target;

        private SkeletComponent skeletComponent;

        public DrillComponent (Entity owner, float damage, float explosionRadiusSqr, float speed) : base(owner) {
            this.damage = damage;
            this.speed = speed;
            this.explosionRadiusSqr = explosionRadiusSqr;
        }

        public override void Prepare ( ) {
            skeletComponent = Owner.GetComponent<SkeletComponent>( );
            Owner.GetComponent<TriggerComponent>( ).Triggered += TriggerComponent_Triggered;

            currentTarget = Owner.Transform.Center;
            UpdateCurrentTarget( );
        }

        private void TriggerComponent_Triggered (Entity entity) {
            if (entity.Domain == EntityDomain.Player) {
                target = entity;
                exploding = true;
                Owner.SetComponentInfo(ComponentData.SpriteAnimation, "explode", true, (AnimationCallback)ExplodeAnimationFinished);
            }
        }

        private void ExplodeAnimationFinished(bool completed) {
            if (completed) {
                if ((target.Transform.Center - Owner.Transform.Center).MagnitudeSqr( ) <= explosionRadiusSqr) {
                    target.SetComponentInfo(ComponentData.Damage, Owner, damage, DamageType.Magical);
                }
            }
            Owner.Destroy( );
        }

        public override void Update (DeltaTime dt) {
            if (exploding) return;
            progress += dt.TotalSeconds / totalTime;
            if (progress < 1) {
                Owner.Transform.Center = Mathf.Interpolate(lastTarget, currentTarget, progress);
            } else {
                Owner.Transform.Center = currentTarget;
                UpdateCurrentTarget( );
            }
        }

        private void UpdateCurrentTarget ( ) {
            List<Vector2> possibleTargetPoint = new List<Vector2>( );
            if (currentTarget.X > 0.5f && currentTarget.Y < Owner.World.Size.Height - 1 && Owner.World.HasCollider((int)currentTarget.X - 1, (int)currentTarget.Y + 1))
                possibleTargetPoint.Add(new Vector2(currentTarget.X - 1, currentTarget.Y + 1));
            if (currentTarget.X > 1 && Owner.World.HasCollider((int)currentTarget.X - 1, (int)currentTarget.Y))
                possibleTargetPoint.Add(new Vector2(currentTarget.X - 1, currentTarget.Y));
            if (currentTarget.X > 1 && currentTarget.Y > 1 && Owner.World.HasCollider((int)currentTarget.X - 1, (int)currentTarget.Y - 1))
                possibleTargetPoint.Add(new Vector2(currentTarget.X - 1, currentTarget.Y - 1));

            if (currentTarget.Y < Owner.World.Size.Height - 1 && Owner.World.HasCollider((int)currentTarget.X, (int)currentTarget.Y + 1))
                possibleTargetPoint.Add(new Vector2(currentTarget.X, currentTarget.Y + 1));
            if (currentTarget.Y > 1 && Owner.World.HasCollider((int)currentTarget.X, (int)currentTarget.Y - 1))
                possibleTargetPoint.Add(new Vector2(currentTarget.X, currentTarget.Y - 1));

            if (currentTarget.X < Owner.World.Size.Width - 1 && currentTarget.Y < Owner.World.Size.Height - 1 && Owner.World.HasCollider((int)currentTarget.X + 1, (int)currentTarget.Y + 1))
                possibleTargetPoint.Add(new Vector2(currentTarget.X + 1, currentTarget.Y + 1));
            if (currentTarget.X < Owner.World.Size.Width - 1 && Owner.World.HasCollider((int)currentTarget.X + 1, (int)currentTarget.Y))
                possibleTargetPoint.Add(new Vector2(currentTarget.X + 1, currentTarget.Y));
            if (currentTarget.X < Owner.World.Size.Width - 1 && currentTarget.Y > 1 && Owner.World.HasCollider((int)currentTarget.X + 1, (int)currentTarget.Y - 1))
                possibleTargetPoint.Add(new Vector2(currentTarget.X + 1, currentTarget.Y - 1));

            lastTarget = currentTarget;
            currentTarget = possibleTargetPoint[Mathi.Random(possibleTargetPoint.Count)];

            Vector2 diff = currentTarget - lastTarget;
            progress = 0f;
            totalTime = (diff).Magnitude( ) / speed;
            skeletComponent.Rotation = 180 - (float)Math.Atan2(diff.X, diff.Y) * 180f / Mathf.PI;
        }

        public new class Configuration : Component.Configuration {
            public float Damage = 3f;
            public float ExplosionRadiusSqr = 4.5f;
            public float Speed = 1f;

            public override Component Create (Entity owner) {
                return new DrillComponent(owner, Damage, ExplosionRadiusSqr, Speed);
            }
        }
    }
}
