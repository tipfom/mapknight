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
            Rotator = 4,
            God = 6,
            Hand = 7,
            Trashcan = 8,
            VectorGrabber = 9,
            Brush = 11,
        }

        private static readonly Style MENU_IMAGE_STYLE = new Style(typeof(Image)) { Triggers = { new Trigger( ) { Value = false, Property = IsEnabledProperty, Setters = { new Setter(Image.OpacityProperty, 0.3) } } } };

        public static readonly RoutedUICommand UndoCommand = new RoutedUICommand(
            "Undo", "UndoCommand", typeof(MapEditor),
            new InputGestureCollection( ) { new KeyGesture(Key.Z, ModifierKeys.Control) });
        public static readonly RoutedUICommand SelectPenCommand = new RoutedUICommand(
            "Select Pen", "SelectPenCommand", typeof(MapEditor),
            new InputGestureCollection( ) { new KeyGesture(Key.D1, ModifierKeys.Control) });
        public static readonly RoutedUICommand SelectEraserCommand = new RoutedUICommand(
            "Select Eraser", "SelectEraserCommand", typeof(MapEditor),
            new InputGestureCollection( ) { new KeyGesture(Key.D2, ModifierKeys.Control) });
        public static readonly RoutedUICommand SelectRotatorCommand = new RoutedUICommand(
            "Select Rotator", "SelectRotatorCommand", typeof(MapEditor),
            new InputGestureCollection( ) { new KeyGesture(Key.D3, ModifierKeys.Control) });
        public static readonly RoutedUICommand DeleteTileCommand = new RoutedUICommand(
            "Delete Tile", "DeleteTileCommand", typeof(MapEditor));
        public static readonly RoutedUICommand ReplaceTileCommand = new RoutedUICommand(
            "Replace Tile", "ReplaceTileCommand", typeof(MapEditor));
        public static readonly RoutedUICommand CreateBrushCommand = new RoutedUICommand(
            "Create Brush", "CreateBrushCommand", typeof(MapEditor));
        public static readonly RoutedUICommand RemoveBrushCommand = new RoutedUICommand(
            "Remove Brush", "RemoveBrushCommand", typeof(MapEditor));

        private List<FrameworkElement> _Menu = new List<FrameworkElement>( ) {
            new Image( ) { Source= (BitmapImage)App.Current.FindResource("img_settings"), Style = MENU_IMAGE_STYLE },
            new MenuItem( ) { Header = "SHOW", IsEnabled = false },
            new CheckBox( ) { IsChecked = true, Margin = new Thickness(-2, 0, -2, 0), VerticalAlignment = VerticalAlignment.Center, ToolTip = new ToolTip( ) { Content = "Show/Hide Background" }, Focusable = false },
            new CheckBox( ) { IsChecked = true, Margin = new Thickness(-2, 0, -2, 0), VerticalAlignment = VerticalAlignment.Center, ToolTip = new ToolTip( ) { Content = "Show/Hide Middle" }, Focusable = false },
            new CheckBox( ) { IsChecked = true, Margin = new Thickness(-2, 0, -2, 0), VerticalAlignment = VerticalAlignment.Center, ToolTip = new ToolTip( ) { Content = "Show/Hide Foreground" }, Focusable = false },
            new Separator( ) { Width = 10 },
            new MenuItem( ) { Header = "LAYER", IsEnabled = false },
            new RadioButton( ) { IsChecked = false, GroupName = "modifylayer", Margin = new Thickness(-2, 0, -2, 0), VerticalAlignment = VerticalAlignment.Center, ToolTip = new ToolTip( ) { Content = "Select Background" }, Focusable = false },
            new RadioButton( ) { IsChecked = true, GroupName = "modifylayer", Margin = new Thickness(-2, 0, -2, 0), VerticalAlignment = VerticalAlignment.Center, ToolTip = new ToolTip( ) { Content = "Select Middle" }, Focusable = false },
            new RadioButton( ) { IsChecked = false, GroupName = "modifylayer", Margin = new Thickness(-2, 0, -2, 0), VerticalAlignment = VerticalAlignment.Center, ToolTip = new ToolTip( ) { Content = "Select Foreground" }, Focusable = false },
            new Separator( ) { Width = 15 },
            new Border( ) { Child = new Image( ) { Source = (BitmapImage)App.Current.FindResource("img_undo"), Style = MENU_IMAGE_STYLE }, Background = Brushes.White, Margin = new Thickness(-6, 0, -6, 0), Padding = new Thickness(6, 0, 6, 0), ToolTip = new ToolTip( ) { Content = "Undo [Ctrl + Z]" } },
            new Border( ) { Child = new Image( ) { Source = (BitmapImage)App.Current.FindResource("img_tool_pencil"), Style = MENU_IMAGE_STYLE }, Background = Brushes.White, BorderBrush = Brushes.DodgerBlue, BorderThickness=  new Thickness(1), Margin = new Thickness(-6, 0, -6, 0), Padding = new Thickness(6, 0, 6, 0), ToolTip = new ToolTip( ) { Content = "Use the Tile Pen [Ctrl + 1]" } },
            new Border( ) { Child = new Image( ) { Source = (BitmapImage)App.Current.FindResource("img_tool_eraser"), Style = MENU_IMAGE_STYLE }, Background = Brushes.White, BorderBrush = Brushes.DodgerBlue, BorderThickness=  new Thickness(0), Margin = new Thickness(-6, 0, -6,0), Padding = new Thickness(6, 0, 6, 0), ToolTip = new ToolTip( ) { Content = "Use the Tile Eraser [Ctrl + 2]" } },
            new Border( ) { Child = new Image( ) { Source = (BitmapImage)App.Current.FindResource("img_tool_fill"), Style = MENU_IMAGE_STYLE }, Background = Brushes.White, BorderBrush = Brushes.DodgerBlue, BorderThickness=  new Thickness(0), Margin = new Thickness(-6, 0, -6 ,0), Padding = new Thickness(6, 0, 6, 0), ToolTip = new ToolTip( ) { Content = "Use the Tile Filltool" } },
            new Border( ) { Child = new Image( ) { Source = (BitmapImage)App.Current.FindResource("img_tool_marker"), Style = MENU_IMAGE_STYLE }, Background = Brushes.White, BorderBrush = Brushes.DodgerBlue, BorderThickness = new Thickness(0), Margin = new Thickness(-6, 0, -6, 0), Padding = new Thickness(6, 0, 6, 0), ToolTip = new ToolTip( ) { Content = "Set the Player Spawnpoint" } },
            new Border( ) { Child = new Image( ) { Source = (BitmapImage)App.Current.FindResource("img_tool_rotate"), Style = MENU_IMAGE_STYLE }, Background = Brushes.White, BorderBrush = Brushes.DodgerBlue, BorderThickness = new Thickness(0), Margin = new Thickness(-6, 0, -6, 0), Padding = new Thickness(6, 0, 6, 0), ToolTip = new ToolTip( ) { Content = "Rotate a Tile [Ctrl + 3]" } },
            new Separator( ) { Width = 8 },
            new Border( ) { Child = new Image( ) { Source = (BitmapImage)App.Current.FindResource("img_tool_summon"), Style = MENU_IMAGE_STYLE }, Background = Brushes.White, BorderBrush = Brushes.DodgerBlue, BorderThickness = new Thickness(0), Margin = new Thickness(-6, 0, -6, 0), Padding = new Thickness(6, 0, 6, 0), ToolTip = new ToolTip( ) { Content = "Place an Entity" } },
            new Border( ) { Child = new Image( ) { Source = (BitmapImage)App.Current.FindResource("img_tool_cursor"), Style = MENU_IMAGE_STYLE }, Background = Brushes.White, BorderBrush = Brushes.DodgerBlue, BorderThickness = new Thickness(0), Margin = new Thickness(-6, 0, -6, 0), Padding = new Thickness(6, 0, 6, 0), ToolTip = new ToolTip( ) { Content = "Select and Move an Entity" } },
            new Border( ) { Child = new Image( ) { Source = (BitmapImage)App.Current.FindResource("img_tool_trash"), Style = MENU_IMAGE_STYLE }, Background = Brushes.White, BorderBrush = Brushes.DodgerBlue, BorderThickness = new Thickness(0), Margin = new Thickness(-6, 0, -6, 0), Padding = new Thickness(6, 0, 6, 0), ToolTip = new ToolTip( ) { Content = "Kill an Entity" } },
            new Border( ) { Child = new Image( ) { Source = (BitmapImage)App.Current.FindResource("img_tool_finish"), Style = MENU_IMAGE_STYLE }, Background = Brushes.White, BorderBrush = Brushes.DodgerBlue, BorderThickness = new Thickness(0), Margin = new Thickness(-6, 0, -6, 0), Padding = new Thickness(6, 0, 6, 0), IsEnabled = false, ToolTip = new ToolTip( ) { Content = "Finish Vector Request" } },
            new Separator( ) { Width = 8 },
            new Border( ) { Child = new Image( ) { Source = (BitmapImage)App.Current.FindResource("img_tool_brush"), Style = MENU_IMAGE_STYLE }, Background = Brushes.White, BorderBrush = Brushes.DodgerBlue, BorderThickness = new Thickness(0), Margin = new Thickness(-6, 0, -6, 0), Padding = new Thickness(6, 0, 6, 0), ToolTip = new ToolTip( ) { Content = "Use the Tile Brush" } },
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
                    InitMap( );
                } else {
                    tilemapview.DeviceInitialized += ( ) => {
                        InitMap( );
                    };
                }

                listview_brushes.Items.Clear( );
                foreach (TileBrush brush in _Map.Brushes)
                    listview_brushes.Items.Add(brush);
            }
        }

        public GraphicsDevice GraphicsDevice {
            get { return tilemapview.GraphicsDevice; }
        }

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
            _Menu[16].MouseDown += (sender, e) => SelectTool(Tool.Rotator);

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

        private void InitMap ( ) {
            _Map.Init(GraphicsDevice);

            wrappanel_tiles.Items.Clear( );
            foreach (Tile tile in Map.Tiles) {
                wrappanel_tiles.Items.Add(new ListViewEntry(Map.WpfTextures[tile.Name]));
            }
            if (wrappanel_tiles.HasItems)
                wrappanel_tiles.SelectedIndex = 0;

            tilemapview.Map = _Map;
            tilemapview.Update( );
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

            ((Border)_Menu[12]).BorderThickness = new Thickness(0);
            ((Border)_Menu[13]).BorderThickness = new Thickness(0);
            ((Border)_Menu[14]).BorderThickness = new Thickness(0);
            ((Border)_Menu[15]).BorderThickness = new Thickness(0);
            ((Border)_Menu[16]).BorderThickness = new Thickness(0);
            ((Border)_Menu[18]).BorderThickness = new Thickness(0);
            ((Border)_Menu[19]).BorderThickness = new Thickness(0);
            ((Border)_Menu[20]).BorderThickness = new Thickness(0);
            ((Border)_Menu[21]).BorderThickness = new Thickness(0);

            ((Border)_Menu[12 + (int)tool]).BorderThickness = new Thickness(1);
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

        private Entity GetClickedEntity (MouseEventArgs e) {
            Vector2 clickedPosition = GetEntityCenterRaw(e);
            for (int i = Map.Entities.Count - 1; i >= 0; i--) {
                if (Map.Entities[i].Transform.Intersects(clickedPosition)) {
                    return Map.Entities[i];
                }
            }
            return null;
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

        private void UndoLast ( ) {
            Map.Undo( );
            tilemapview.Update( );
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

        private void HandleMapVectorListRequest (Func<Vector2, bool> callback) {
            _Menu[23].IsEnabled = true;
            currentVectorRequestCallback = callback;
            SelectTool(Tool.VectorGrabber);
        }
     
        private void Button_ExportTileTemplate_Click (object sender, RoutedEventArgs e) {
            // export current maps tileset
            SaveFileDialog exportDialog = new SaveFileDialog( );
            exportDialog.Filter = "TileTemplate|*.mkttemplate";
            if (exportDialog.ShowDialog( ) ?? false) {
                using (Stream stream = File.OpenWrite(exportDialog.FileName))
                    TileSerializer.Serialize(stream, Map.Tiles, Map.XnaTextures, tilemapview.GraphicsDevice);
            }
        }

        private void Button_Settings_Click (object sender, RoutedEventArgs e) {
            EditDefaultTileAttributesWindow dialog = new EditDefaultTileAttributesWindow(defaultAttributes);
            if (dialog.ShowDialog( ) ?? false) {
                defaultAttributes = dialog.NewDefault;
            }
        }

        private void Button_AddTile_Click (object sender, RoutedEventArgs e) {
            OpenFileDialog opendialog = new OpenFileDialog( );
            opendialog.Filter = "Images|*.png;*.jpg;*.jpeg";
            opendialog.Multiselect = true;
            if (opendialog.ShowDialog( ) ?? false) {
                foreach (string file in opendialog.FileNames) {
                    AddTile(file);
                }
            }
        }
        
        private void Scrollbar_Horizontal_ValueChanged (object sender, RoutedPropertyChangedEventArgs<double> e) {
            tilemapview.Offset = new Microsoft.Xna.Framework.Vector2((float)scrollbar_horizontal.Value, tilemapview.Offset.Y);
        }

        private void Scrollbar_Vertical_ValueChanged (object sender, RoutedPropertyChangedEventArgs<double> e) {
            tilemapview.Offset = new Microsoft.Xna.Framework.Vector2(tilemapview.Offset.X, (float)scrollbar_vertical.Value);
        }

        private void TabControl_ToolSelect_SelectionChanged (object sender, SelectionChangedEventArgs e) {
            tilemapview.Mode = tabcontrol_toolselect.SelectedIndex;
            if (tabcontrol_toolselect.SelectedIndex == 0) {
                SelectTool(Tool.Pen);
            } else if (tabcontrol_toolselect.SelectedIndex == 1) {
                SelectTool(Tool.Brush);
            } else {
                SelectTool(Tool.God);
            }
        }
        
        #region tilemapview
        private void tilemapview_MouseDown (object sender, MouseButtonEventArgs e) {
            switch (tabcontrol_toolselect.SelectedIndex) {
                case 0:
                    if (tilemapview.IsLayerActive(currentLayer))
                        HandleMouseDown_Tiles(sender, e);
                    break;
                case 1:
                    HandleMouseDown_Entities(sender, e);
                    break;
                case 2:
                    if (tilemapview.IsLayerActive(currentLayer))
                        HandleMouseDown_Brushes(sender, e);
                    break;
            }
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
                    if (tilemapview.IsLayerActive(currentLayer))
                        HandleMouseMove_Tiles(sender, e, UpdateSelectedTile(e));
                    break;
                case 1:
                    HandleMouseMove_Entities(sender, e);
                    break;
                case 2:
                    if (tilemapview.IsLayerActive(currentLayer))
                        HandleMouseMove_Brushes(sender, e, UpdateSelectedTile(e));
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

        private void tilemapview_SizeChanged (object sender, SizeChangedEventArgs e) {
            scrollbar_horizontal.Minimum = -tilemapview.ActualWidth / tilemapview.TileSize;
            scrollbar_vertical.Minimum = -tilemapview.ActualHeight / tilemapview.TileSize;
        }

        private void HandleMouseDown_Tiles (object sender, MouseButtonEventArgs e) {
            Point clickedTile = new Point(Math.Floor(selectedTile.X), Map.Height - Math.Floor(selectedTile.Y) - 1);
            if (e.RightButton == MouseButtonState.Pressed) {
                Map.Preserve((int)clickedTile.X, (int)clickedTile.Y, currentLayer, false);
                Map.Data[(int)clickedTile.X, (int)clickedTile.Y, currentLayer] = 0;
            } else if (e.LeftButton == MouseButtonState.Pressed) {
                switch (currentTool) {
                    case Tool.Eraser:
                        Map.Preserve((int)clickedTile.X, (int)clickedTile.Y, currentLayer, false);
                        Map.Data[(int)clickedTile.X, (int)clickedTile.Y, currentLayer] = 0;
                        Map.Rotations[(int)clickedTile.X, (int)clickedTile.Y, currentLayer] = 0;
                        break;

                    case Tool.Filler:
                        if (currentTileIndex == -1 || Map.Data[(int)clickedTile.X, (int)clickedTile.Y, currentLayer] == currentTileIndex)
                            break;
                        int searching = Map.Data[(int)clickedTile.X, (int)clickedTile.Y, currentLayer];
                        int replacing = currentTileIndex;
                        Queue<Point> pointQueue = new Queue<Point>( );
                        pointQueue.Enqueue(clickedTile);
                        bool append = false;
                        while (pointQueue.Count > 0) {
                            Point current = pointQueue.Dequeue( );
                            if (current.X < 0 || current.X >= Map.Width || current.Y < 0 || current.Y >= Map.Height)
                                continue;
                            if (Map.Data[(int)current.X, (int)current.Y, currentLayer] == searching) {
                                Map.Preserve((int)current.X, (int)current.Y, currentLayer, append);
                                append = true;

                                Map.Data[(int)current.X, (int)current.Y, currentLayer] = replacing;
                                Map.Rotations[(int)clickedTile.X, (int)clickedTile.Y, currentLayer] = 0;

                                pointQueue.Enqueue(new Point(current.X - 1, current.Y));
                                pointQueue.Enqueue(new Point(current.X + 1, current.Y));
                                pointQueue.Enqueue(new Point(current.X, current.Y - 1));
                                pointQueue.Enqueue(new Point(current.X, current.Y + 1));
                            }
                        }
                        break;

                    case Tool.Pen:
                        if (currentTileIndex == -1) break;
                        Map.Preserve((int)clickedTile.X, (int)clickedTile.Y, currentLayer, false);
                        Map.Data[(int)clickedTile.X, (int)clickedTile.Y, currentLayer] = currentTileIndex;
                        Map.Rotations[(int)clickedTile.X, (int)clickedTile.Y, currentLayer] = 0;
                        break;

                    case Tool.Pointer:
                        Map.SpawnPoint = new Vector2((int)clickedTile.X, (int)clickedTile.Y);
                        break;

                    case Tool.Rotator:
                        if (Map.Tiles[Map.Data[(int)clickedTile.X, (int)clickedTile.Y, currentLayer]].Name == "None") {
                            for (int i = 2; i > -1; i--) {
                                if (Map.Tiles[Map.Data[(int)clickedTile.X, (int)clickedTile.Y, i]].Name != "None") {
                                    ((RadioButton)_Menu[9 + i]).IsChecked = true;
                                }
                            }
                        }
                        float tileRotation = Map.Rotations[(int)clickedTile.X, (int)clickedTile.Y, currentLayer];
                        Map.Preserve((int)clickedTile.X, (int)clickedTile.Y, currentLayer, false);
                        tileRotation += 0.5f;
                        tileRotation %= 2f;
                        if (Map.GetTile((int)clickedTile.X, (int)clickedTile.Y, currentLayer).Name != "None")
                            Map.Rotations[(int)clickedTile.X, (int)clickedTile.Y, currentLayer] = tileRotation;
                        break;
                }
                tilemapview.Update( );
            }
        }

        private void HandleMouseDown_Entities (object sender, MouseButtonEventArgs e) {
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

        private void HandleMouseDown_Brushes (object sender, MouseButtonEventArgs e) {
            if (currentBrush == null) return;

            if (e.LeftButton == MouseButtonState.Pressed) {
                int px0 = (int)selectedTile.X;
                int py0 = Map.Height - (int)selectedTile.Y - 1;
                Map.Preserve(px0, py0, currentLayer, false);
                Map.Data[px0, py0, currentLayer] = Array.FindIndex(Map.Tiles, tile => tile.Name == currentBrush.Centre[0].Tile.Name);

                for (int x = -1; x <= 1; x++) {
                    for (int y = -1; y <= 1; y++) {
                        int px = px0 + x;
                        int py = py0 + y;

                        if (px >= 0 && px < Map.Width && py >= 0 && py < Map.Height && currentBrush.Contains(Map.Tiles[Map.Data[px, py, currentLayer]].Name)) {
                            (Tile tile, float rotation) replacement = currentBrush.Get(Map.GetTile(px, py, currentLayer), Map.Rotations[px, py, currentLayer], Map.GetBrushData(px, py, currentLayer, currentBrush));
                            Map.Preserve(px0, py0, currentLayer, true);
                            Map.Data[px, py, currentLayer] = Array.FindIndex(Map.Tiles, tile => tile.Name == replacement.tile.Name);
                            Map.Rotations[px, py, currentLayer] = replacement.rotation;
                        }
                    }
                }

                tilemapview.Update( );
            }
        }
        
        private void HandleMouseMove_Tiles (object sender, MouseEventArgs e, bool updated) {
            if (!updated) return;

            Point clickedTile = new Point(Math.Floor(selectedTile.X), Map.Height - Math.Floor(selectedTile.Y) - 1);
            if (e.LeftButton == MouseButtonState.Pressed) {
                switch (currentTool) {
                    case Tool.Eraser:
                        Map.Preserve((int)clickedTile.X, (int)clickedTile.Y, currentLayer, true);
                        Map.Data[(int)clickedTile.X, (int)clickedTile.Y, currentLayer] = 0;
                        Map.Rotations[(int)clickedTile.X, (int)clickedTile.Y, currentLayer] = 0;
                        break;

                    case Tool.Pen:
                        if (currentTileIndex == -1) break;
                        Map.Preserve((int)clickedTile.X, (int)clickedTile.Y, currentLayer, true);
                        Map.Data[(int)clickedTile.X, (int)clickedTile.Y, currentLayer] = currentTileIndex;
                        Map.Rotations[(int)clickedTile.X, (int)clickedTile.Y, currentLayer] = 0;
                        break;

                    case Tool.Pointer:
                        Map.SpawnPoint = new Vector2((int)clickedTile.X, (int)clickedTile.Y);
                        break;
                }
                updated = updated || (currentTool != Tool.Filler && currentTool != Tool.Rotator);
            } else if (e.RightButton == MouseButtonState.Pressed) {
                Map.Preserve((int)clickedTile.X, (int)clickedTile.Y, currentLayer, true);
                Map.Data[(int)clickedTile.X, (int)clickedTile.Y, currentLayer] = 0;
                updated = true;
            }
            if (updated)
                tilemapview.Update( );
        }

        private void HandleMouseMove_Entities (object sender, MouseEventArgs e) {
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

        private void HandleMouseMove_Brushes (object sender, MouseEventArgs e, bool updated) {
            if (!updated) return;

            if (e.LeftButton == MouseButtonState.Pressed && currentBrush != null) {
                int px0 = (int)selectedTile.X;
                int py0 = Map.Height - (int)selectedTile.Y - 1;
                Map.Preserve(px0, py0, currentLayer, true);
                Map.Data[px0, py0, currentLayer] = Array.FindIndex(Map.Tiles, tile => tile.Name == currentBrush.Centre[0].Tile.Name);

                for (int x = -1; x <= 1; x++) {
                    for (int y = -1; y <= 1; y++) {
                        int px = px0 + x;
                        int py = py0 + y;

                        if (px >= 0 && px < Map.Width && py >= 0 && py < Map.Height && currentBrush.Contains(Map.Tiles[Map.Data[px, py, currentLayer]].Name)) {
                            (Tile tile, float rotation) replacement = currentBrush.Get(Map.GetTile(px, py, currentLayer), Map.Rotations[px, py, currentLayer], Map.GetBrushData(px, py, currentLayer, currentBrush));
                            Map.Preserve(px, py, currentLayer, true);
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
        #endregion

        #region wrappanel_tiles
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
                    Map.ChangeTextureName(Map.Tiles[oldSelection].Name, textbox_tile_name.Text);
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
        #endregion

        #region CommandBindings
        private void CommandBinding_DeleteTile_CanExecute (object sender, CanExecuteRoutedEventArgs e) {
            e.CanExecute = wrappanel_tiles?.SelectedItem != null;
        }

        private void CommandBinding_DeleteTile_Executed (object sender, ExecutedRoutedEventArgs e) {
            int index = wrappanel_tiles.SelectedIndex;
            if (index > 0) {
                wrappanel_tiles.Items.RemoveAt(index);
                Map.RemoveTile(index);
                tilemapview.Update( );
            }
        }

        private void CommandBinding_ReplaceTile_CanExecute (object sender, CanExecuteRoutedEventArgs e) {
            e.CanExecute = wrappanel_tiles?.SelectedItem != null;
        }

        private void CommandBinding_ReplaceTile_Executed (object sender, ExecutedRoutedEventArgs e) {
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

        private void CommandBinding_CreateBrush_CanExecute (object sender, CanExecuteRoutedEventArgs e) {
            e.CanExecute = true;
        }

        private void CommandBinding_CreateBrush_Executed (object sender, ExecutedRoutedEventArgs e) {
            AddBrushDialog addBrushDialog = new AddBrushDialog(Map);
            if (addBrushDialog.ShowDialog( ) ?? false) {
                listview_brushes.Items.Add(addBrushDialog.DialogResultBrush);
                Map.Brushes.Add(addBrushDialog.DialogResultBrush);
            }
        }

        private void CommandBinding_RemoveBrush_Executed (object sender, ExecutedRoutedEventArgs e) {
            Map.Brushes.RemoveAt(listview_brushes.SelectedIndex);
            listview_brushes.Items.RemoveAt(listview_brushes.SelectedIndex);
        }

        private void CommandBinding_Undo_CanExecute (object sender, CanExecuteRoutedEventArgs e) {
            e.CanExecute = (Map?.Cache.Count ?? 0) > 0;
        }

        private void CommandBinding_Undo_Executed (object sender, ExecutedRoutedEventArgs e) {
            UndoLast( );
        }

        private void CommandBinding_SelectPen_CanExecute (object sender, CanExecuteRoutedEventArgs e) {
            e.CanExecute = currentTool != Tool.Pen;
        }

        private void CommandBinding_SelectPen_Executed (object sender, ExecutedRoutedEventArgs e) {
            SelectTool(Tool.Pen);
        }

        private void CommandBinding_SelectEraser_CanExecute (object sender, CanExecuteRoutedEventArgs e) {
            e.CanExecute = currentTool != Tool.Eraser;
        }

        private void CommandBinding_SelectEraser_Executed (object sender, ExecutedRoutedEventArgs e) {
            SelectTool(Tool.Eraser);
        }

        private void CommandBinding_SelectRotator_CanExecute (object sender, CanExecuteRoutedEventArgs e) {
            e.CanExecute = currentTool != Tool.Rotator;
        }

        private void CommandBinding_SelectRotator_Executed (object sender, ExecutedRoutedEventArgs e) {
            SelectTool(Tool.Rotator);
        }

        private void CommandBinding_RemoveBrush_CanExecute (object sender, CanExecuteRoutedEventArgs e) {
            e.CanExecute = listview_brushes?.SelectedItem != null;
        }
        #endregion

        private struct ListViewEntry {
            public BitmapImage Image { get; private set; }

            public ListViewEntry (BitmapImage image) {
                Image = image;
            }
        }

        private struct AttributeListViewEntry {
            public bool Active { get; set; }
            public string Attribute { get; set; }
            public string Value { get; set; }

            public AttributeListViewEntry (bool active, string attribute, string value) {
                Active = active;
                Attribute = attribute;
                Value = value;
            }
        }
    }
}
