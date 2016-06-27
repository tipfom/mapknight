using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using mapKnight.Core;
using Microsoft.Win32;
using Brushes = System.Windows.Media.Brushes;
using Image = System.Windows.Controls.Image;
using Point = System.Windows.Point;

namespace mapKnight.ToolKit {
    /// <summary>
    /// Interaktionslogik für MapEditor.xaml
    /// </summary>
    public partial class MapEditor : UserControl {
        private enum Tool {
            Pen,
            Eraser,
            Filler,
            Pointer,
            Rotater
        }

        private List<UIElement> _Menu = new List<UIElement>( ) {
            new MenuItem( ) { Header = "MAP", Items = {
                    new MenuItem() { Header = "NEW", Height = 22, Icon = App.Current.FindResource("image_map_new") },
                    new MenuItem() { Header = "LOAD", Height = 22 }
                } },
            new ComboBox() { Width = 260, Margin = new Thickness(-6, 0, -6, 0), VerticalAlignment = VerticalAlignment.Center },
            new MenuItem() { Header = "SHOW LAYER", IsEnabled = false },
            new CheckBox() { IsChecked = true, Margin = new Thickness(-2, 0, -2, 0), VerticalAlignment = VerticalAlignment.Center },
            new CheckBox() { IsChecked = true, Margin = new Thickness(-2, 0, -2, 0), VerticalAlignment = VerticalAlignment.Center },
            new CheckBox() { IsChecked = true, Margin = new Thickness(-2, 0, -2, 0), VerticalAlignment = VerticalAlignment.Center },
            new Separator() { },
            new MenuItem() {Header ="MODIFY LAYER",IsEnabled =false },
            new RadioButton() {IsChecked = false, GroupName="modifylayer", Margin = new Thickness(-2, 0, -2, 0), VerticalAlignment = VerticalAlignment.Center },
            new RadioButton() {IsChecked = true, GroupName="modifylayer", Margin = new Thickness(-2, 0, -2, 0), VerticalAlignment = VerticalAlignment.Center },
            new RadioButton() {IsChecked = false, GroupName="modifylayer", Margin = new Thickness(-2, 0, -2, 0), VerticalAlignment = VerticalAlignment.Center },
            new Separator(),
            new Border() { Child = (Image)App.Current.FindResource("image_map_undo"), Margin = new Thickness(-6, 0, -6, 0), Padding = new Thickness(6, 0, 6, 0) },
            new Border() { Child = (Image)App.Current.FindResource("image_map_pen"), BorderBrush = Brushes.DodgerBlue, BorderThickness=  new Thickness(1), Margin = new Thickness(-6, 0, -6, 0), Padding = new Thickness(6, 0, 6, 0) },
            new Border() { Child = (Image)App.Current.FindResource("image_map_eraser"), BorderBrush = Brushes.DodgerBlue, BorderThickness=  new Thickness(0), Margin = new Thickness(-6, 0, -6,0), Padding = new Thickness(6, 0, 6, 0) },
            new Border() { Child = (Image)App.Current.FindResource("image_map_fill"), BorderBrush = Brushes.DodgerBlue, BorderThickness=  new Thickness(0), Margin = new Thickness(-6, 0, -6 ,0), Padding = new Thickness(6, 0, 6, 0) },
            new Border() { Child = (Image)App.Current.FindResource("image_map_pointer"), BorderBrush = Brushes.DodgerBlue, BorderThickness = new Thickness(0), Margin = new Thickness(-6, 0, -6, 0), Padding = new Thickness(6, 0, 6, 0)},
            new Border() { Child = (Image)App.Current.FindResource("image_map_rotate"), BorderBrush = Brushes.DodgerBlue, BorderThickness = new Thickness(0), Margin = new Thickness(-6, 0, -6, 0), Padding = new Thickness(6, 0, 6, 0)}
        };

        public List<UIElement> Menu { get { return _Menu; } }

        private Map currentMap { get { return (Map)((ComboBox)_Menu[1]).SelectedItem; } }
        private Tile currentTile { get { return (wrappanel_tiles.SelectedIndex != -1) ? currentMap.Tiles[wrappanel_tiles.SelectedIndex] : currentMap.Tiles[0]; } }
        private int currentMapIndex { get { return (int)((ComboBox)_Menu[1]).SelectedIndex; } }
        private int currentTileIndex { get { return wrappanel_tiles.SelectedIndex; } }
        private int currentlyEditionTilesMap = -1;
        private int currentlyEditingTile = -1;
        private int currentLayer = 1;
        //                 Map  Cache List von   Koord  Layer alter Wert IsRotation
        private Dictionary<Map, Stack<List<Tuple<Point, int, int, bool>>>> Cache = new Dictionary<Map, Stack<List<Tuple<Point, int, int, bool>>>>( );
        private Tool currentTool = Tool.Pen;
        private Point lastClickedTile = new Point(-1, -1);

