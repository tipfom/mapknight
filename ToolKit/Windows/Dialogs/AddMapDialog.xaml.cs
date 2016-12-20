using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using mapKnight.Core;
using mapKnight.ToolKit.Serializer;
using Microsoft.Win32;
using Microsoft.Xna.Framework.Graphics;

namespace mapKnight.ToolKit.Windows {
    /// <summary>
    /// Interaktionslogik für CreateMapWindow.xaml
    /// </summary>
    public partial class CreateMapWindow : Window {
        const string WARN_MESSAGE =
            "The size of the map you are trying to create is very large.\n" +
            "This might cause extremly large files. Do you want to continue?";

        private Tuple<Tile[ ], Dictionary<string, Texture2D>> template;
        private GraphicsDevice graphicsDevice;
        private Action<Map> AddMap;
        private Action<Map, string, Texture2D> AddTexture;

        public CreateMapWindow (GraphicsDevice g, Action<Map> addmap, Action<Map, string, Texture2D> addtexture) {
            InitializeComponent( );
            graphicsDevice = g;
            AddMap = addmap;
            AddTexture = addtexture;
            this.Owner = App.Current.MainWindow;
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
            Core.Vector2 gravity = new Vector2(0, 0);
            if (ValidName( ) && ValidCreator( ) && ValidSize(ref mapSize) && ValidGravity(ref gravity)) {
                Map createdMap = new Map(mapSize, textbox_creator.Text, textbox_name.Text) { Gravity = gravity };
                AddMap(createdMap);
                if (template != null) {
                    foreach (Tile tile in template.Item1) {
                        if (tile.Name == "None")
                            continue;
                        AddTexture(createdMap, tile.Name, template.Item2[tile.Name]);
                        createdMap.AddTile(tile);
                    }
                }
                DialogResult = true;
                Close( );
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
            int width, height;
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

        private bool ValidGravity (ref Vector2 gravity) {
            float x, y = 0;
            if (textbox_gravityx.Text == "" || !float.TryParse(textbox_gravityx.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out x)) {
                MessageBox.Show("x value of gravity is not valied", "invalid gravity", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (textbox_gravityy.Text == "" || !float.TryParse(textbox_gravityy.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out y)) {
                MessageBox.Show("y value of gravity is not valied", "invalid gravity", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            gravity = new Vector2(x, y);
            return true;
        }
    }
}
