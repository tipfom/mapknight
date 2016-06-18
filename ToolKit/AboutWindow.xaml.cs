using System;
using System.Reflection;
using System.Windows;

namespace mapKnight.ToolKit {
    /// <summary>
    /// Interaktionslogik für AboutWindow.xaml
    /// </summary>
    public partial class AboutWindow : Window {
        Version supportingVersion = new Version(2, 1, 106);

        public AboutWindow ( ) {
            this.Owner = App.Current.MainWindow;
            InitializeComponent( );
        }

        private void Window_Loaded (object sender, RoutedEventArgs e) {
            label_version.Text += " " + Assembly.GetExecutingAssembly( ).GetName( ).Version.ToString(3);
            label_mkversion.Text += " " + supportingVersion.ToString(3);
        }
    }
}
