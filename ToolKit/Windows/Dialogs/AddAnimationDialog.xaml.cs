using System.Globalization;
using System.Windows;

namespace mapKnight.ToolKit.Windows {
    /// <summary>
    /// Interaktionslogik für AddAnimationWindow.xaml
    /// </summary>
    public partial class AddAnimationWindow : Window {
        public float Ratio;

        public AddAnimationWindow ( ) {
            Owner = App.Current.MainWindow;
            InitializeComponent( );
        }

        private void Button_Create_Click (object sender, RoutedEventArgs e) {
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
