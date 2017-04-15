using System.Collections.Generic;
using mapKnight.Extended.Graphics.UI.Layout;
using mapKnight.Core;
using System;

namespace mapKnight.Extended.Graphics.UI {
    public class UIMap : UIItem {
        private const float TRAIN_SPEED = 50f / 229f;
        private static readonly Station[ ] STATIONS;
        private static readonly Vector2[ ] WAYPOINTS;

        static UIMap ( ) {
            STATIONS = new Station[ ] {
                new Station(0, Line.LightGreen, 0, "green_0"),
                new Station(1, Line.LightGreen, 3, null),
                new Station(2, Line.LightGreen, 4, null),
                new Station(3, Line.LightGreen, 8, null),
                new Station(4, Line.LightGreen, 10, null),
                new Station(5, Line.LightGreen, 12, null),

                new Station(0, Line.Red, 15, null),
                new Station(1, Line.Red, 17, null),
                new Station(2, Line.Red, 20, null),
                new Station(3, Line.Red, 23, null),
                new Station(4, Line.Red, 26, null),
                new Station(5, Line.Red, 31, null),

                new Station(0, Line.Yellow, 33, null),
                new Station(1, Line.Yellow, 36, null),
                new Station(2, Line.Yellow, 39, null),

                new Station(0, Line.Violett, 41, null),
                new Station(1, Line.Violett, 44, null),
                new Station(2, Line.Violett, 47, null),
                new Station(3, Line.Violett, 53, null),
                new Station(4, Line.Violett, 55, null),

                new Station(0, Line.Blue, 56, null),
                new Station(1, Line.Blue, 59, null),
                new Station(2, Line.Blue, 62, null),
                new Station(3, Line.Blue, 67, null),
                new Station(4, Line.Blue, 69, null),
                new Station(5, Line.Blue, 72, null),
                new Station(6, Line.Blue, 75, null),
                new Station(7, Line.Blue, 78, null),

                new Station(0, Line.DarkGreen, 80, null),
                new Station(1, Line.DarkGreen, 83, null),
                new Station(2, Line.DarkGreen, 84, null),
            };

            WAYPOINTS = new Vector2[ ] {
                new Vector2(97.5f / 229f, 224.5f / 229f), // LIGHT GREEN 0
                new Vector2(97.5f / 229f, 215.5f / 229f),
                new Vector2(92.5f / 229f, 207.5f / 229f),
                new Vector2(92.5f / 229f, 202.5f / 229f), // 1
                new Vector2(108.5f / 229f, 185.5f / 229f), // 2
                new Vector2(113.5f / 229f, 179.5f / 229f),
                new Vector2(113.5f / 229f, 170.5f / 229f),
                new Vector2(103.5f / 229f, 160.5f / 229f),
                new Vector2(78.5f / 229f, 160.5f / 229f), // 3
                new Vector2(45.5f / 229f, 192.5f / 229f),
                new Vector2(38.5f / 229f, 192.5f / 229f), // 4
                new Vector2(26.5f / 229f, 192.5f / 229f),
                new Vector2(15.5f / 229f, 182.5f / 229f), // 5
                new Vector2(15.5f / 229f, 170.5f / 229f),
                new Vector2(24.5f / 229f, 160.5f / 229f),
                new Vector2(24.5f / 229f, 154.5f / 229f), // RED 0
                new Vector2(24.5f / 229f, 142.5f / 229f),
                new Vector2(15.5f / 229f, 133.5f / 229f), // 1
                new Vector2(15.5f / 229f, 117.5f / 229f),
                new Vector2(21.5f / 229f, 111.5f / 229f),
                new Vector2(27.5f / 229f, 111.5f / 229f), // 2
                new Vector2(36.5f / 229f, 111.5f / 229f),
                new Vector2(42.5f / 229f, 105.5f / 229f),
                new Vector2(42.5f / 229f, 95.5f / 229f), // 3
                new Vector2(42.5f / 229f, 90.5f / 229f),
                new Vector2(40.5f / 229f, 88.5f / 229f),
                new Vector2(24.5f / 229f, 88.5f / 229f), // 4
                new Vector2(18.5f / 229f, 88.5f / 229f),
                new Vector2(10.5f / 229f, 80.5f / 229f),
                new Vector2(10.5f / 229f, 66.5f / 229f),
                new Vector2(21.5f / 229f, 55.5f / 229f),
                new Vector2(31.5f / 229f, 55.5f / 229f), // 5
                new Vector2(48.5f / 229f, 38.5f / 229f),
                new Vector2(48.5f / 229f, 28.5f / 229f), // YELLOW 0
                new Vector2(48.5f / 229f, 12.5f / 229f),
                new Vector2(55.5f / 229f, 5.5f / 229f),
                new Vector2(71.5f / 229f, 5.5f / 229f), // 1
                new Vector2(74.5f / 229f, 5.5f / 229f),
                new Vector2(77.5f / 229f, 7.5f / 229f),
                new Vector2(99.5f / 229f, 7.5f / 229f), // 2
                new Vector2(135.5f / 229f, 7.5f / 229f),
                new Vector2(145.5f / 229f, 16.5f / 229f), // VIOLETT 0
                new Vector2(145.5f / 229f, 29.5f / 229f),
                new Vector2(138.5f / 229f, 36.5f / 229f),
                new Vector2(122.5f / 229f, 36.5f / 229f), // 1
                new Vector2(117.5f / 229f, 36.5f / 229f),
                new Vector2(105.5f / 229f, 24.5f / 229),
                new Vector2(98.5f / 229f, 24.5f / 229f), // 2
                new Vector2(90.5f / 229f, 24.5f / 229f),
                new Vector2(87.5f / 229f, 27.5f / 229f),
                new Vector2(87.5f / 229f, 42.5f / 229f),
                new Vector2(99.5f / 229f, 53.5f / 229f),
                new Vector2(141.5f / 229f, 53.5f / 229f),
                new Vector2(152.5f / 229f, 42.5f / 229f), // 3
                new Vector2(170.5f / 229f, 24.5f / 229f),
                new Vector2(189.5f / 229f, 24.5f / 229f), // 4
                new Vector2(206.5f / 229f, 41.5f / 229f), // BLUE 0
                new Vector2(206.5f / 229f, 69.5f / 229f),
                new Vector2(201.5f / 229f, 74.5f / 229f),
                new Vector2(196.5f / 229f, 74.5f / 229f), // 1
                new Vector2(177.5f / 229f, 74.5f / 229f),
                new Vector2(164.5f / 229f, 86.5f / 229f),
                new Vector2(155.5f / 229f, 86.5f / 229f), // 2
                new Vector2(143.5f / 229f, 86.5f / 229f),
                new Vector2(136.5f / 229f, 94.5f / 229f),
                new Vector2(136.5f / 229f, 112.5f / 229f),
                new Vector2(145.5f / 229f, 120.5f / 229f),
                new Vector2(184.5f / 229f, 120.5f / 229f), // 3
                new Vector2(210.5f / 229f, 147.5f / 229f),
                new Vector2(210.5f / 229f, 162.5f / 229f), // 4
                new Vector2(210.5f / 229f, 176.5f / 229f),
                new Vector2(198.5f / 229f, 188.5f / 229f),
                new Vector2(187.5f / 229f, 188.5f / 229f), // 5
                new Vector2(174.5f / 229f, 188.5f / 229f),
                new Vector2(164.5f / 229f, 178.5f / 229f),
                new Vector2(164.5f / 229f, 166.5f / 229f), // 6
                new Vector2(164.5f / 229f, 157.5f / 229f),
                new Vector2(155.5f / 229f, 148.5f / 229f),
                new Vector2(135.5f / 229f, 148.5f / 229f), // 7
                new Vector2(126.5f / 229f, 142.5f / 229f),
                new Vector2(126.5f / 229f, 126.5f / 229f), // DARKGREEN 0
                new Vector2(126.5f / 229f, 111.5f / 229f),
                new Vector2(122.5f / 229f, 107.5f / 229f),
                new Vector2(101.5f / 229f, 107.5f / 229f), // 1
                new Vector2(78.5f / 229f, 130.5f / 229f), // 2
            };
        }

