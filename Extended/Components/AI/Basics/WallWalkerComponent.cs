using mapKnight.Core;
using mapKnight.Core.World;
using mapKnight.Core.World.Components;
using mapKnight.Extended.Components.Graphics;
using mapKnight.Extended.Components.Stats;

namespace mapKnight.Extended.Components.AI.Basics {
    [UpdateAfter(typeof(SpeedComponent))]
    [UpdateBefore(typeof(SkeletComponent))]
    public class WallWalkerComponent : Component {
        protected Direction CurrentMoveDir;
        protected Direction CurrentWallDir;
        protected Direction NextMoveDir = Direction.Right;
        protected Direction NextWallDir = Direction.Down;
        private SpeedComponent speedComponent;
        private SkeletComponent skeletComponent;
        private float targetLoc;

        public WallWalkerComponent (Entity owner) : base(owner) {
        }

        protected enum Direction : sbyte {
            Up = 0,
            Down = 1,
            Left = 2,
            Right = -1
        }

        public override void Prepare ( ) {
            speedComponent = Owner.GetComponent<SpeedComponent>( );
            skeletComponent = Owner.GetComponent<SkeletComponent>( );
            FindWaypoint( );
        }

        public override void Update (DeltaTime dt) {
            switch (CurrentMoveDir) {
                case Direction.Left:
                    Owner.Transform.X = Owner.Transform.Center.X - speedComponent.Speed.X * dt.TotalSeconds;
                    if (Owner.Transform.Center.X < targetLoc) {
                        Owner.Transform.X = targetLoc;
                        FindWaypoint( );
                    }
                    break;

                case Direction.Right:
                    Owner.Transform.X = Owner.Transform.Center.X + speedComponent.Speed.X * dt.TotalSeconds;
                    if (Owner.Transform.Center.X > targetLoc) {
                        Owner.Transform.X = targetLoc;
                        FindWaypoint( );
                    }
                    break;

                case Direction.Up:
                    Owner.Transform.Y = Owner.Transform.Center.Y + speedComponent.Speed.Y * dt.TotalSeconds;
                    if (Owner.Transform.Center.Y > targetLoc) {
                        Owner.Transform.Y = targetLoc;
                        FindWaypoint( );
                    }
                    break;

                case Direction.Down:
                    Owner.Transform.Y = Owner.Transform.Center.Y - speedComponent.Speed.Y * dt.TotalSeconds;
                    if (Owner.Transform.Center.Y < targetLoc) {
                        Owner.Transform.Y = targetLoc;
                        FindWaypoint( );
                    }
                    break;
            }
        }

