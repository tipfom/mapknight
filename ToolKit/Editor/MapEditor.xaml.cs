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
using mapKnight.ToolKit.Windows.Dialogs;

namespace mapKnight.ToolKit.Editor {

    /// <summary>
    /// Interaktionslogik für MapEditor.xaml
    /// </summary>
    public partial class MapEditor : UserControl {
        private enum Tool {
            Pen = 0,
            Eraser = 1,
            Filler = 2,
            Pointer = 3,
            Rotater = 4,
            God = 6,
            Hand = 7,
            Trashcan = 8,
            VectorGrabber = 9,
            Brush = 11,
        }

        public static readonly RoutedUICommand CreateBrush = new RoutedUICommand("CreateBrush", "CreateBrush", typeof(MapEditor));
        public static readonly RoutedUICommand RemoveBrush = new RoutedUICommand("RemoveBrush", "RemoveBrush", typeof(MapEditor));

        private static Style imageStyle = new Style(typeof(Image)) { Triggers = { new Trigger( ) { Value = false, Property = IsEnabledProperty, Setters = { new Setter(Image.OpacityProperty, 0.5) } } } };

        private List<FrameworkElement> _Menu = new List<FrameworkElement>( ) {
            new Image() { Source= (BitmapImage)App.Current.FindResource("image_map_mapsettings"), Style = imageStyle },
            new MenuItem() { Header = "SHOW", IsEnabled = false },
            new CheckBox() { IsChecked = true, Margin = new Thickness(-2, 0, -2, 0), VerticalAlignment = VerticalAlignment.Center, ToolTip = new ToolTip() { Content = "Show/Hide Background" }, Focusable = false },
            new CheckBox() { IsChecked = true, Margin = new Thickness(-2, 0, -2, 0), VerticalAlignment = VerticalAlignment.Center, ToolTip = new ToolTip() { Content = "Show/Hide Middle" }, Focusable = false },
            new CheckBox() { IsChecked = true, Margin = new Thickness(-2, 0, -2, 0), VerticalAlignment = VerticalAlignment.Center, ToolTip = new ToolTip() { Content = "Show/Hide Foreground" }, Focusable = false },
            new Separator() { Width = 10 },
            new MenuItem() { Header = "LAYER", IsEnabled = false },
            new RadioButton() { IsChecked = false, GroupName = "modifylayer", Margin = new Thickness(-2, 0, -2, 0), VerticalAlignment = VerticalAlignment.Center, ToolTip = new ToolTip() { Content = "Select Background" }, Focusable = false },
            new RadioButton() { IsChecked = true, GroupName = "modifylayer", Margin = new Thickness(-2, 0, -2, 0), VerticalAlignment = VerticalAlignment.Center, ToolTip = new ToolTip() { Content = "Select Middle" }, Focusable = false },
            new RadioButton() { IsChecked = false, GroupName = "modifylayer", Margin = new Thickness(-2, 0, -2, 0), VerticalAlignment = VerticalAlignment.Center, ToolTip = new ToolTip() { Content = "Select Foreground" }, Focusable = false },
            new Separator() { Width = 10 },
            new Border() { Child = new Image() { Source = (BitmapImage)App.Current.FindResource("image_map_undo"), Style = imageStyle }, Background = Brushes.White, Margin = new Thickness(-6, 0, -6, 0), Padding = new Thickness(6, 0, 6, 0), ToolTip = new ToolTip() { Content = "Undo [Ctrl + Z]" } },
            new Border() { Child = new Image() { Source = (BitmapImage)App.Current.FindResource("image_map_pen"), Style = imageStyle }, Background = Brushes.White, BorderBrush = Brushes.DodgerBlue, BorderThickness=  new Thickness(1), Margin = new Thickness(-6, 0, -6, 0), Padding = new Thickness(6, 0, 6, 0), ToolTip = new ToolTip() { Content = "Use the Tile Pen [Alt + A]" } },
            new Border() { Child = new Image() { Source = (BitmapImage)App.Current.FindResource("image_map_eraser"), Style = imageStyle }, Background = Brushes.White, BorderBrush = Brushes.DodgerBlue, BorderThickness=  new Thickness(0), Margin = new Thickness(-6, 0, -6,0), Padding = new Thickness(6, 0, 6, 0), ToolTip = new ToolTip() { Content = "Use the Tile Eraser [Alt + S]" } },
            new Border() { Child = new Image() { Source = (BitmapImage)App.Current.FindResource("image_map_fill"), Style = imageStyle }, Background = Brushes.White, BorderBrush = Brushes.DodgerBlue, BorderThickness=  new Thickness(0), Margin = new Thickness(-6, 0, -6 ,0), Padding = new Thickness(6, 0, 6, 0), ToolTip = new ToolTip() { Content = "Use the Tile Filltool [Ctrl + D]" } },
            new Border() { Child = new Image() { Source = (BitmapImage)App.Current.FindResource("image_map_pointer"), Style = imageStyle }, Background = Brushes.White, BorderBrush = Brushes.DodgerBlue, BorderThickness = new Thickness(0), Margin = new Thickness(-6, 0, -6, 0), Padding = new Thickness(6, 0, 6, 0), ToolTip = new ToolTip() { Content = "Set the Player Spawnpoint" } },
            new Border() { Child = new Image() { Source = (BitmapImage)App.Current.FindResource("image_map_rotate"), Style = imageStyle }, Background = Brushes.White, BorderBrush = Brushes.DodgerBlue, BorderThickness = new Thickness(0), Margin = new Thickness(-6, 0, -6, 0), Padding = new Thickness(6, 0, 6, 0), ToolTip = new ToolTip() { Content = "Rotate a Tile [Ctrl + F]" } },
            new Separator() { Width = 5 },
            new Border() { Child = new Image() { Source = (BitmapImage)App.Current.FindResource("image_map_placeentity"), Style = imageStyle }, Background = Brushes.White, BorderBrush = Brushes.DodgerBlue, BorderThickness = new Thickness(0), Margin = new Thickness(-6, 0, -6, 0), Padding = new Thickness(6, 0, 6, 0), ToolTip = new ToolTip() { Content = "Place an Entity" } },
            new Border() { Child = new Image() { Source = (BitmapImage)App.Current.FindResource("image_map_selectentity"), Style = imageStyle }, Background = Brushes.White, BorderBrush = Brushes.DodgerBlue, BorderThickness = new Thickness(0), Margin = new Thickness(-6, 0, -6, 0), Padding = new Thickness(6, 0, 6, 0), ToolTip = new ToolTip() { Content = "Select and Move an Entity" } },
            new Border() { Child = new Image() { Source = (BitmapImage)App.Current.FindResource("image_map_killentity"), Style = imageStyle }, Background = Brushes.White, BorderBrush = Brushes.DodgerBlue, BorderThickness = new Thickness(0), Margin = new Thickness(-6, 0, -6, 0), Padding = new Thickness(6, 0, 6, 0), ToolTip = new ToolTip() { Content = "Kill an Entity" } },
            new Border() { Child = new Image() { Source = (BitmapImage)App.Current.FindResource("image_map_finishvectorrequest"), Style = imageStyle }, Background = Brushes.White, BorderBrush = Brushes.DodgerBlue, BorderThickness = new Thickness(0), Margin = new Thickness(-6, 0, -6, 0), Padding = new Thickness(6, 0, 6, 0), IsEnabled = false, ToolTip = new ToolTip() { Content = "Finish Vector Request" } },
            new Separator() { Width = 5 },
            new Border() { Child = new Image() { Source = (BitmapImage)App.Current.FindResource("image_map_tilebrush"), Style = imageStyle }, Background = Brushes.White, BorderBrush = Brushes.DodgerBlue, BorderThickness = new Thickness(0), Margin = new Thickness(-6, 0, -6, 0), Padding = new Thickness(6, 0, 6, 0), ToolTip = new ToolTip() { Content = "Use the Tile Brush" } },
        };
        public List<FrameworkElement> Menu { get { return _Menu; } }

