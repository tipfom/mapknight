using System;
using mapKnight.Core;
using mapKnight.Core.World;
using mapKnight.Extended.Components.AI.Basics;
using mapKnight.Extended.Components.Stats;
using mapKnight.Core.World.Components;
using mapKnight.Core.World.Serialization;
using System.Collections.Generic;

namespace mapKnight.Extended.Components.AI {
    public class SlimeComponent : WallWalkerComponent {
        private Rainbowdizer rainbowdizer = new Rainbowdizer(500);
        private Entity.Configuration slimeConfig;
        private int splitCount;
        private float sizeMultiplier;
        private float rageRange;
        private int nextRageStage;
        private int rageTime;
        private bool isEscaping;
        private Vector2 rageSpeedMultiplier;

        public SlimeComponent (Entity owner, Entity.Configuration slimeConfig, int splitCount, float sizeMultiplier, float rageRange, float rageSpeedMultiplier) : base(owner) {
            owner.Domain = EntityDomain.Enemy;

            this.slimeConfig = slimeConfig;
            this.splitCount = splitCount;
            this.sizeMultiplier = sizeMultiplier;
            this.rageRange = rageRange;
            this.rageSpeedMultiplier = Vector2.One * rageSpeedMultiplier;
            this.rainbowdizer.Normal( );
        }

        public override void Load(Dictionary<DataID, object> data) {
            NextMoveDir = (Direction)((sbyte)data[DataID.SLIME_InitialMoveDirection]);
            NextWallDir = (Direction)((sbyte)data[DataID.SLIME_InitialWallDirection]);
            base.Prepare( );
        }

        public override void Prepare ( ) {
            rageTime = (int)(rageRange / Owner.GetComponent<SpeedComponent>( ).Speed.X * 1000 / rageSpeedMultiplier.X);
            base.Prepare( );
        }

        public override void Collision (Entity collidingEntity) {
            if (collidingEntity.Domain == EntityDomain.Player) {
                isEscaping = true;
                nextRageStage = Environment.TickCount + rageTime;
                if (!rainbowdizer.Raging) {
                    rainbowdizer.Rage( );
                    Owner.SetComponentInfo(ComponentData.SpriteAnimation, "attack", true);
                }
            }
        }

        public override void Destroy ( ) {
            float ratio = Owner.Transform.Width / slimeConfig.Transform.Width;
            double max = Mathf.Pow(sizeMultiplier, splitCount - 1);
            if (ratio > max) {
                Vector2 originalSize = new Vector2(slimeConfig.Transform.Size);
                slimeConfig.Transform.Size = Owner.Transform.Size * sizeMultiplier;
                SummonDescendants( );
                slimeConfig.Transform.Size = originalSize;
            }
            base.Destroy( );
        }

        private void SummonDescendants ( ) {
            // get the vector that is on the ground
            Vector2 groundVec = Owner.Transform.Center;
            switch (CurrentWallDir) {
                case Direction.Up:
                    groundVec.Y = Owner.Transform.TR.Y - slimeConfig.Transform.HalfSize.Y;
                    break;
                case Direction.Down:
                    groundVec.Y = Owner.Transform.BL.Y + slimeConfig.Transform.HalfSize.Y;
                    break;
                case Direction.Left:
                    groundVec.X = Owner.Transform.BL.X + slimeConfig.Transform.HalfSize.X;
                    break;
                case Direction.Right:
                    groundVec.X = Owner.Transform.TR.X - slimeConfig.Transform.HalfSize.X;
                    break;
            }

            switch (CurrentMoveDir) {
                case Direction.Up:
                    Append(slimeConfig.Create(groundVec, Owner.World, false));
                    Leave(slimeConfig.Create(groundVec, Owner.World, false));
                    break;
                case Direction.Down:
                    Leave(slimeConfig.Create(groundVec, Owner.World, false));
                    Append(slimeConfig.Create(groundVec, Owner.World, false));
                    break;
                case Direction.Left:
                    Append(slimeConfig.Create(groundVec, Owner.World, false));
                    Leave(slimeConfig.Create(groundVec, Owner.World, false));
                    break;
                case Direction.Right:
                    Leave(slimeConfig.Create(groundVec, Owner.World, false));
                    Append(slimeConfig.Create(groundVec, Owner.World, false));
                    break;
            }
        }

