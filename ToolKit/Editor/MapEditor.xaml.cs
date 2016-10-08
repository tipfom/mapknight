using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using mapKnight.Core;
using mapKnight.ToolKit.Controls.Xna;
using mapKnight.ToolKit.Serializer;
using mapKnight.ToolKit.Windows;
using Microsoft.Win32;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using Brushes = System.Windows.Media.Brushes;
using Image = System.Windows.Controls.Image;
using Point = System.Windows.Point;

namespace mapKnight.ToolKit.Editor {

    /// <summary>
    /// Interaktionslogik für MapEditor.xaml
    /// </summary>
    public partial class MapEditor : UserControl {
        private static Style imageStyle = new Style(typeof(Image)) { Triggers = { new Trigger( ) { Value = false, Property = IsEnabledProperty, Setters = { new Setter(Image.OpacityProperty, 0.5) } } } };

        private List<FrameworkElement> _Menu = new List<FrameworkElement>( ) {
            new MenuItem( ) { Header = "MAP", Items = {
                    new MenuItem() { Header = "NEW", Height = 22, Icon = App.Current.FindResource("image_map_new") },
                    new MenuItem() { Header = "LOAD", Height = 22 }
                } },
            new ComboBox() { Width = 260, Margin = new Thickness(-6, 0, -6, 0), VerticalAlignment = VerticalAlignment.Center },
            new Image() { Source= (BitmapImage)App.Current.FindResource("image_map_mapsettings"), Style = imageStyle },
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
            new Border() { Child = new Image() { Source = (BitmapImage)App.Current.FindResource("image_map_undo"), Style = imageStyle }, Margin = new Thickness(-6, 0, -6, 0), Padding = new Thickness(6, 0, 6, 0), ToolTip = new ToolTip() { Content = "Ctrl + Z" } },
            new Border() { Child = new Image() { Source = (BitmapImage)App.Current.FindResource("image_map_pen"), Style = imageStyle }, BorderBrush = Brushes.DodgerBlue, BorderThickness=  new Thickness(1), Margin = new Thickness(-6, 0, -6, 0), Padding = new Thickness(6, 0, 6, 0), ToolTip = new ToolTip() { Content = "Alt + A" } },
            new Border() { Child = new Image() { Source = (BitmapImage)App.Current.FindResource("image_map_eraser"), Style = imageStyle }, BorderBrush = Brushes.DodgerBlue, BorderThickness=  new Thickness(0), Margin = new Thickness(-6, 0, -6,0), Padding = new Thickness(6, 0, 6, 0), ToolTip = new ToolTip() { Content = "Alt + S" } },
            new Border() { Child = new Image() { Source = (BitmapImage)App.Current.FindResource("image_map_fill"), Style = imageStyle }, BorderBrush = Brushes.DodgerBlue, BorderThickness=  new Thickness(0), Margin = new Thickness(-6, 0, -6 ,0), Padding = new Thickness(6, 0, 6, 0), ToolTip = new ToolTip() { Content = "Ctrl + D" } },
            new Border() { Child = new Image() { Source = (BitmapImage)App.Current.FindResource("image_map_pointer"), Style = imageStyle }, BorderBrush = Brushes.DodgerBlue, BorderThickness = new Thickness(0), Margin = new Thickness(-6, 0, -6, 0), Padding = new Thickness(6, 0, 6, 0) },
            new Border() { Child = new Image() { Source = (BitmapImage)App.Current.FindResource("image_map_rotate"), Style = imageStyle }, BorderBrush = Brushes.DodgerBlue, BorderThickness = new Thickness(0), Margin = new Thickness(-6, 0, -6, 0), Padding = new Thickness(6, 0, 6, 0), ToolTip = new ToolTip() { Content = "Ctrl + F" } }
        };

        // Map Cache List von Koord Layer alter Wert IsRotation
        private Dictionary<Map, Stack<List<Tuple<Point, int, int, bool>>>> Cache = new Dictionary<Map, Stack<List<Tuple<Point, int, int, bool>>>>( );

