using System;
using mapKnight.Core;

namespace mapKnight.Extended.Components.Movement {

    public abstract class WaypointComponent : Component {
        public Vector2 Velocity;
        private Vector2 currentMoveDistance;
        private int currentMoveDuration;
        private Vector2 currentWaypoint;
        private int timeTillNextMove;
        private Vector2 nextWaypoint;
        private float speed;
        private Vector2[ ] waypoints;

        public WaypointComponent (Entity owner, Vector2[ ] waypoints, float speed) : base(owner) {
            Array.Copy(waypoints, this.waypoints = new Vector2[waypoints.Length], waypoints.Length);
            for (int i = 0; i < waypoints.Length; i++)
                this.waypoints[i] += Owner.Transform.Center;
            this.speed = speed * speed;
        }

        protected int waypointCount { get { return waypoints.Length; } }

        public override void Prepare ( ) {
            nextWaypoint = waypoints[0];
            PrepareNextMove( );
        }

        public override void Update (DeltaTime dt) {
            timeTillNextMove -= (int)dt.Milliseconds;
            if (timeTillNextMove < 0)
                PrepareNextMove( );

            float progressPercent = 1f - (timeTillNextMove) / (float)currentMoveDuration;
            Vector2 nextPosition = currentWaypoint + currentMoveDistance * GetPositionInterpolationPercent(progressPercent);
            Owner.Transform.Center = nextPosition;
        }

        protected abstract int GetNextWaypoint ( );

        protected abstract float GetPositionInterpolationPercent (float progressPercent);

        private int GetCurrentMoveDuration ( ) {
            return (int)(currentMoveDistance.MagnitudeSqr( ) / speed * 1000);
        }

        private void PrepareNextMove ( ) {
            currentWaypoint = nextWaypoint;
            nextWaypoint = waypoints[GetNextWaypoint( )];
            currentMoveDistance = nextWaypoint - currentWaypoint;
            currentMoveDuration = GetCurrentMoveDuration( );
            timeTillNextMove = currentMoveDuration;
            Velocity = currentMoveDistance / (currentMoveDuration / 1000f);
        }
    }
}