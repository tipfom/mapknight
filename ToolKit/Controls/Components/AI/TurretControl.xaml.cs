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
    /// Interaktionslogik für TurretControl.xaml
    /// </summary>
    public partial class TurretControl : UserControl {
        public TurretControl ( ) {
            InitializeComponent( );
        }

        private void TextBoxFloat_PreviewTextInput (object sender, TextCompositionEventArgs e) {
            float value;
            e.Handled = !(float.TryParse(((TextBox)sender).Text + e.Text, out value) && value > 0);
        }

        private void TextBoxInteger_PreviewTextInput (object sender, TextCompositionEventArgs e) {
            int value;
            e.Handled = !(int.TryParse(((TextBox)sender).Text + e.Text, out value) && value > 0);
        }
    }
}
