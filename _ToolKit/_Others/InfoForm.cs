using System;
using System.Windows.Forms;

namespace mapKnight.ToolKit
{
	public partial class InfoWindow : Form
	{
		public InfoWindow (Values.Version version)
		{
			InitializeComponent ();
			this.versionlabel.Text += version.ToString (false);
			this.compiledonlabel.Text += version.BuildDate.ToString ("dd/MM/yyyy HH:mm:ss") + " UTC+00";
		}

		private void InfoWindow_Load (object sender, EventArgs e)
		{

		}
	}
}