        private int currentLayer = 1;
        private int currentlyEditingTile = -1;
        private int currentlyEditionTilesMap = -1;
        private Tool currentTool = Tool.Pen;

        private Dictionary<TileAttribute, string> defaultAttributes = new Dictionary<TileAttribute, string>( );

        public void Load (Project project) {
            xnaTextures.Clear( );
            wpfTextures.Clear( );
            mapRotations.Clear( );

            foreach (string mapfile in project.GetAllEntries("maps")) {
                if (Path.GetExtension(mapfile) == ".map") {
                    string dir = Path.GetDirectoryName(mapfile);
                    string name = Path.GetFileNameWithoutExtension(mapfile);

                    using (Stream mapStream = project.GetOrCreateStream(mapfile)) {
                        Map map = LoadMap(mapStream);
                        using (Stream imageStream = project.GetOrCreateStream(Path.Combine(dir, name + ".png")))
                            LoadImages(imageStream, map);
                        AddMap(map);
                    }
                }
            }
        }

        public void Compile (string mappath) {
            foreach (Map map in GetMaps( )) {
                string basedirectory = Path.Combine(mappath, map.Name);
                if (!Directory.Exists(basedirectory)) Directory.CreateDirectory(basedirectory);
                // build texture
                Texture2D packedTexture = TileSerializer.BuildTexture(map.Tiles, xnaTextures[map], GraphicsDevice);
                using (Stream stream = File.Open(Path.Combine(basedirectory, map.Name + ".png"), FileMode.Create))
                    packedTexture.SaveAsPng(stream, packedTexture.Width, packedTexture.Height);

                map.Texture = Path.GetFileNameWithoutExtension(map.Name + ".png");
                using (Stream stream = File.Open(Path.Combine(basedirectory, map.Name + ".map"), FileMode.Create))
                    map.MergeRotations(mapRotations[map]).Serialize(stream);
            }
        }

        private GraphicsDevice GraphicsDevice;
        private Point lastClickedTile = new Point(-1, -1);
        private Dictionary<Map, float[ , , ]> mapRotations = new Dictionary<Map, float[ , , ]>( );
        private Dictionary<Map, Dictionary<string, BitmapImage>> wpfTextures = new Dictionary<Map, Dictionary<string, BitmapImage>>( );
        private Dictionary<Map, Dictionary<string, Texture2D>> xnaTextures = new Dictionary<Map, Dictionary<string, Texture2D>>( );

