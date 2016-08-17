using System;
using System.Collections.Generic;
using mapKnight.Core;

namespace mapKnight.Extended.Components {

    public class PlatformComponent : WaypointComponent {
        private int currentWaypoint = 0;

        public PlatformComponent (Entity owner, Vector2[ ] waypoints, float speed) : base(owner, waypoints, speed) {
        }

        protected override int GetNextWaypoint ( ) {
            if (currentWaypoint == waypointCount - 1)
                currentWaypoint = 0;
            else currentWaypoint++;
            return currentWaypoint;
        }

        protected override float GetPositionInterpolationPercent (float progressPercent) {
            return progressPercent;
        }

        public new class Configuration : Component.Configuration {
            public float Speed;
            public Vector2[ ] Waypoints;

            public override Component Create (Entity owner) {
                return new PlatformComponent(owner, Waypoints, Speed);
            }
        }
    }
}