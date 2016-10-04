using System;
using System.Windows;
using System.Windows.Controls;
using FolderBrowserDialog = System.Windows.Forms.FolderBrowserDialog;

namespace mapKnight.ToolKit {
    /// <summary>
    /// Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App : Application {
        public App ( ) {
            EmbeddedAssemblies.Serve( );
        }
    }
}