        public MapEditor ( ) {
            InitializeComponent( );
            defaultAttributes = JsonConvert.DeserializeObject<Dictionary<TileAttribute, string>>(Properties.Settings.Default.DefaultTileAttributes);

            for (int i = 1; i < _Menu.Count; i++) {
                if (_Menu[i].IsEnabled) {
                    _Menu[i].DataContext = this;
                    Binding binding = new Binding( ) { Path = new PropertyPath("IsEnabled") };
                    BindingOperations.SetBinding(_Menu[i], UIElement.IsEnabledProperty, binding);
                }
            }

            ((MenuItem)((MenuItem)_Menu[0]).Items[0]).Click += create_map_Click;
            ((MenuItem)((MenuItem)_Menu[0]).Items[1]).Click += load_map_Click;
            ((ComboBox)_Menu[1]).SelectionChanged += CurrentMapChanged;

            ((Image)_Menu[2]).MouseDown += (sender, e) => {
                if (currentMap == null) return;
                new ModifyMapWindow(currentMap).ShowDialog( );
            };

            ((CheckBox)_Menu[4]).Checked += (sender, e) => { tilemapview.ShowBackground = true; };
            ((CheckBox)_Menu[4]).Unchecked += (sender, e) => { tilemapview.ShowBackground = false; };
            ((CheckBox)_Menu[5]).Checked += (sender, e) => { tilemapview.ShowMiddle = true; };
            ((CheckBox)_Menu[5]).Unchecked += (sender, e) => { tilemapview.ShowMiddle = false; };
            ((CheckBox)_Menu[6]).Checked += (sender, e) => { tilemapview.ShowForeground = true; };
            ((CheckBox)_Menu[6]).Unchecked += (sender, e) => { tilemapview.ShowForeground = false; };

            ((RadioButton)_Menu[9]).Checked += (sender, e) => { currentLayer = 0; };
            ((RadioButton)_Menu[10]).Checked += (sender, e) => { currentLayer = 1; };
            ((RadioButton)_Menu[11]).Checked += (sender, e) => { currentLayer = 2; };

            _Menu[13].MouseDown += (sender, e) => UndoLast( );

            _Menu[14].MouseDown += (sender, e) => SelectTool(Tool.Pen);
            _Menu[15].MouseDown += (sender, e) => SelectTool(Tool.Eraser);
            _Menu[16].MouseDown += (sender, e) => SelectTool(Tool.Filler);
            _Menu[17].MouseDown += (sender, e) => SelectTool(Tool.Pointer);
            _Menu[18].MouseDown += (sender, e) => SelectTool(Tool.Rotater);

            App.Current.MainWindow.Closing += (sender, e) => {
                Properties.Settings.Default.DefaultTileAttributes = JsonConvert.SerializeObject(defaultAttributes);
            };

            IsVisibleChanged += (sender, e) => {
                if (GraphicsDevice != null)
                    return;
                if (IsVisible) {
                    HwndSource source = PresentationSource.FromVisual(this) as HwndSource;
                    if (source == null)
                        return;
                    GraphicsDevice = GraphicsDeviceService.AddRef(source.Handle).GraphicsDevice;
                }
            };

            tilemapview.SetReceiveFuncs(GetXNATextures, (Map m, int x, int y, int l) => { return mapRotations[m][x, y, l]; });
        }

        private enum Tool {
            Pen = 0,
            Eraser = 1,
            Filler = 2,
            Pointer = 3,
            Rotater = 4
        }

        public List<FrameworkElement> Menu { get { return _Menu; } }

        private Map currentMap { get { return (Map)((ComboBox)_Menu[1]).SelectedItem; } }

        private int currentMapIndex { get { return (int)((ComboBox)_Menu[1]).SelectedIndex; } }

        private Tile currentTile { get { return (wrappanel_tiles.SelectedIndex != -1) ? currentMap.Tiles[wrappanel_tiles.SelectedIndex] : currentMap.Tiles[0]; } }

        private int currentTileIndex { get { return wrappanel_tiles.SelectedIndex; } }

        public void ChangeTextureName (Map map, string oldname, string newname) {
            Texture2D texture = xnaTextures[map][oldname];
            xnaTextures[map].Remove(oldname);
            xnaTextures[map].Add(newname, texture);

            BitmapImage image = wpfTextures[map][oldname];
            wpfTextures[map].Remove(oldname);
            wpfTextures[map].Add(newname, image);
        }

        public void Save (Project project) {
            foreach (Map map in GetMaps( )) {
                // build texture
                Texture2D packedTexture = TileSerializer.BuildTexture(map.Tiles, xnaTextures[map], GraphicsDevice);
                using (Stream stream = project.GetOrCreateStream("maps", map.Name, map.Name + ".png"))
                    packedTexture.SaveAsPng(stream, packedTexture.Width, packedTexture.Height);

                map.Texture = Path.GetFileNameWithoutExtension(map.Name + ".png");
                using (Stream stream = project.GetOrCreateStream("maps", map.Name, map.Name + ".map"))
                    map.MergeRotations(mapRotations[map]).Serialize(stream);
            }
        }

        public void SetRotation (Map map, int x, int y, int layer, float value) {
            float[ , , ] data = mapRotations[map];
            data[x, y, layer] = value;
            mapRotations[map] = data;
        }

