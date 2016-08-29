using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace mapKnight.ToolKit.Controls.Components {
    /// <summary>
    /// Interaktionslogik für BulletControl.xaml
    /// </summary>
    public partial class BulletControl : UserControl, IComponentControl {
        public BulletControl ( ) {
            InitializeComponent( );
        }

        public string Category {
            get {
                return "ai";
            }
        }

        public List<Control> Menu {
            get {
                throw new NotImplementedException( );
            }
        }

        public Dictionary<string, string> Compile ( ) {
            throw new NotImplementedException( );
        }
    }
}
