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

namespace mapKnight.ToolKit.Controls {
    /// <summary>
    /// Interaktionslogik für IntegerTextbox.xaml
    /// </summary>
    public partial class IntegerTextbox : TextBox {
        public IntegerTextbox ( ) {
            InitializeComponent( );
        }

        private void TextBox_PreviewTextInput (object sender, TextCompositionEventArgs e) {
            int value;
            e.Handled = !int.TryParse(this.Text + e.Text, out value);
        }
    }
}
