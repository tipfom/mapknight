using System.Collections.Generic;
using mapKnight.Basic;

namespace mapKnight.Android.CGL.GUI {
    public class GUIBar : GUIItem {
        const float BORDER_BOUNDS_RATIO = 10f / 10f; // width / height of image

        private float currentPercent;

        public GUIBar (ChangingProperty binder, Rectangle bounds) : base (bounds) {
            binder.Changed += binder_Changed;
            currentPercent = binder.Percent;
        }

        private void binder_Changed (object sender, float e) {
            currentPercent = e;
            RequestUpdate ( );
        }

        public override List<CGLVertexData> GetVertexData () {
            List<CGLVertexData> vertexData = new List<CGLVertexData> ( );

            Vector2 globalPosition = Screen.ToGlobal (this.Bounds.Position);
            Vector2 globalSize = new Vector2 (this.Bounds.Size.X * Screen.ScreenRatio, this.Bounds.Size.Y) * 2;
            float borderWidthHalf = globalSize.Y * BORDER_BOUNDS_RATIO;
            float borderPosition = globalPosition.X + globalSize.X * currentPercent;

            float[ ] filled_verticies = new float[ ] {
                globalPosition.X,globalPosition.Y,
                globalPosition.X,globalPosition.Y - globalSize.Y,
                borderPosition, globalPosition.Y - globalSize.Y,
                borderPosition, globalPosition.Y};
            float[ ] empty_verticies = new float[ ] {
                borderPosition, globalPosition.Y,
                borderPosition, globalPosition.Y - globalSize.Y,
                globalPosition.X + globalSize.X, globalPosition.Y - globalSize.Y,
                globalPosition.X + globalSize.X, globalPosition.Y};
            float[ ] border_verticies = new float[ ] {
                borderPosition - borderWidthHalf / 2f, globalPosition.Y,
                borderPosition - borderWidthHalf / 2f, globalPosition.Y - globalSize.Y,
                borderPosition + borderWidthHalf, globalPosition.Y - globalSize.Y,
                borderPosition + borderWidthHalf,globalPosition.Y
            };

            vertexData.Add (new CGLVertexData (filled_verticies, "bar_filled", Color.White));
            vertexData.Add (new CGLVertexData (empty_verticies, "bar_empty", Color.White));
            vertexData.Add (new CGLVertexData (border_verticies, "bar_border", Color.White));

            return vertexData;
        }
    }
}