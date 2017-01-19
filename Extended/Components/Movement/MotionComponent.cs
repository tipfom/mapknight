using System;
using System.Collections.Generic;
using mapKnight.Core;
using mapKnight.Extended.Components.Attributes;

namespace mapKnight.Extended.Components.Movement {

    [Instantiatable]
    public class MotionComponent : Component {
        public Vector2 AimedVelocity;
        public float ScaleX = 1f;

        public readonly bool HasMapCollider;
        public readonly bool HasPlatformCollider;
        public readonly float GravityInfluence;

        private Vector2 enforcedVelocity;
        private PlatformComponent platformStandingOn;

        public MotionComponent (Entity owner, bool mapCollider, bool platformCollider, float gravityinfluence) : base(owner) {
            HasMapCollider = mapCollider;
            HasPlatformCollider = platformCollider;
            GravityInfluence = gravityinfluence;
        }

        public bool IsAtWall { get; private set; }
        public bool IsOnGround { get; private set; }
        public bool IsOnPlatform { get; private set; }
        public Vector2 Velocity { get; private set; } = new Vector2( );

        public override void Collision (Entity collidingEntity) {
            if (HasPlatformCollider && collidingEntity.Domain == EntityDomain.Platform && !IsOnPlatform) {
                platformStandingOn = collidingEntity.GetComponent<PlatformComponent>( );
                if (Owner.Transform.BL.Y > collidingEntity.Transform.TR.Y - 0.3 && Velocity.Y <= platformStandingOn.Velocity.Y) {
                    Owner.Transform.Center = new Vector2(Owner.Transform.Center.X, collidingEntity.Transform.TR.Y + Owner.Transform.HalfSize.Y);
                    IsOnPlatform = true;
                } else {
                    platformStandingOn = null;
                }
            }
        }

        public override void Update (DeltaTime dt) {
            if(platformStandingOn == null)
                IsOnPlatform = false;

            if (IsOnPlatform)
                enforcedVelocity.X = platformStandingOn.Velocity.X;

            while (Owner.HasComponentInfo(ComponentData.Velocity)) {
                enforcedVelocity += (Vector2)Owner.GetComponentInfo(ComponentData.Velocity)[0];
            }
            while (Owner.HasComponentInfo(ComponentData.Acceleration)) {
                enforcedVelocity += (Vector2)Owner.GetComponentInfo(ComponentData.Acceleration)[0] * dt.TotalSeconds;
            }

            if (IsOnPlatform) {
                enforcedVelocity.Y = Math.Max(platformStandingOn.Velocity.Y, enforcedVelocity.Y);
                platformStandingOn = null;
            } else {
                enforcedVelocity += Owner.World.Gravity * GravityInfluence * dt.TotalSeconds;
            }

            Velocity = AimedVelocity + enforcedVelocity;

            Transform newTransform = new Transform(Owner.Transform.Center + Velocity * dt.TotalSeconds, Owner.Transform.Size);
            if (HasMapCollider) {
                IsAtWall = MoveHorizontally(Owner.Transform, newTransform);
                IsOnGround = MoveVertically(Owner.Transform, newTransform);

                if (IsOnGround) {
                    enforcedVelocity.X = 0;
                    enforcedVelocity.Y = 0;
                }
                if (IsAtWall) {
                    enforcedVelocity.X = 0;
                }
            }

            Owner.Transform = newTransform;

            if (AimedVelocity.X > 0)
                ScaleX = 1;
            else if (AimedVelocity.X < 0)
                ScaleX = -1;
            Owner.SetComponentInfo(ComponentData.ScaleX, ScaleX);
        }

        private bool MoveHorizontally (Transform oldTransform, Transform targetTransform) {
            // returns true if any collision happened and modifies the transform
            int ylimit = Mathi.Floor(oldTransform.TR.Y);
            if (ylimit == oldTransform.TR.Y)
                ylimit--;
            if (targetTransform.Center.X > oldTransform.Center.X) {
                // moves to the right
                int xlimit = Mathi.Floor(targetTransform.TR.X);
                for (int x = (int)oldTransform.TR.X; x <= xlimit; x++) {
                    for (int y = Mathi.Floor(oldTransform.BL.Y); y <= ylimit; y++) {
                        if (x >= Owner.World.Size.Width || Owner.World.HasCollider(x, y)) {
                            targetTransform.X = x - targetTransform.Size.X / 2;
                            return true;
                        }
                    }
                }
            } else if (targetTransform.Center.X < oldTransform.Center.X) {
                // moves to the left
                int xlimit = Mathi.Floor(targetTransform.BL.X);
                for (int x = (int)oldTransform.BL.X; x >= xlimit; x--) {
                    for (int y = Mathi.Floor(oldTransform.BL.Y); y <= ylimit; y++) {
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

        private bool MoveVertically (Transform oldTransform, Transform targetTransform) {
            if (oldTransform.Center.Y < targetTransform.Center.Y) {
                // goes up
                int ylimit = Mathi.Floor(targetTransform.TR.Y);
                int xlimit = ((targetTransform.TR.X == Mathi.Floor(targetTransform.TR.X)) ? (int)targetTransform.TR.X - 1 : (int)targetTransform.TR.X);
                for (int y = (int)oldTransform.TR.Y; y <= ylimit; y++) {
                    for (int x = (int)targetTransform.BL.X; x <= xlimit; x++) {
                        if (y >= Owner.World.Size.Height || Owner.World.HasCollider(x, y)) {
                            targetTransform.Y = y - targetTransform.Size.Y / 2f;
                            enforcedVelocity.Y = 0;
                            AimedVelocity.Y = 0;
                            return false; // you dont want to be stuck at the rooms ceiling
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
            public float GravityInfluence = 1f;
            public bool MapCollider = true;
            public bool PlatformCollider = false;

            public override Component Create (Entity owner) {
                return new MotionComponent(owner, MapCollider, PlatformCollider, GravityInfluence);
            }
        }
    }
}