        public void UpdateListbox ( ) {
            wrappanel_tiles.Items.Clear( );
            foreach (Tile tile in currentMap.Tiles) {
                wrappanel_tiles.Items.Add(new ListViewEntry(wpfTextures[currentMap][tile.Name]));
            }
            if (wrappanel_tiles.HasItems)
                wrappanel_tiles.SelectedIndex = 0;
        }

        private void AddMap (Map map) {
            if (!IsEnabled)
                IsEnabled = true;

            foreach (FrameworkElement element in _Menu) {
                BindingExpression binding = element.GetBindingExpression(UIElement.IsEnabledProperty);
                if (binding != null)
                    binding.UpdateSource( );
            }

            ((ComboBox)_Menu[1]).Items.Add(map);

            if (!mapRotations.ContainsKey(map))
                mapRotations.Add(map, new float[map.Width, map.Height, 3]);
            if (!xnaTextures.ContainsKey(map)) {
                xnaTextures.Add(map, new Dictionary<string, Texture2D>( ));
                wpfTextures.Add(map, new Dictionary<string, BitmapImage>( ));
            }

            MemoryStream memoryStream = new MemoryStream( );
            memoryStream.Position = 0;
            new Bitmap(1, 1).Save(memoryStream, ImageFormat.Png);
            BitmapImage emptyImage = new BitmapImage( );
            emptyImage.BeginInit( );
            emptyImage.StreamSource = memoryStream;
            emptyImage.EndInit( );

            if ((map.Tiles?.Length ?? 0) == 0) {
                AddTexture(map, "None", emptyImage);
                map.Tiles = new Tile[ ] { new Tile( ) { Name = "None", Attributes = new Dictionary<TileAttribute, string>( ) } };
            }

            ((ComboBox)_Menu[1]).SelectedIndex = ((ComboBox)_Menu[1]).Items.Count - 1;
            UpdateListbox( );
            Cache.Add(map, new Stack<List<Tuple<Point, int, int, bool>>>( ));
        }

        private void AddTexture (Map map, string name, BitmapImage image) {
            if (xnaTextures[map].ContainsKey(name))
                return;
            xnaTextures[map].Add(name, image.ToTexture2D(GraphicsDevice));
            wpfTextures[map].Add(name, image);
        }

        private void AddTexture (Map map, string name, Texture2D image) {
            if (xnaTextures[map].ContainsKey(name))
                return;
            xnaTextures[map].Add(name, image);
            wpfTextures[map].Add(name, image.ToBitmapImage( ));
        }

        private void buttonexport_Click (object sender, RoutedEventArgs e) {
            // export current maps tileset
            SaveFileDialog exportDialog = new SaveFileDialog( );
            exportDialog.Filter = "TileTemplate|*.mkttemplate";
            if (exportDialog.ShowDialog( ) ?? false) {
                using (Stream stream = File.OpenWrite(exportDialog.FileName))
                    TileSerializer.Serialize(stream, currentMap.Tiles, xnaTextures[currentMap], tilemapview.GraphicsDevice);
            }
        }

        private void ButtonSettings_Click (object sender, RoutedEventArgs e) {
            EditDefaultTileAttributesWindow dialog = new EditDefaultTileAttributesWindow(defaultAttributes);
            if (dialog.ShowDialog( ) ?? false) {
                defaultAttributes = dialog.NewDefault;
            }
        }

        private void CommandBinding_CanExecuteAlways (object sender, CanExecuteRoutedEventArgs e) {
            e.CanExecute = true;
        }

        private void CommandBinding_ToolSelection_Executed (object sender, ExecutedRoutedEventArgs e) {
            switch (((RoutedUICommand)e.Command).Name.Last( )) {
                case 'A': SelectTool(Tool.Pen); break;
                case 'S': SelectTool(Tool.Eraser); break;
                case 'D': SelectTool(Tool.Filler); break;
                case 'F': SelectTool(Tool.Rotater); break;
            }
        }

