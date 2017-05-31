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
            new Border() { Child = new Image() { Source = (BitmapImage)App.Current.FindResource("image_map_finishvectorrequest"), Style = imageStyle }, Background = Brushes.White, BorderBrush = Brushes.DodgerBlue, BorderThickness = new Thickness(0), Margin = new Thickness(-6, 0, -6, 0), Padding = new Thickness(6, 0, 6, 0), IsEnabled = false, ToolTip = new ToolTip() { Content = "Finish Vector Request" } },
        };

        // Map Cache List von Koord Layer alter Wert IsRotation
        private Stack<List<Tuple<Point, int, int, bool>>> Cache = new Stack<List<Tuple<Point, int, int, bool>>>( );

        private int currentLayer = 1;
        private Tool currentTool = Tool.Pen;
        private Dictionary<TileAttribute, string> defaultAttributes = new Dictionary<TileAttribute, string>( );
        private Entity cachedEntity, currentlySelectingEntity, currentlySelectedEntity;
        private Func<Vector2, bool> currentVectorRequestCallback;
        private Microsoft.Xna.Framework.Vector2 selectedTile;
        private EditorMap map;

        public GraphicsDevice GraphicsDevice;
        public string Description { get { return ToString( ); } }

        public MapEditor (EditorMap map) {
            InitializeComponent( );
            this.map = map;

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
                map.Init(GraphicsDevice);
                UpdateListbox( );

                entitylistbox.Init(tilemapview.GraphicsDevice);
                tilemapview.EntityData = entitylistbox.GetEntityData( );
            };

            ((Image)_Menu[0]).MouseDown += (sender, e) => {
                new ModifyMapWindow(map).ShowDialog( );
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

            defaultAttributes = JsonConvert.DeserializeObject<Dictionary<TileAttribute, string>>(Properties.Settings.Default.DefaultTileAttributes);

            App.Current.MainWindow.Closing += (sender, e) => {
                Properties.Settings.Default.DefaultTileAttributes = JsonConvert.SerializeObject(defaultAttributes);
            };

            Loaded += (sender, e) => {
                scrollbar_horizontal.Minimum = -tilemapview.ActualWidth / tilemapview.TileSize;
                scrollbar_horizontal.Maximum = map.Width;
                scrollbar_horizontal.Value = 0;
                scrollbar_vertical.Minimum = -tilemapview.ActualHeight / tilemapview.TileSize;
                scrollbar_vertical.Maximum = map.Height;
                scrollbar_vertical.Value = map.Height - tilemapview.RenderSize.Height / tilemapview.TileSize + 2;
            };

            tilemapview.Map = map;
        }

        private enum Tool {
            Pen = 0,
            Eraser = 1,
            Filler = 2,
            Pointer = 3,
            Rotater = 4,
            God = 6,
            Hand = 7,
            Trashcan = 8,
            VectorGrabber = 9
        }

        public List<FrameworkElement> Menu { get { return _Menu; } }

        private Tile currentTile { get { return (wrappanel_tiles.SelectedIndex != -1) ? map.Tiles[wrappanel_tiles.SelectedIndex] : map.Tiles[0]; } }
        private int currentTileIndex { get { return wrappanel_tiles.SelectedIndex; } }

        public void LoadTextures (Dictionary<string, BitmapImage> bitmapImages) {
            if (GraphicsDevice != null) {
                map.Init(GraphicsDevice, bitmapImages);
            } else {
                tilemapview.DeviceInitialized += ( ) => {
                    map.Init(GraphicsDevice, bitmapImages);
                };
            }
        }

        public void ChangeTextureName (string oldname, string newname) {
            Texture2D texture = map.XnaTextures[oldname];
            map.XnaTextures.Remove(oldname);
            map.XnaTextures.Add(newname, texture);

            BitmapImage image = map.WpfTextures[oldname];
            map.WpfTextures.Remove(oldname);
            map.WpfTextures.Add(newname, image);
        }

        public void UpdateListbox ( ) {
            wrappanel_tiles.Items.Clear( );
            foreach (Tile tile in map.Tiles) {
                wrappanel_tiles.Items.Add(new ListViewEntry(map.WpfTextures[tile.Name]));
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
                    TileSerializer.Serialize(stream, map.Tiles, map.XnaTextures, tilemapview.GraphicsDevice);
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
                map.Entities.Remove(cachedEntity);
                cachedEntity = null;
            }

            if ((int)tool < 5) {
                tabcontrol_toolselect.SelectedIndex = 0;
            } else {
                tabcontrol_toolselect.SelectedIndex = 1;
            }
            currentTool = tool;
            ResetToolBorders( );
            ((Border)_Menu[12 + (int)tool]).BorderThickness = new Thickness(1);
        }

        private void tilemapview_MouseDown (object sender, MouseButtonEventArgs e) {
            if (currentTileIndex == -1 || !tilemapview.IsLayerActive(currentLayer))
                return;

            if (tabcontrol_toolselect.SelectedIndex == 0) {
                // tiles
                HandleTilemapViewClickTiles(sender, e);
            } else {
                // entities
                HandleTilemapViewClickEntities(sender, e);
            }
        }

        private void HandleTilemapViewClickTiles (object sender, MouseButtonEventArgs e) {
            Point clickedTile = new Point(Math.Floor(selectedTile.X), map.Height - Math.Floor(selectedTile.Y) - 1);
            if (e.RightButton == MouseButtonState.Pressed) {
                Cache.Push(new List<Tuple<Point, int, int, bool>>( ) {
                                    Tuple.Create(clickedTile,currentLayer, map.Data[(int)clickedTile.X, (int)clickedTile.Y, currentLayer], false)});
                map.Data[(int)clickedTile.X, (int)clickedTile.Y, currentLayer] = 0;
            } else if (e.LeftButton == MouseButtonState.Pressed) {
                switch (currentTool) {
                    case Tool.Eraser:
                        Cache.Push(new List<Tuple<Point, int, int, bool>>( ) {
                                    new Tuple<Point, int, int,bool>(clickedTile,currentLayer, map.Data[(int)clickedTile.X, (int)clickedTile.Y, currentLayer], false)});
                        map.Data[(int)clickedTile.X, (int)clickedTile.Y, currentLayer] = 0;
                        map.Rotations[(int)clickedTile.X, (int)clickedTile.Y, currentLayer] = 0;
                        break;

                    case Tool.Filler:
                        if (map.Data[(int)clickedTile.X, (int)clickedTile.Y, currentLayer] == currentTileIndex)
                            break;
                        int searching = map.Data[(int)clickedTile.X, (int)clickedTile.Y, currentLayer];
                        int replacing = currentTileIndex;
                        Queue<Point> pointQueue = new Queue<Point>( );
                        pointQueue.Enqueue(clickedTile);
                        List<Tuple<Point, int, int, bool>> changesForCache = new List<Tuple<Point, int, int, bool>>( );
                        while (pointQueue.Count > 0) {
                            Point current = pointQueue.Dequeue( );
                            if (current.X < 0 || current.X >= map.Width || current.Y < 0 || current.Y >= map.Height)
                                continue;
                            if (map.Data[(int)current.X, (int)current.Y, currentLayer] == searching) {
                                map.Data[(int)current.X, (int)current.Y, currentLayer] = replacing;
                                map.Rotations[(int)clickedTile.X, (int)clickedTile.Y, currentLayer] = 0;
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
                                    Tuple.Create(clickedTile,currentLayer, map.Data[(int)clickedTile.X, (int)clickedTile.Y, currentLayer], false)});
                        map.Data[(int)clickedTile.X, (int)clickedTile.Y, currentLayer] = currentTileIndex;
                        map.Rotations[(int)clickedTile.X, (int)clickedTile.Y, currentLayer] = 0;
                        break;

                    case Tool.Pointer:
                        map.SpawnPoint = new Vector2((int)clickedTile.X, (int)clickedTile.Y);
                        break;

                    case Tool.Rotater:
                        if (map.Tiles[map.Data[(int)clickedTile.X, (int)clickedTile.Y, currentLayer]].Name == "None") {
                            for (int i = 2; i > -1; i--) {
                                if (map.Tiles[map.Data[(int)clickedTile.X, (int)clickedTile.Y, i]].Name != "None") {
                                    ((RadioButton)_Menu[9 + i]).IsChecked = true;
                                }
                            }
                        }
                        float tileRotation = map.Rotations[(int)clickedTile.X, (int)clickedTile.Y, currentLayer];
                        Cache.Push(new List<Tuple<Point, int, int, bool>>( ) {
                                    Tuple.Create(clickedTile,currentLayer, (int)(tileRotation * 2), true)});
                        tileRotation += 0.5f;
                        tileRotation %= 2f;
                        if (map.GetTile((int)clickedTile.X, (int)clickedTile.Y, currentLayer).Name != "None")
                            map.Rotations[(int)clickedTile.X, (int)clickedTile.Y, currentLayer] = tileRotation;
                        break;
                }
                tilemapview.Update( );
            }
        }

        private Vector2 GetEntityCenterRaw (MouseEventArgs e) {
            Point positionOnControl = e.GetPosition(tilemapview);
            Vector2 selectedTile = new Vector2(
                (float)Math.Max(0, Math.Min(positionOnControl.X / tilemapview.TileSize + tilemapview.Offset.X, map.Width)),
                (float)Math.Max(0, Math.Min(map.Size.Height - positionOnControl.Y / tilemapview.TileSize - tilemapview.Offset.Y, map.Height)));
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
                    entitylistbox.GetCurrentFinalConfiguration( ).Create(clickedPosition, map, Keyboard.IsKeyDown(Key.LeftShift));
                    if (cachedEntity != null) {
                        map.Entities.Remove(cachedEntity);
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
                        map.Entities.Remove(clickedEntity);
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

        private void HandleMapVectorListRequest (Func<Vector2, bool> callback) {
            _Menu[23].IsEnabled = true;
            currentVectorRequestCallback = callback;
            SelectTool(Tool.VectorGrabber);
        }

        private Entity GetClickedEntity (MouseEventArgs e) {
            Vector2 clickedPosition = GetEntityCenterRaw(e);
            for (int i = map.Entities.Count - 1; i >= 0; i--) {
                if (map.Entities[i].Transform.Intersects(clickedPosition)) {
                    return map.Entities[i];
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
                map.Entities.Remove(cachedEntity);
                cachedEntity = null;
            }
            tilemapview.Update( );
        }

        private void tilemapview_MouseMove (object sender, MouseEventArgs e) {
            if (tabcontrol_toolselect.SelectedIndex == 0) {
                bool updated = UpdateSelectedTile(e);
                if (!updated)
                    return;
                HandleTilemapViewMoveTiles(sender, e, updated);
            } else {
                HandleTilemapViewMoveEntities(sender, e);
                return;
            }

        }

        private void HandleTilemapViewMoveTiles (object sender, MouseEventArgs e, bool updated) {
            Point clickedTile = new Point(Math.Floor(selectedTile.X), map.Height - Math.Floor(selectedTile.Y) - 1);
            if (e.RightButton == MouseButtonState.Pressed) {
                Cache.Push(new List<Tuple<Point, int, int, bool>>( ) {
                                    Tuple.Create(clickedTile,currentLayer, map.Data[(int)clickedTile.X, (int)clickedTile.Y, currentLayer], false)});
                map.Data[(int)clickedTile.X, (int)clickedTile.Y, currentLayer] = 0;
                updated = true;
            } else if (e.LeftButton == MouseButtonState.Pressed) {
                switch (currentTool) {
                    case Tool.Eraser:
                        Cache.Push(new List<Tuple<Point, int, int, bool>>( ) {
                                    Tuple.Create(clickedTile,currentLayer, map.Data[(int)clickedTile.X, (int)clickedTile.Y, currentLayer], false)});
                        map.Data[(int)clickedTile.X, (int)clickedTile.Y, currentLayer] = 0;
                        map.Rotations[(int)clickedTile.X, (int)clickedTile.Y, currentLayer] = 0;
                        break;

                    case Tool.Pen:
                        Cache.Push(new List<Tuple<Point, int, int, bool>>( ) {
                                    Tuple.Create(clickedTile,currentLayer, map.Data[(int)clickedTile.X, (int)clickedTile.Y, currentLayer], false)});
                        map.Data[(int)clickedTile.X, (int)clickedTile.Y, currentLayer] = currentTileIndex;
                        map.Rotations[(int)clickedTile.X, (int)clickedTile.Y, currentLayer] = 0;
                        break;

                    case Tool.Pointer:
                        map.SpawnPoint = new Vector2((int)clickedTile.X, (int)clickedTile.Y);
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
                    tilemapview.CurrentSelection = new Microsoft.Xna.Framework.Vector2(entityLocation.X - tilemapview.Offset.X, map.Height - entityLocation.Y - tilemapview.Offset.Y);
                    if (cachedEntity == null) {
                        cachedEntity = entitylistbox.GetCurrentShadowConfiguration( ).Create(entityLocation, map, Keyboard.IsKeyDown(Key.LeftShift));
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
                    float rx = clickedPosition.X - tilemapview.Offset.X, ry = map.Height - clickedPosition.Y - tilemapview.Offset.Y;
                    if (rx != tilemapview.CurrentSelection.X || ry != tilemapview.CurrentSelection.Y) {
                        tilemapview.CurrentSelection = new Microsoft.Xna.Framework.Vector2(rx, ry);
                        tilemapview.Update( );
                    }
                    break;
            }
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
                    map.Rotations[(int)undoData.Item1.X, (int)undoData.Item1.Y, undoData.Item2] = undoData.Item3 / 2f;
                } else {
                    // tile got changed
                    map.Data[(int)undoData.Item1.X, (int)undoData.Item1.Y, undoData.Item2] = undoData.Item3;
                }
            }
            tilemapview.Update( );
        }

        private bool UpdateSelectedTile (MouseEventArgs e) {
            Point positionOnControl = e.GetPosition(tilemapview);
            Microsoft.Xna.Framework.Vector2 nextSelectedTile = new Microsoft.Xna.Framework.Vector2(
                (float)Math.Max(0, Math.Min(positionOnControl.X / tilemapview.TileSize + tilemapview.Offset.X, map.Width - 1)),
                (float)Math.Max(0, Math.Min(positionOnControl.Y / tilemapview.TileSize + tilemapview.Offset.Y, map.Height - 1)));
            bool changed = Math.Floor(selectedTile.X) != Math.Floor(nextSelectedTile.X) || Math.Floor(selectedTile.Y) != Math.Floor(nextSelectedTile.Y);

            selectedTile = nextSelectedTile;
            text_xpos.Text = Math.Floor(selectedTile.X).ToString( );
            text_ypos.Text = Math.Floor(map.Height - selectedTile.Y).ToString( );
            tilemapview.CurrentSelection = new Microsoft.Xna.Framework.Vector2((float)Math.Floor(selectedTile.X), (float)Math.Floor(selectedTile.Y)) - tilemapview.Offset;
            return changed;
        }

        private void wrappanel_tiles_DragEnter (object sender, DragEventArgs e) {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effects = DragDropEffects.Copy;
        }

        private void wrappanel_tiles_Drop (object sender, DragEventArgs e) {
            if (map == null)
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
                if (!map.Tiles.Any(t => t.Name == textbox_tile_name.Text)) {
                    ChangeTextureName(map.Tiles[oldSelection].Name, textbox_tile_name.Text);
                    map.Tiles[oldSelection].Name = textbox_tile_name.Text;
                }
                map.Tiles[oldSelection].Attributes.Clear( );
                foreach (AttributeListViewEntry entry in listview_tile_attributes.Items) {
                    if (entry.Active)
                        map.Tiles[oldSelection].Attributes.Add((TileAttribute)Enum.Parse(typeof(TileAttribute), entry.Attribute), entry.Value);
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
                map.RemoveTile(index);
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
                Tile prevTile = map.Tiles[index];
                map.WpfTextures.Remove(prevTile.Name);
                map.XnaTextures.Remove(prevTile.Name);

                map.LoadTexture(tileName, tileImage, GraphicsDevice);
                map.Tiles[index].Name = tileName;
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
            if (checkbox_auto.IsChecked ?? false && !map.Tiles.Any(t => t.Name == tileName)) {
                // add tile
                BitmapImage tileImage = LoadTileImage(imagefile);

                map.LoadTexture(tileName, tileImage, GraphicsDevice);
                map.AddTile(new Tile( ) { Attributes = new Dictionary<TileAttribute, string>(defaultAttributes), Name = tileName });
                wrappanel_tiles.Items.Add(new ListViewEntry(tileImage));
            } else {
                // open add tile window
                AddTileWindow addTileDialog = new AddTileWindow(imagefile, map.Tiles.Select(t => t.Name), defaultAttributes);
                if (addTileDialog.ShowDialog( ) ?? false) {
                    if (map.Tiles.Where(t => t.Name == addTileDialog.Created.Item1.Name) != null) {
                        map.LoadTexture(addTileDialog.Created.Item1.Name, addTileDialog.Created.Item2, GraphicsDevice);
                        map.AddTile(addTileDialog.Created.Item1);
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
            image.DecodePixelWidth = Map.TILE_PXL_SIZE;
            image.DecodePixelHeight = Map.TILE_PXL_SIZE;
            image.UriSource = new Uri(file);
            image.EndInit( );
            return image;
        }

        private void tabcontrol_toolselect_SelectionChanged (object sender, SelectionChangedEventArgs e) {
            tilemapview.Mode = tabcontrol_toolselect.SelectedIndex;
            if (tabcontrol_toolselect.SelectedIndex == 0) {
                SelectTool(Tool.Pen);
            } else {
                SelectTool(Tool.God);
            }
        }

        public override string ToString ( ) {
            return $"{map.Name}, Width: {map.Width}, Height: {map.Height}";
        }
    }
}
