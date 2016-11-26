﻿using mapKnight.Core;
using mapKnight.Extended.Components.Attributes;
using mapKnight.Extended.Components.Stats;

namespace mapKnight.Extended.Components.AI.Basics {

    [UpdateAfter(typeof(SpeedComponent))]
    public class WallWalkerComponent : Component {
        private Direction currentMoveDir;
        private Direction currentWallDir;
        private Direction nextMoveDir = Direction.Left;
        private Direction nextWallDir = Direction.Down;
        private SpeedComponent speedComponent;
        private float targetLoc;

        public WallWalkerComponent (Entity owner) : base(owner) {
        }

        private enum Direction : sbyte {
            Up = 0,
            Down = 1,
            Left = 2,
            Right = -1
        }

        public override void Prepare ( ) {
            speedComponent = Owner.GetComponent<SpeedComponent>( );
            FindWaypoint( );
        }

        public override void Update (DeltaTime dt) {
            switch (currentMoveDir) {
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

        private void FindWaypoint ( ) {
            currentMoveDir = nextMoveDir;
            currentWallDir = nextWallDir;
            switch (currentMoveDir) {
                case Direction.Left:
                    targetLoc = Owner.Transform.HalfSize.X;
                    int ywalkingon = currentWallDir == Direction.Down ? Mathi.Floor(Owner.Transform.BL.Y) - 1 : Mathi.Floor(Owner.Transform.TR.Y);
                    int ywalkingagainst = currentWallDir == Direction.Down ? ywalkingon + 1 : ywalkingon - 1;
                    for (int x = Mathi.Floor(Owner.Transform.BL.X) - 1; x > -1; x--) {
                        if (!Owner.World.HasCollider(x, ywalkingon)) {
                            targetLoc = x + 1 - Owner.Transform.HalfSize.X;
                            nextMoveDir = currentWallDir;
                            nextWallDir = Direction.Right;
                            break;
                        } else if (Owner.World.HasCollider(x, ywalkingagainst)) {
                            targetLoc = x + 1 + Owner.Transform.HalfSize.X;
                            nextMoveDir = 1 - currentWallDir;
                            nextWallDir = Direction.Left;
                            break;
                        }
                    }
                    break;

                case Direction.Right:
                    targetLoc = Owner.World.Size.Width - Owner.Transform.HalfSize.X;
                    ywalkingon = currentWallDir == Direction.Down ? Mathi.Floor(Owner.Transform.BL.Y) - 1 : Mathi.Floor(Owner.Transform.TR.Y);
                    ywalkingagainst = currentWallDir == Direction.Down ? ywalkingon + 1 : ywalkingon - 1;
                    for (int x = Mathi.Floor(Owner.Transform.TR.X); x < Owner.World.Size.Width; x++) {
                        if (!Owner.World.HasCollider(x, ywalkingon)) {
                            targetLoc = x + Owner.Transform.HalfSize.X;
                            nextMoveDir = currentWallDir;
                            nextWallDir = Direction.Left;
                            break;
                        } else if (Owner.World.HasCollider(x, ywalkingagainst)) {
                            targetLoc = x - Owner.Transform.HalfSize.X;
                            nextMoveDir = 1 - currentWallDir;
                            nextWallDir = Direction.Right;
                            break;
                        }
                    }
                    break;

                case Direction.Up:
                    targetLoc = Owner.World.Size.Height - Owner.Transform.HalfSize.Y;
                    int xwalkingon = currentWallDir == Direction.Left ? Mathi.Floor(Owner.Transform.BL.X) - 1 : Mathi.Floor(Owner.Transform.TR.X);
                    int xwalkingagainst = currentWallDir == Direction.Left ? xwalkingon + 1 : xwalkingon - 1;
                    for (int y = Mathi.Floor(Owner.Transform.TR.Y); y < Owner.World.Size.Height; y++) {
                        if (!Owner.World.HasCollider(xwalkingon, y)) {
                            targetLoc = y + Owner.Transform.HalfSize.Y;
                            nextMoveDir = currentWallDir;
                            nextWallDir = Direction.Down;
                            break;
                        } else if (Owner.World.HasCollider(xwalkingagainst, y)) {
                            targetLoc = y - Owner.Transform.HalfSize.Y;
                            nextMoveDir = 1 - currentWallDir;
                            nextWallDir = Direction.Up;
                            break;
                        }
                    }
                    break;

                case Direction.Down:
                    targetLoc = Owner.Transform.HalfSize.Y;
                    xwalkingon = currentWallDir == Direction.Left ? Mathi.Floor(Owner.Transform.BL.X) - 1 : Mathi.Floor(Owner.Transform.TR.X);
                    xwalkingagainst = currentWallDir == Direction.Left ? xwalkingon + 1 : xwalkingon - 1;
                    for (int y = Mathi.Floor(Owner.Transform.BL.Y) - 1; y > -1; y--) {
                        if (!Owner.World.HasCollider(xwalkingon, y)) {
                            targetLoc = y + 1 - Owner.Transform.HalfSize.Y;
                            nextMoveDir = currentWallDir;
                            nextWallDir = Direction.Up;
                            break;
                        } else if (Owner.World.HasCollider(xwalkingagainst, y)) {
                            targetLoc = y + 1 + Owner.Transform.HalfSize.Y;
                            nextMoveDir = 1 - currentWallDir;
                            nextWallDir = Direction.Down;
                            break;
                        }
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