using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace mapKnight.ToolKit {
    class BeautifulListView : ListView {
        private const int SELECTION_BORDER_HEIGHT = 3;

        private Brush selectedItemBrush = new SolidBrush (Properties.Settings.Default.ListViewSelectedColor);

        public BeautifulListView () : base () {
            this.SetStyle (ControlStyles.UserPaint, true);
            this.View = View.Tile;
            this.BorderStyle = BorderStyle.None;
            this.MultiSelect = false;
            this.TileSize = new Size (70, 70);
        }

        protected override void OnPaint (PaintEventArgs e) {
            e.Graphics.Clear (Properties.Settings.Default.ListViewBackColor);
            e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
            e.Graphics.PixelOffsetMode = PixelOffsetMode.Half;

            for (int i = 0; i < this.Items.Count; i++) {
                Rectangle rect = this.GetItemRect (i, ItemBoundsPortion.ItemOnly);
                int x = rect.X + rect.Width / 2 - rect.Height / 2 + SELECTION_BORDER_HEIGHT;
                int y = rect.Y + SELECTION_BORDER_HEIGHT;
                int width = rect.Height - 2 * SELECTION_BORDER_HEIGHT;
                int height = rect.Height - 2 * SELECTION_BORDER_HEIGHT;

                if (this.Items[i].Selected) {
                    e.Graphics.FillRectangle (selectedItemBrush, x - SELECTION_BORDER_HEIGHT, y - SELECTION_BORDER_HEIGHT, width + 2 * SELECTION_BORDER_HEIGHT, height + 2 * SELECTION_BORDER_HEIGHT);
                }

                if (this.Items[i].ImageIndex != -1)
                    e.Graphics.DrawImage (this.LargeImageList.Images[this.Items[i].ImageIndex], x, y, width, height);
                TextRenderer.DrawText (e.Graphics, this.Items[i].Text, Properties.Settings.Default.ListViewFont, rect, Properties.Settings.Default.ListViewForeColor);
            }
        }

        protected override void OnPaintBackground (PaintEventArgs e) {
        }
    }
}
