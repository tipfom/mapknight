using System;
using System.Windows.Forms;
using System.Drawing;

namespace mapKnight_Editor
{
    class CustomToolStripRenderer : ToolStripSystemRenderer
    {
        bool RenderBorder;

        public CustomToolStripRenderer(bool renderborder) : base()
        {
            RenderBorder = renderborder;
        }

        protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
        {
            if (RenderBorder)
                e.Graphics.DrawRectangle(Pens.Gray, new Rectangle(1, 0, e.ToolStrip.Width - 2, e.ToolStrip.Height - 1));
        }
    }
}