        public MapEditor ( ) {
            InitializeComponent( );

            ((MenuItem)((MenuItem)_Menu[0]).Items[0]).Click += create_map_Click;
            ((MenuItem)((MenuItem)_Menu[0]).Items[1]).Click += load_map_Click;
            ((ComboBox)_Menu[1]).SelectionChanged += CurrentMapChanged;

            ((CheckBox)_Menu[3]).Checked += (sender, e) => { tilemapview.ShowBackground = true; };
            ((CheckBox)_Menu[3]).Unchecked += (sender, e) => { tilemapview.ShowBackground = false; };
            ((CheckBox)_Menu[4]).Checked += (sender, e) => { tilemapview.ShowMiddle = true; };
            ((CheckBox)_Menu[4]).Unchecked += (sender, e) => { tilemapview.ShowMiddle = false; };
            ((CheckBox)_Menu[5]).Checked += (sender, e) => { tilemapview.ShowForeground = true; };
            ((CheckBox)_Menu[5]).Unchecked += (sender, e) => { tilemapview.ShowForeground = false; };

            ((RadioButton)_Menu[8]).Checked += (sender, e) => { currentLayer = 0; };
            ((RadioButton)_Menu[9]).Checked += (sender, e) => { currentLayer = 1; };
            ((RadioButton)_Menu[10]).Checked += (sender, e) => { currentLayer = 2; };

            _Menu[12].MouseDown += (sender, e) => {
                UndoLast( );
            };
            _Menu[13].MouseDown += (sender, e) => {
                currentTool = Tool.Pen;
                ResetToolBorders( );
                ((Border)_Menu[13]).BorderThickness = new Thickness(1);
            };
            _Menu[14].MouseDown += (sender, e) => {
                currentTool = Tool.Eraser;
                ResetToolBorders( );
                ((Border)_Menu[14]).BorderThickness = new Thickness(1);
            };
            _Menu[15].MouseDown += (sender, e) => {
                currentTool = Tool.Filler;
                ResetToolBorders( );
                ((Border)_Menu[15]).BorderThickness = new Thickness(1);
            };
            _Menu[16].MouseDown += (sender, e) => {
                currentTool = Tool.Pointer;
                ResetToolBorders( );
                ((Border)_Menu[16]).BorderThickness = new Thickness(1);
            };
            _Menu[17].MouseDown += (sender, e) => {
                currentTool = Tool.Rotater;
                ResetToolBorders( );
                ((Border)_Menu[17]).BorderThickness = new Thickness(1);
            };

            App.ProjectChanged += ( ) => {
                App.Project.MapAdded += Project_MapAdded;
                Reset( );
            };
        }

        private void Reset ( ) {
            ((ComboBox)_Menu[1]).Items.Clear( );
            Cache.Clear( );
            currentlyEditingTile = -1;
            currentlyEditionTilesMap = -1;
            wrappanel_tiles.Items.Clear( );
            if (tilemapview.CurrentMap != null)
                tilemapview.CurrentMap = null;
            foreach (Map map in App.Project.GetMaps( )) {
                Project_MapAdded(map);
            }
        }

        private void ResetToolBorders ( ) {
            ((Border)_Menu[13]).BorderThickness = new Thickness(0);
            ((Border)_Menu[14]).BorderThickness = new Thickness(0);
            ((Border)_Menu[15]).BorderThickness = new Thickness(0);
            ((Border)_Menu[16]).BorderThickness = new Thickness(0);
            ((Border)_Menu[17]).BorderThickness = new Thickness(0);
        }

        private void UndoLast ( ) {
            if (currentMap == null || Cache[currentMap].Count == 0)
                return;

            foreach (Tuple<Point, int, int, bool> undoData in Cache[currentMap].Pop( )) {
                if (undoData.Item4) {
                    // tile got rotated
                    currentMap.SetRotation((int)undoData.Item1.X, (int)undoData.Item1.Y, undoData.Item2, undoData.Item3 / 2f);
                } else {
                    // tile got changed
                    currentMap.Data[(int)undoData.Item1.X, (int)undoData.Item1.Y, undoData.Item2] = undoData.Item3;
                }
            }
            tilemapview.Update( );
        }

