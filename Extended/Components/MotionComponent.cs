using System;
using System.Collections.Generic;
using System.Diagnostics;
using mapKnight.Core;

namespace mapKnight.Extended.Components {
    public class MotionComponent : Component {
        const int MAX_DELTA_TIME = 100; // 0.1 sec
        const bool DEFAULT_COLLIDER_MAP = true;
        const bool DEFAULT_COLLIDER_PLATFORM = true;

        public Vector2 Velocity;

        public bool IsOnGround;
        public bool IsOnPlatform;
        public readonly bool HasMapCollider;
        public readonly bool HasPlatformCollider;

        public MotionComponent (Entity owner) : this(owner, DEFAULT_COLLIDER_MAP, DEFAULT_COLLIDER_PLATFORM) { }

        public MotionComponent (Entity owner, bool mapCollider, bool platformCollider) : base(owner) {
            Velocity = new Vector2( );
            HasMapCollider = mapCollider;
            HasPlatformCollider = platformCollider;
        }

        public override void Update (TimeSpan dt) {
            if (Math.Abs(dt.Milliseconds) > MAX_DELTA_TIME)
                return;

            Vector2 appliedAcceleration = new Vector2( ); // reset acceleration
            List<Vector2> appliedVelocities = new List<Vector2>( );

            while (Owner.HasComponentInfo(ComponentEnum.Motion)) {
                ComponentInfo componentInfo = Owner.GetComponentInfo(ComponentEnum.Motion);
                switch (componentInfo.Action) {
                    case ComponentData.Velocity:
                        appliedVelocities.Add((Vector2)componentInfo.Data);
                        break;
                    case ComponentData.Acceleration:
                        appliedAcceleration += (Vector2)componentInfo.Data;
                        break;
                }
            }

            if (HasPlatformCollider) {
                bool wasOnPlatform = IsOnPlatform;
                IsOnPlatform = false;
                foreach (Entity platform in Entity.Platforms) {
                    if (Owner.Transform.Touches(platform.Transform)) {
                        if (!wasOnPlatform) // align with platform
                            Owner.Transform.Align(platform.Transform);

                        IsOnPlatform = true;
                        this.Velocity = (Vector2)platform.GetComponentState(ComponentEnum.Platform);
                        // player cant go below the platform so all velocities regarding to lower the y value of the player
                        // need to be removed
                        for (int i = 0; i < appliedVelocities.Count; i++)
                            appliedVelocities[i] = new Vector2(appliedVelocities[i].X, Math.Min(0, appliedVelocities[i].Y));
                        appliedAcceleration.Y = Math.Max(0, appliedAcceleration.Y);
                        break;
                    }
                }
            }

            // update velocity
            this.Velocity += appliedAcceleration * (float)dt.TotalSeconds;
            foreach (Vector2 velocity in appliedVelocities)
                this.Velocity += velocity;

            Transform newTransform = new Transform(Owner.Transform.Center + Velocity * (float)dt.TotalSeconds, Owner.Transform.Bounds);
            if (HasMapCollider) {
                moveHorizontally(Owner.Transform, newTransform);
                IsOnGround = moveVertically(Owner.Transform, newTransform);
            }

            Owner.Transform = newTransform;

            if (IsOnGround)
                this.Velocity.Y = 0;

            this.State = Velocity;
        }

        private bool moveHorizontally (Transform oldTransform, Transform targetTransform) {
            // returns true if any collision happened and modifies the transform
            if (targetTransform.Center.X > oldTransform.Center.X) {
                // moves to the right
                int xlimit = (int)Math.Floor(targetTransform.TR.X);
                int ylimit = ((oldTransform.TR.Y == Math.Floor(oldTransform.TR.Y)) ? (int)(oldTransform.TR.Y - 1) : (int)oldTransform.TR.Y);
                for (int x = (int)oldTransform.TR.X; x <= xlimit; x++) {
                    for (int y = (int)oldTransform.BL.Y; y <= ylimit; y++) {
                        if (x >= Owner.Owner.Bounds.X || Owner.Owner.HasCollider(x, y)) {
                            targetTransform.TranslateX(x - targetTransform.Bounds.X / 2);
                            return true;
                        }
                    }
                }
            } else if (targetTransform.Center.X < oldTransform.Center.X) {
                // moves to the left
                int xlimit = (int)Math.Floor(targetTransform.BL.X);
                int ylimit = ((oldTransform.TR.Y == Math.Floor(oldTransform.TR.Y)) ? (int)(oldTransform.TR.Y - 1) : (int)oldTransform.TR.Y);
                for (int x = (int)oldTransform.BL.X; x >= xlimit; x--) {
                    for (int y = (int)oldTransform.BL.Y; y <= ylimit; y++) {
                        if (x < 0 || Owner.Owner.HasCollider(x, y)) {
                            targetTransform.TranslateX(x + 1 + targetTransform.Bounds.X / 2);
                            return true;
                        }
                    }
                }
            }
            // no collision or no movement happened
            return false;
        }

        private bool moveVertically (Transform oldTransform, Transform targetTransform) {
            if (oldTransform.Center.Y < targetTransform.Center.Y) {
                // goes up
                int ylimit = (int)Math.Floor(targetTransform.TR.Y);
                int xlimit = ((targetTransform.TR.X == Math.Floor(targetTransform.TR.X)) ? (int)targetTransform.TR.X - 1 : (int)targetTransform.TR.X);
                for (int y = (int)oldTransform.TR.Y; y <= ylimit; y++) {
                    for (int x = (int)targetTransform.BL.X; x <= xlimit; x++) {
                        if (y >= Owner.Owner.Bounds.Y || Owner.Owner.HasCollider(x, y)) {
                            targetTransform.TranslateY(y - targetTransform.Bounds.Y / 2f);
                            return true;
                        }
                    }
                }
            } else if (oldTransform.Center.Y > targetTransform.Center.Y) {
                // goes down
                int ylimit = (int)Math.Floor(targetTransform.BL.Y);
                int xlimit = ((targetTransform.TR.X == Math.Floor(targetTransform.TR.X)) ? (int)targetTransform.TR.X - 1 : (int)targetTransform.TR.X);
                for (int y = (int)oldTransform.BL.Y; y >= ylimit; y--) {
                    for (int x = (int)targetTransform.BL.X; x <= xlimit; x++) {
                        if (y == -1 || Owner.Owner.HasCollider(x, y)) {
                            targetTransform.TranslateY(y + 1 + targetTransform.Bounds.Y / 2f);
                            return true;
                        }
                    }
                }
            }
            // no collision or no movement
            return false;
        }

        public new class Configuration : Component.Configuration {
            public override Component Create (Entity owner) {
                return new MotionComponent(owner);
            }
        }
    }
}