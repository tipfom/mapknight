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
    /// Interaktionslogik für HastorControl.xaml
    /// </summary>
    public partial class HastorControl : UserControl {
        public HastorControl ( ) {
            InitializeComponent( );
        }

        private void textbox_frenzyspeed_PreviewTextInput (object sender, TextCompositionEventArgs e) {
            float value;
            e.Handled = !float.TryParse(textbox_frenzyspeed.Text + e.Text, out value);
        }

        private void textbox_frenzytime_PreviewTextInput (object sender, TextCompositionEventArgs e) {
            int value;
            e.Handled = !(int.TryParse(textbox_frenzytime.Text + e.Text, out value) && value > 0);
        }
    }
}
