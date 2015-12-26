using System;
using System.Drawing;
using System.Windows.Forms;
using System.Reflection;

namespace mapKnight.ToolKit
{
	public partial class LoadForm : Form
	{
		public LoadForm ()
		{
			InitializeComponent ();
		}

		private void LoadForm_Load (object sender, EventArgs e)
		{
			this.Location = new Point (Screen.PrimaryScreen.Bounds.Width / 2 - this.Bounds.Width / 2, Screen.PrimaryScreen.Bounds.Height / 2 - this.Bounds.Height / 2);
			this.label_version.Text = "Version : " + new Values.Version (Assembly.GetExecutingAssembly ().GetName ().Version.ToString ()).ToString (true);
		}

		private void LoadForm_Paint (object sender, PaintEventArgs e)
		{
			e.Graphics.DrawRectangle (Pens.LightGray, 0, 0, this.Width - 1, this.Height - 1);
		}
	}
}