        private void Project_MapAdded (Map obj) {
            MemoryStream memoryStream = new MemoryStream( );
            memoryStream.Position = 0;
            new Bitmap(1, 1).Save(memoryStream, ImageFormat.Png);
            BitmapImage emptyImage = new BitmapImage( );
            emptyImage.BeginInit( );
            emptyImage.StreamSource = memoryStream;
            emptyImage.EndInit( );

            if ((obj.Tiles?.Length ?? 0) == 0) {
                App.Project.AddTexture(obj, "None", emptyImage);
                obj.Tiles = new Tile[ ] { new Tile( ) { Name = "None", Attributes = new Dictionary<TileAttribute, string>( ) } };
            }

            ((ComboBox)_Menu[1]).Items.Add(obj);
            ((ComboBox)_Menu[1]).SelectedIndex = ((ComboBox)_Menu[1]).Items.Count - 1;

            UpdateListbox( );
            Cache.Add(obj, new Stack<List<Tuple<Point, int, int, bool>>>( ));
        }

        private void CurrentMapChanged (object sender, SelectionChangedEventArgs e) {
            if (currentMap == null)
                return;

            UpdateListbox( );
            tilemapview.CurrentMap = currentMap;
            // reset scrollbars
            scrollbar_horizontal.Value = 0;
            scrollbar_horizontal.Minimum = 0;
            scrollbar_horizontal.Maximum = currentMap.Width;
            scrollbar_vertical.Value = 0;
            scrollbar_vertical.Minimum = 0;
            scrollbar_vertical.Maximum = currentMap.Height;
        }

        private void create_map_Click (object sender, RoutedEventArgs e) {
            new CreateMapWindow(tilemapview.GraphicsDevice).ShowDialog( );
            if (currentMap != null)
                UpdateListbox( );
        }

        private void load_map_Click (object sender, RoutedEventArgs e) {
            OpenFileDialog mapOpenDialog = new OpenFileDialog( );
            mapOpenDialog.Filter = "MAP-Files|*.map";
            mapOpenDialog.CheckFileExists = true;
            if (mapOpenDialog.ShowDialog( ) ?? false) {
                App.Project.LoadMap(mapOpenDialog.FileName);
            }
        }

        public void UpdateListbox ( ) {
            wrappanel_tiles.Items.Clear( );
            foreach (Tile tile in currentMap.Tiles) {
                wrappanel_tiles.Items.Add(new ListViewEntry(App.Project.GetMapWPFTextures(currentMap)[tile.Name]));
            }
            if (wrappanel_tiles.HasItems)
                wrappanel_tiles.SelectedIndex = 0;
        }

        private void wrappanel_tiles_DragEnter (object sender, DragEventArgs e) {
            if (currentMap == null)
                return;

            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effects = DragDropEffects.Copy;
        }

