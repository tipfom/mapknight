using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using SaveFileDialog = Microsoft.Win32.SaveFileDialog;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using System.IO;
using mapKnight.ToolKit.Data;
using mapKnight.ToolKit.Controls;
using mapKnight.ToolKit.Editor;
using System.Windows.Data;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media.Imaging;
using mapKnight.ToolKit.Windows.Dialogs;

namespace mapKnight.ToolKit.Windows {
    /// <summary>
    /// Interaktionslogik für EditorWindow.xaml
    /// </summary>
    public partial class EditorWindow : Window {
        public const string PROJECT_FILTER = "PROJECT-Files|*" + Project.EXTENSION;

        private Project project;
        private SaveFileDialog saveDialog = new SaveFileDialog( ) { Filter = PROJECT_FILTER, OverwritePrompt = true, AddExtension = false };
        private OpenFileDialog openDialog = new OpenFileDialog( ) { Filter = PROJECT_FILTER, Multiselect = false, AddExtension = false, ReadOnlyChecked = false };
        private System.Windows.Forms.FolderBrowserDialog compileDialog = new System.Windows.Forms.FolderBrowserDialog( ) { ShowNewFolderButton = true };
        private ObservableCollection<object> menuItems = new ObservableCollection<object>( );

        private MapEditor mapEditorInstance;
        private AnimationControl animationEditorInstance;

        public EditorWindow ( ) {
            InitializeComponent( );
            LoadConfig( );
            App.Current.MainWindow = this;

            foreach (var item in menu_editor.Items) {
                menuItems.Add(item);
            }
            menu_editor.Items.Clear( );
            menu_editor.ItemsSource = menuItems;

            mapEditorInstance = new MapEditor( );
            animationEditorInstance = new AnimationControl( );
        }

        private void LoadConfig ( ) {
            this.Top = Properties.Settings.Default.Top;
            this.Left = Properties.Settings.Default.Left;
            this.Height = Properties.Settings.Default.Height;
            this.Width = Properties.Settings.Default.Width;
            // Very quick and dirty - but it does the job
            if (Properties.Settings.Default.Maximized) {
                WindowState = WindowState.Maximized;
            }
        }

        private void SetTabPageMenu (List<FrameworkElement> items) {
            while (menu_editor.Items.Count > 3)
                menuItems.RemoveAt(3);

            foreach (FrameworkElement item in items)
                menuItems.Add(item);
        }

        private void Window_Closing (object sender, System.ComponentModel.CancelEventArgs e) {
            if (WindowState == WindowState.Maximized) {
                // Use the RestoreBounds as the current values will be 0, 0 and the size of the screen
                Properties.Settings.Default.Top = RestoreBounds.Top;
                Properties.Settings.Default.Left = RestoreBounds.Left;
                Properties.Settings.Default.Height = RestoreBounds.Height;
                Properties.Settings.Default.Width = RestoreBounds.Width;
                Properties.Settings.Default.Maximized = true;
            } else {
                Properties.Settings.Default.Top = this.Top;
                Properties.Settings.Default.Left = this.Left;
                Properties.Settings.Default.Height = this.Height;
                Properties.Settings.Default.Width = this.Width;
                Properties.Settings.Default.Maximized = false;
            }

            Properties.Settings.Default.Save( );
        }

        private void NewCommand_CanExecute (object sender, CanExecuteRoutedEventArgs e) {
            e.CanExecute = true;
        }

        private void NewCommand_Executed (object sender, ExecutedRoutedEventArgs e) {
            project = new Project( );
            tabcontrol_editor.Items.Clear( );
        }

        private void OpenCommand_CanExecute (object sender, CanExecuteRoutedEventArgs e) {
            e.CanExecute = true;
        }

        private void SaveCommand_CanExecute (object sender, CanExecuteRoutedEventArgs e) {
            e.CanExecute = true;
        }

        private void SaveCommand_Executed (object sender, ExecutedRoutedEventArgs e) {
            if (!project.HasPath) {
                if (saveDialog.ShowDialog( ) ?? false) {
                    project.Path = saveDialog.FileName;
                    project.Save( );
                }
            } else {
                project.Save( );
            }
        }

        private void Window_Loaded (object sender, RoutedEventArgs e) {
            if (App.StartupFile != null && Path.GetExtension(App.StartupFile) == ".mkproj" && File.Exists(App.StartupFile)) {
                project = new Project(App.StartupFile, this);
                foreach (EditorMap map in project.Maps) {
                    CreateMapEditor(map, null);
                }
                foreach (VertexAnimationData data in project.Animations) {
                    CreateAnimationControl(data);
                }
            } else {
                project = new Project( );
            }
        }

        private void About_Click (object sender, RoutedEventArgs e) {
            new AboutWindow( ).ShowDialog( );
        }

        private void CommandOpen_CanExecute (object sender, CanExecuteRoutedEventArgs e) {
            e.CanExecute = true;
        }