        private Station selectedStation;
        private UnlockedState unlockedState = new UnlockedState(Line.LightGreen, 0);
        private bool isTrainMoving;
        private Vector2 currentPosition;
        private int currentWaypointIndex;
        private Vector2 currentWaypoint { get { return WAYPOINTS[currentWaypointIndex]; } }
        private int targetWaypointIndex;
        private int currentDirection = 1;
        private int currentTextureIndex = 0;
        private int lastTextureChange;

        public string CurrentSelection { get { return (selectedStation?.IsAvailable(unlockedState) ?? false) ? selectedStation.Map : null; } }

        public UIMap (Screen owner, UIMargin hmargin, UIMargin vmargin, IUISize size, int depth) : base(owner, hmargin, vmargin, size, depth, false) {
            IsDirty = true;
            currentPosition = currentWaypoint;
        }

        public override void HandleTouch (UITouchAction action, UITouch touch) {
            Vector2 clickedPosition = (touch.RelativePosition - Position).Abs( ) / Size.Size;
            Station clickedStation = FindClosestStation(clickedPosition);
            switch (action) {
                case UITouchAction.Begin:
                    break;
                case UITouchAction.End:
                    IsDirty = selectedStation != clickedStation;
                    selectedStation = clickedStation;
                    if(selectedStation != null) {
                        MoveTrain(selectedStation.Position);
                    }
                    break;
            }
        }

