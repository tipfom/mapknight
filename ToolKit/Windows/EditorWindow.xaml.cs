using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using mapKnight.ToolKit.Controls;
using mapKnight.ToolKit.Data;
using mapKnight.ToolKit.Editor;
using mapKnight.ToolKit.Windows.Dialogs;
using SaveFileDialog = Microsoft.Win32.SaveFileDialog;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using FolderBrowserDialog = System.Windows.Forms.FolderBrowserDialog;

namespace mapKnight.ToolKit.Windows {
    /// <summary>
    /// Interaktionslogik für EditorWindow.xaml
    /// </summary>
    public partial class EditorWindow : Window {
        public const string PROJECT_FILTER = "PROJECT-Files|*.mkproj";
        private static readonly SaveFileDialog saveDialog = new SaveFileDialog( ) { Filter = PROJECT_FILTER, OverwritePrompt = true, AddExtension = false };
        private static readonly OpenFileDialog openDialog = new OpenFileDialog( ) { Filter = PROJECT_FILTER, Multiselect = false, AddExtension = false, ReadOnlyChecked = false };
        private static readonly FolderBrowserDialog compileDialog = new FolderBrowserDialog( ) { ShowNewFolderButton = true };

        private Project _CurrentProject;
        public Project CurrentProject {
            get { return _CurrentProject; }
            set {
                _CurrentProject = value;
                _CurrentProject.LocationChanged += ( ) => Title = GetTitle( );
                Title = GetTitle( );
            }
        }

        private ObservableCollection<object> menuItems = new ObservableCollection<object>( );
        private MapEditor mapEditorInstance;
        private AnimationEditor animationEditorInstance;

        public EditorWindow ( ) {
            InitializeComponent( );
            LoadWindowProperties( );
            App.Current.MainWindow = this;

            foreach (var item in menu_editor.Items) {
                menuItems.Add(item);
            }
            menu_editor.Items.Clear( );
            menu_editor.ItemsSource = menuItems;

            mapEditorInstance = new MapEditor( );
            animationEditorInstance = new AnimationEditor( );
        }

        private void Window_Closing (object sender, System.ComponentModel.CancelEventArgs e) {
            SaveWindowProperties( );
        }

        private void Window_Loaded (object sender, RoutedEventArgs e) {
            if (App.StartupFile != null && Path.GetExtension(App.StartupFile) == ".mkproj" && File.Exists(App.StartupFile)) {
                CurrentProject = new Project(App.StartupFile, this);
                foreach (EditorMap map in CurrentProject.Maps) {
                    CreateMapEditor(map, null);
                }
                foreach (VertexAnimationData data in CurrentProject.Animations) {
                    CreateAnimationControl(data);
                }
            } else {
                CurrentProject = new Project( );
            }
        }

        private void TabControl_Editor_SelectionChanged (object sender, SelectionChangedEventArgs e) {
            if (e.Source != tabcontrol_editor) return;
            if (e.AddedItems.Count > 0) {
                if (e.AddedItems[0] is ClosableTabItem) {
                    if (e.RemovedItems.Count > 0 && e.RemovedItems[0] is ClosableTabItem) {
                        ((ClosableTabItem)e.RemovedItems[0]).Content = null;
                    }
                    ClosableTabItem selectedItem = (ClosableTabItem)e.AddedItems[0];
                    switch (selectedItem.DataContext) {
                        case EditorMap map:
                            mapEditorInstance.Map = map;
                            selectedItem.Content = mapEditorInstance;
                            SetTabPageMenu(mapEditorInstance.Menu);
                            mapEditorInstance.Focus( );
                            break;
                        case VertexAnimationData animationData:
                            animationEditorInstance.Data = animationData;
                            selectedItem.Content = animationEditorInstance;
                            SetTabPageMenu(animationEditorInstance.Menu);
                            animationEditorInstance.Focus( );
                            break;
                    }
                }
            } else {
                SetTabPageMenu(new List<FrameworkElement>( ));
            }
        }

