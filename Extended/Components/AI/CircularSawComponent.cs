using System;
using mapKnight.Core;
using mapKnight.Core.World;
using mapKnight.Core.World.Components;
using mapKnight.Extended.Components.Movement;

namespace mapKnight.Extended.Components.AI {
    public class CircularSawComponent : WaypointComponent {
        private int currentwaypoint;
        private int direction;
        private float sawRadius;

        public CircularSawComponent (Entity owner, float speed, float sawradius) : base(owner, speed) {
            owner.Domain = EntityDomain.Obstacle;

            sawRadius = sawradius;
        }

        public override void Collision (Entity collidingEntity) {
            if (collidingEntity.Domain == EntityDomain.Player) {
                Vector2 closestPointToPlayer = Owner.Transform.Center + (Owner.Transform.Center - collidingEntity.Transform.Center) * sawRadius;
                if (Owner.Transform.Intersects(closestPointToPlayer)) {
                    collidingEntity.SetComponentInfo(ComponentData.Damage, float.PositiveInfinity);
                }
            }
        }

        public override void Prepare ( ) {
            direction = 1;
            currentwaypoint = 0;
            base.Prepare( );
        }

        protected override int GetNextWaypoint ( ) {
            if (currentwaypoint <= 0) direction = 1;
            else if (currentwaypoint >= waypointCount - 1) direction = -1;

            currentwaypoint += direction;
            return currentwaypoint;
        }

        protected override float GetPositionInterpolationPercent (float progressPercent) {
            if (progressPercent > 0.5)
                return 1 + 0.5f * (float)Math.Pow(2 * progressPercent - 2, 3);
            else
                return 4f * (float)Math.Pow(progressPercent, 3);
        }

        public new class Configuration : Component.Configuration {
            public float Speed;

            public override Component Create (Entity owner) {
                return new CircularSawComponent(owner, Speed, owner.Transform.HalfSize.X);
            }
        }
    }
}