        public void Turn ( ) {
            NextMoveDir = 1 - CurrentMoveDir;
            NextWallDir = CurrentWallDir;
            FindWaypoint( );
        }

        private void Append (Entity e) {
            SlimeComponent slimeComponent = e.GetComponent<SlimeComponent>( );
            slimeComponent.NextMoveDir = CurrentMoveDir;
            slimeComponent.NextWallDir = CurrentWallDir;
            slimeComponent.FindWaypoint( );
        }

        private void Leave (Entity e) {
            SlimeComponent slimeComponent = e.GetComponent<SlimeComponent>( );
            slimeComponent.NextMoveDir = 1 - CurrentMoveDir;
            slimeComponent.NextWallDir = CurrentWallDir;
            slimeComponent.FindWaypoint( );
        }

        public override void Update (DeltaTime dt) {
            if (Environment.TickCount > nextRageStage) {
                if (isEscaping) {
                    Turn( );
                    nextRageStage += 5 * rageTime / 3;
                    isEscaping = false;
                } else {
                    rainbowdizer.Normal( );
                    nextRageStage = int.MaxValue;
                    Owner.SetComponentInfo(ComponentData.SpriteAnimation, "wobble", true);
                }
            }
            if (rainbowdizer.Raging) {
                Owner.SetComponentInfo(ComponentData.SlowDown, rageSpeedMultiplier);
            }

            rainbowdizer.Update(dt);
            Owner.SetComponentInfo(ComponentData.Color, rainbowdizer.Color);
            base.Update(dt);
        }

        public new class Configuration : Component.Configuration {
            public Entity.Configuration SlimeConfig;
            public int SplitCount;
            public float SizeMultiplier;
            public float RageRange;
            public float RageSpeedMultiplier;

            public override Component Create (Entity owner) {
                return new SlimeComponent(owner, SlimeConfig, SplitCount, SizeMultiplier, RageRange, RageSpeedMultiplier);
            }
        }

        private class Rainbowdizer {
            private const float RAGE_RED_THRESHOLD = 50f;

            public Color Color = new Color(0, 0, 0, 255);
            public int CycleTime;
            private bool increasing = false;
            public bool Raging = false;
            private float r, g, b;

            public Rainbowdizer (int cycleTime) {
                this.CycleTime = cycleTime;
            }

            public void Update (DeltaTime dt) {
                float d = 255 * dt.TotalMilliseconds / CycleTime;

                if (Raging) {
                    if (increasing) {
                        r += d;
                        if (r >= 255) {
                            r = 255 - r % 255;
                            increasing = false;
                        }
                    } else {
                        r -= d;
                        if (r <= RAGE_RED_THRESHOLD) {
                            r = RAGE_RED_THRESHOLD + RAGE_RED_THRESHOLD - r;
                            increasing = true;
                        }
                    }
                } else {
                    if (increasing) {
                        g += d;
                        b -= d;
                        if (g >= 255) {
                            b = g % 255;
                            g = 255 - b;
                            increasing = false;
                        }
                    } else {
                        g -= d;
                        b += d;
                        if (b >= 255) {
                            g = b % 255;
                            b = 255 - g;
                            increasing = true;
                        }
                    }
                }

                Color.R = (byte)r;
                Color.G = (byte)g;
                Color.B = (byte)b;
            }

            public void Rage ( ) {
                if (Raging) return;
                Raging = true;
                increasing = true;
                r = RAGE_RED_THRESHOLD;
                b = 10;
                g = 10;
            }

            public void Normal ( ) {
                if (!Raging) return;
                Raging = false;
                increasing = true;
                r = 20;
                b = 255;
                g = 0;
            }
        }
    }
}
