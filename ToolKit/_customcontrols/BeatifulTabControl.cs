using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;

namespace mapKnight.ToolKit {
    class BeatifulTabControl : TabControl {
        public const int OUTLINE_HEIGHT = 2;

        private Brush backBrush = new SolidBrush (Properties.Settings.Default.BackColor);
        private Brush tabActiveBackBrush = new SolidBrush (Properties.Settings.Default.TabActiveBackColor);
        private Color tabActiveForeColor = Properties.Settings.Default.TabActiveForeColor;
        private Brush tabInActiveBackBrush = new SolidBrush (Properties.Settings.Default.TabInActiveBackColor);
        private Color tabInActiveForeColor = Properties.Settings.Default.TabInActiveForeColor;
        private Brush outlineBrush = new SolidBrush (Properties.Settings.Default.TabControlOutlineColor);

        StringFormat format = new StringFormat (); //for tab header text

        public BeatifulTabControl () {
            this.SetStyle (ControlStyles.UserPaint | ControlStyles.DoubleBuffer, true);
            this.DrawMode = TabDrawMode.OwnerDrawFixed;
            this.format.Alignment = StringAlignment.Center;
            this.format.LineAlignment = StringAlignment.Center;
            this.SizeMode = TabSizeMode.Normal;
        }

        protected override void InitLayout () {
            base.InitLayout ();
        }

        protected override void OnPaintBackground (PaintEventArgs e) {
            e.Graphics.FillRectangle (backBrush, this.ClientRectangle);
            if (this.TabPages.Count > 0)
                e.Graphics.FillRectangle (outlineBrush, new Rectangle (this.ClientRectangle.X, this.GetTabRect (0).Y + this.GetTabRect (0).Height, this.ClientRectangle.Width, OUTLINE_HEIGHT));
            for (int i = 0; i < this.TabPages.Count; i++) {

                if (this.SelectedIndex == i) {
                    Rectangle rect = this.GetTabRect (i);
                    e.Graphics.FillRectangle (tabActiveBackBrush, rect.X, rect.Y, rect.Width, rect.Height + OUTLINE_HEIGHT);

                    // draw outline around selected tab
                    e.Graphics.FillRectangle (outlineBrush, rect.X, rect.Y, OUTLINE_HEIGHT, rect.Height + OUTLINE_HEIGHT);
                    e.Graphics.FillRectangle (outlineBrush, rect.X, rect.Y, rect.Width, OUTLINE_HEIGHT);
                    e.Graphics.FillRectangle (outlineBrush, rect.X + rect.Width - OUTLINE_HEIGHT, rect.Y, OUTLINE_HEIGHT, rect.Height + OUTLINE_HEIGHT);

                    TextRenderer.DrawText (e.Graphics, this.TabPages[i].Text, Properties.Settings.Default.TabControlFont, this.GetTabRect (i), tabActiveForeColor, TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter);
                } else {
                    e.Graphics.FillRectangle (tabInActiveBackBrush, this.GetTabRect (i));
                    TextRenderer.DrawText (e.Graphics, this.TabPages[i].Text, Properties.Settings.Default.TabControlFont, this.GetTabRect (i), tabInActiveForeColor, TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter);
                }
            }

        }

    }
}
