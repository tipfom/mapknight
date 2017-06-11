using System.Windows;
using Microsoft.Win32;

namespace mapKnight.ToolKit.Windows.Dialogs {
    /// <summary>
    /// Interaktionslogik für PackTextureDialog.xaml
    /// </summary>
    public partial class PackTextureDialog : Window {
        public PackTextureDialog ( ) {
            InitializeComponent( );
        }

        private void Button_Click (object sender, RoutedEventArgs e) {
            SaveFileDialog dialog = new SaveFileDialog( );
            dialog.OverwritePrompt = true;
            dialog.Filter = "Zip-File|*.zip";
            if (dialog.ShowDialog( ) ?? false) {
                texturecreationcontrol.Build(dialog.FileName);
            }
        }

        private void Button_Clear_Click (object sender, RoutedEventArgs e) {
            texturecreationcontrol.Clear( );
        }
    }
}
