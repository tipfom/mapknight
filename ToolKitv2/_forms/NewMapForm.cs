using System;
using System.Windows.Forms;

namespace mapKnight.ToolKit {
    public partial class NewMapForm : Form {
        public NewMapForm (Tileset[] tileset) {
            InitializeComponent ();

            this.combobox_tileset.Items.AddRange (tileset);
        }

        private void button1_Click (object sender, EventArgs e) {
            if (this.combobox_tileset.SelectedItem != null) {
                this.DialogResult = DialogResult.OK;
                this.Close ();
            } else {
                MessageBox.Show ("Please select a tileset!", "select", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
