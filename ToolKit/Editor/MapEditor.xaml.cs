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
using mapKnight.Core.World;
using mapKnight.ToolKit.Data;

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
            new ComboBox() { Width = 260, Margin = new Thickness(-6, 0, -6, 0), VerticalAlignment = VerticalAlignment.Center, Focusable = false },
            new Image() { Source= (BitmapImage)App.Current.FindResource("image_map_mapsettings"), Style = imageStyle },
            new MenuItem() { Header = "SHOW LAYER", IsEnabled = false },
            new CheckBox() { IsChecked = true, Margin = new Thickness(-2, 0, -2, 0), VerticalAlignment = VerticalAlignment.Center, ToolTip = new ToolTip() { Content = "Show/Hide Background" }, Focusable = false },
            new CheckBox() { IsChecked = true, Margin = new Thickness(-2, 0, -2, 0), VerticalAlignment = VerticalAlignment.Center, ToolTip = new ToolTip() { Content = "Show/Hide Middle" }, Focusable = false },
            new CheckBox() { IsChecked = true, Margin = new Thickness(-2, 0, -2, 0), VerticalAlignment = VerticalAlignment.Center, ToolTip = new ToolTip() { Content = "Show/Hide Foreground" }, Focusable = false },
            new Separator() { },
            new MenuItem() {Header ="MODIFY LAYER",IsEnabled =false },
            new RadioButton() {IsChecked = false, GroupName="modifylayer", Margin = new Thickness(-2, 0, -2, 0), VerticalAlignment = VerticalAlignment.Center, ToolTip = new ToolTip(){ Content = "Select Background" }, Focusable = false },
            new RadioButton() {IsChecked = true, GroupName="modifylayer", Margin = new Thickness(-2, 0, -2, 0), VerticalAlignment = VerticalAlignment.Center, ToolTip = new ToolTip() { Content = "Select Middle" }, Focusable = false },
            new RadioButton() {IsChecked = false, GroupName="modifylayer", Margin = new Thickness(-2, 0, -2, 0), VerticalAlignment = VerticalAlignment.Center, ToolTip = new ToolTip() { Content = "Select Foreground" }, Focusable = false },
            new Separator(),
            new Border() { Child = new Image() { Source = (BitmapImage)App.Current.FindResource("image_map_undo"), Style = imageStyle }, Background = Brushes.White, Margin = new Thickness(-6, 0, -6, 0), Padding = new Thickness(6, 0, 6, 0), ToolTip = new ToolTip() { Content = "Ctrl + Z" } },
            new Border() { Child = new Image() { Source = (BitmapImage)App.Current.FindResource("image_map_pen"), Style = imageStyle }, Background = Brushes.White, BorderBrush = Brushes.DodgerBlue, BorderThickness=  new Thickness(1), Margin = new Thickness(-6, 0, -6, 0), Padding = new Thickness(6, 0, 6, 0), ToolTip = new ToolTip() { Content = "Alt + A" } },
            new Border() { Child = new Image() { Source = (BitmapImage)App.Current.FindResource("image_map_eraser"), Style = imageStyle }, Background = Brushes.White, BorderBrush = Brushes.DodgerBlue, BorderThickness=  new Thickness(0), Margin = new Thickness(-6, 0, -6,0), Padding = new Thickness(6, 0, 6, 0), ToolTip = new ToolTip() { Content = "Alt + S" } },
            new Border() { Child = new Image() { Source = (BitmapImage)App.Current.FindResource("image_map_fill"), Style = imageStyle }, Background = Brushes.White, BorderBrush = Brushes.DodgerBlue, BorderThickness=  new Thickness(0), Margin = new Thickness(-6, 0, -6 ,0), Padding = new Thickness(6, 0, 6, 0), ToolTip = new ToolTip() { Content = "Ctrl + D" } },
            new Border() { Child = new Image() { Source = (BitmapImage)App.Current.FindResource("image_map_pointer"), Style = imageStyle }, Background = Brushes.White, BorderBrush = Brushes.DodgerBlue, BorderThickness = new Thickness(0), Margin = new Thickness(-6, 0, -6, 0), Padding = new Thickness(6, 0, 6, 0) },
            new Border() { Child = new Image() { Source = (BitmapImage)App.Current.FindResource("image_map_rotate"), Style = imageStyle }, Background = Brushes.White, BorderBrush = Brushes.DodgerBlue, BorderThickness = new Thickness(0), Margin = new Thickness(-6, 0, -6, 0), Padding = new Thickness(6, 0, 6, 0), ToolTip = new ToolTip() { Content = "Ctrl + F" } },
            new Separator(),
            new Border() { Child = new Image() { Source = (BitmapImage)App.Current.FindResource("image_map_placeentity"), Style = imageStyle }, Background = Brushes.White, BorderBrush = Brushes.DodgerBlue, BorderThickness = new Thickness(0), Margin = new Thickness(-6, 0, -6, 0), Padding = new Thickness(6, 0, 6, 0), ToolTip = new ToolTip() { Content = "Place Entity" } },
            new Border() { Child = new Image() { Source = (BitmapImage)App.Current.FindResource("image_map_selectentity"), Style = imageStyle }, Background = Brushes.White, BorderBrush = Brushes.DodgerBlue, BorderThickness = new Thickness(0), Margin = new Thickness(-6, 0, -6, 0), Padding = new Thickness(6, 0, 6, 0), ToolTip = new ToolTip() { Content = "Select and Move Entity" } },
            new Border() { Child = new Image() { Source = (BitmapImage)App.Current.FindResource("image_map_killentity"), Style = imageStyle }, Background = Brushes.White, BorderBrush = Brushes.DodgerBlue, BorderThickness = new Thickness(0), Margin = new Thickness(-6, 0, -6, 0), Padding = new Thickness(6, 0, 6, 0), ToolTip = new ToolTip() { Content = "Kill Entity" } },
        };

        // Map Cache List von Koord Layer alter Wert IsRotation
        private Dictionary<Map, Stack<List<Tuple<Point, int, int, bool>>>> Cache = new Dictionary<Map, Stack<List<Tuple<Point, int, int, bool>>>>( );

        private int currentLayer = 1;
        private Tool currentTool = Tool.Pen;

        private Dictionary<TileAttribute, string> defaultAttributes = new Dictionary<TileAttribute, string>( );

        private Entity cachedEntity, currentlySelectedEntity;

        public void Load (Project project) {
            xnaTextures.Clear( );
            wpfTextures.Clear( );
            mapRotations.Clear( );

            foreach (string mapfile in project.GetAllEntries("maps")) {
                if (Path.GetExtension(mapfile) == ".map") {
                    string dir = Path.GetDirectoryName(mapfile);
                    string name = Path.GetFileNameWithoutExtension(mapfile);

                    using (Stream mapStream = project.GetOrCreateStream(false, mapfile)) {
                        Controls.TileMapView.EditorMap map = LoadMap(mapStream);
                        using (Stream imageStream = project.GetOrCreateStream(false, Path.Combine(dir, name + ".png")))
                            LoadImages(imageStream, map);
                        AddMap(map);
                    }
                }
            }
        }

        public void Compile (string mappath) {
            foreach (Map map in GetMaps( )) {
                string basedirectory = Path.Combine(mappath, map.Name);
                if (!Directory.Exists(basedirectory))
                    Directory.CreateDirectory(basedirectory);
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
                if (currentMap == null)
                    return;
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

            _Menu[20].MouseDown += (sender, e) => SelectTool(Tool.God);
            _Menu[21].MouseDown += (sender, e) => SelectTool(Tool.Hand);
            _Menu[22].MouseDown += (sender, e) => SelectTool(Tool.Trashcan);

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

            tilemapview.DeviceInitialized += ( ) => {
                entitylistbox.Init(tilemapview.GraphicsDevice);
            };

            tilemapview.SetReceiveFuncs((Map m) => { return xnaTextures[m]; }, (Map m, int x, int y, int l) => { return mapRotations[m][x, y, l]; }, (string name) => {
                Data.EntityData d = entitylistbox.Find(name);
                return d.Texture;
            });
        }

        private enum Tool {
            Pen = 0,
            Eraser = 1,
            Filler = 2,
            Pointer = 3,
            Rotater = 4,
            God = 6,
            Hand = 7,
            Trashcan = 8
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
                using (Stream stream = project.GetOrCreateStream(false, "maps", map.Name, map.Name + ".png"))
                    packedTexture.SaveAsPng(stream, packedTexture.Width, packedTexture.Height);

                map.Texture = Path.GetFileNameWithoutExtension(map.Name + ".png");
                using (Stream stream = project.GetOrCreateStream(false, "maps", map.Name, map.Name + ".map"))
                    map.MergeRotations(mapRotations[map]).Serialize(stream);
            }
        }

        public void SetRotation (Map map, int x, int y, int layer, float value) {
            mapRotations[map].SetValue(value, x, y, layer);
        }

        public void UpdateListbox ( ) {
            wrappanel_tiles.Items.Clear( );
            foreach (Tile tile in currentMap.Tiles) {
                wrappanel_tiles.Items.Add(new ListViewEntry(wpfTextures[currentMap][tile.Name]));
            }
            if (wrappanel_tiles.HasItems)
                wrappanel_tiles.SelectedIndex = 0;
        }

        private void AddMap (Controls.TileMapView.EditorMap map) {
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
                case 'A':
                    SelectTool(Tool.Pen);
                    break;
                case 'S':
                    SelectTool(Tool.Eraser);
                    break;
                case 'D':
                    SelectTool(Tool.Filler);
                    break;
                case 'F':
                    SelectTool(Tool.Rotater);
                    break;
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

        private Point GetClickedTile ( ) {
            return new Point(Math.Floor(tilemapview.Offset.X) + tilemapview.CurrentSelection.X, currentMap.Height - Math.Floor(tilemapview.Offset.Y) - tilemapview.CurrentSelection.Y - 1);
        }

        private IEnumerable<Map> GetMaps ( ) {
            foreach (object map in ((ComboBox)_Menu[1]).Items)
                yield return (Map)map;
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
                Controls.TileMapView.EditorMap map = LoadMap(mapStream);
                using (Stream imageStream = File.OpenRead(Path.Combine(Path.GetDirectoryName(path), map.Texture + ".png")))
                    LoadImages(imageStream, map);
                AddMap(map);
            }
        }

        private Controls.TileMapView.EditorMap LoadMap (Stream mapStream) {
            Controls.TileMapView.EditorMap loadedMap = new Controls.TileMapView.EditorMap(mapStream);
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
            wrappanel_tiles.Items.Clear( );
            if (tilemapview.CurrentMap != null)
                tilemapview.CurrentMap = null;
            foreach (object map in ((ComboBox)_Menu[1]).Items) {
                AddMap((Controls.TileMapView.EditorMap)map);
            }
        }

        private void ResetToolBorders ( ) {
            ((Border)_Menu[14]).BorderThickness = new Thickness(0);
            ((Border)_Menu[15]).BorderThickness = new Thickness(0);
            ((Border)_Menu[16]).BorderThickness = new Thickness(0);
            ((Border)_Menu[17]).BorderThickness = new Thickness(0);
            ((Border)_Menu[18]).BorderThickness = new Thickness(0);
            ((Border)_Menu[20]).BorderThickness = new Thickness(0);
            ((Border)_Menu[21]).BorderThickness = new Thickness(0);
            ((Border)_Menu[22]).BorderThickness = new Thickness(0);
        }

        private void scrollbar_horizontal_ValueChanged (object sender, RoutedPropertyChangedEventArgs<double> e) {
            tilemapview.Offset = new Microsoft.Xna.Framework.Vector2((float)scrollbar_horizontal.Value, tilemapview.Offset.Y);
        }

        private void scrollbar_vertical_ValueChanged (object sender, RoutedPropertyChangedEventArgs<double> e) {
            tilemapview.Offset = new Microsoft.Xna.Framework.Vector2(tilemapview.Offset.X, (float)scrollbar_vertical.Value);
        }

        private void SelectTool (Tool tool) {
            if (cachedEntity != null) {
                currentMap.Entities.Remove(cachedEntity);
                cachedEntity = null;
            }

            if ((int)tool < 5) {
                tabcontrol_toolselect.SelectedIndex = 0;
            } else {
                tabcontrol_toolselect.SelectedIndex = 1;
            }
            currentTool = tool;
            ResetToolBorders( );
            ((Border)_Menu[14 + (int)tool]).BorderThickness = new Thickness(1);
        }

        private void tilemapview_MouseDown (object sender, MouseButtonEventArgs e) {
            if (currentMap == null || currentTileIndex == -1 || !tilemapview.IsLayerActive(currentLayer))
                return;

            if(tabcontrol_toolselect.SelectedIndex == 0) {
                // tiles
                HandleTilemapViewClickTiles(sender, e);
            } else {
                // entities
                HandleTilemapViewClickEntities(sender, e);
            }
        }

        private void HandleTilemapViewClickTiles(object sender, MouseButtonEventArgs e) {
            Point clickedTile = GetClickedTile( );
            if (e.RightButton == MouseButtonState.Pressed) {
                Cache[currentMap].Push(new List<Tuple<Point, int, int, bool>>( ) {
                                    Tuple.Create(clickedTile,currentLayer, currentMap.Data[(int)clickedTile.X, (int)clickedTile.Y, currentLayer], false)});
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
                                changesForCache.Add(Tuple.Create(current, currentLayer, searching, false));

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
                                    Tuple.Create(clickedTile,currentLayer, currentMap.Data[(int)clickedTile.X, (int)clickedTile.Y, currentLayer], false)});
                        currentMap.Data[(int)clickedTile.X, (int)clickedTile.Y, currentLayer] = currentTileIndex;
                        mapRotations[currentMap][(int)clickedTile.X, (int)clickedTile.Y, currentLayer] = 0;
                        break;

                    case Tool.Pointer:
                        currentMap.SpawnPoint = new Vector2((int)clickedTile.X, (int)clickedTile.Y);
                        break;

                    case Tool.Rotater:
                        if (currentMap.Tiles[currentMap.Data[(int)clickedTile.X, (int)clickedTile.Y, currentLayer]].Name == "None") {
                            for (int i = 2; i > -1; i--) {
                                if (currentMap.Tiles[currentMap.Data[(int)clickedTile.X, (int)clickedTile.Y, i]].Name != "None") {
                                    ((RadioButton)_Menu[9 + i]).IsChecked = true;
                                }
                            }
                        }
                        float tileRotation = mapRotations[currentMap][(int)clickedTile.X, (int)clickedTile.Y, currentLayer];
                        Cache[currentMap].Push(new List<Tuple<Point, int, int, bool>>( ) {
                                    Tuple.Create(clickedTile,currentLayer, (int)(tileRotation * 2), true)});
                        tileRotation += 0.5f;
                        tileRotation %= 2f;
                        if (currentMap.GetTile((int)clickedTile.X, (int)clickedTile.Y, currentLayer).Name != "None")
                            mapRotations[currentMap][(int)clickedTile.X, (int)clickedTile.Y, currentLayer] = tileRotation;
                        break;
                }
                tilemapview.Update( );
            }
        }

        private Vector2 GetEntityCenterRaw(MouseEventArgs e) {
            Point positionOnControl = e.GetPosition(tilemapview);
            Vector2 selectedTile = new Vector2(
                (float)Math.Min(positionOnControl.X / tilemapview.TileSize + tilemapview.Offset.X, currentMap.Width - tilemapview.Offset.X - 1),
                currentMap.Size.Height - (float)Math.Min(positionOnControl.Y / tilemapview.TileSize, currentMap.Height - Math.Floor(tilemapview.Offset.Y)) - (float)Math.Floor(tilemapview.Offset.Y));
            return selectedTile;
        }


        private Vector2 GetEntityCenter( MouseEventArgs e) {
            Vector2 selectedTile = GetEntityCenterRaw(e);
            selectedTile.Y = (float)Math.Floor(selectedTile.Y);
            return selectedTile;
        }

        private void HandleTilemapViewClickEntities(object sender, MouseButtonEventArgs e) {
            contentpresenter_entitydata.Content = null;
            switch (currentTool) {
                case Tool.God:
                    entitylistbox.GetCurrentFinalConfiguration( ).Create(GetEntityCenter(e), (Controls.TileMapView.EditorMap)currentMap, true);
                    if (cachedEntity != null) {
                        currentMap.Entities.Remove(cachedEntity);
                        cachedEntity = null;
                    }
                    tilemapview.Update( );
                    break;
                case Tool.Hand:
                    Entity clickedEntity = GetClickedEntity(e);
                    if(clickedEntity != null) {
                        foreach(Component c in clickedEntity.GetComponents( )) {
                            if (c is IUserControlComponent) {
                                contentpresenter_entitydata.Content = (c as IUserControlComponent).Control;
                            }
                        }
                    }
                    break;
                case Tool.Trashcan:
                    clickedEntity = GetClickedEntity(e);
                    if(clickedEntity != null) {
                        currentMap.Entities.Remove(clickedEntity);
                        tilemapview.Update( );
                        currentlySelectedEntity = null;
                    }
                    break;
            }
        }

        private Entity GetClickedEntity(MouseEventArgs e) {
            Vector2 clickedPosition = GetEntityCenterRaw(e);
            for(int i = currentMap.Entities.Count -1; i >= 0; i--) {
                if (currentMap.Entities[i].Transform.Intersects(clickedPosition)) {
                    return currentMap.Entities[i];
                }
            }
            return null;
        }

        private void tilemapview_MouseEnter (object sender, MouseEventArgs e) {
            if (currentMap != null)
                UpdateSelectedTile(e);
        }

        private void tilemapview_MouseLeave (object sender, MouseEventArgs e) {
            tilemapview.CurrentSelection = new Microsoft.Xna.Framework.Point(-1, -1);
            if(cachedEntity != null) {
                currentMap.Entities.Remove(cachedEntity);
                cachedEntity = null;
            }
            tilemapview.Update( );
        }

        private void tilemapview_MouseMove (object sender, MouseEventArgs e) {
            if (currentMap == null)
                return;
            bool updated = UpdateSelectedTile(e);

            if (tabcontrol_toolselect.SelectedIndex == 0) {
                if (!updated)
                    return;
                HandleTilemapViewMoveTiles(sender, e, updated);
            } else {
                HandleTilemapViewMoveEntities(sender, e);
                if (updated)
                    tilemapview.Update( );
                return;
            }

        }

        private void HandleTilemapViewMoveTiles(object sender, MouseEventArgs e, bool updated) {
            Point clickedTile = GetClickedTile( );
            if (e.RightButton == MouseButtonState.Pressed) {
                Cache[currentMap].Push(new List<Tuple<Point, int, int, bool>>( ) {
                                    Tuple.Create(clickedTile,currentLayer, currentMap.Data[(int)clickedTile.X, (int)clickedTile.Y, currentLayer], false)});
                currentMap.Data[(int)clickedTile.X, (int)clickedTile.Y, currentLayer] = 0;
                updated = true;
            } else if (e.LeftButton == MouseButtonState.Pressed) {
                switch (currentTool) {
                    case Tool.Eraser:
                        Cache[currentMap].Push(new List<Tuple<Point, int, int, bool>>( ) {
                                    Tuple.Create(clickedTile,currentLayer, currentMap.Data[(int)clickedTile.X, (int)clickedTile.Y, currentLayer], false)});
                        currentMap.Data[(int)clickedTile.X, (int)clickedTile.Y, currentLayer] = 0;
                        mapRotations[currentMap][(int)clickedTile.X, (int)clickedTile.Y, currentLayer] = 0;
                        break;

                    case Tool.Pen:
                        Cache[currentMap].Push(new List<Tuple<Point, int, int, bool>>( ) {
                                    Tuple.Create(clickedTile,currentLayer, currentMap.Data[(int)clickedTile.X, (int)clickedTile.Y, currentLayer], false)});
                        currentMap.Data[(int)clickedTile.X, (int)clickedTile.Y, currentLayer] = currentTileIndex;
                        mapRotations[currentMap][(int)clickedTile.X, (int)clickedTile.Y, currentLayer] = 0;
                        break;

                    case Tool.Pointer:
                        currentMap.SpawnPoint = new Vector2((int)clickedTile.X, (int)clickedTile.Y);
                        break;
                }
                updated = updated || (currentTool != Tool.Filler && currentTool != Tool.Rotater);
            }
            if (updated)
                tilemapview.Update( );
        }

        private void HandleTilemapViewMoveEntities(object sender, MouseEventArgs e) {
            switch (currentTool) {
                case Tool.God:
                    Vector2 entityLocation = GetEntityCenter(e);
                    if(cachedEntity == null) {
                        cachedEntity = entitylistbox.GetCurrentShadowConfiguration( ).Create(entityLocation, (Controls.TileMapView.EditorMap)currentMap, true);
                    } else {
                        entityLocation.Y += cachedEntity.Transform.Height / 2;
                        cachedEntity.Transform.Center = entityLocation;
                    }
                    tilemapview.Update( );
                    break;
                case Tool.Hand:
                    Entity selectedEntity = GetClickedEntity(e);
                    bool changed = (currentlySelectedEntity != null || selectedEntity != null) && currentlySelectedEntity != selectedEntity;
                    if (currentlySelectedEntity != null) {
                        currentlySelectedEntity.Domain = EntityDomain.Enemy;
                        currentlySelectedEntity = null;
                    }
                    if (selectedEntity != null) {
                        selectedEntity.Domain = EntityDomain.Obstacle;
                        currentlySelectedEntity = selectedEntity;
                    }
                    if (changed)
                        tilemapview.Update( );
                    break;
                case Tool.Trashcan:
                    selectedEntity = GetClickedEntity(e);
                    changed = (currentlySelectedEntity != null || selectedEntity != null) && currentlySelectedEntity != selectedEntity;
                    if (currentlySelectedEntity != null) {
                        currentlySelectedEntity.Domain = EntityDomain.Enemy;
                        currentlySelectedEntity = null;
                    }
                    if (selectedEntity != null) {
                        selectedEntity.Domain = EntityDomain.NPC;
                        currentlySelectedEntity = selectedEntity;
                    }
                    if (changed)
                        tilemapview.Update( );
                    break;
            }
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
                (int)Math.Min(positionOnControl.X / tilemapview.TileSize, currentMap.Width - Math.Floor(tilemapview.Offset.X) - 1),
                (int)Math.Min(positionOnControl.Y / tilemapview.TileSize, currentMap.Height - Math.Floor(tilemapview.Offset.Y) - 1)
                );
            if (selectedTile.X != tilemapview.CurrentSelection.X || selectedTile.Y != tilemapview.CurrentSelection.Y) {
                text_xpos.Text = Math.Round(selectedTile.X + Math.Floor(tilemapview.Offset.X) + 1).ToString( );
                text_ypos.Text = Math.Round(currentMap.Size.Height - selectedTile.Y - Math.Floor(tilemapview.Offset.Y)).ToString( );
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
                        AddTile(file);
                    }
                }
            }
        }

        private void wrappanel_tiles_SelectionChanged (object sender, SelectionChangedEventArgs e) {
            int oldSelection = (e.RemovedItems.Count > 0) ? wrappanel_tiles.Items.IndexOf(e.RemovedItems[0]) : -1;
            if (oldSelection > -1) {
                if (!currentMap.Tiles.Any(t => t.Name == textbox_tile_name.Text)) {
                    ChangeTextureName(currentMap, currentMap.Tiles[oldSelection].Name, textbox_tile_name.Text);
                    currentMap.Tiles[oldSelection].Name = textbox_tile_name.Text;
                }
                currentMap.Tiles[oldSelection].Attributes.Clear( );
                foreach (AttributeListViewEntry entry in listview_tile_attributes.Items) {
                    if (entry.Active)
                        currentMap.Tiles[oldSelection].Attributes.Add((TileAttribute)Enum.Parse(typeof(TileAttribute), entry.Attribute), entry.Value);
                }
            }

            if (e.AddedItems.Count > 0) {
                textbox_tile_name.Text = currentTile.Name;
                listview_tile_attributes.Items.Clear( );
                foreach (TileAttribute attribute in Enum.GetValues(typeof(TileAttribute))) {
                    if (currentTile.HasFlag(attribute)) {
                        listview_tile_attributes.Items.Add(new AttributeListViewEntry(true, attribute.ToString( ), currentTile.Attributes[attribute]));
                    } else {
                        listview_tile_attributes.Items.Add(new AttributeListViewEntry(false, attribute.ToString( ), ""));
                    }
                }
                SelectTool(Tool.Pen);
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

        private void CommandDelete_Executed (object sender, ExecutedRoutedEventArgs e) {
            int index = wrappanel_tiles.SelectedIndex;
            wrappanel_tiles.Items.RemoveAt(index);
            currentMap.RemoveTile(index);
            tilemapview.Update( );
        }

        private void CommandDelete_CanExecute (object sender, CanExecuteRoutedEventArgs e) {
            e.CanExecute = wrappanel_tiles?.SelectedItem != null;
        }

        private void CommandReplace_Executed (object sender, ExecutedRoutedEventArgs e) {
            OpenFileDialog opendialog = new OpenFileDialog( );
            opendialog.Filter = "Images|*.png;*.jpg;*.jpeg";
            opendialog.Multiselect = false;
            if (opendialog.ShowDialog( ) ?? false) {
                string tileName = Path.GetFileNameWithoutExtension(opendialog.FileName);
                BitmapImage tileImage = LoadTileImage(opendialog.FileName);

                int index = wrappanel_tiles.SelectedIndex;
                Tile prevTile = currentMap.Tiles[index];
                wpfTextures[currentMap].Remove(prevTile.Name);
                xnaTextures[currentMap].Remove(prevTile.Name);

                AddTexture(currentMap, tileName, tileImage);
                currentMap.Tiles[index].Name = tileName;
                wrappanel_tiles.Items[wrappanel_tiles.SelectedIndex] = new ListViewEntry(tileImage);

                tilemapview.Update( );
            }
        }

        private void CommandReplace_CanExecute (object sender, CanExecuteRoutedEventArgs e) {
            e.CanExecute = wrappanel_tiles?.SelectedItem != null;
        }

        private void ButtonAdd_Click (object sender, RoutedEventArgs e) {
            OpenFileDialog opendialog = new OpenFileDialog( );
            opendialog.Filter = "Images|*.png;*.jpg;*.jpeg";
            opendialog.Multiselect = true;
            if (opendialog.ShowDialog( ) ?? false) {
                foreach (string file in opendialog.FileNames) {
                    AddTile(file);
                }
            }
        }

        private void AddTile (string imagefile) {
            string tileName = Path.GetFileNameWithoutExtension(imagefile);
            if (checkbox_auto.IsChecked ?? false && !currentMap.Tiles.Any(t => t.Name == tileName)) {
                // add tile
                BitmapImage tileImage = LoadTileImage(imagefile);

                AddTexture(currentMap, tileName, tileImage);
                currentMap.AddTile(new Tile( ) { Attributes = new Dictionary<TileAttribute, string>(defaultAttributes), Name = tileName });
                wrappanel_tiles.Items.Add(new ListViewEntry(tileImage));
            } else {
                // open add tile window
                AddTileWindow addTileDialog = new AddTileWindow(imagefile, currentMap.Tiles.Select(t => t.Name), defaultAttributes);
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

        private BitmapImage LoadTileImage (string file) {
            BitmapImage image = new BitmapImage( );
            image.BeginInit( );
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.CreateOptions = BitmapCreateOptions.None;
            image.DecodePixelWidth = Map.TILE_PXL_SIZE;
            image.DecodePixelHeight = Map.TILE_PXL_SIZE;
            image.UriSource = new Uri(file);
            image.EndInit( );
            return image;
        }

        private void tabcontrol_toolselect_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if(tabcontrol_toolselect.SelectedIndex == 0) {
                SelectTool(Tool.Pen);
            } else {
                SelectTool(Tool.God);
            }
        }

    }
}
