using System;
using System.Collections.Generic;
using System.Globalization;
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
using System.Windows.Shapes;

namespace mapKnight.ToolKit.Windows {
    /// <summary>
    /// Interaktionslogik für AddAnimationWindow.xaml
    /// </summary>
    public partial class AddAnimationWindow : Window {
        public float Ratio;

        public AddAnimationWindow ( ) {
            this.Owner = App.Current.MainWindow;
            InitializeComponent( );
        }

        private void TextBoxFloat_PreviewTextInput (object sender, TextCompositionEventArgs e) {
            float value;
            e.Handled = !(float.TryParse(((TextBox)sender).Text + e.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out value) && value > 0);
        }

        private void Button_Click (object sender, RoutedEventArgs e) {
            float width, height;
            if(float.TryParse(textbox_width.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out width) && float.TryParse(textbox_height.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out height)) {
                Ratio = width / height;
                DialogResult = true;
                Close( );
            } else {
                MessageBox.Show("please enter an valid width and height", "error");
            }
        }
    }
}
