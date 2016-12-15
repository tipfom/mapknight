using System.Collections.Generic;
using mapKnight.Core;
using mapKnight.Extended.Graphics.UI.Layout;

namespace mapKnight.Extended.Graphics.UI {
    public class UIBar : UIItem {
        const float BORDER_BOUNDS_RATIO = 10f / 10f; // width / height of image

        private float currentPercent;

        public UIBar (Screen owner, UIMargin hmargin, UIMargin vmargin, Vector2 size, int depth) : base(owner, hmargin, vmargin, size, depth, false) {
            //binder.Changed += binder_Changed;
            //currentPercent = binder.Percent;
        }

        private void binder_Changed (object sender, float e) {
            currentPercent = e;
            IsDirty = true;
        }

        public override List<DepthVertexData> ConstructVertexData ( ) {
            List<DepthVertexData> vertexData = new List<DepthVertexData>( );

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

            vertexData.Add(new DepthVertexData(filled_verticies, "bar_filled", Depth, Color.White));
            vertexData.Add(new DepthVertexData(empty_verticies, "bar_empty", Depth, Color.White));
            vertexData.Add(new DepthVertexData(border_verticies, "bar_border", Depth, Color.White));

            return vertexData;
        }
    }
}