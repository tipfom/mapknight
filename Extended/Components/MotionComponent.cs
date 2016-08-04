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

        public bool IsOnGround { get; private set; }
        public bool IsOnPlatform { get; private set; }
        public bool IsAtWall { get; private set; }
        public readonly bool HasMapCollider;
        public readonly bool HasPlatformCollider;
        private bool wasOnPlatform = false;

        public MotionComponent (Entity owner) : this(owner, DEFAULT_COLLIDER_MAP, DEFAULT_COLLIDER_PLATFORM) { }

        public MotionComponent (Entity owner, bool mapCollider, bool platformCollider) : base(owner) {
            Velocity = new Vector2( );
            HasMapCollider = mapCollider;
            HasPlatformCollider = platformCollider;
        }

        public override void Update (TimeSpan dt) {
            if (Math.Abs(dt.Milliseconds) > MAX_DELTA_TIME)
                return;

            wasOnPlatform = IsOnPlatform;
            IsOnPlatform = false;

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

            // update velocity
            this.Velocity += appliedAcceleration * (float)dt.TotalSeconds;
            foreach (Vector2 velocity in appliedVelocities)
                this.Velocity += velocity;

            Transform newTransform = new Transform(Owner.Transform.Center + Velocity * (float)dt.TotalSeconds, Owner.Transform.Bounds);
            if (HasMapCollider) {
                IsAtWall = moveHorizontally(Owner.Transform, newTransform);
                IsOnGround = moveVertically(Owner.Transform, newTransform);
            }

            Owner.Transform = newTransform;

            if (IsOnGround)
                this.Velocity.Y = 0;
        }

        private bool moveHorizontally (Transform oldTransform, Transform targetTransform) {
            // returns true if any collision happened and modifies the transform
            if (targetTransform.Center.X > oldTransform.Center.X) {
                // moves to the right
                int xlimit = Mathi.Floor(targetTransform.TR.X);
                int ylimit = ((oldTransform.TR.Y == Mathi.Floor(oldTransform.TR.Y)) ? (int)(oldTransform.TR.Y - 1) : (int)oldTransform.TR.Y);
                for (int x = (int)oldTransform.TR.X; x <= xlimit; x++) {
                    for (int y = (int)oldTransform.BL.Y; y <= ylimit; y++) {
                        if (x >= Owner.Owner.Size.Width || Owner.Owner.HasCollider(x, y)) {
                            targetTransform.TranslateX(x - targetTransform.Bounds.X / 2);
                            return true;
                        }
                    }
                }
            } else if (targetTransform.Center.X < oldTransform.Center.X) {
                // moves to the left
                int xlimit = Mathi.Floor(targetTransform.BL.X);
                int ylimit = ((oldTransform.TR.Y == Mathi.Floor(oldTransform.TR.Y)) ? (int)(oldTransform.TR.Y - 1) : (int)oldTransform.TR.Y);
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
                int ylimit = Mathi.Floor(targetTransform.TR.Y);
                int xlimit = ((targetTransform.TR.X == Mathi.Floor(targetTransform.TR.X)) ? (int)targetTransform.TR.X - 1 : (int)targetTransform.TR.X);
                for (int y = (int)oldTransform.TR.Y; y <= ylimit; y++) {
                    for (int x = (int)targetTransform.BL.X; x <= xlimit; x++) {
                        if (y >= Owner.Owner.Size.Height || Owner.Owner.HasCollider(x, y)) {
                            targetTransform.TranslateY(y - targetTransform.Bounds.Y / 2f);
                            return true;
                        }
                    }
                }
            } else if (oldTransform.Center.Y > targetTransform.Center.Y) {
                // goes down
                int ylimit = Mathi.Floor(targetTransform.BL.Y);
                int xlimit = ((targetTransform.TR.X == Mathi.Floor(targetTransform.TR.X)) ? (int)targetTransform.TR.X - 1 : (int)targetTransform.TR.X);
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

        public override void Collision (Entity collidingEntity) {
            if (HasPlatformCollider && collidingEntity.IsPlatform) {
                IsOnPlatform = true;
                PlatformComponent platform = collidingEntity.GetComponent<PlatformComponent>( );
                Velocity.Y = platform.Velocity.Y;
                Velocity.X += platform.Velocity.X;
            }
        }

        public new class Configuration : Component.Configuration {
            public override Component Create (Entity owner) {
                return new MotionComponent(owner);
            }
        }
    }
}