        private void LoadWindowProperties ( ) {
            Top = Properties.Settings.Default.Top;
            Left = Properties.Settings.Default.Left;
            Height = Properties.Settings.Default.Height;
            Width = Properties.Settings.Default.Width;
            if (Properties.Settings.Default.Maximized) {
                WindowState = WindowState.Maximized;
            }
        }

        private void SaveWindowProperties ( ) {
            bool maximized = WindowState == WindowState.Maximized;
            Properties.Settings.Default.Maximized = maximized;
            Properties.Settings.Default.Top = maximized ? RestoreBounds.Top : Top;
            Properties.Settings.Default.Left = maximized ? RestoreBounds.Left : Left;
            Properties.Settings.Default.Height = maximized ? RestoreBounds.Height : Height;
            Properties.Settings.Default.Width = maximized ? RestoreBounds.Width : Width;
            Properties.Settings.Default.Save( );
        }

        private void SetTabPageMenu (List<FrameworkElement> items) {
            while (menu_editor.Items.Count > 3)
                menuItems.RemoveAt(3);

            foreach (FrameworkElement item in items)
                menuItems.Add(item);
        }

        private void CreateMapEditor (EditorMap map, Dictionary<string, BitmapImage> template) {
            if (template != null) {
                map.WpfTextures = template;
            }

            ToolTip toolTip = new ToolTip( );
            toolTip.SetBinding(ContentProperty, new Binding("Description") { Source = map });
            ClosableTabItem tabItem = new ClosableTabItem( ) { Header = new TextBlock( ) { Text = "MAP", ToolTip = toolTip }, DataContext = map };
            tabcontrol_editor.Items.Add(tabItem);
            tabcontrol_editor.SelectedIndex = tabcontrol_editor.Items.Count - 1;
            tabItem.CloseRequested += (sender) => {
                MessageBoxResult msgBoxResult = MessageBox.Show("Do you want to remove the associated map files from the project as well?", "Remove Animation", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                switch (msgBoxResult) {
                    case MessageBoxResult.Cancel:
                        return;
                    case MessageBoxResult.Yes:
                        CurrentProject.Maps.Remove(map);
                        break;
                }
                tabcontrol_editor.Items.Remove(sender);
            };
        }

        private void CreateAnimationControl (VertexAnimationData data) {
            ToolTip toolTip = new ToolTip( );
            toolTip.SetBinding(ContentProperty, new Binding("Description") { Source = data });
            ClosableTabItem tabItem = new ClosableTabItem( ) { Header = new TextBlock( ) { Text = "ANIMATION", ToolTip = toolTip }, DataContext = data };
            tabcontrol_editor.Items.Add(tabItem);
            tabcontrol_editor.SelectedIndex = tabcontrol_editor.Items.Count - 1;
            tabItem.CloseRequested += (sender) => {
                MessageBoxResult msgBoxResult = MessageBox.Show("Do you want to remove the associated animation files from the project as well?", "Remove Animation", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                switch (msgBoxResult) {
                    case MessageBoxResult.Cancel:
                        return;
                    case MessageBoxResult.Yes:
                        CurrentProject.Animations.Remove(data);
                        break;
                }
                tabcontrol_editor.Items.Remove(sender);
            };
        }

        private void RunAssoc (string args) {
            string path = Path.ChangeExtension(Path.GetTempFileName( ), "exe");
            using (Stream stream = Assembly.GetExecutingAssembly( ).GetManifestResourceStream("mapKnight.ToolKit.Resources.assoc.exe")) {
                byte[ ] bytes = new byte[(int)stream.Length];
                stream.Read(bytes, 0, bytes.Length);
                File.WriteAllBytes(path, bytes);
            }
            try {
                Process.Start(path, args);
            } catch { }
        }

        private string GetTitle ( ) {
            if (CurrentProject.HasLocation) {
                return "Pluto Alpha - " + CurrentProject.Location;
            }
            return "Pluto Alpha";
        }

        #region CommandBindings
        private void CommandBinding_New_CanExecute (object sender, CanExecuteRoutedEventArgs e) {
            e.CanExecute = true;
        }

        private void CommandBinding_New_Executed (object sender, ExecutedRoutedEventArgs e) {
            CurrentProject = new Project( );
            tabcontrol_editor.Items.Clear( );
        }

        private void CommandBinding_Save_CanExecute (object sender, CanExecuteRoutedEventArgs e) {
            e.CanExecute = true;
        }

        private void CommandBinding_Save_Executed (object sender, ExecutedRoutedEventArgs e) {
            if (!CurrentProject.HasLocation) {
                if (saveDialog.ShowDialog( ) ?? false) {
                    CurrentProject.Location = saveDialog.FileName;
                    CurrentProject.Save( );
                }
            } else {
                CurrentProject.Save( );
            }
        }

        private void CommandBinding_Open_CanExecute (object sender, CanExecuteRoutedEventArgs e) {
            e.CanExecute = true;
        }

        private void CommandBinding_Open_Executed (object sender, ExecutedRoutedEventArgs e) {
            if (openDialog.ShowDialog( ) ?? false) {
                CurrentProject = new Project(openDialog.FileName, this);

                foreach (EditorMap map in CurrentProject.Maps) {
                    CreateMapEditor(map, null);
                }
                foreach (VertexAnimationData data in CurrentProject.Animations) {
                    CreateAnimationControl(data);
                }
            }
        }

        private void CommandBinding_SaveAs_CanExecute (object sender, CanExecuteRoutedEventArgs e) {
            e.CanExecute = true;
        }

        private void CommandBinding_SaveAs_Executed (object sender, ExecutedRoutedEventArgs e) {
            if (saveDialog.ShowDialog( ) ?? false) {
                CurrentProject.Location = saveDialog.FileName;
                CurrentProject.Save( );
            }
        }
        #endregion

        #region Menu Events
        private void MenuItem_Project_Compile_Click (object sender, RoutedEventArgs e) {
            if (compileDialog.ShowDialog( ) == System.Windows.Forms.DialogResult.OK) {
                CurrentProject.Compile(compileDialog.SelectedPath);
                MessageBox.Show("Finished!", "Success!", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK);
            }
        }

        private void MenuItem_Project_Add_Map_Click (object sender, RoutedEventArgs e) {
            AddMapDialog addMapDialog = new AddMapDialog(CurrentProject.GraphicsDevice);
            if (addMapDialog.ShowDialog( ) ?? false) {
                CurrentProject.Maps.Add(addMapDialog.DialogResultMap);
                CreateMapEditor(addMapDialog.DialogResultMap, addMapDialog.DialogResultTextures);
            }
        }

        private void MenuItem_Project_Add_Animation_Click (object sender, RoutedEventArgs e) {
            AddAnimationWindow dialog = new AddAnimationWindow( );
            if (dialog.ShowDialog( ) ?? false) {
                if (!CurrentProject.Animations.Any(item => item.Meta.Entity == dialog.textbox_name.Text)) {
                    VertexAnimationData data = new VertexAnimationData( ) { Meta = new AnimationMetaData( ) { Entity = dialog.textbox_name.Text, Ratio = dialog.Ratio } };
                    CurrentProject.Animations.Add(data);
                    CreateAnimationControl(data);
                } else {
                    MessageBox.Show("please dont add entities with the same name");
                }
            }
        }

        private void MenuItem_Tools_PackTexture_Click (object sender, RoutedEventArgs e) {
            new PackTextureDialog( ).ShowDialog( );
        }

        private void MenuItem_Tools_LinkFiles_Click (object sender, RoutedEventArgs e) {
            RunAssoc($"-a .mkproj \"{Assembly.GetExecutingAssembly( ).Location}\" toolkit -fd \"MapKnight Project\" -pd \"Pluto\"");
        }

        private void MenuItem_Tools_UnlinkFiles_Click (object sender, RoutedEventArgs e) {
            RunAssoc($"-r .mkproj toolkit");
        }

        private void MenuItem_About_Click (object sender, RoutedEventArgs e) {
            new AboutWindow( ).ShowDialog( );
        }
        #endregion
    }
}