        private void CommandBinding_Undo_Executed (object sender, ExecutedRoutedEventArgs e) {
            UndoLast( );
        }

        private void create_map_Click (object sender, RoutedEventArgs e) {
            new CreateMapWindow(tilemapview.GraphicsDevice, AddMap, AddTexture).ShowDialog( );
            if (currentMap != null)
                UpdateListbox( );
        }

        private void CurrentMapChanged (object sender, SelectionChangedEventArgs e) {
            if (currentMap == null)
                return;

            UpdateListbox( );
            tilemapview.CurrentMap = currentMap;
            // reset scrollbars
            scrollbar_horizontal.Minimum = 0;
            scrollbar_horizontal.Maximum = currentMap.Width;
            scrollbar_horizontal.Value = 0;
            scrollbar_vertical.Minimum = 0;
            scrollbar_vertical.Maximum = currentMap.Height;
            scrollbar_vertical.Value = currentMap.Height - tilemapview.RenderSize.Height / tilemapview.TileSize + 2;
        }

        private bool GetClickedTile (MouseEventArgs e, out Point tile) {
            Point positionOnControl = e.GetPosition(tilemapview);
            tile = new Point(
                    positionOnControl.X / tilemapview.TileSize + tilemapview.Offset.X,
                    currentMap.Height - positionOnControl.Y / tilemapview.TileSize - tilemapview.Offset.Y
                );
            return (tile.X >= 0 && tile.X < currentMap.Width && tile.Y >= 0 && tile.Y < currentMap.Height);
        }

        private List<Map> GetMaps ( ) {
            List<Map> result = new List<Map>( );
            foreach (object map in ((ComboBox)_Menu[1]).Items)
                result.Add((Map)map);
            return result;
        }

        private Dictionary<string, Texture2D> GetXNATextures (Map map) {
            return xnaTextures[map];
        }

        private void load_map_Click (object sender, RoutedEventArgs e) {
            OpenFileDialog mapOpenDialog = new OpenFileDialog( );
            mapOpenDialog.Filter = "MAP-Files|*.map";
            mapOpenDialog.CheckFileExists = true;
            if (mapOpenDialog.ShowDialog( ) ?? false) {
                LoadMap(mapOpenDialog.FileName);
            }
        }

        private void LoadMap (string path) {
            using (Stream mapStream = File.OpenRead(path)) {
                Map map = LoadMap(mapStream);
                using (Stream imageStream = File.OpenRead(Path.Combine(Path.GetDirectoryName(path), map.Texture + ".png")))
                    LoadImages(imageStream, map);
                AddMap(map);
            }
        }

        private Map LoadMap (Stream mapStream) {
            Map loadedMap = new Map(mapStream);
            mapRotations.Add(loadedMap, loadedMap.ExtractRotations( ));
            return loadedMap;
        }

        private void LoadImages (Stream imageStream, Map map) {
            xnaTextures.Add(map, TileSerializer.ExtractTextures(Texture2D.FromStream(GraphicsDevice, imageStream), map.Tiles, GraphicsDevice));
            wpfTextures.Add(map, new Dictionary<string, BitmapImage>( ));
            foreach (var entry in xnaTextures[map])
                wpfTextures[map].Add(entry.Key, entry.Value.ToBitmapImage( ));
        }

        public void Reset ( ) {
            IsEnabled = false;
            scrollbar_horizontal.Value = 0;
            scrollbar_vertical.Value = 0;

            ((ComboBox)_Menu[1]).Items.Clear( );
            Cache.Clear( );
            currentlyEditingTile = -1;
            currentlyEditionTilesMap = -1;
            wrappanel_tiles.Items.Clear( );
            if (tilemapview.CurrentMap != null)
                tilemapview.CurrentMap = null;
            foreach (object map in ((ComboBox)_Menu[1]).Items) {
                AddMap((Map)map);
            }
        }

