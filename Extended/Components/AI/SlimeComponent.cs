using System;
using mapKnight.Core;
using mapKnight.Extended.Components.AI.Basics;

namespace mapKnight.Extended.Components.AI {
    public class SlimeComponent : WallWalkerComponent {
        private Rainbowdizer rainbowdizer = new Rainbowdizer(100);
        private Entity.Configuration slimeConfig;
        private int splitCount;
        private float sizeMultiplier;

        public SlimeComponent (Entity owner, Entity.Configuration slimeConfig, int splitCount, float sizeMultiplier) : base(owner) {
            this.slimeConfig = slimeConfig;
            this.splitCount = splitCount;
            this.sizeMultiplier = sizeMultiplier;
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
                    Append(slimeConfig.Create(groundVec + new Vector2(0, slimeConfig.Transform.HalfSize.Y), Owner.World));
                    Turn(slimeConfig.Create(groundVec + new Vector2(0, -slimeConfig.Transform.HalfSize.Y), Owner.World));
                    break;
                case Direction.Down:
                    Turn(slimeConfig.Create(groundVec + new Vector2(0, slimeConfig.Transform.HalfSize.Y), Owner.World));
                    Append(slimeConfig.Create(groundVec + new Vector2(0, -slimeConfig.Transform.HalfSize.Y), Owner.World));
                    break;
                case Direction.Left:
                    Append(slimeConfig.Create(groundVec + new Vector2(-Owner.Transform.HalfSize.X, 0), Owner.World));
                    Turn(slimeConfig.Create(groundVec + new Vector2(Owner.Transform.HalfSize.X, 0), Owner.World));
                    break;
                case Direction.Right:
                    Turn(slimeConfig.Create(groundVec + new Vector2(-Owner.Transform.HalfSize.X, 0), Owner.World));
                    Append(slimeConfig.Create(groundVec + new Vector2(Owner.Transform.HalfSize.X, 0), Owner.World));
                    break;
            }
        }

        private void Turn(Entity e) {
            SlimeComponent slimeComponent = e.GetComponent<SlimeComponent>( );
            slimeComponent.NextMoveDir = 1 - CurrentMoveDir;
            slimeComponent.NextWallDir = CurrentWallDir;
            slimeComponent.FindWaypoint( );
        }

        private void Append(Entity e) {
            SlimeComponent slimeComponent = e.GetComponent<SlimeComponent>( );
            slimeComponent.NextMoveDir = CurrentMoveDir;
            slimeComponent.NextWallDir = CurrentWallDir;
            slimeComponent.FindWaypoint( );
        }

        public override void Update (DeltaTime dt) {
            rainbowdizer.Update(dt);
            Owner.SetComponentInfo(ComponentData.Color, rainbowdizer.Color);
            base.Update(dt);
        }

        public new class Configuration : Component.Configuration {
            public Entity.Configuration SlimeConfig;
            public int SplitCount;
            public float SizeMultiplier;

            public override Component Create (Entity owner) {
                return new SlimeComponent(owner, SlimeConfig, SplitCount, SizeMultiplier);
            }
        }

        private class Rainbowdizer {
            public Color Color = new Color(0, 0, 0, 255);
            public int CycleTime;
            private byte mode = 0;
            private float r = 255, g, b;


            public Rainbowdizer (int cycleTime) {
                this.CycleTime = cycleTime;
            }

            public void Update (DeltaTime dt) {
                float d = 255 * dt.TotalMilliseconds / CycleTime;
                switch (mode) {
                    case 0:
                        r -= d;
                        b += d;
                        if (b > 255) {
                            r = 0;
                            g = b % 255;
                            b = 255;
                            mode++;
                        }
                        break;
                    case 1:
                        b -= d;
                        g += d;
                        if (g > 255) {
                            b = 0;
                            r = g % 255;
                            g = 255;
                            mode++;
                        }
                        break;
                    case 2:
                        r += d;
                        g -= d;
                        if (r > 255) {
                            g = 0;
                            b = r % 255;
                            r = 255;
                            mode = 0;
                        }
                        break;
                }
                Color.R = (byte)r;
                Color.G = (byte)g;
                Color.B = (byte)b;
            }
        }
    }
}