        private void CommandOpen_Executed (object sender, ExecutedRoutedEventArgs e) {
            if (openDialog.ShowDialog( ) ?? false) {
                project = new Project(openDialog.FileName, this);

                foreach(EditorMap map in project.Maps) {
                    CreateMapEditor(map, null);
                }
                foreach(VertexAnimationData data in project.Animations) {
                    CreateAnimationControl(data);
                }
            }
        }

        private void MenuItemCompile_Click (object sender, RoutedEventArgs e) {
            if (compileDialog.ShowDialog( ) == System.Windows.Forms.DialogResult.OK) {
                project.Compile(compileDialog.SelectedPath);
                MessageBox.Show("Finished!", "Success!", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK);
            }
        }

        private void MenuItemAddMap_Click (object sender, RoutedEventArgs e) {
            AddMapDialog addMapDialog = new AddMapDialog(project.GraphicsDevice);
            if (addMapDialog.ShowDialog( ) ?? false) {
                project.Maps.Add(addMapDialog.DialogResultMap);
                CreateMapEditor(addMapDialog.DialogResultMap, addMapDialog.DialogResultTextures);
            }
        }

        private void MenuItemAddAnimation_Click (object sender, RoutedEventArgs e) {
            AddAnimationWindow dialog = new AddAnimationWindow( );
            if (dialog.ShowDialog( ) ?? false) {
                if (!project.Animations.Any(item => item.Meta.Entity == dialog.textbox_name.Text)) {
                    VertexAnimationData data = new VertexAnimationData( ) { Meta = new AnimationMetaData( ) { Entity = dialog.textbox_name.Text, Ratio = dialog.Ratio } };
                    project.Animations.Add(data);
                    CreateAnimationControl(data);
                } else {
                    MessageBox.Show("please dont add entities with the same name");
                }
            }
        }

        private void MenuItem_PackTexture_Click (object sender, RoutedEventArgs e) {
            new PackTextureDialog( ).ShowDialog( );
        }

        private void CreateMapEditor(EditorMap map, Dictionary<string, BitmapImage> template) {
            if (template != null) {
                map.WpfTextures = template;
            }

            ToolTip toolTip = new ToolTip( );
            toolTip.SetBinding(ContentProperty, new Binding("Description") { Source = map });
            ClosableTabItem tabItem = new ClosableTabItem( ) { Header = "MAP", DataContext = map, ToolTip = ToolTip };
            tabcontrol_editor.Items.Add(tabItem);
            tabcontrol_editor.SelectedIndex = tabcontrol_editor.Items.Count - 1;
            tabItem.CloseRequested += (sender) => {
                MessageBoxResult msgBoxResult = MessageBox.Show("Do you want to remove the associated map files from the project as well?", "Remove Animation", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                switch (msgBoxResult) {
                    case MessageBoxResult.Cancel:
                        return;
                    case MessageBoxResult.Yes:
                        project.Maps.Remove(map);
                        break;
                }
                tabcontrol_editor.Items.Remove(sender);
            };
        }

        private void CreateAnimationControl(VertexAnimationData data) {
            ToolTip toolTip = new ToolTip( );
            toolTip.SetBinding(ContentProperty, new Binding("Description") { Source = data });
            ClosableTabItem tabItem = new ClosableTabItem( ) { Header = "ANIMATION", DataContext = data, ToolTip = toolTip };
            tabcontrol_editor.Items.Add(tabItem);
            tabcontrol_editor.SelectedIndex = tabcontrol_editor.Items.Count - 1;
            tabItem.CloseRequested += (sender) => {
                MessageBoxResult msgBoxResult = MessageBox.Show("Do you want to remove the associated animation files from the project as well?", "Remove Animation", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                switch (msgBoxResult) {
                    case MessageBoxResult.Cancel:
                        return;
                    case MessageBoxResult.Yes:
                        project.Animations.Remove(data);
                        break;
                }
                tabcontrol_editor.Items.Remove(sender);
            };
        }

        public void CRASH_SAVE (string path) {
            string projectfile = path + @"\project.mkproj";
            File.Create(projectfile).Close( );

            // experimental
            project.Path = projectfile;
            project.Save( );
            RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap((int)Width, (int)Height, 96, 96, System.Windows.Media.PixelFormats.Pbgra32);
            renderTargetBitmap.Render(this);
            PngBitmapEncoder pngImage = new PngBitmapEncoder( );
            pngImage.Frames.Add(BitmapFrame.Create(renderTargetBitmap));
            using (Stream fileStream = File.Create(path + @"screenshot.png")) {
                pngImage.Save(fileStream);
            }
        }

        private void tabcontrol_editor_SelectionChanged (object sender, SelectionChangedEventArgs e) {
            if (e.Source != tabcontrol_editor) return;
            if (e.AddedItems.Count > 0) {
                if (e.AddedItems[0] is ClosableTabItem) {
                    if(e.RemovedItems.Count > 0 && e.RemovedItems[0] is ClosableTabItem) {
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
    }
}
