using System.Collections.Generic;
using mapKnight.Extended.Graphics.UI.Layout;
using mapKnight.Core;

namespace mapKnight.Extended.Graphics.UI {
    public class UIMap : UIItem {
        private static readonly Station[ ] STATIONS;

        static UIMap( ) {
            STATIONS = new Station[ ] {
                new Station(0, Line.LightGreen, new Vector2(97.5f / 229f, 224.5f / 229f), "green_0"),
                new Station(1, Line.LightGreen, new Vector2(92.5f / 229f, 202.5f / 229f), ""),
                new Station(2, Line.LightGreen, new Vector2(108.5f / 229f, 185.5f / 229f), ""),
                new Station(3, Line.LightGreen, new Vector2(78.5f / 229f, 160.5f / 229f), ""),
                new Station(4, Line.LightGreen, new Vector2(38.5f / 229f, 192.5f / 229f), ""),
                new Station(5, Line.LightGreen, new Vector2(15.5f / 229f, 182.5f / 229f), ""),

                new Station(0, Line.Red, new Vector2(24.5f / 229f, 154.5f / 229f), ""),
                new Station(1, Line.Red, new Vector2(15.5f / 229f, 133.5f / 229f), ""),
                new Station(2, Line.Red, new Vector2(27.5f / 229f, 111.5f / 229f), ""),
                new Station(3, Line.Red, new Vector2(42.5f / 229f, 95.5f / 229f), ""),
                new Station(4, Line.Red, new Vector2(24.5f / 229f, 88.5f / 229f), ""),
                new Station(5, Line.Red, new Vector2(31.5f / 229f, 55.5f / 229f), ""),

                new Station(0, Line.Yellow, new Vector2(48.5f / 229f, 28.5f / 229f), ""),
                new Station(1, Line.Yellow, new Vector2(71.5f / 229f, 5.5f / 229f), ""),
                new Station(2, Line.Yellow, new Vector2(99.5f / 229f, 7.5f / 229f), ""),

                new Station(0, Line.Violett, new Vector2(145.5f / 229f, 16.5f / 229f), ""),
                new Station(1, Line.Violett, new Vector2(122.5f / 229f, 36.5f / 229f), ""),
                new Station(2, Line.Violett, new Vector2(98.5f / 229f, 24.5f / 229f), ""),
                new Station(3, Line.Violett, new Vector2(152.5f / 229f, 42.5f / 229f), ""),
                new Station(4, Line.Violett, new Vector2(189.5f / 229f, 24.5f / 229f), ""),

                new Station(0, Line.Blue, new Vector2(206.5f / 229f, 41.5f / 229f), ""),
                new Station(1, Line.Blue, new Vector2(196.5f / 229f, 74.5f / 229f), ""),
                new Station(2, Line.Blue, new Vector2(155.5f / 229f, 86.5f / 229f), ""),
                new Station(3, Line.Blue, new Vector2(184.5f / 229f, 120.5f / 229f), ""),
                new Station(4, Line.Blue, new Vector2(210.5f / 229f, 162.5f / 229f), ""),
                new Station(5, Line.Blue, new Vector2(187.5f / 229f, 188.5f / 229f), ""),
                new Station(6, Line.Blue, new Vector2(164.5f / 229f, 166.5f / 229f), ""),
                new Station(7, Line.Blue, new Vector2(135.5f / 229f, 148.5f / 229f), ""),

                new Station(0, Line.DarkGreen, new Vector2(126.5f / 229f, 126.5f / 229f), ""),
                new Station(1, Line.DarkGreen, new Vector2(101.5f / 229f, 107.5f / 229f), ""),
                new Station(2, Line.DarkGreen, new Vector2(78.5f / 229f, 130.5f / 229f), ""),

            };
        }

        private Station selectedStation;
        private UnlockedState unlockedState = new UnlockedState(Line.LightGreen, 0);

        public string CurrentSelection { get { return selectedStation?.Map ?? null; } }

        public UIMap(Screen owner, UIMargin hmargin, UIMargin vmargin, IUISize size, int depth) : base(owner, hmargin, vmargin, size, depth, false) {
            IsDirty = true;
        }

        public override void HandleTouch(UITouchAction action, UITouch touch) {
            Vector2 clickedPosition = (touch.RelativePosition - Position).Abs( ) / Size.Size;
            Station clickedStation = FindClosestStation(clickedPosition);
            switch (action) {
                case UITouchAction.Begin:
                    break;
                case UITouchAction.End:
                    IsDirty = selectedStation != clickedStation;
                    selectedStation = clickedStation;
                    break;
            }
        }

        private Station FindClosestStation(Vector2 clickedPosition) {
            Station currentClosestStation = null;
            float currentClosestDist = float.PositiveInfinity;
            for (int i = 0; i < STATIONS.Length; i++) {
                float dist = clickedPosition.DistanceSqr(STATIONS[i].Position);
                if (dist < currentClosestDist && dist < (10f / 229f) * (10f / 229f)) {
                    currentClosestDist = dist;
                    currentClosestStation = STATIONS[i];
                }
            }
            return currentClosestStation;
        }

        public override IEnumerable<DepthVertexData> ConstructVertexData( ) {
            yield return new DepthVertexData(Bounds.Verticies, "map", Depth, Color.White);
            if (selectedStation != null)
                yield return new DepthVertexData(UIRectangle.GetVerticies(Position.X + selectedStation.Position.X * Size.Size.X - .05f, Position.Y - selectedStation.Position.Y * Size.Size.Y + .09f, .1f, .1f), "marker", Depth + 1, selectedStation.IsAvailable(unlockedState) ? Color.Green : Color.Red);
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
            public Vector2 Position;
            public string Map;

            public Station(int ID, Line Line, Vector2 Position, string Map) {
                this.ID = ID;
                this.Line = Line;
                this.Position = Position;
                this.Map = Map;
            }

            public bool IsAvailable(UnlockedState state) {
                return Line < state.Line || (Line == state.Line && ID <= state.ID);
            }
        }

        public struct UnlockedState {
            public Line Line;
            public int ID;

            public UnlockedState(Line Line, int ID) {
                this.Line = Line;
                this.ID = ID;
            }
        }
    }
}
