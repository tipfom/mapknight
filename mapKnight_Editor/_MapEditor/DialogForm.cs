using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace mapKnight_Editor
{
    public partial class DialogForm : Form
    {
        public bool Finished = false;

        public DialogForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Finished = true;
            this.Close();
        }
    }
}
