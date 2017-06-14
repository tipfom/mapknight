using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;

namespace mapKnight.ToolKit.Windows {
    /// <summary>
    /// Interaktionslogik für AboutWindow.xaml
    /// </summary>
    public partial class AboutWindow : Window {
        public AboutWindow ( ) {
            this.Owner = App.Current.MainWindow;
            InitializeComponent( );
        }

        private void Window_Loaded (object sender, RoutedEventArgs e) {
            Version version = Assembly.GetExecutingAssembly( ).GetName( ).Version;
            label_version.Text += " " + version.ToString(3) + " built on " + new DateTime(2000, 1, 1).AddDays(version.Build).AddSeconds(version.Revision * 2) +
#if DEBUG
                " - Debug"
#else
                " - Release"
#endif
                ;
        }

        private void Hyperlink_RequestNavigate (object sender, System.Windows.Navigation.RequestNavigateEventArgs e) {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }
    }
}
