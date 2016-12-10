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

        private void ButtonLink_Click (object sender, RoutedEventArgs e) {
            RunAssoc($"-a .mkproj \"{Assembly.GetExecutingAssembly( ).Location}\" toolkit -fd \"MapKnight Project\" -pd \"Pluto\"");
        }

        private void ButtonUnlink_Click (object sender, RoutedEventArgs e) {
            RunAssoc($"-r .mkproj toolkit");
        }

        private void RunAssoc (string args) {
            string path = Path.ChangeExtension(Path.GetTempFileName( ), "exe");
            using (Stream stream = Assembly.GetExecutingAssembly( ).GetManifestResourceStream("mapKnight.ToolKit.Resources.assoc.exe")) {
                byte[ ] bytes = new byte[(int)stream.Length];
                stream.Read(bytes, 0, bytes.Length);
                File.WriteAllBytes(path, bytes);
            }
            Process.Start(path, args);
        }
    }
}
