using System;
using mapKnight.Core;
using mapKnight.Core.World;

namespace mapKnight.Extended.Components.Movement {
    public abstract class WaypointComponent : Component {
        public Vector2 Velocity;
        public Action<Vector2> VelocityChanged;
        private Vector2 currentMoveDistance;
        private int currentMoveDuration;
        private Vector2 currentWaypoint;
        private int timeTillNextMove;
        private Vector2 nextWaypoint;
        private float speed;
        private Vector2[ ] waypoints;

        public WaypointComponent (Entity owner, float speed) : base(owner) {
            this.speed = speed;
        }

        protected int waypointCount { get { return waypoints.Length; } }

        protected void SetWaypoints (Vector2[ ] waypoints) {
            Array.Copy(waypoints, this.waypoints = new Vector2[waypoints.Length], waypoints.Length);
            for (int i = 0; i < waypoints.Length; i++)
                this.waypoints[i] += Owner.Transform.Center;
        }

        public override void Prepare ( ) {
            currentWaypoint = waypoints[0];
            nextWaypoint = waypoints.Length > 1 ? waypoints[1] : waypoints[0];
            PrepareNextMove( );
        }

        public override void Update (DeltaTime dt) {
            timeTillNextMove -= (int)dt.TotalMilliseconds;

            if (timeTillNextMove < 0)
                PrepareNextMove( );

            Owner.Transform.Center = Mathf.Interpolate(currentWaypoint, nextWaypoint, GetPositionInterpolationPercent(Mathf.Clamp01(1f - (timeTillNextMove) / (float)currentMoveDuration)));
        }

        protected abstract int GetNextWaypoint ( );

        protected abstract float GetPositionInterpolationPercent (float progressPercent);

        private int GetCurrentMoveDuration ( ) {
            return (int)(currentMoveDistance.Magnitude( ) / speed * 1000);
        }

        private void PrepareNextMove ( ) {
            currentWaypoint = nextWaypoint;
            nextWaypoint = waypoints[GetNextWaypoint( )];
            currentMoveDistance = nextWaypoint - currentWaypoint;
            currentMoveDuration = GetCurrentMoveDuration( );
            timeTillNextMove += currentMoveDuration;
            Velocity = currentMoveDistance / (currentMoveDuration / 1000f);
            VelocityChanged?.Invoke(Velocity);
        }
    }
}