using mapKnight.Core;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Point = System.Windows.Point;

namespace mapKnight.ToolKit {
    /// <summary>
    /// Interaktionslogik für MapEditor.xaml
    /// </summary>
    public partial class MapEditor : UserControl {
        private List<UIElement> _Menu = new List<UIElement>( ) {
            new MenuItem( ) { Header = "MAP", Items = {
                    new MenuItem() { Header = "NEW" , Icon = App.Current.FindResource("add_image") },
                    new MenuItem() { Header = "LOAD" }
                } },
            new ComboBox() { Width = 130 }
        };

        public List<UIElement> Menu { get { return _Menu; } }
        public Dictionary<Map, Dictionary<string, BitmapImage>> Images = new Dictionary<Map, Dictionary<string, BitmapImage>>( );

        private Map CurrentMap { get { return (Map)((ComboBox)_Menu[1]).SelectedItem; } }
        private Tile CurrentTile { get { return (wrappanel_tiles.SelectedIndex != -1) ? CurrentMap.Tiles[wrappanel_tiles.SelectedIndex] : CurrentMap.Tiles[0]; } }
        private int CurrentMapIndex { get { return (int)((ComboBox)_Menu[1]).SelectedIndex; } }
        private int CurrentTileIndex { get { return wrappanel_tiles.SelectedIndex; } }

        public MapEditor( ) {
            InitializeComponent( );

            ((MenuItem)((MenuItem)_Menu[0]).Items[0]).Click += create_map_Click;
            ((ComboBox)_Menu[1]).SelectionChanged += CurrentMapChanged;
        }

        private void CurrentMapChanged(object sender, SelectionChangedEventArgs e) {
            UpdateListbox( );
            tilemapview.CurrentMap = CurrentMap;
            // reset scrollbars
            scrollbar_horizontal.Value = 0;
            scrollbar_horizontal.Minimum = 0;
            scrollbar_horizontal.Maximum = 4;
            scrollbar_horizontal.ViewportSize = scrollbar_horizontal.ActualWidth / (scrollbar_horizontal.Maximum - scrollbar_horizontal.Minimum);
            scrollbar_vertical.Value = 0;
            scrollbar_vertical.Minimum = 0;
            scrollbar_vertical.Maximum = 4;
        }

        private void create_map_Click(object sender, RoutedEventArgs e) {
            CreateMapWindow createWindow = new CreateMapWindow( );
            if (createWindow.ShowDialog( ) ?? false) {
                AddMap(createWindow.CreatedMap);
            }
        }

        private void AddMap(Map map) {
            MemoryStream memoryStream = new MemoryStream( );
            memoryStream.Position = 0;
            new Bitmap(1, 1).Save(memoryStream, ImageFormat.Png);
            BitmapImage emptyImage = new BitmapImage( );
            emptyImage.BeginInit( );
            emptyImage.StreamSource = memoryStream;
            emptyImage.EndInit( );

            tilemapview.AddTexture(map, "None", emptyImage);
            Images.Add(map, new Dictionary<string, BitmapImage>( ));
            Images[map].Add("None", emptyImage);

            ((ComboBox)_Menu[1]).Items.Add(map);
            ((ComboBox)_Menu[1]).SelectedIndex = ((ComboBox)_Menu[1]).Items.Count - 1;

            UpdateListbox( );
        }

        public void UpdateListbox( ) {
            wrappanel_tiles.Items.Clear( );
            foreach (Tile tile in CurrentMap.Tiles) {
                wrappanel_tiles.Items.Add(new ListViewEntry(Images[CurrentMap][tile.Name]));
            }
        }

        private void wrappanel_tiles_DragEnter(object sender, DragEventArgs e) {
            if (CurrentMap == null)
                return;

            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effects = DragDropEffects.Copy;
        }

        private void wrappanel_tiles_Drop(object sender, DragEventArgs e) {
            if (CurrentMap == null)
                return;

            if (e.Data.GetDataPresent(DataFormats.FileDrop)) {
                string[ ] files = (string[ ])e.Data.GetData(DataFormats.FileDrop);
                foreach (string file in files) {
                    if (Path.GetExtension(file) == ".png") {
                        AddTileWindow addTileDialog = new AddTileWindow(file);
                        if (addTileDialog.ShowDialog( ) ?? false) {
                            if (CurrentMap.Tiles.Where(t => t.Name == addTileDialog.Created.Item1.Name) != null) {
                                CurrentMap.AddTile(addTileDialog.Created.Item1);
                                tilemapview.AddTexture(addTileDialog.Created.Item1.Name, addTileDialog.Created.Item2);
                                Images[CurrentMap].Add(addTileDialog.Created.Item1.Name, addTileDialog.Created.Item2);
                                wrappanel_tiles.Items.Add(new ListViewEntry(addTileDialog.Created.Item2));
                            } else {
                                MessageBox.Show("Please don't add tiles with the same name twice!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                    }
                }
            }
        }

        private void tilemapview_MouseDown(object sender, MouseButtonEventArgs e) {
            if (CurrentMap == null || CurrentTileIndex == -1)
                return;

            if (e.LeftButton == MouseButtonState.Pressed) {
                Point positionInMap = e.GetPosition(tilemapview);
                Point clickedTile = new Point(positionInMap.X / tilemapview.TileSize + tilemapview.Offset.X, positionInMap.Y / tilemapview.TileSize + tilemapview.Offset.Y);
                if (clickedTile.X >= CurrentMap.Width || clickedTile.Y >= CurrentMap.Height)
                    return;
                CurrentMap.Data[(int)clickedTile.X, (int)clickedTile.Y, 0] = CurrentTileIndex;
                tilemapview.Update = true;
            }
        }

        private void tilemapview_MouseMove(object sender, MouseEventArgs e) {
            if (CurrentMap == null || CurrentTileIndex == -1)
                return;

            if (e.LeftButton == MouseButtonState.Pressed) {
                Point positionInMap = e.GetPosition(tilemapview);
                Point clickedTile = new Point(positionInMap.X / tilemapview.TileSize + tilemapview.Offset.X, positionInMap.Y / tilemapview.TileSize + tilemapview.Offset.Y);
                if (clickedTile.X >= CurrentMap.Width || clickedTile.Y >= CurrentMap.Height)
                    return;
                CurrentMap.Data[(int)clickedTile.X, (int)clickedTile.Y, 0] = CurrentTileIndex;
                tilemapview.Update = true;
            }
        }

        private void scrollbar_horizontal_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
            tilemapview.Offset = new Microsoft.Xna.Framework.Point((int)scrollbar_horizontal.Value, tilemapview.Offset.Y);
        }

        private void scrollbar_vertical_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
            tilemapview.Offset = new Microsoft.Xna.Framework.Point(tilemapview.Offset.X, (int)scrollbar_vertical.Value);
        }

        private struct ListViewEntry {
            public ListViewEntry(BitmapImage image) {
                Image = image;
            }

            public BitmapImage Image { get; private set; }
        }
    }
}
