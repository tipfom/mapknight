using System.Collections.Generic;
using mapKnight.Core;

namespace mapKnight.Extended.Graphics.GUI {
    public class GUIBar : GUIItem {
        const float BORDER_BOUNDS_RATIO = 10f / 10f; // width / height of image

        private float currentPercent;

        public GUIBar (Screen owner, Rectangle bounds, int depth) : base(owner, bounds, depth, false) {
            //binder.Changed += binder_Changed;
            //currentPercent = binder.Percent;
        }

        private void binder_Changed (object sender, float e) {
            currentPercent = e;
            RequestUpdate( );
        }

        public override List<VertexData> GetVertexData ( ) {
            List<VertexData> vertexData = new List<VertexData>( );

            float borderWidthHalf = Size.Y * BORDER_BOUNDS_RATIO;
            float borderPosition = Position.X + Size.X * currentPercent;

            float[ ] filled_verticies = new float[ ] {
                Position.X,Position.Y,
                Position.X,Position.Y - Size.Y,
                borderPosition, Position.Y - Size.Y,
                borderPosition, Position.Y};
            float[ ] empty_verticies = new float[ ] {
                borderPosition, Position.Y,
                borderPosition, Position.Y - Size.Y,
                Position.X + Size.X, Position.Y - Size.Y,
                Position.X + Size.X, Position.Y};
            float[ ] border_verticies = new float[ ] {
                borderPosition - borderWidthHalf / 2f, Position.Y,
                borderPosition - borderWidthHalf / 2f, Position.Y - Size.Y,
                borderPosition + borderWidthHalf, Position.Y - Size.Y,
                borderPosition + borderWidthHalf,Position.Y
            };

            vertexData.Add(new VertexData(filled_verticies, "bar_filled", DepthOnScreen, Color.White));
            vertexData.Add(new VertexData(empty_verticies, "bar_empty", DepthOnScreen, Color.White));
            vertexData.Add(new VertexData(border_verticies, "bar_border", DepthOnScreen, Color.White));

            return vertexData;
        }
    }
}