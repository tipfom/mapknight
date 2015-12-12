using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Drawing;

namespace mapKnight.ToolKit
{
	class ItemPictureBox : PictureBox
	{
		public ItemPictureBox () : base ()
		{
			this.BorderStyle = BorderStyle.FixedSingle;
			this.BackgroundImageLayout = ImageLayout.Zoom;
		}

		protected override void OnPaint (PaintEventArgs pe)
		{
			if (this.Enabled) {
				pe.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
				pe.Graphics.Clear (Color.White);

				if (this.BackgroundImage != null) {
					float scaleFactorX = (float)pe.ClipRectangle.Width / (float)this.BackgroundImage.Size.Width;
					float scaleFactorY = (float)pe.ClipRectangle.Height / (float)this.BackgroundImage.Size.Height;

					if (scaleFactorX > scaleFactorY) {
						pe.Graphics.DrawImage (this.BackgroundImage, new Rectangle ((pe.ClipRectangle.Size.Width - (int)(scaleFactorY * this.BackgroundImage.Size.Width)) / 2, 0, (int)(scaleFactorY * this.BackgroundImage.Size.Width), (int)(scaleFactorY * this.BackgroundImage.Size.Height)));
					} else {
						pe.Graphics.DrawImage (this.BackgroundImage, new Rectangle (0, (pe.ClipRectangle.Size.Height - (int)(scaleFactorX * this.BackgroundImage.Size.Height)) / 2, (int)(scaleFactorX * this.BackgroundImage.Size.Width), (int)(scaleFactorX * this.BackgroundImage.Size.Height)));
					}
				}
			} else {
				pe.Graphics.Clear (Color.FromArgb (240, 240, 240));
			}
		}
	}
}
