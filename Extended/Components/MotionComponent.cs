using System;
using System.Collections.Generic;
using System.Diagnostics;
using mapKnight.Core;
using mapKnight.Extended.Components.Attributes;

namespace mapKnight.Extended.Components {

    [Instantiatable]
    public class MotionComponent : Component {
        public readonly bool HasMapCollider;
        public readonly bool HasPlatformCollider;
        public float BouncyMultiplier;
        public Vector2 Velocity;
        private bool wasOnPlatform = false;

        public MotionComponent (Entity owner, bool mapCollider, bool platformCollider, float bouncymult) : base(owner) {
            Velocity = new Vector2( );
            HasMapCollider = mapCollider;
            HasPlatformCollider = platformCollider;
            BouncyMultiplier = bouncymult;
        }

        public bool IsAtWall { get; private set; }
        public bool IsOnGround { get; private set; }
        public bool IsOnPlatform { get; private set; }

        public override void Collision (Entity collidingEntity) {
            if (HasPlatformCollider && collidingEntity.Info.IsPlatform) {
                IsOnPlatform = true;
                PlatformComponent platform = collidingEntity.GetComponent<PlatformComponent>( );
                Velocity.Y = platform.Velocity.Y;
                Velocity.X += platform.Velocity.X;
            }
        }

        public override void Update (DeltaTime dt) {
            wasOnPlatform = IsOnPlatform;
            IsOnPlatform = false;

            Vector2 appliedAcceleration = new Vector2( ); // reset acceleration
            List<Vector2> appliedVelocities = new List<Vector2>( );

            while (Owner.HasComponentInfo(ComponentEnum.Motion)) {
                Tuple<ComponentData, Vector2> componentInfo = (Tuple<ComponentData, Vector2>)Owner.GetComponentInfo(ComponentEnum.Motion);
                switch (componentInfo.Item1) {
                    case ComponentData.Velocity:
                        appliedVelocities.Add(componentInfo.Item2);
                        break;

                    case ComponentData.Acceleration:
                        appliedAcceleration += componentInfo.Item2;
                        break;
                }
            }

            // update velocity
            this.Velocity += appliedAcceleration * .5f * (float)dt.TotalSeconds;
            foreach (Vector2 velocity in appliedVelocities)
                this.Velocity += velocity;

            Transform newTransform = new Transform(Owner.Transform.Center + Velocity * (float)dt.TotalSeconds, Owner.Transform.Size);
            if (HasMapCollider) {
                IsAtWall = moveHorizontally(Owner.Transform, newTransform);
                IsOnGround = moveVertically(Owner.Transform, newTransform);
            }
            this.Velocity += appliedAcceleration * .5f * dt.TotalSeconds;
            if (IsOnGround)
                this.Velocity.Y = -Velocity.Y * BouncyMultiplier;

            Owner.Transform = newTransform;
        }

        private bool moveHorizontally (Transform oldTransform, Transform targetTransform) {
            // returns true if any collision happened and modifies the transform
            if (targetTransform.Center.X > oldTransform.Center.X) {
                // moves to the right
                int xlimit = Mathi.Floor(targetTransform.TR.X);
                int ylimit = ((oldTransform.TR.Y == Mathi.Floor(oldTransform.TR.Y)) ? (int)(oldTransform.TR.Y - 1) : (int)oldTransform.TR.Y);
                for (int x = (int)oldTransform.TR.X; x <= xlimit; x++) {
                    for (int y = (int)oldTransform.BL.Y; y <= ylimit; y++) {
                        if (x >= Owner.World.Size.Width || Owner.World.HasCollider(x, y)) {
                            targetTransform.X = x - targetTransform.Size.X / 2;
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
                        if (x < 0 || Owner.World.HasCollider(x, y)) {
                            targetTransform.X = x + 1 + targetTransform.Size.X / 2;
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
                        if (y >= Owner.World.Size.Height || Owner.World.HasCollider(x, y)) {
                            targetTransform.Y = y - targetTransform.Size.Y / 2f;
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
                        if (y == -1 || Owner.World.HasCollider(x, y)) {
                            targetTransform.Y = y + 1 + targetTransform.Size.Y / 2f;
                            return true;
                        }
                    }
                }
            }
            // no collision or no movement
            return false;
        }

        public new class Configuration : Component.Configuration {
            public float BounceMultiplier = 0;
            public bool MapCollider = true;
            public bool PlatformCollider = false;

            public override Component Create (Entity owner) {
                return new MotionComponent(owner, MapCollider, PlatformCollider, BounceMultiplier);
            }
        }
    }
}