        private void wrappanel_tiles_Drop (object sender, DragEventArgs e) {
            if (currentMap == null)
                return;

            if (e.Data.GetDataPresent(DataFormats.FileDrop)) {
                string[ ] files = (string[ ])e.Data.GetData(DataFormats.FileDrop);
                foreach (string file in files) {
                    if (Path.GetExtension(file) == ".png") {
                        App.Project.HasChanged = true;
                        if (checkbox_auto.IsChecked ?? false && currentMap.Tiles.Where(t => t.Name == Path.GetFileNameWithoutExtension(file)) != null) {
                            // add tile
                            string tileName = Path.GetFileNameWithoutExtension(file);
                            BitmapImage tileImage = new BitmapImage( );
                            tileImage.BeginInit( );
                            tileImage.CacheOption = BitmapCacheOption.OnLoad;
                            tileImage.CreateOptions = BitmapCreateOptions.None;
                            tileImage.DecodePixelWidth = Map.TILE_PXL_SIZE;
                            tileImage.DecodePixelHeight = Map.TILE_PXL_SIZE;
                            tileImage.UriSource = new Uri(file);
                            tileImage.EndInit( );

                            App.Project.AddTexture(currentMap, tileName, tileImage);
                            currentMap.AddTile(new Tile( ) { Attributes = new Dictionary<TileAttribute, string>( ), Name = tileName });
                            wrappanel_tiles.Items.Add(new ListViewEntry(tileImage));
                        } else {
                            // open add tile window
                            AddTileWindow addTileDialog = new AddTileWindow(file, currentMap.Tiles.Select(t => t.Name));
                            if (addTileDialog.ShowDialog( ) ?? false) {
                                if (currentMap.Tiles.Where(t => t.Name == addTileDialog.Created.Item1.Name) != null) {
                                    App.Project.AddTexture(currentMap, addTileDialog.Created.Item1.Name, addTileDialog.Created.Item2);
                                    currentMap.AddTile(addTileDialog.Created.Item1);
                                    wrappanel_tiles.Items.Add(new ListViewEntry(addTileDialog.Created.Item2));
                                } else {
                                    MessageBox.Show("Please don't add tiles with the same name twice!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void tilemapview_MouseDown (object sender, MouseButtonEventArgs e) {
            if (currentMap == null || currentTileIndex == -1 || !tilemapview.IsLayerActive(currentLayer))
                return;

            Point clickedTile;
            if (GetClickedTile(e, out clickedTile) && (e.LeftButton == MouseButtonState.Pressed || e.RightButton == MouseButtonState.Pressed)) {
                lastClickedTile = clickedTile;
                if (e.RightButton == MouseButtonState.Pressed) {
                    Cache[currentMap].Push(new List<Tuple<Point, int, int, bool>>( ) {
                                    new Tuple<Point, int, int,bool>(clickedTile,currentLayer, currentMap.Data[(int)clickedTile.X, (int)clickedTile.Y, currentLayer], false)});
                    currentMap.Data[(int)clickedTile.X, (int)clickedTile.Y, currentLayer] = 0;
                } else if (e.LeftButton == MouseButtonState.Pressed) {
                    switch (currentTool) {
                        case Tool.Eraser:
                            Cache[currentMap].Push(new List<Tuple<Point, int, int, bool>>( ) {
                                    new Tuple<Point, int, int,bool>(clickedTile,currentLayer, currentMap.Data[(int)clickedTile.X, (int)clickedTile.Y, currentLayer], false)});
                            currentMap.Data[(int)clickedTile.X, (int)clickedTile.Y, currentLayer] = 0;
                            break;
                        case Tool.Filler:
                            if (currentMap.Data[(int)clickedTile.X, (int)clickedTile.Y, currentLayer] == currentTileIndex)
                                break;
                            int searching = currentMap.Data[(int)clickedTile.X, (int)clickedTile.Y, currentLayer];
                            int replacing = currentTileIndex;
                            Queue<Point> pointQueue = new Queue<Point>( );
                            pointQueue.Enqueue(clickedTile);
                            List<Tuple<Point, int, int, bool>> changesForCache = new List<Tuple<Point, int, int, bool>>( );
                            while (pointQueue.Count > 0) {
                                Point current = pointQueue.Dequeue( );
                                if (current.X < 0 || current.X >= currentMap.Width || current.Y < 0 || current.Y >= currentMap.Height)
                                    continue;
                                if (currentMap.Data[(int)current.X, (int)current.Y, currentLayer] == searching) {
                                    currentMap.Data[(int)current.X, (int)current.Y, currentLayer] = replacing;
                                    changesForCache.Add(new Tuple<Point, int, int, bool>(current, currentLayer, searching, false));

                                    pointQueue.Enqueue(new Point(current.X - 1, current.Y));
                                    pointQueue.Enqueue(new Point(current.X + 1, current.Y));
                                    pointQueue.Enqueue(new Point(current.X, current.Y - 1));
                                    pointQueue.Enqueue(new Point(current.X, current.Y + 1));
                                }
                            }
                            Cache[currentMap].Push(changesForCache);
                            break;
                        case Tool.Pen:
                            Cache[currentMap].Push(new List<Tuple<Point, int, int, bool>>( ) {
                                    new Tuple<Point, int, int, bool>(clickedTile,currentLayer, currentMap.Data[(int)clickedTile.X, (int)clickedTile.Y, currentLayer], false)});
                            currentMap.Data[(int)clickedTile.X, (int)clickedTile.Y, currentLayer] = currentTileIndex;
                            break;
                        case Tool.Pointer:
                            currentMap.SpawnPoint = new Vector2((int)clickedTile.X, (int)clickedTile.Y);
                            break;
                        case Tool.Rotater:
                            float tileRotation = currentMap.GetRotation((int)clickedTile.X, (int)clickedTile.Y, currentLayer);
                            Cache[currentMap].Push(new List<Tuple<Point, int, int, bool>>( ) {
                                    new Tuple<Point, int, int, bool>(clickedTile,currentLayer, (int)(tileRotation * 2), true)});
                            tileRotation += 0.5f;
                            tileRotation %= 2f;
                            currentMap.SetRotation((int)clickedTile.X, (int)clickedTile.Y, currentLayer, tileRotation);
                            break;
                    }
                }
                App.Project.HasChanged = e.LeftButton == MouseButtonState.Pressed || e.RightButton == MouseButtonState.Pressed;
                tilemapview.Update( );
            }
        }

        private void tilemapview_MouseMove (object sender, MouseEventArgs e) {
            if (currentMap == null)
                return;
            bool update = UpdateSelectedTile(e);
            if (currentTileIndex == -1 || !tilemapview.IsLayerActive(currentLayer))
                return;

            Point clickedTile;
            if (GetClickedTile(e, out clickedTile) && ((int)clickedTile.X != (int)lastClickedTile.X || (int)clickedTile.Y != (int)lastClickedTile.Y) && (e.LeftButton == MouseButtonState.Pressed || e.RightButton == MouseButtonState.Pressed)) {
                lastClickedTile = clickedTile;
                if (e.RightButton == MouseButtonState.Pressed) {
                    Cache[currentMap].Push(new List<Tuple<Point, int, int, bool>>( ) {
                                    new Tuple<Point, int, int,bool>(clickedTile,currentLayer, currentMap.Data[(int)clickedTile.X, (int)clickedTile.Y, currentLayer], false)});
                    currentMap.Data[(int)clickedTile.X, (int)clickedTile.Y, currentLayer] = 0;
                    update = true;
                } else if (e.LeftButton == MouseButtonState.Pressed) {
                    switch (currentTool) {
                        case Tool.Eraser:
                            Cache[currentMap].Push(new List<Tuple<Point, int, int, bool>>( ) {
                                    new Tuple<Point, int, int, bool>(clickedTile,currentLayer, currentMap.Data[(int)clickedTile.X, (int)clickedTile.Y, currentLayer], false)});
                            currentMap.Data[(int)clickedTile.X, (int)clickedTile.Y, currentLayer] = 0;
                            break;
                        case Tool.Pen:
                            Cache[currentMap].Push(new List<Tuple<Point, int, int, bool>>( ) {
                                    new Tuple<Point, int, int, bool>(clickedTile,currentLayer, currentMap.Data[(int)clickedTile.X, (int)clickedTile.Y, currentLayer], false)});
                            currentMap.Data[(int)clickedTile.X, (int)clickedTile.Y, currentLayer] = currentTileIndex;
                            break;
                        case Tool.Pointer:
                            currentMap.SpawnPoint = new Vector2((int)clickedTile.X, (int)clickedTile.Y);
                            break;
                    }
                    update = update || (currentTool != Tool.Filler && currentTool != Tool.Rotater);
                }
            }
            App.Project.HasChanged = update && ((e.LeftButton == MouseButtonState.Pressed && currentTool != Tool.Filler) || e.RightButton == MouseButtonState.Pressed);
            if (update)
                tilemapview.Update( );
        }

        private bool GetClickedTile (MouseEventArgs e, out Point tile) {
            Point positionOnControl = e.GetPosition(tilemapview);
            tile = new Point(
                    positionOnControl.X / tilemapview.TileSize + tilemapview.Offset.X,
                    currentMap.Height - positionOnControl.Y / tilemapview.TileSize - tilemapview.Offset.Y
                );
            return (tile.X >= 0 && tile.X < currentMap.Width && tile.Y >= 0 && tile.Y < currentMap.Height);
        }

        private void scrollbar_horizontal_ValueChanged (object sender, RoutedPropertyChangedEventArgs<double> e) {
            tilemapview.Offset = new Microsoft.Xna.Framework.Point((int)scrollbar_horizontal.Value, tilemapview.Offset.Y);
        }

        private void scrollbar_vertical_ValueChanged (object sender, RoutedPropertyChangedEventArgs<double> e) {
            tilemapview.Offset = new Microsoft.Xna.Framework.Point(tilemapview.Offset.X, (int)scrollbar_vertical.Value);
        }

        private void tilemapview_MouseLeave (object sender, MouseEventArgs e) {
            tilemapview.CurrentSelection = new Microsoft.Xna.Framework.Point(-1, -1);
            tilemapview.Update( );
        }

        private void tilemapview_MouseEnter (object sender, MouseEventArgs e) {
            if (currentMap != null)
                UpdateSelectedTile(e);
        }

        private bool UpdateSelectedTile (MouseEventArgs e) {
            Point positionOnControl = e.GetPosition(tilemapview);
            Microsoft.Xna.Framework.Point selectedTile = new Microsoft.Xna.Framework.Point(
                (int)Math.Min(positionOnControl.X / tilemapview.TileSize, currentMap.Width - tilemapview.Offset.X - 1),
                (int)Math.Min(positionOnControl.Y / tilemapview.TileSize, currentMap.Height - tilemapview.Offset.Y - 1)
                );
            if (selectedTile.X != tilemapview.CurrentSelection.X || selectedTile.Y != tilemapview.CurrentSelection.Y) {
                tilemapview.CurrentSelection = selectedTile;
                return true;
            } else
                return false;
        }

        private void buttonexport_Click (object sender, RoutedEventArgs e) {
            // export current maps tileset
            SaveFileDialog exportDialog = new SaveFileDialog( );
            exportDialog.Filter = "TileTemplate|*.mkttemplate";
            if (exportDialog.ShowDialog( ) ?? false) {
                using (Stream stream = File.OpenWrite(exportDialog.FileName))
                    TileSerializer.Serialize(stream, currentMap.Tiles, App.Project.GetMapXNATextures(currentMap), tilemapview.GraphicsDevice);
            }
        }

        private void wrappanel_tiles_Selected (object sender, RoutedEventArgs e) {
            if (currentlyEditingTile >= 0 || currentlyEditionTilesMap >= 0) {
                if (App.Project.GetMaps( )[currentlyEditionTilesMap].Tiles.Where(t => t.Name == textbox_tile_name.Text) != null) {
                    App.Project.ChangeTextureName(App.Project.GetMaps( )[currentlyEditionTilesMap], App.Project.GetMaps( )[currentlyEditionTilesMap].Tiles[currentlyEditingTile].Name, textbox_tile_name.Text);
                    App.Project.GetMaps( )[currentlyEditionTilesMap].Tiles[currentlyEditingTile].Name = textbox_tile_name.Text;
                }
                App.Project.GetMaps( )[currentlyEditionTilesMap].Tiles[currentlyEditingTile].Attributes.Clear( );
                foreach (AttributeListViewEntry entry in listview_tile_attributes.Items) {
                    if (entry.Active)
                        App.Project.GetMaps( )[currentlyEditionTilesMap].Tiles[currentlyEditingTile].Attributes.Add((TileAttribute)Enum.Parse(typeof(TileAttribute), entry.Attribute), entry.Value);
                }
            }
            if (currentMapIndex == -1 || currentTileIndex == -1)
                return;
            currentlyEditionTilesMap = currentMapIndex;
            currentlyEditingTile = currentTileIndex;
            textbox_tile_name.Text = currentTile.Name;
            listview_tile_attributes.Items.Clear( );
            foreach (TileAttribute attribute in Enum.GetValues(typeof(TileAttribute))) {
                if (currentTile.HasFlag(attribute)) {
                    listview_tile_attributes.Items.Add(new AttributeListViewEntry(true, attribute.ToString( ), currentTile.Attributes[attribute]));
                } else {
                    listview_tile_attributes.Items.Add(new AttributeListViewEntry(false, attribute.ToString( ), ""));
                }
            }
        }

        private void tilemapview_PreviewMouseWheel (object sender, MouseWheelEventArgs e) {
            if (e.Delta > 0) {
                tilemapview.ZoomLevel++;
            } else {
                tilemapview.ZoomLevel--;
            }
        }

        #region templates
        private struct ListViewEntry {
            public ListViewEntry (BitmapImage image) {
                Image = image;
            }

            public BitmapImage Image { get; private set; }
        }

        private class AttributeListViewEntry {

            public AttributeListViewEntry (bool active, string attribute, string value) {
                Active = active;
                Attribute = attribute;
                Value = value;
            }

            public bool Active { get; set; }
            public string Attribute { get; set; }
            public string Value { get; set; }
        }
        #endregion
    }
}
