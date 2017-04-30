using System.Collections.Generic;
using mapKnight.Extended.Graphics.UI.Layout;
using mapKnight.Core;

namespace mapKnight.Extended.Graphics.UI {
    public class UIItemFrame : UIItem {
        const int PIXEL_SIZE = 34;

        private bool _Selected;
        public bool Selected { get { return _Selected; } set { IsDirty |= _Selected != value; _Selected = value; } }

        private string item;
        private float relativeWidth;
        private float relativeHeight;

        public UIItemFrame (Screen owner, UILayout layout, string item, int depth) : base(owner, layout, depth, false) {
            this.item = item;

            float[ ] data = UIRenderer.Texture[item];
            float width = (data[4] - data[0]) * UIRenderer.Texture.Width;
            float height = (data[3] - data[1]) * UIRenderer.Texture.Height;
            relativeWidth = width / PIXEL_SIZE;
            relativeHeight = height / PIXEL_SIZE;
            IsDirty = true;
        }

        public override IEnumerable<DepthVertexData> ConstructVertexData ( ) {
            yield return new DepthVertexData(Layout, "frame_" + (_Selected ? "s" : "u"), Depth);
            Vector2 center = new Vector2(Layout.X + Layout.Width / 2f, Layout.Y - Layout.Height / 2f);
            float totalWidth = relativeWidth * Layout.Width, totalHeight = relativeHeight * Layout.Height;
            yield return new DepthVertexData(UIRectangle.GetVerticies(center.X - totalWidth / 2f, center.Y + totalHeight / 2f, totalWidth, totalHeight), item, Depth);
        }
    }
}
