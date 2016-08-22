using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
using mapKnight.Core;

namespace mapKnight.ToolKit {
    /// <summary>
    /// Interaktionslogik für AddTileWindow.xaml
    /// </summary>
    public partial class AddTileWindow : Window {
        public Tuple<Tile, BitmapImage> Created;
        public string Path;
        private IEnumerable<string> forbiddenNames;

#if log
        public AddTileWindow ( ) {
            InitializeComponent( );
        }
#endif

        public AddTileWindow (string path, IEnumerable<string> forbiddennames) {
            Path = path;
            forbiddenNames = forbiddennames;
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

        private void button_submit_Click (object sender, RoutedEventArgs e) {
            if (forbiddenNames.Contains(textbox_name.Text)) {
                MessageBox.Show("name is allready present", "invalid name", MessageBoxButton.OK, MessageBoxImage.Error);
            } else {
                Created = new Tuple<Tile, BitmapImage>(
                    new Tile( ) { Name = textbox_name.Text, Attributes = new Dictionary<TileAttribute, string>( ) },
                    (BitmapImage)image_tile.Source);
                DialogResult = true;
            }
        }
    }
}
