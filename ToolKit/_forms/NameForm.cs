using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace mapKnight.ToolKit {
    public partial class NameForm : Form {
        public string Result { get { return this.textBox1.Text; } }

        public NameForm (string nameof) {
            InitializeComponent ();

            this.Text += nameof;
        }

        private void button1_Click (object sender, EventArgs e) {
            this.Close ();
        }

        private void button2_Click (object sender, EventArgs e) {
            this.Close ();
        }

        private void textBox1_KeyDown (object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Enter) {
                this.DialogResult = DialogResult.OK;
                this.Close ();
            }
        }

        private void NameForm_Load (object sender, EventArgs e) {

        }
    }
}
