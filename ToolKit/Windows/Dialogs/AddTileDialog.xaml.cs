using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
using mapKnight.Core;

namespace mapKnight.ToolKit.Windows {
    /// <summary>
    /// Interaktionslogik für AddTileWindow.xaml
    /// </summary>
    public partial class AddTileWindow : Window {
        public Tuple<Tile, BitmapImage> Created;
        public string Path;
        private IEnumerable<string> forbiddenNames;
        private Dictionary<TileAttribute, string> defaultAttributes;

        public AddTileWindow (string path, IEnumerable<string> forbiddennames, Dictionary<TileAttribute, string> defaultattributes) {
            Path = path;
            forbiddenNames = forbiddennames;
            defaultAttributes = defaultattributes;
            InitializeComponent( );
            this.Owner = App.Current.MainWindow;

            BitmapImage image = new BitmapImage( );
            image.BeginInit( );
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.CreateOptions = BitmapCreateOptions.None;
            image.DecodePixelWidth = Map.TILE_PXL_SIZE;
            image.DecodePixelHeight = Map.TILE_PXL_SIZE;
            image.UriSource = new Uri(path);
            image.EndInit( );

            image_tile.Source = image;
            textbox_name.Text = System.IO.Path.GetFileNameWithoutExtension(path);
        }

        private void Button_Submit_Click (object sender, RoutedEventArgs e) {
            if (forbiddenNames.Contains(textbox_name.Text)) {
                MessageBox.Show("name is allready present", "invalid name", MessageBoxButton.OK, MessageBoxImage.Error);
            } else {
                Created = new Tuple<Tile, BitmapImage>(
                    new Tile( ) { Name = textbox_name.Text, Attributes = new Dictionary<TileAttribute, string>(defaultAttributes) },
                    (BitmapImage)image_tile.Source);
                DialogResult = true;
            }
        }
    }
}
