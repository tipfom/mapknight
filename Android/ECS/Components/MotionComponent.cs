using mapKnight.Basic;
using System;
using System.Collections.Generic;

namespace mapKnight.Android.ECS.Components {
    public class MotionComponent : Component {
        const int MAX_DELTA_TIME = 1000; // 1 sec
        const bool DEFAULT_COLLIDER_MAP = true;
        const bool DEFAULT_COLLIDER_PLATFORM = true;

        public Vector2 Velocity;

        public bool IsOnGround;
        public bool IsOnPlatform;
        public readonly bool HasMapCollider;
        public readonly bool HasPlatformCollider;

        public MotionComponent (Entity owner) : this (owner, DEFAULT_COLLIDER_MAP, DEFAULT_COLLIDER_PLATFORM) { }

        public MotionComponent (Entity owner, bool mapCollider, bool platformCollider) : base (owner) {
            Velocity = new Vector2 ();
            HasMapCollider = mapCollider;
            HasPlatformCollider = platformCollider;
        }

        public override void Update (float dt) {
            if (Math.Abs (dt) > MAX_DELTA_TIME)
                return;
            dt /= 1000f;

            Vector2 appliedAcceleration = new Vector2 (); // reset acceleration
            List<Vector2> appliedVelocities = new List<Vector2> ();

            while (Owner.HasComponentInfo (ComponentType.Motion)) {
                ComponentInfo ComponentInfo = Owner.GetComponentInfo (ComponentType.Motion);
                switch (ComponentInfo.Action) {
                    case ComponentAction.Velocity:
                        appliedVelocities.Add ((Vector2)ComponentInfo.Data);
                        break;
                    case ComponentAction.Acceleration:
                        appliedAcceleration += (Vector2)ComponentInfo.Data;
                        break;
                }
            }

            if (HasPlatformCollider) {
                bool wasOnPlatform = IsOnPlatform;
                IsOnPlatform = false;
                foreach (Entity platform in Owner.Owner.GetEntities (entity => entity.HasComponent (ComponentType.Platform) && entity.IsOnScreen)) {
                    if (Owner.Transform.Touches (platform.Transform)) {
                        if (!wasOnPlatform) // align with platform
                            Owner.Transform.Align (platform.Transform);

                        IsOnPlatform = true;
                        this.Velocity = (Vector2)platform.GetComponentState (ComponentType.Platform);
                        // player cant go below the platform so all velocities regarding to lower the y value of the player
                        // need to be removed
                        appliedVelocities.ForEach (velocity => velocity.Y = Math.Min (0, velocity.Y));
                        appliedAcceleration.Y = Math.Max (0, appliedAcceleration.Y);
                        break;
                    }
                }
            }
            
            // update velocity
            this.Velocity += appliedAcceleration * dt;
            appliedVelocities.ForEach (velocity => this.Velocity += velocity);
            
            Transform newTransform = new Transform (Owner.Transform.Center + Velocity * dt, Owner.Transform.Bounds);
            if (HasMapCollider) {
                moveHorizontally (Owner.Transform, newTransform);
                IsOnGround = moveVertically (Owner.Transform, newTransform);
            }

            Owner.Transform = newTransform;


            if (IsOnGround)
                this.Velocity = Vector2.Zero;

            this.State = Velocity;
        }

        private bool moveHorizontally (Transform oldTransform, Transform targetTransform) {
            // returns true if any collision happened and modifies the transform
            if (targetTransform.Center.X > oldTransform.Center.X) {
                if (targetTransform.TR.X >= Owner.Owner.Bounds.X) {
                    targetTransform.TranslateX (Owner.Owner.Bounds.X - targetTransform.Bounds.X / 2);
                    return true;
                }

                // moves to the right
                for (int x = (int)oldTransform.TR.X; x <= (int)(targetTransform.TR.X); x++) {
                    for (int y = (int)oldTransform.BL.Y; y <= ((oldTransform.TR.Y == Math.Floor (oldTransform.TR.Y)) ? (int)(oldTransform.TR.Y - 1) : (int)oldTransform.TR.Y); y++) {
                        if (x > Owner.Owner.Bounds.X || Owner.Owner.HasCollider (x, y)) {
                            targetTransform.TranslateX (x - targetTransform.Bounds.X / 2);
                            return true;
                        }
                    }
                }
            } else if (targetTransform.Center.X < oldTransform.Center.X) {
                // moves to the left
                if (targetTransform.BL.X <= 0) {
                    targetTransform.TranslateX (targetTransform.Bounds.X / 2);
                    return true;
                }

                for (int x = (int)oldTransform.BL.X; x >= (int)(targetTransform.BL.X); x--) {
                    for (int y = (int)oldTransform.BL.Y; y <= ((oldTransform.TR.Y == Math.Floor (oldTransform.TR.Y)) ? (int)(oldTransform.TR.Y - 1) : (int)oldTransform.TR.Y); y++) {
                        if (x < 0 || Owner.Owner.HasCollider (x, y)) {
                            targetTransform.TranslateX (x + 1 + targetTransform.Bounds.X / 2);
                            return true;
                        }
                    }
                }
            } else {
                // no movement happens
                return false;
            }

            // no collision happened
            return false;
        }

        private bool moveVertically (Transform oldTransform, Transform targetTransform) {
            if (oldTransform.Center.Y < targetTransform.Center.Y) {
                // goes up
                for (int y = (int)oldTransform.TR.Y; y <= (int)targetTransform.TR.Y; y++) {
                    for (int x = (int)targetTransform.BL.X; x <= ((targetTransform.TR.X == Math.Floor (targetTransform.TR.X)) ? (int)targetTransform.TR.X - 1 : (int)targetTransform.TR.X); x++) {
                        if (y >= Owner.Owner.Bounds.Y || Owner.Owner.HasCollider (x, y)) {
                            targetTransform.TranslateY (y - targetTransform.Bounds.Y / 2f);
                            return true;
                        }
                    }
                }
            } else if (oldTransform.Center.Y > targetTransform.Center.Y) {
                // goes down
                for (int y = (int)oldTransform.BL.Y; y >= (int)targetTransform.BL.Y; y--) {
                    for (int x = (int)targetTransform.BL.X; x <= ((targetTransform.TR.X == Math.Floor (targetTransform.TR.X)) ? (int)targetTransform.TR.X - 1 : (int)targetTransform.TR.X); x++) {
                        if (y == -1 || Owner.Owner.HasCollider (x, y)) {
                            targetTransform.TranslateY (y + 1 + targetTransform.Bounds.Y / 2f);
                            return true;
                        }
                    }
                }
            } else {
                // no movement on y
                return false;
            }

            // no collision
            return false;
        }
    }
}