        private EditorMap _Map;
        public EditorMap Map {
            get { return _Map; }
            set {
                _Map = value;

                scrollbar_horizontal.Minimum = -tilemapview.ActualWidth / tilemapview.TileSize;
                scrollbar_horizontal.Maximum = _Map.Width;
                scrollbar_horizontal.Value = 0;
                scrollbar_vertical.Minimum = -tilemapview.ActualHeight / tilemapview.TileSize;
                scrollbar_vertical.Maximum = _Map.Height;
                scrollbar_vertical.Value = 0;

                if (GraphicsDevice != null) {
                    _Map.Init(GraphicsDevice);
                    UpdateListbox( );
                    tilemapview.Map = _Map;
                    tilemapview.Update( );
                } else {
                    tilemapview.DeviceInitialized += ( ) => {
                        _Map.Init(GraphicsDevice);
                        UpdateListbox( );
                        tilemapview.Map = _Map;
                        tilemapview.Update( );
                    };
                }

                listview_brushes.Items.Clear( );
                foreach (TileBrush brush in _Map.Brushes)
                    listview_brushes.Items.Add(brush);
            }
        }

        public GraphicsDevice GraphicsDevice;

        // Map Cache List von Koord Layer alter Wert IsRotation
        private Stack<List<Tuple<Point, int, int, bool>>> Cache = new Stack<List<Tuple<Point, int, int, bool>>>( );

        private int currentLayer = 1;
        private Tool currentTool = Tool.Pen;
        private Dictionary<TileAttribute, string> defaultAttributes = new Dictionary<TileAttribute, string>( );
        private Entity cachedEntity, currentlySelectingEntity, currentlySelectedEntity;
        private Func<Vector2, bool> currentVectorRequestCallback;
        private Microsoft.Xna.Framework.Vector2 selectedTile;

        private Tile currentTile { get { return (wrappanel_tiles.SelectedIndex != -1) ? Map.Tiles[wrappanel_tiles.SelectedIndex] : Map.Tiles[0]; } }
        private int currentTileIndex { get { return wrappanel_tiles.SelectedIndex; } }
        private TileBrush currentBrush { get { return (TileBrush)listview_brushes.SelectedItem; } }

        public MapEditor ( ) {
            InitializeComponent( );

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
                tilemapview.EntityData = entitylistbox.GetEntityData( );
            };

            ((Image)_Menu[0]).MouseDown += (sender, e) => {
                new ModifyMapWindow(Map).ShowDialog( );
            };

            ((CheckBox)_Menu[2]).Checked += (sender, e) => { tilemapview.ShowBackground = true; };
            ((CheckBox)_Menu[2]).Unchecked += (sender, e) => { tilemapview.ShowBackground = false; };
            ((CheckBox)_Menu[3]).Checked += (sender, e) => { tilemapview.ShowMiddle = true; };
            ((CheckBox)_Menu[3]).Unchecked += (sender, e) => { tilemapview.ShowMiddle = false; };
            ((CheckBox)_Menu[4]).Checked += (sender, e) => { tilemapview.ShowForeground = true; };
            ((CheckBox)_Menu[4]).Unchecked += (sender, e) => { tilemapview.ShowForeground = false; };

