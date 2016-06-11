using mapKnight.Core;
using Microsoft.Win32;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace mapKnight.ToolKit {
    /// <summary>
    /// Interaktionslogik für CreateMapWindow.xaml
    /// </summary>
    public partial class CreateMapWindow : Window {
        const string WARN_MESSAGE =
            "The size of the map you are trying to create is very large.\n" +
            "This might cause extremly large files. Do you want to continue?";

        private Tuple<Tile[ ], Dictionary<string, Texture2D>> template;
        private GraphicsDevice graphicsDevice;

        public CreateMapWindow (GraphicsDevice g) {
            InitializeComponent( );
            graphicsDevice = g;
            this.Owner = App.Current.MainWindow;
        }

        private void CheckNumericPreviewTextInput (object sender, TextCompositionEventArgs e) {
            e.Handled = !char.IsNumber(e.Text[0]);
        }

        private void button_import_Click (object sender, RoutedEventArgs e) {
            OpenFileDialog importDialog = new OpenFileDialog( );
            importDialog.DefaultExt = "TileTemplates|*.mkttemplate";
            importDialog.Multiselect = false;
            importDialog.AddExtension = false;
            importDialog.CheckFileExists = true;
            importDialog.Filter = "TileTemplates|*.mkttemplate";

            if (importDialog.ShowDialog( ) ?? false) {
                using (Stream stream = File.OpenRead(importDialog.FileName)) {
                    template = TileSerializer.Deserialize(stream, graphicsDevice);
                }
                label_template.Content = importDialog.FileName;
            }
        }

        private void button_create_Click (object sender, RoutedEventArgs e) {
            Core.Size mapSize = new Core.Size(0, 0);
            if (ValidName( ) && ValidCreator( ) && ValidSize(ref mapSize)) {
                Map createdMap = new Map(mapSize, textbox_creator.Text, textbox_name.Text);
                App.Project.AddMap(createdMap);
                if (template != null) {
                    foreach (Tile tile in template.Item1) {
                        if (tile.Name == "None")
                            continue;
                        App.Project.AddTexture(createdMap, tile.Name, template.Item2[tile.Name]);
                        createdMap.AddTile(tile);
                    }
                }
                DialogResult = true;
                this.Close( );
            }
        }

        private bool ValidName ( ) {
            if (textbox_name.Text.Replace(" ", "") != "")
                return true;
            else {
                MessageBox.Show("name is not valid", "invalid name", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        private bool ValidCreator ( ) {
            if (textbox_creator.Text.Replace(" ", "") != "") {
                return true;
            } else {
                MessageBox.Show("creator is not valid", "invalid creator", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        private bool ValidSize (ref Core.Size size) {
            int width = 0, height = 0;
            if (textbox_width.Text == "" || !int.TryParse(textbox_width.Text, out width) || width <= 0) {
                MessageBox.Show("width is not valid", "invalid width", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (textbox_height.Text == "" || !int.TryParse(textbox_height.Text, out height) || height <= 0) {
                MessageBox.Show("height is not valid", "invalid height", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            size = new Core.Size(width, height);

            if (size.Area >= Math.Pow(2, 16) && MessageBox.Show(WARN_MESSAGE, "Size", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.No)
                return false;
            return true;
        }
    }
}