        protected void FindWaypoint ( ) {
            CurrentMoveDir = NextMoveDir;
            CurrentWallDir = NextWallDir;
            switch (CurrentMoveDir) {
                case Direction.Left:
                    targetLoc = Owner.Transform.HalfSize.X;
                    int ywalkingon = CurrentWallDir == Direction.Down ? Mathi.Floor(Owner.Transform.BL.Y) - 1 : Mathi.Floor(Owner.Transform.TR.Y);
                    int ywalkingagainst = CurrentWallDir == Direction.Down ? ywalkingon + 1 : ywalkingon - 1;
                    int xstart = Mathi.Floor(Owner.Transform.BL.X) - 1;
                    if (!Owner.World.HasCollider(xstart, ywalkingon)) xstart++;
                    for (int x = xstart; x > -1; x--) {
                        if (Owner.World.HasCollider(x, ywalkingagainst)) {
                            targetLoc = x + 1 + Owner.Transform.HalfSize.X;
                            NextMoveDir = 1 - CurrentWallDir;
                            NextWallDir = Direction.Left;
                            break;
                        } else if (!Owner.World.HasCollider(x, ywalkingon)) {
                            targetLoc = x + 1 - Owner.Transform.HalfSize.X;
                            NextMoveDir = CurrentWallDir;
                            NextWallDir = Direction.Right;
                            break;
                        }
                    }
                    break;

                case Direction.Right:
                    targetLoc = Owner.World.Size.Width - Owner.Transform.HalfSize.X;
                    ywalkingon = CurrentWallDir == Direction.Down ? Mathi.Floor(Owner.Transform.BL.Y) - 1 : Mathi.Ceil(Owner.Transform.TR.Y);
                    ywalkingagainst = CurrentWallDir == Direction.Down ? ywalkingon + 1 : ywalkingon - 1;
                    xstart = Mathi.Floor(Owner.Transform.TR.X);
                    if (!Owner.World.HasCollider(xstart, ywalkingon)) xstart--;
                    for (int x = xstart; x < Owner.World.Size.Width; x++) {
                        if (Owner.World.HasCollider(x, ywalkingagainst)) {
                            targetLoc = x - Owner.Transform.HalfSize.X;
                            NextMoveDir = 1 - CurrentWallDir;
                            NextWallDir = Direction.Right;
                            break;
                        } else if (!Owner.World.HasCollider(x, ywalkingon)) {
                            targetLoc = x + Owner.Transform.HalfSize.X;
                            NextMoveDir = CurrentWallDir;
                            NextWallDir = Direction.Left;
                            break;
                        }
                    }
                    break;

                case Direction.Up:
                    targetLoc = Owner.World.Size.Height - Owner.Transform.HalfSize.Y;
                    int xwalkingon = CurrentWallDir == Direction.Left ? Mathi.Floor(Owner.Transform.BL.X + 0.00001f) - 1 : Mathi.Floor(Owner.Transform.TR.X);
                    int xwalkingagainst = CurrentWallDir == Direction.Left ? xwalkingon + 1 : xwalkingon - 1;
                    int ystart = Mathi.Floor(Owner.Transform.TR.Y);
                    if (!Owner.World.HasCollider(xwalkingon, ystart)) ystart--;
                    for (int y = ystart; y < Owner.World.Size.Height; y++) {
                        if (Owner.World.HasCollider(xwalkingagainst, y)) {
                            targetLoc = y - Owner.Transform.HalfSize.Y;
                            NextMoveDir = 1 - CurrentWallDir;
                            NextWallDir = Direction.Up;
                            break;
                        } else if (!Owner.World.HasCollider(xwalkingon, y)) {
                            targetLoc = y + Owner.Transform.HalfSize.Y;
                            NextMoveDir = CurrentWallDir;
                            NextWallDir = Direction.Down;
                            break;
                        }
                    }
                    break;

                case Direction.Down:
                    targetLoc = Owner.Transform.HalfSize.Y;
                    xwalkingon = CurrentWallDir == Direction.Left ? Mathi.Floor(Owner.Transform.BL.X) - 1 : Mathi.Floor(Owner.Transform.TR.X);
                    xwalkingagainst = CurrentWallDir == Direction.Left ? xwalkingon + 1 : xwalkingon - 1;
                    ystart = Mathi.Floor(Owner.Transform.BL.Y) - 1;
                    if (!Owner.World.HasCollider(xwalkingon, ystart)) ystart++;
                    for (int y = ystart; y > -1; y--) {
                        if (Owner.World.HasCollider(xwalkingagainst, y)) {
                            targetLoc = y + 1 + Owner.Transform.HalfSize.Y;
                            NextMoveDir = 1 - CurrentWallDir;
                            NextWallDir = Direction.Down;
                            break;
                        } else if (!Owner.World.HasCollider(xwalkingon, y)) {
                            targetLoc = y + 1 - Owner.Transform.HalfSize.Y;
                            NextMoveDir = CurrentWallDir;
                            NextWallDir = Direction.Up;
                            break;
                        }
                    }
                    break;
            }

            AdjustGraphics( );
        }

        private void AdjustGraphics ( ) {
            switch (CurrentMoveDir) {
                case Direction.Up:
                    skeletComponent.Rotation = 90f;
                    skeletComponent.Mirrored = CurrentWallDir == Direction.Left;
                    break;

                case Direction.Down:
                    skeletComponent.Rotation = 270f;
                    skeletComponent.Mirrored = CurrentWallDir == Direction.Right;
                    break;

                case Direction.Left:
                    if (CurrentWallDir == Direction.Down) {
                        skeletComponent.Rotation = 0f;
                        skeletComponent.Mirrored = true;
                    } else {
                        skeletComponent.Rotation = 180f;
                        skeletComponent.Mirrored = false;
                    }
                    break;

                case Direction.Right:
                    if (CurrentWallDir == Direction.Down) {
                        skeletComponent.Rotation = 0f;
                        skeletComponent.Mirrored = false;
                    } else {
                        skeletComponent.Rotation = 180f;
                        skeletComponent.Mirrored = true;
                    }
                    break;
            }
        }

        public new class Configuration : Component.Configuration {

            public override Component Create (Entity owner) {
                return new WallWalkerComponent(owner);
            }
        }
    }
}