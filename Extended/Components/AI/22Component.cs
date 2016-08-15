using mapKnight.Core;
using mapKnight.Extended.Components.Stats;

namespace mapKnight.Extended.Components.AI {

    [ComponentRequirement(typeof(SpeedComponent))]
    [ComponentOrder(ComponentEnum.Stats_Speed, false)]
    public class _2Component : Component {
        private Direction currentMoveDir;
        private Direction currentWallDir;
        private Direction nextMoveDir = Direction.Left;
        private Direction nextWallDir = Direction.Down;
        private SpeedComponent speedComponent;
        private float targetLoc;

        public _2Component (Entity owner) : base(owner) {
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
                    Owner.Transform.TranslateX(Owner.Transform.Center.X - speedComponent.Speed.X * dt.TotalSeconds);
                    if (Owner.Transform.Center.X < targetLoc) {
                        Owner.Transform.TranslateX(targetLoc);
                        FindWaypoint( );
                    }
                    break;

                case Direction.Right:
                    Owner.Transform.TranslateX(Owner.Transform.Center.X + speedComponent.Speed.X * dt.TotalSeconds);
                    if (Owner.Transform.Center.X > targetLoc) {
                        Owner.Transform.TranslateX(targetLoc);
                        FindWaypoint( );
                    }
                    break;

                case Direction.Up:
                    Owner.Transform.TranslateY(Owner.Transform.Center.Y + speedComponent.Speed.Y * dt.TotalSeconds);
                    if (Owner.Transform.Center.Y > targetLoc) {
                        Owner.Transform.TranslateY(targetLoc);
                        FindWaypoint( );
                    }
                    break;

                case Direction.Down:
                    Owner.Transform.TranslateY(Owner.Transform.Center.Y - speedComponent.Speed.Y * dt.TotalSeconds);
                    if (Owner.Transform.Center.Y < targetLoc) {
                        Owner.Transform.TranslateY(targetLoc);
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
                    targetLoc = Owner.Transform.BoundsHalf.X;
                    int ywalkingon = currentWallDir == Direction.Down ? Mathi.Floor(Owner.Transform.BL.Y) - 1 : Mathi.Floor(Owner.Transform.TR.Y);
                    int ywalkingagainst = currentWallDir == Direction.Down ? ywalkingon + 1 : ywalkingon - 1;
                    for (int x = Mathi.Floor(Owner.Transform.BL.X) - 1; x > -1; x--) {
                        if (!Owner.Owner.HasCollider(x, ywalkingon)) {
                            targetLoc = x + 1 - Owner.Transform.BoundsHalf.X;
                            nextMoveDir = currentWallDir;
                            nextWallDir = Direction.Right;
                            break;
                        } else if (Owner.Owner.HasCollider(x, ywalkingagainst)) {
                            targetLoc = x + 1 + Owner.Transform.BoundsHalf.X;
                            nextMoveDir = 1 - currentWallDir;
                            nextWallDir = Direction.Left;
                            break;
                        }
                    }
                    break;

                case Direction.Right:
                    targetLoc = Owner.Owner.Size.Width - Owner.Transform.BoundsHalf.X;
                    ywalkingon = currentWallDir == Direction.Down ? Mathi.Floor(Owner.Transform.BL.Y) - 1 : Mathi.Floor(Owner.Transform.TR.Y);
                    ywalkingagainst = currentWallDir == Direction.Down ? ywalkingon + 1 : ywalkingon - 1;
                    for (int x = Mathi.Floor(Owner.Transform.TR.X); x < Owner.Owner.Size.Width; x++) {
                        if (!Owner.Owner.HasCollider(x, ywalkingon)) {
                            targetLoc = x + Owner.Transform.BoundsHalf.X;
                            nextMoveDir = currentWallDir;
                            nextWallDir = Direction.Left;
                            break;
                        } else if (Owner.Owner.HasCollider(x, ywalkingagainst)) {
                            targetLoc = x - Owner.Transform.BoundsHalf.X;
                            nextMoveDir = 1 - currentWallDir;
                            nextWallDir = Direction.Right;
                            break;
                        }
                    }
                    break;

                case Direction.Up:
                    targetLoc = Owner.Owner.Size.Height - Owner.Transform.BoundsHalf.Y;
                    int xwalkingon = currentWallDir == Direction.Left ? Mathi.Floor(Owner.Transform.BL.X) - 1 : Mathi.Floor(Owner.Transform.TR.X);
                    int xwalkingagainst = currentWallDir == Direction.Left ? xwalkingon + 1 : xwalkingon - 1;
                    for (int y = Mathi.Floor(Owner.Transform.TR.Y); y < Owner.Owner.Size.Height; y++) {
                        if (!Owner.Owner.HasCollider(xwalkingon, y)) {
                            targetLoc = y + Owner.Transform.BoundsHalf.Y;
                            nextMoveDir = currentWallDir;
                            nextWallDir = Direction.Down;
                            break;
                        } else if (Owner.Owner.HasCollider(xwalkingagainst, y)) {
                            targetLoc = y - Owner.Transform.BoundsHalf.Y;
                            nextMoveDir = 1 - currentWallDir;
                            nextWallDir = Direction.Up;
                            break;
                        }
                    }
                    break;

                case Direction.Down:
                    targetLoc = Owner.Transform.BoundsHalf.Y;
                    xwalkingon = currentWallDir == Direction.Left ? Mathi.Floor(Owner.Transform.BL.X) - 1 : Mathi.Floor(Owner.Transform.TR.X);
                    xwalkingagainst = currentWallDir == Direction.Left ? xwalkingon + 1 : xwalkingon - 1;
                    for (int y = Mathi.Floor(Owner.Transform.BL.Y) - 1; y > -1; y--) {
                        if (!Owner.Owner.HasCollider(xwalkingon, y)) {
                            targetLoc = y + 1 - Owner.Transform.BoundsHalf.Y;
                            nextMoveDir = currentWallDir;
                            nextWallDir = Direction.Up;
                            break;
                        } else if (Owner.Owner.HasCollider(xwalkingagainst, y)) {
                            targetLoc = y + 1 + Owner.Transform.BoundsHalf.Y;
                            nextMoveDir = 1 - currentWallDir;
                            nextWallDir = Direction.Down;
                            break;
                        }
                    }
                    break;
            }
            Debug.Print(this, currentMoveDir + " " + currentWallDir + " " + targetLoc.ToString( ) + " " + nextMoveDir + " " + nextWallDir);
        }

        public new class Configuration : Component.Configuration {

            public override Component Create (Entity owner) {
                return new _2Component(owner);
            }
        }
    }
}