            ((RadioButton)_Menu[7]).Checked += (sender, e) => { currentLayer = 0; };
            ((RadioButton)_Menu[8]).Checked += (sender, e) => { currentLayer = 1; };
            ((RadioButton)_Menu[9]).Checked += (sender, e) => { currentLayer = 2; };

            _Menu[11].MouseDown += (sender, e) => UndoLast( );

            _Menu[12].MouseDown += (sender, e) => SelectTool(Tool.Pen);
            _Menu[13].MouseDown += (sender, e) => SelectTool(Tool.Eraser);
            _Menu[14].MouseDown += (sender, e) => SelectTool(Tool.Filler);
            _Menu[15].MouseDown += (sender, e) => SelectTool(Tool.Pointer);
            _Menu[16].MouseDown += (sender, e) => SelectTool(Tool.Rotater);

            _Menu[18].MouseDown += (sender, e) => SelectTool(Tool.God);
            _Menu[19].MouseDown += (sender, e) => SelectTool(Tool.Hand);
            _Menu[20].MouseDown += (sender, e) => SelectTool(Tool.Trashcan);
            _Menu[21].MouseDown += (sender, e) => SelectTool(Tool.Hand);

            _Menu[23].MouseDown += (sender, e) => SelectTool(Tool.Brush);

            defaultAttributes = JsonConvert.DeserializeObject<Dictionary<TileAttribute, string>>(Properties.Settings.Default.DefaultTileAttributes);

            App.Current.MainWindow.Closing += (sender, e) => {
                Properties.Settings.Default.DefaultTileAttributes = JsonConvert.SerializeObject(defaultAttributes);
            };
        }

        public void ChangeTextureName (string oldname, string newname) {
            Texture2D texture = _Map.XnaTextures[oldname];
            Map.XnaTextures.Remove(oldname);
            Map.XnaTextures.Add(newname, texture);

            BitmapImage image = Map.WpfTextures[oldname];
            Map.WpfTextures.Remove(oldname);
            Map.WpfTextures.Add(newname, image);
        }

        public void UpdateListbox ( ) {
            wrappanel_tiles.Items.Clear( );
            foreach (Tile tile in Map.Tiles) {
                wrappanel_tiles.Items.Add(new ListViewEntry(Map.WpfTextures[tile.Name]));
            }
            if (wrappanel_tiles.HasItems)
                wrappanel_tiles.SelectedIndex = 0;
        }

