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
    public partial class SlotEditForm : Form
    {
        AttributeListView attrlistview;

        public SlotEditForm(string slot, string partname)
        {
            InitializeComponent();

            this.Text = "Editing attributes of " + partname + " (" + slot + ")";
        }

        private void SlotEditForm_Load(object sender, EventArgs e)
        {
            attrlistview = new AttributeListView(this, new Dictionary<Attribute, string>());
        }
    }
}
