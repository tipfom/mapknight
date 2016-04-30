using System;
using System.Drawing;
using System.Windows.Forms;

namespace mapKnight.ToolKit {
    public partial class AddTileForm : Form {
        public AddTileForm (Bitmap image) {
            InitializeComponent ();

            this.picturebox_tile.Image = image;
            this.picturebox_tile.SizeMode = PictureBoxSizeMode.StretchImage;
        }

        private void button2_Click (object sender, EventArgs e) {
            this.Close ();
        }

        private void textbox_name_KeyDown (object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Enter) {
                this.DialogResult = DialogResult.OK;
                this.Close ();
            }
        }
    }
}