        private void buttonexport_Click (object sender, RoutedEventArgs e) {
            // export current maps tileset
            SaveFileDialog exportDialog = new SaveFileDialog( );
            exportDialog.Filter = "TileTemplate|*.mkttemplate";
            if (exportDialog.ShowDialog( ) ?? false) {
                using (Stream stream = File.OpenWrite(exportDialog.FileName))
                    TileSerializer.Serialize(stream, Map.Tiles, Map.XnaTextures, tilemapview.GraphicsDevice);
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

        private void ResetToolBorders ( ) {
            ((Border)_Menu[12]).BorderThickness = new Thickness(0);
            ((Border)_Menu[13]).BorderThickness = new Thickness(0);
            ((Border)_Menu[14]).BorderThickness = new Thickness(0);
            ((Border)_Menu[15]).BorderThickness = new Thickness(0);
            ((Border)_Menu[16]).BorderThickness = new Thickness(0);
            ((Border)_Menu[18]).BorderThickness = new Thickness(0);
            ((Border)_Menu[19]).BorderThickness = new Thickness(0);
            ((Border)_Menu[20]).BorderThickness = new Thickness(0);
            ((Border)_Menu[21]).BorderThickness = new Thickness(0);
        }

        private void scrollbar_horizontal_ValueChanged (object sender, RoutedPropertyChangedEventArgs<double> e) {
            tilemapview.Offset = new Microsoft.Xna.Framework.Vector2((float)scrollbar_horizontal.Value, tilemapview.Offset.Y);
        }

        private void scrollbar_vertical_ValueChanged (object sender, RoutedPropertyChangedEventArgs<double> e) {
            tilemapview.Offset = new Microsoft.Xna.Framework.Vector2(tilemapview.Offset.X, (float)scrollbar_vertical.Value);
        }

        private void SelectTool (Tool tool) {
            if (currentTool == Tool.VectorGrabber) {
                _Menu[23].IsEnabled = false;
                tilemapview.BorderBrush = Brushes.White;
                tilemapview.CurrentSelection = new Microsoft.Xna.Framework.Vector2(-1, -1);
            } else if (tool == Tool.VectorGrabber) {
                tilemapview.BorderBrush = Brushes.OrangeRed;
            }
            if (tool != Tool.Hand && tool != Tool.VectorGrabber) {
                contentpresenter_entitydata.Content = null;
                tilemapview.AdditionalRenderCall = null;
                if (currentlySelectedEntity != null) {
                    currentlySelectedEntity.Domain = EntityDomain.Enemy;
                    currentlySelectedEntity = null;
                }
            }

            if (cachedEntity != null) {
                Map.Entities.Remove(cachedEntity);
                cachedEntity = null;
            }

            if ((int)tool < 5) {
                tabcontrol_toolselect.SelectedIndex = 0;
            } else if ((int)tool < 10) {
                tabcontrol_toolselect.SelectedIndex = 1;
            } else {
                tabcontrol_toolselect.SelectedIndex = 2;
            }
            currentTool = tool;
            ResetToolBorders( );
            ((Border)_Menu[12 + (int)tool]).BorderThickness = new Thickness(1);
        }

        private void tilemapview_MouseDown (object sender, MouseButtonEventArgs e) {
            switch (tabcontrol_toolselect.SelectedIndex) {
                case 0:
                    if (currentTileIndex == -1 || !tilemapview.IsLayerActive(currentLayer))
                        HandleTilemapViewClickTiles(sender, e);
                    break;
                case 1:
                    HandleTilemapViewClickEntities(sender, e);
                    break;
                case 2:
                    HandleTilemapViewClickBrush(sender, e);
                    break;
            }
        }

        private void HandleTilemapViewClickTiles (object sender, MouseButtonEventArgs e) {
            Point clickedTile = new Point(Math.Floor(selectedTile.X), Map.Height - Math.Floor(selectedTile.Y) - 1);
            if (e.RightButton == MouseButtonState.Pressed) {
                Cache.Push(new List<Tuple<Point, int, int, bool>>( ) {
                                    Tuple.Create(clickedTile,currentLayer, Map.Data[(int)clickedTile.X, (int)clickedTile.Y, currentLayer], false)});
                Map.Data[(int)clickedTile.X, (int)clickedTile.Y, currentLayer] = 0;
            } else if (e.LeftButton == MouseButtonState.Pressed) {
                switch (currentTool) {
                    case Tool.Eraser:
                        Cache.Push(new List<Tuple<Point, int, int, bool>>( ) {
                                    new Tuple<Point, int, int,bool>(clickedTile,currentLayer, Map.Data[(int)clickedTile.X, (int)clickedTile.Y, currentLayer], false)});
                        Map.Data[(int)clickedTile.X, (int)clickedTile.Y, currentLayer] = 0;
                        Map.Rotations[(int)clickedTile.X, (int)clickedTile.Y, currentLayer] = 0;
                        break;

                    case Tool.Filler:
                        if (Map.Data[(int)clickedTile.X, (int)clickedTile.Y, currentLayer] == currentTileIndex)
                            break;
                        int searching = Map.Data[(int)clickedTile.X, (int)clickedTile.Y, currentLayer];
                        int replacing = currentTileIndex;
                        Queue<Point> pointQueue = new Queue<Point>( );
                        pointQueue.Enqueue(clickedTile);
                        List<Tuple<Point, int, int, bool>> changesForCache = new List<Tuple<Point, int, int, bool>>( );
                        while (pointQueue.Count > 0) {
                            Point current = pointQueue.Dequeue( );
                            if (current.X < 0 || current.X >= Map.Width || current.Y < 0 || current.Y >= Map.Height)
                                continue;
                            if (Map.Data[(int)current.X, (int)current.Y, currentLayer] == searching) {
                                Map.Data[(int)current.X, (int)current.Y, currentLayer] = replacing;
                                Map.Rotations[(int)clickedTile.X, (int)clickedTile.Y, currentLayer] = 0;
                                changesForCache.Add(Tuple.Create(current, currentLayer, searching, false));

                                pointQueue.Enqueue(new Point(current.X - 1, current.Y));
                                pointQueue.Enqueue(new Point(current.X + 1, current.Y));
                                pointQueue.Enqueue(new Point(current.X, current.Y - 1));
                                pointQueue.Enqueue(new Point(current.X, current.Y + 1));
                            }
                        }
                        Cache.Push(changesForCache);
                        break;

                    case Tool.Pen:
                        Cache.Push(new List<Tuple<Point, int, int, bool>>( ) {
                                    Tuple.Create(clickedTile,currentLayer, Map.Data[(int)clickedTile.X, (int)clickedTile.Y, currentLayer], false)});
                        Map.Data[(int)clickedTile.X, (int)clickedTile.Y, currentLayer] = currentTileIndex;
                        Map.Rotations[(int)clickedTile.X, (int)clickedTile.Y, currentLayer] = 0;
                        break;

                    case Tool.Pointer:
                        Map.SpawnPoint = new Vector2((int)clickedTile.X, (int)clickedTile.Y);
                        break;

                    case Tool.Rotater:
                        if (Map.Tiles[Map.Data[(int)clickedTile.X, (int)clickedTile.Y, currentLayer]].Name == "None") {
                            for (int i = 2; i > -1; i--) {
                                if (Map.Tiles[Map.Data[(int)clickedTile.X, (int)clickedTile.Y, i]].Name != "None") {
                                    ((RadioButton)_Menu[9 + i]).IsChecked = true;
                                }
                            }
                        }
                        float tileRotation = Map.Rotations[(int)clickedTile.X, (int)clickedTile.Y, currentLayer];
                        Cache.Push(new List<Tuple<Point, int, int, bool>>( ) {
                                    Tuple.Create(clickedTile,currentLayer, (int)(tileRotation * 2), true)});
                        tileRotation += 0.5f;
                        tileRotation %= 2f;
                        if (Map.GetTile((int)clickedTile.X, (int)clickedTile.Y, currentLayer).Name != "None")
                            Map.Rotations[(int)clickedTile.X, (int)clickedTile.Y, currentLayer] = tileRotation;
                        break;
                }
                tilemapview.Update( );
            }
        }

        private Vector2 GetEntityCenterRaw (MouseEventArgs e) {
            Point positionOnControl = e.GetPosition(tilemapview);
            Vector2 selectedTile = new Vector2(
                (float)Math.Max(0, Math.Min(positionOnControl.X / tilemapview.TileSize + tilemapview.Offset.X, Map.Width)),
                (float)Math.Max(0, Math.Min(Map.Size.Height - positionOnControl.Y / tilemapview.TileSize - tilemapview.Offset.Y, Map.Height)));
            return selectedTile;
        }


        private Vector2 GetEntityCenter (MouseEventArgs e) {
            Vector2 selectedTile = GetEntityCenterRaw(e);
            selectedTile.Y = (float)Math.Floor(selectedTile.Y);
            return selectedTile;
        }

        private void HandleTilemapViewClickEntities (object sender, MouseButtonEventArgs e) {
            if (currentTool == Tool.God || currentTool == Tool.Hand) {
                contentpresenter_entitydata.Content = null;
                tilemapview.AdditionalRenderCall = null;
            }
            switch (currentTool) {
                case Tool.God:
                    Vector2 clickedPosition = Keyboard.IsKeyDown(Key.LeftShift) ? GetEntityCenter(e) : GetEntityCenterRaw(e);
                    entitylistbox.GetCurrentFinalConfiguration( ).Create(clickedPosition, Map, Keyboard.IsKeyDown(Key.LeftShift));
                    if (cachedEntity != null) {
                        Map.Entities.Remove(cachedEntity);
                        cachedEntity = null;
                    }
                    tilemapview.Update( );
                    break;
                case Tool.Hand:
                    Entity clickedEntity = GetClickedEntity(e);
                    if (clickedEntity != null) {
                        if (currentlySelectedEntity != null) {
                            currentlySelectedEntity.Domain = EntityDomain.Enemy;
                        }
                        currentlySelectedEntity = clickedEntity;
                        currentlySelectedEntity.Domain = EntityDomain.Obstacle;
                        foreach (Component c in clickedEntity.GetComponents( )) {
                            if (c is IUserControlComponent) {
                                IUserControlComponent uc = c as IUserControlComponent;
                                uc.RequestMapVectorList = HandleMapVectorListRequest;
                                contentpresenter_entitydata.Content = uc.Control;
                                tilemapview.AdditionalRenderCall = uc.Render;
                                uc.RequestRender += ( ) => tilemapview.Update( );
                            }
                        }
                        tilemapview.Update( );
                    }
                    break;
                case Tool.Trashcan:
                    clickedEntity = GetClickedEntity(e);
                    if (clickedEntity != null) {
                        Map.Entities.Remove(clickedEntity);
                        tilemapview.Update( );
                        currentlySelectingEntity = null;
                    }
                    break;
                case Tool.VectorGrabber:
                    clickedPosition = GetEntityCenterRaw(e);
                    if (Keyboard.IsKeyDown(Key.LeftShift)) {
                        clickedPosition.X = (float)Math.Floor(clickedPosition.X) + 0.5f;
                        clickedPosition.Y = (float)Math.Floor(clickedPosition.Y) + 0.5f;
                    }
                    if (!currentVectorRequestCallback(clickedPosition))
                        SelectTool(Tool.Hand);
                    break;
            }
        }

        private void HandleTilemapViewClickBrush (object sender, MouseButtonEventArgs e) {
            if (currentBrush == null) return;

            if (e.LeftButton == MouseButtonState.Pressed) {
                int px0 = (int)selectedTile.X;
                int py0 = Map.Height - (int)selectedTile.Y - 1;
                Map.Data[px0, py0, currentLayer] = Array.FindIndex(Map.Tiles, tile => tile.Name == currentBrush.Centre[0].Tile.Name);

                for (int x = -1; x <= 1; x++) {
                    for (int y = -1; y <= 1; y++) {
                        int px = px0 + x;
                        int py = py0 + y;

                        if (px >= 0 && px < Map.Width && py >= 0 && py < Map.Height && currentBrush.Contains(Map.Tiles[Map.Data[px, py, currentLayer]].Name)) {
                            (Tile tile, float rotation) replacement = currentBrush.Get(Map.GetTile(px, py, currentLayer), Map.Rotations[px, py, currentLayer], GetBrushData(px, py));
                            Map.Data[px, py, currentLayer] = Array.FindIndex(Map.Tiles, tile => tile.Name == replacement.tile.Name);
                            Map.Rotations[px, py, currentLayer] = replacement.rotation;
                        }
                    }
                }

                tilemapview.Update( );
            }
        }

        private void HandleMapVectorListRequest (Func<Vector2, bool> callback) {
            _Menu[23].IsEnabled = true;
            currentVectorRequestCallback = callback;
            SelectTool(Tool.VectorGrabber);
        }

        private Entity GetClickedEntity (MouseEventArgs e) {
            Vector2 clickedPosition = GetEntityCenterRaw(e);
            for (int i = Map.Entities.Count - 1; i >= 0; i--) {
                if (Map.Entities[i].Transform.Intersects(clickedPosition)) {
                    return Map.Entities[i];
                }
            }
            return null;
        }

        private void tilemapview_MouseEnter (object sender, MouseEventArgs e) {
            if (tabcontrol_toolselect.SelectedIndex == 0)
                UpdateSelectedTile(e);
        }

        private void tilemapview_MouseLeave (object sender, MouseEventArgs e) {
            tilemapview.CurrentSelection = new Microsoft.Xna.Framework.Vector2(-1, -1);
            if (cachedEntity != null) {
                Map.Entities.Remove(cachedEntity);
                cachedEntity = null;
            }
            tilemapview.Update( );
        }

        private void tilemapview_MouseMove (object sender, MouseEventArgs e) {
            switch (tabcontrol_toolselect.SelectedIndex) {
                case 0:
                    HandleTilemapViewMoveTiles(sender, e, UpdateSelectedTile(e));
                    break;
                case 1:
                    HandleTilemapViewMoveEntities(sender, e);
                    break;
                case 2:
                    HandleTilemapViewMoveBrush(sender, e, UpdateSelectedTile(e));
                    break;
            }
        }

        private void HandleTilemapViewMoveTiles (object sender, MouseEventArgs e, bool updated) {
            if (!updated) return;

            Point clickedTile = new Point(Math.Floor(selectedTile.X), Map.Height - Math.Floor(selectedTile.Y) - 1);
            if (e.RightButton == MouseButtonState.Pressed) {
                Tuple<Point, int, int, bool> delta = Tuple.Create(clickedTile, currentLayer, Map.Data[(int)clickedTile.X, (int)clickedTile.Y, currentLayer], false);
                if (Cache.Count > 0) {
                    Tuple<Point, int, int, bool> last = Cache.Peek( ).Last( );
                    if (Map.Data[(int)last.Item1.X, (int)last.Item1.Y, last.Item2] == 0) {
                        Cache.Peek( ).Add(delta);
                    } else {
                        Cache.Push(new List<Tuple<Point, int, int, bool>>( ) { delta });
                    }
                } else {
                    Cache.Push(new List<Tuple<Point, int, int, bool>>( ) { delta });
                }
                Map.Data[(int)clickedTile.X, (int)clickedTile.Y, currentLayer] = 0;
                updated = true;
            } else if (e.LeftButton == MouseButtonState.Pressed) {
                switch (currentTool) {
                    case Tool.Eraser:
                        Cache.Peek( ).Add(Tuple.Create(clickedTile, currentLayer, Map.Data[(int)clickedTile.X, (int)clickedTile.Y, currentLayer], false));
                        Map.Data[(int)clickedTile.X, (int)clickedTile.Y, currentLayer] = 0;
                        Map.Rotations[(int)clickedTile.X, (int)clickedTile.Y, currentLayer] = 0;
                        break;

                    case Tool.Pen:
                        Cache.Peek( ).Add(Tuple.Create(clickedTile, currentLayer, Map.Data[(int)clickedTile.X, (int)clickedTile.Y, currentLayer], false));
                        Map.Data[(int)clickedTile.X, (int)clickedTile.Y, currentLayer] = currentTileIndex;
                        Map.Rotations[(int)clickedTile.X, (int)clickedTile.Y, currentLayer] = 0;
                        break;

                    case Tool.Pointer:
                        Map.SpawnPoint = new Vector2((int)clickedTile.X, (int)clickedTile.Y);
                        break;
                }
                updated = updated || (currentTool != Tool.Filler && currentTool != Tool.Rotater);
            }
            if (updated)
                tilemapview.Update( );
        }

        private void HandleTilemapViewMoveEntities (object sender, MouseEventArgs e) {
            Vector2 entityLocation = Keyboard.IsKeyDown(Key.LeftShift) ? GetEntityCenter(e) : GetEntityCenterRaw(e);
            switch (currentTool) {
                case Tool.God:
                    tilemapview.CurrentSelection = new Microsoft.Xna.Framework.Vector2(entityLocation.X - tilemapview.Offset.X, Map.Height - entityLocation.Y - tilemapview.Offset.Y);
                    if (cachedEntity == null) {
                        cachedEntity = entitylistbox.GetCurrentShadowConfiguration( ).Create(entityLocation, Map, Keyboard.IsKeyDown(Key.LeftShift));
                    } else {
                        if (Keyboard.IsKeyDown(Key.LeftShift))
                            entityLocation.Y += cachedEntity.Transform.Height / 2;
                        cachedEntity.Transform.Center = entityLocation;
                    }
                    tilemapview.Update( );
                    break;
                case Tool.Trashcan:
                    Entity selectedEntity = GetClickedEntity(e);
                    bool changed = (currentlySelectingEntity != null || selectedEntity != null) && currentlySelectingEntity != selectedEntity;
                    if (currentlySelectingEntity != null) {
                        currentlySelectingEntity.Domain = EntityDomain.Enemy;
                        currentlySelectingEntity = null;
                    }
                    if (selectedEntity != null) {
                        selectedEntity.Domain = EntityDomain.NPC;
                        currentlySelectingEntity = selectedEntity;
                    }
                    if (changed)
                        tilemapview.Update( );
                    break;
                case Tool.Hand:
                    if (currentlySelectedEntity != null && Mouse.RightButton == MouseButtonState.Pressed) {
                        currentlySelectedEntity.Transform.Center = Keyboard.IsKeyDown(Key.LeftShift) ? (GetEntityCenter(e) + new Vector2(0f, currentlySelectedEntity.Transform.HalfSize.Y)) : GetEntityCenterRaw(e);
                        currentlySelectedEntity.Update(default(DeltaTime));
                        tilemapview.Update( );
                    } else {
                        selectedEntity = GetClickedEntity(e);
                        changed = (currentlySelectingEntity != null || selectedEntity != null) && currentlySelectingEntity != selectedEntity;
                        if (currentlySelectingEntity != null && currentlySelectedEntity != currentlySelectingEntity) {
                            currentlySelectingEntity.Domain = EntityDomain.Enemy;
                            currentlySelectingEntity = null;
                        }
                        if (selectedEntity != null) {
                            selectedEntity.Domain = EntityDomain.Obstacle;
                            currentlySelectingEntity = selectedEntity;
                        }
                        if (changed)
                            tilemapview.Update( );
                    }
                    break;
                case Tool.VectorGrabber:
                    Vector2 clickedPosition = GetEntityCenterRaw(e);
                    if (Keyboard.IsKeyDown(Key.LeftShift)) {
                        clickedPosition.X = (float)Math.Floor(clickedPosition.X) + 0.5f;
                        clickedPosition.Y = (float)Math.Floor(clickedPosition.Y) + 0.5f;
                    }
                    float rx = clickedPosition.X - tilemapview.Offset.X, ry = Map.Height - clickedPosition.Y - tilemapview.Offset.Y;
                    if (rx != tilemapview.CurrentSelection.X || ry != tilemapview.CurrentSelection.Y) {
                        tilemapview.CurrentSelection = new Microsoft.Xna.Framework.Vector2(rx, ry);
                        tilemapview.Update( );
                    }
                    break;
            }
        }

        private void HandleTilemapViewMoveBrush (object sender, MouseEventArgs e, bool updated) {
            if (!updated) return;

            if (e.LeftButton == MouseButtonState.Pressed && currentBrush != null) {
                int px0 = (int)selectedTile.X;
                int py0 = Map.Height - (int)selectedTile.Y - 1;
                Map.Data[px0, py0, currentLayer] = Array.FindIndex(Map.Tiles, tile => tile.Name == currentBrush.Centre[0].Tile.Name);

                for (int x = -1; x <= 1; x++) {
                    for (int y = -1; y <= 1; y++) {
                        int px = px0 + x;
                        int py = py0 + y;

                        if (px >= 0 && px < Map.Width && py >= 0 && py < Map.Height && currentBrush.Contains(Map.Tiles[Map.Data[px, py, currentLayer]].Name)) {
                            (Tile tile, float rotation) replacement = currentBrush.Get(Map.GetTile(px, py, currentLayer), Map.Rotations[px, py, currentLayer], GetBrushData(px, py));
                            Map.Data[px, py, currentLayer] = Array.FindIndex(Map.Tiles, tile => tile.Name == replacement.tile.Name);
                            Map.Rotations[px, py, currentLayer] = replacement.rotation;
                        }
                    }
                }

                tilemapview.Update( );
            } else if (updated) {
                tilemapview.Update( );
            }
        }

        private bool[ ] GetBrushData (int x, int y) {
            /* 0 1 2
             * 3 - 4 
             * 5 6 7
             */
            return new[ ] {
                ((x - 1 >= 0 && y + 1 < Map.Height) ? currentBrush.Contains(Map.Tiles[Map.Data[x - 1, y + 1, currentLayer]].Name): true),
                ((y + 1 < Map.Height) ? currentBrush.Contains(Map.Tiles[Map.Data[x, y + 1, currentLayer]].Name): true),
                ((x + 1 < Map.Width && y + 1 < Map.Height) ? currentBrush.Contains(Map.Tiles[Map.Data[x + 1, y + 1, currentLayer]].Name): true),

                ((x - 1 >= 0) ? currentBrush.Contains(Map.Tiles[Map.Data[x - 1, y, currentLayer]].Name): true),
                ((x + 1 < Map.Width) ? currentBrush.Contains(Map.Tiles[Map.Data[x + 1, y, currentLayer]].Name): true),

                ((x - 1 >= 0 && y - 1 >= 0) ? currentBrush.Contains(Map.Tiles[Map.Data[x - 1, y - 1, currentLayer]].Name): true),
                ((y - 1 >= 0) ? currentBrush.Contains(Map.Tiles[Map.Data[x, y - 1, currentLayer]].Name): true),
                ((x + 1 < Map.Width && y - 1 >= 0) ? currentBrush.Contains(Map.Tiles[Map.Data[x + 1, y - 1, currentLayer]].Name): true),
            };
        }

        private void tilemapview_PreviewMouseWheel (object sender, MouseWheelEventArgs e) {
            scrollbar_horizontal.Minimum = -tilemapview.ActualWidth / tilemapview.TileSize;
            scrollbar_vertical.Minimum = -tilemapview.ActualHeight / tilemapview.TileSize;
            if (e.Delta > 0) {
                tilemapview.ZoomLevel++;
            } else {
                tilemapview.ZoomLevel--;
            }
        }

        private void UndoLast ( ) {
            if (Cache.Count == 0)
                return;

            foreach (Tuple<Point, int, int, bool> undoData in Cache.Pop( )) {
                if (undoData.Item4) {
                    // tile got rotated
                    Map.Rotations[(int)undoData.Item1.X, (int)undoData.Item1.Y, undoData.Item2] = undoData.Item3 / 2f;
                } else {
                    // tile got changed
                    Map.Data[(int)undoData.Item1.X, (int)undoData.Item1.Y, undoData.Item2] = undoData.Item3;
                }
            }
            tilemapview.Update( );
        }

        private bool UpdateSelectedTile (MouseEventArgs e) {
            Point positionOnControl = e.GetPosition(tilemapview);
            Microsoft.Xna.Framework.Vector2 nextSelectedTile = new Microsoft.Xna.Framework.Vector2(
                (float)Math.Max(0, Math.Min(positionOnControl.X / tilemapview.TileSize + tilemapview.Offset.X, Map.Width - 1)),
                (float)Math.Max(0, Math.Min(positionOnControl.Y / tilemapview.TileSize + tilemapview.Offset.Y, Map.Height - 1)));
            bool changed = Math.Floor(selectedTile.X) != Math.Floor(nextSelectedTile.X) || Math.Floor(selectedTile.Y) != Math.Floor(nextSelectedTile.Y);

            selectedTile = nextSelectedTile;
            text_xpos.Text = Math.Floor(selectedTile.X).ToString( );
            text_ypos.Text = Math.Floor(Map.Height - selectedTile.Y).ToString( );
            tilemapview.CurrentSelection = new Microsoft.Xna.Framework.Vector2((float)Math.Floor(selectedTile.X), (float)Math.Floor(selectedTile.Y)) - tilemapview.Offset;
            return changed;
        }

        private void wrappanel_tiles_DragEnter (object sender, DragEventArgs e) {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effects = DragDropEffects.Copy;
        }

        private void wrappanel_tiles_Drop (object sender, DragEventArgs e) {
            if (Map == null)
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
                if (!Map.Tiles.Any(t => t.Name == textbox_tile_name.Text)) {
                    ChangeTextureName(Map.Tiles[oldSelection].Name, textbox_tile_name.Text);
                    Map.Tiles[oldSelection].Name = textbox_tile_name.Text;
                }
                Map.Tiles[oldSelection].Attributes.Clear( );
                foreach (AttributeListViewEntry entry in listview_tile_attributes.Items) {
                    if (entry.Active)
                        Map.Tiles[oldSelection].Attributes.Add((TileAttribute)Enum.Parse(typeof(TileAttribute), entry.Attribute), entry.Value);
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
            } else {
                wrappanel_tiles.SelectedIndex = 0;
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
            if (index > 0) {
                wrappanel_tiles.Items.RemoveAt(index);
                Map.RemoveTile(index);
                tilemapview.Update( );
            }
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
                Tile prevTile = Map.Tiles[index];
                Map.WpfTextures.Remove(prevTile.Name);
                Map.XnaTextures.Remove(prevTile.Name);

                Map.LoadTexture(tileName, tileImage, GraphicsDevice);
                Map.Tiles[index].Name = tileName;
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
            if (checkbox_auto.IsChecked ?? false && !Map.Tiles.Any(t => t.Name == tileName)) {
                // add tile
                BitmapImage tileImage = LoadTileImage(imagefile);

                Map.LoadTexture(tileName, tileImage, GraphicsDevice);
                Map.AddTile(new Tile( ) { Attributes = new Dictionary<TileAttribute, string>(defaultAttributes), Name = tileName });
                wrappanel_tiles.Items.Add(new ListViewEntry(tileImage));
            } else {
                // open add tile window
                AddTileWindow addTileDialog = new AddTileWindow(imagefile, Map.Tiles.Select(t => t.Name), defaultAttributes);
                if (addTileDialog.ShowDialog( ) ?? false) {
                    if (Map.Tiles.Where(t => t.Name == addTileDialog.Created.Item1.Name) != null) {
                        Map.LoadTexture(addTileDialog.Created.Item1.Name, addTileDialog.Created.Item2, GraphicsDevice);
                        Map.AddTile(addTileDialog.Created.Item1);
                        wrappanel_tiles.Items.Add(new ListViewEntry(addTileDialog.Created.Item2));
                    } else {
                        MessageBox.Show("Please don't add tiles with the same name twice!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void tilemapview_SizeChanged (object sender, SizeChangedEventArgs e) {
            scrollbar_horizontal.Minimum = -tilemapview.ActualWidth / tilemapview.TileSize;
            scrollbar_vertical.Minimum = -tilemapview.ActualHeight / tilemapview.TileSize;
        }

        private BitmapImage LoadTileImage (string file) {
            BitmapImage image = new BitmapImage( );
            image.BeginInit( );
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.CreateOptions = BitmapCreateOptions.None;
            image.DecodePixelWidth = Core.Map.TILE_PXL_SIZE;
            image.DecodePixelHeight = Core.Map.TILE_PXL_SIZE;
            image.UriSource = new Uri(file);
            image.EndInit( );
            return image;
        }

        private void CommandBinding_CreateBrush_Executed (object sender, ExecutedRoutedEventArgs e) {
            AddBrushDialog addBrushDialog = new AddBrushDialog(Map);
            if (addBrushDialog.ShowDialog( ) ?? false) {
                listview_brushes.Items.Add(addBrushDialog.DialogResultBrush);
                Map.Brushes.Add(addBrushDialog.DialogResultBrush);
            }
        }

        private void CommandBinding_CreateBrush_CanExecute (object sender, CanExecuteRoutedEventArgs e) {
            e.CanExecute = true;
        }

        private void CommandBinding_RemoveBrush_Executed (object sender, ExecutedRoutedEventArgs e) {
            listview_brushes.Items.RemoveAt(listview_brushes.SelectedIndex);
            Map.Brushes.RemoveAt(listview_brushes.SelectedIndex);
        }

        private void CommandBinding_RemoveBrush_CanExecute (object sender, CanExecuteRoutedEventArgs e) {
            e.CanExecute = listview_brushes?.SelectedItem != null;
        }

        private void tabcontrol_toolselect_SelectionChanged (object sender, SelectionChangedEventArgs e) {
            tilemapview.Mode = tabcontrol_toolselect.SelectedIndex;
            if (tabcontrol_toolselect.SelectedIndex == 0) {
                SelectTool(Tool.Pen);
            } else if (tabcontrol_toolselect.SelectedIndex == 1) {
                SelectTool(Tool.God);
            } else {
                SelectTool(Tool.Brush);
            }
        }
    }
}