        private void ResetToolBorders ( ) {
            ((Border)_Menu[14]).BorderThickness = new Thickness(0);
            ((Border)_Menu[15]).BorderThickness = new Thickness(0);
            ((Border)_Menu[16]).BorderThickness = new Thickness(0);
            ((Border)_Menu[17]).BorderThickness = new Thickness(0);
            ((Border)_Menu[18]).BorderThickness = new Thickness(0);
        }

        private void scrollbar_horizontal_ValueChanged (object sender, RoutedPropertyChangedEventArgs<double> e) {
            tilemapview.Offset = new Microsoft.Xna.Framework.Point((int)scrollbar_horizontal.Value, tilemapview.Offset.Y);
        }

        private void scrollbar_vertical_ValueChanged (object sender, RoutedPropertyChangedEventArgs<double> e) {
            tilemapview.Offset = new Microsoft.Xna.Framework.Point(tilemapview.Offset.X, (int)scrollbar_vertical.Value);
        }

        private void SelectTool (Tool tool) {
            currentTool = tool;
            ResetToolBorders( );
            ((Border)_Menu[14 + (int)tool]).BorderThickness = new Thickness(1);
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
                            mapRotations[currentMap][(int)clickedTile.X, (int)clickedTile.Y, currentLayer] = 0;
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
                                    mapRotations[currentMap][(int)clickedTile.X, (int)clickedTile.Y, currentLayer] = 0;
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
                            mapRotations[currentMap][(int)clickedTile.X, (int)clickedTile.Y, currentLayer] = 0;
                            break;

                        case Tool.Pointer:
                            currentMap.SpawnPoint = new Vector2((int)clickedTile.X, (int)clickedTile.Y);
                            break;

                        case Tool.Rotater:
                            float tileRotation = mapRotations[currentMap][(int)clickedTile.X, (int)clickedTile.Y, currentLayer];
                            Cache[currentMap].Push(new List<Tuple<Point, int, int, bool>>( ) {
                                    new Tuple<Point, int, int, bool>(clickedTile,currentLayer, (int)(tileRotation * 2), true)});
                            tileRotation += 0.5f;
                            tileRotation %= 2f;
                            if (currentMap.GetTile((int)clickedTile.X, (int)clickedTile.Y).Name != "None")
                                mapRotations[currentMap][(int)clickedTile.X, (int)clickedTile.Y, currentLayer] = tileRotation;
                            break;
                    }
                }
                tilemapview.Update( );
            }
        }

        private void tilemapview_MouseEnter (object sender, MouseEventArgs e) {
            if (currentMap != null)
                UpdateSelectedTile(e);
        }

        private void tilemapview_MouseLeave (object sender, MouseEventArgs e) {
            tilemapview.CurrentSelection = new Microsoft.Xna.Framework.Point(-1, -1);
            tilemapview.Update( );
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
                            mapRotations[currentMap][(int)clickedTile.X, (int)clickedTile.Y, currentLayer] = 0;
                            break;

                        case Tool.Pen:
                            Cache[currentMap].Push(new List<Tuple<Point, int, int, bool>>( ) {
                                    new Tuple<Point, int, int, bool>(clickedTile,currentLayer, currentMap.Data[(int)clickedTile.X, (int)clickedTile.Y, currentLayer], false)});
                            currentMap.Data[(int)clickedTile.X, (int)clickedTile.Y, currentLayer] = currentTileIndex;
                            mapRotations[currentMap][(int)clickedTile.X, (int)clickedTile.Y, currentLayer] = 0;
                            break;

                        case Tool.Pointer:
                            currentMap.SpawnPoint = new Vector2((int)clickedTile.X, (int)clickedTile.Y);
                            break;
                    }
                    update = update || (currentTool != Tool.Filler && currentTool != Tool.Rotater);
                }
            }
            if (update)
                tilemapview.Update( );
        }

        private void tilemapview_PreviewMouseWheel (object sender, MouseWheelEventArgs e) {
            if (e.Delta > 0) {
                tilemapview.ZoomLevel++;
            } else {
                tilemapview.ZoomLevel--;
            }
        }

        private void UndoLast ( ) {
            if (currentMap == null || Cache[currentMap].Count == 0)
                return;

            foreach (Tuple<Point, int, int, bool> undoData in Cache[currentMap].Pop( )) {
                if (undoData.Item4) {
                    // tile got rotated
                    mapRotations[currentMap][(int)undoData.Item1.X, (int)undoData.Item1.Y, undoData.Item2] = undoData.Item3 / 2f;
                } else {
                    // tile got changed
                    currentMap.Data[(int)undoData.Item1.X, (int)undoData.Item1.Y, undoData.Item2] = undoData.Item3;
                }
            }
            tilemapview.Update( );
        }

        private bool UpdateSelectedTile (MouseEventArgs e) {
            Point positionOnControl = e.GetPosition(tilemapview);
            Microsoft.Xna.Framework.Point selectedTile = new Microsoft.Xna.Framework.Point(
                (int)Math.Min(positionOnControl.X / tilemapview.TileSize, currentMap.Width - tilemapview.Offset.X - 1),
                (int)Math.Min(positionOnControl.Y / tilemapview.TileSize, currentMap.Height - tilemapview.Offset.Y - 1)
                );
            if (selectedTile.X != tilemapview.CurrentSelection.X || selectedTile.Y != tilemapview.CurrentSelection.Y) {
                text_xpos.Text = (selectedTile.X + tilemapview.Offset.X + 1).ToString( );
                text_ypos.Text = (currentMap.Size.Height - selectedTile.Y - tilemapview.Offset.Y).ToString( );
                tilemapview.CurrentSelection = selectedTile;
                return true;
            } else
                return false;
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

                            AddTexture(currentMap, tileName, tileImage);
                            currentMap.AddTile(new Tile( ) { Attributes = new Dictionary<TileAttribute, string>(defaultAttributes), Name = tileName });
                            wrappanel_tiles.Items.Add(new ListViewEntry(tileImage));
                        } else {
                            // open add tile window
                            AddTileWindow addTileDialog = new AddTileWindow(file, currentMap.Tiles.Select(t => t.Name), defaultAttributes);
                            if (addTileDialog.ShowDialog( ) ?? false) {
                                if (currentMap.Tiles.Where(t => t.Name == addTileDialog.Created.Item1.Name) != null) {
                                    AddTexture(currentMap, addTileDialog.Created.Item1.Name, addTileDialog.Created.Item2);
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

        private void wrappanel_tiles_Selected (object sender, RoutedEventArgs e) {
            if (currentlyEditingTile >= 0 || currentlyEditionTilesMap >= 0) {
                if (GetMaps( )[currentlyEditionTilesMap].Tiles.Where(t => t.Name == textbox_tile_name.Text) != null) {
                    ChangeTextureName(GetMaps( )[currentlyEditionTilesMap], GetMaps( )[currentlyEditionTilesMap].Tiles[currentlyEditingTile].Name, textbox_tile_name.Text);
                    GetMaps( )[currentlyEditionTilesMap].Tiles[currentlyEditingTile].Name = textbox_tile_name.Text;
                }
                GetMaps( )[currentlyEditionTilesMap].Tiles[currentlyEditingTile].Attributes.Clear( );
                foreach (AttributeListViewEntry entry in listview_tile_attributes.Items) {
                    if (entry.Active)
                        GetMaps( )[currentlyEditionTilesMap].Tiles[currentlyEditingTile].Attributes.Add((TileAttribute)Enum.Parse(typeof(TileAttribute), entry.Attribute), entry.Value);
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

        #endregion templates
    }
}