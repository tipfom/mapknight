using System.Collections.Generic;
using mapKnight.Basic;

namespace mapKnight.Android.CGL.GUI {
    public class GUIBar : GUIClickItem {
        const float BORDER_BOUNDS_RATIO = 10f / 10f; // width / height of image

        private float currentPercent;

        public GUIBar (ChangingProperty binder, fRectangle bounds) : base (bounds) {
            binder.Changed += binder_Changed;
            currentPercent = binder.Percent;
        }

        private void binder_Changed (object sender, float e) {
            currentPercent = e;
            RequestUpdate ( );
        }

        public override List<VertexData> GetVertexData () {
            List<VertexData> vertexData = new List<VertexData> ( );

            fVector2D globalPosition = Screen.ToGlobal (this.Bounds.Position);
            fVector2D globalSize = new fVector2D (this.Bounds.Size.X * Screen.ScreenRatio, this.Bounds.Size.Y) * 2;
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

            vertexData.Add (new VertexData (filled_verticies, "bar_filled", Color.White));
            vertexData.Add (new VertexData (empty_verticies, "bar_empty", Color.White));
            vertexData.Add (new VertexData (border_verticies, "bar_border", Color.White));

            return vertexData;
        }
    }
}