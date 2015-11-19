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
    public partial class InfoWindow : Form
    {
        public InfoWindow(XML.Version version)
        {
            InitializeComponent();
            this.versionlabel.Text += version.ToString();
            this.compiledonlabel.Text += version.BuildDate.ToString("dd/MM/yyyy HH:mm:ss") + " UTC+00";
        }
    }
}
