using System;
using mapKnight.Core;
using mapKnight.Extended.Components.Attributes;
using mapKnight.Extended.Components.Stats;

namespace mapKnight.Extended.Components.AI {

    [ComponentRequirement(typeof(DamageComponent))]
    public class CircularSawComponent : WaypointComponent {
        private int currentwaypoint;
        private DamageComponent damageComponent;
        private int direction;
        private float sawRadius;

        public CircularSawComponent (Entity owner, Vector2[ ] waypoints, float speed, float sawradius) : base(owner, waypoints, speed) {
            sawRadius = sawradius;
        }

        public override void Collision (Entity collidingEntity) {
            if (collidingEntity.Info.IsPlayer) {
                Vector2 closestPointToPlayer = Owner.Transform.Center + (Owner.Transform.Center - collidingEntity.Transform.Center) * sawRadius;
                if (Owner.Transform.Intersects(closestPointToPlayer)) {
                    collidingEntity.SetComponentInfo(ComponentEnum.Stats_Health, damageComponent.OnTouch);
                }
            }
        }

        public override void Prepare ( ) {
            direction = 1;
            currentwaypoint = 0;
            damageComponent = Owner.GetComponent<DamageComponent>( );
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
            public Vector2[ ] Waypoints;

            public override Component Create (Entity owner) {
                return new CircularSawComponent(owner, Waypoints, Speed, owner.Transform.HalfSize.X);
            }
        }
    }
}