        private Station FindClosestStation (Vector2 clickedPosition) {
            Station currentClosestStation = null;
            float currentClosestDist = float.PositiveInfinity;
            for (int i = 0; i < STATIONS.Length; i++) {
                float dist = clickedPosition.DistanceSqr(WAYPOINTS[STATIONS[i].Position]);
                if (dist < currentClosestDist && dist < (10f / 229f) * (10f / 229f)) {
                    currentClosestDist = dist;
                    currentClosestStation = STATIONS[i];
                }
            }
            return currentClosestStation;
        }

        private void MoveTrain (int targetIndex) {
            targetWaypointIndex = targetIndex;
            isTrainMoving = true;
            lastTextureChange = Environment.TickCount;
        }

        public override void Update (DeltaTime dt) {
            if (isTrainMoving) {
                if (currentPosition == currentWaypoint) {
                    int nextWaypointIndex = currentWaypointIndex;
                    if (targetWaypointIndex < currentWaypointIndex) {
                        nextWaypointIndex--;
                    } else if (targetWaypointIndex > currentWaypointIndex) {
                        nextWaypointIndex++;
                    } else {
                        currentTextureIndex = 0;
                        isTrainMoving = false;
                    }
                    if(currentDirection + Math.Sign(WAYPOINTS[nextWaypointIndex].X - WAYPOINTS[currentWaypointIndex].X) == 0) {
                        currentDirection = Math.Sign(WAYPOINTS[nextWaypointIndex].X - WAYPOINTS[currentWaypointIndex].X);
                    }
                    currentWaypointIndex = nextWaypointIndex;
                } else {
                    float interpolation = Mathf.Clamp01((TRAIN_SPEED * dt.TotalSeconds) / (currentPosition - currentWaypoint).Magnitude( ));
                    currentPosition = Mathf.Interpolate(currentPosition, currentWaypoint, interpolation);

                    if (Environment.TickCount  - lastTextureChange > 200) {
                        currentTextureIndex = (currentTextureIndex + 1) % 3;
                        lastTextureChange = Environment.TickCount;
                    }
                }
                IsDirty = true;
            }
            base.Update(dt);
        }

        public override IEnumerable<DepthVertexData> ConstructVertexData ( ) {
            yield return new DepthVertexData(Bounds.Verticies, "map", Depth, Color.White);
            if (selectedStation != null) {
                Vector2 selectedStationPosition = WAYPOINTS[selectedStation.Position];
                yield return new DepthVertexData(UIRectangle.GetVerticies(Position.X + selectedStationPosition.X * Size.Size.X - .05f, Position.Y - selectedStationPosition.Y * Size.Size.Y + .1f * 31f / 19f, .1f, .1f * 32f / 19f), "marker_" + (selectedStation.IsAvailable(unlockedState) ? "a" : "d"), Depth + 1, Color.White);
            }
            yield return new DepthVertexData(UIRectangle.GetVerticies(Position.X + currentPosition.X * Size.Size.X - currentDirection * .05f, Position.Y - currentPosition.Y * Size.Y + .1f * 25f / 32f / 2f, currentDirection * .1f, .1f * 25f / 32f + (isTrainMoving ? 0.005f * (float)Math.Cos(Environment.TickCount * Math.PI / 250d) : 0f)), "train_" + currentTextureIndex, Depth + 1, Color.White);
        }

        public enum Line {
            LightGreen = 1,
            Red = 2,
            Yellow = 3,
            Violett = 4,
            Blue = 5,
            DarkGreen = 6,
        }

        public class Station {
            public int ID;
            public Line Line;
            public int Position;
            public string Map;

            public Station (int ID, Line Line, int Position, string Map) {
                this.ID = ID;
                this.Line = Line;
                this.Position = Position;
                this.Map = Map;
            }

            public bool IsAvailable (UnlockedState state) {
                return Line < state.Line || (Line == state.Line && ID <= state.ID);
            }
        }

        public struct UnlockedState {
            public Line Line;
            public int ID;

            public UnlockedState (Line Line, int ID) {
                this.Line = Line;
                this.ID = ID;
            }
        }

        public struct Location {
            public int Index;
            public Vector2 Position;
            public int Direction;

            public Location (int Index, Vector2 Position, int Direction) {
                this.Index = Index;
                this.Position = Position;
                this.Direction = Direction;
            }
        }
    }
}
