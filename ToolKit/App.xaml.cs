using System;
using System.Windows;
using System.Windows.Controls;
using FolderBrowserDialog = System.Windows.Forms.FolderBrowserDialog;

namespace mapKnight.ToolKit {
    /// <summary>
    /// Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App : Application {
        public static string StartupFile = null;

        public App ( ) {
            EmbeddedAssemblies.Serve( );
        }

        private void Application_Startup (object sender, StartupEventArgs e) {
            if (e.Args.Length == 1) {
                StartupFile = e.Args[0];
            }
        }
    }
}
