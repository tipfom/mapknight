using System.Collections.Generic;
using mapKnight.Core;
using mapKnight.Core.World;
using mapKnight.Core.World.Serialization;

namespace mapKnight.Extended.Components.Movement {
    public class PlatformComponent : WaypointComponent {
        private int currentWaypoint = 1;
        private bool increasing = true;

        public PlatformComponent (Entity owner, float speed) : base(owner, speed) {
            owner.Domain = EntityDomain.Platform;
        }

        protected override int GetNextWaypoint ( ) {
            if (increasing) {
                currentWaypoint++;
                if (currentWaypoint == waypointCount - 1) {
                    increasing = false;
                }
            } else {
                currentWaypoint--;
                if (currentWaypoint == 0) {
                    increasing = true;
                }
            }
            return currentWaypoint;
        }

        public override void Load (Dictionary<DataID, object> data) {
            SetWaypoints((Vector2[ ])data[DataID.PLATFORM_Waypoint]);
        }

        protected override float GetPositionInterpolationPercent (float progressPercent) {
            return progressPercent;
        }

        public new class Configuration : Component.Configuration {
            public float Speed;

            public override Component Create (Entity owner) {
                return new PlatformComponent(owner, Speed);
            }
        }
    }
}