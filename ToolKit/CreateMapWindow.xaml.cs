using mapKnight.Core;
using Microsoft.Win32;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace mapKnight.ToolKit {
    /// <summary>
    /// Interaktionslogik für CreateMapWindow.xaml
    /// </summary>
    public partial class CreateMapWindow : Window {
        public Map CreatedMap;
        private string importedTileTemplate;

        public CreateMapWindow( ) {
            InitializeComponent( );
        }

        private void CheckNumericPreviewTextInput(object sender, TextCompositionEventArgs e) {
            e.Handled = !char.IsNumber(e.Text[0]);
        }

        private void button_import_Click(object sender, RoutedEventArgs e) {
            OpenFileDialog importDialog = new OpenFileDialog( );
            importDialog.DefaultExt = "TileTemplates|*.mktiletemplate";
            importDialog.Multiselect = false;
            importDialog.AddExtension = false;
            importDialog.CheckFileExists = true;
            importDialog.Filter = "TileTemplates|*.mktiletemplate";
            importDialog.ShowDialog( );
            if (File.Exists(importDialog.FileName)) {
                importedTileTemplate = importDialog.FileName;
            }
        }

        private void button_create_Click(object sender, RoutedEventArgs e) {
            int width, height;
            if (textbox_creator.Text.Replace(" ", "") != "" &&
                textbox_name.Text.Replace(" ", "") != "" &&
                textbox_width.Text != "" && int.TryParse(textbox_width.Text, out width) &&
                textbox_height.Text != "" && int.TryParse(textbox_height.Text, out height)) {

                CreatedMap = new Map(new Core.Size(width, height), textbox_creator.Text, textbox_name.Text);
                CreatedMap.AddTile(new Tile( ) { Name = "None", Attributes = new Dictionary<TileAttribute, string>( ) });
                DialogResult = true;
                this.Close( );
            } else {
                MessageBox.Show("Invalid data has been entered", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
