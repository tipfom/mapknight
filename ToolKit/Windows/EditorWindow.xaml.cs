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

        public EditorWindow ( ) {
            InitializeComponent( );
            LoadConfig( );
            App.Current.MainWindow = this;

            foreach (var item in menu_editor.Items) {
                menuItems.Add(item);
            }
            menu_editor.Items.Clear( );
            menu_editor.ItemsSource = menuItems;
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
            while (menu_editor.Items.Count > 2)
                menuItems.RemoveAt(2);

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

        private void CreateMapEditor(EditorMap map, Dictionary<string, BitmapImage> template) {
            MapEditor mapEditor = new MapEditor(map);
            if (template != null) {
                mapEditor.LoadTextures(template);
            }

            ToolTip toolTip = new ToolTip( );
            toolTip.SetBinding(ContentProperty, new Binding("Description") { Source = mapEditor });
            ClosableTabItem tabItem = new ClosableTabItem( ) { Content = mapEditor, Header = "MAP", ToolTip = ToolTip };
            tabcontrol_editor.Items.Add(tabItem);
            tabcontrol_editor.SelectedIndex = tabcontrol_editor.Items.Count - 1;
            tabItem.CloseRequested += (item) => tabcontrol_editor.Items.Remove(item);

        }

        private void CreateAnimationControl(VertexAnimationData data) {
            AnimationControl animationControl = new AnimationControl(data);

            ToolTip toolTip = new ToolTip( );
            toolTip.SetBinding(ContentProperty, new Binding("Description") { Source = animationControl });
            ClosableTabItem tabItem = new ClosableTabItem( ) { Content = animationControl, Header = "ANIMATION", ToolTip = toolTip };
            tabcontrol_editor.Items.Add(tabItem);
            tabcontrol_editor.SelectedIndex = tabcontrol_editor.Items.Count - 1;
            tabItem.CloseRequested += (item) => tabcontrol_editor.Items.Remove(item);
        }

        public void CRASH_SAVE (string path) {
            // experimental
            project.Path = path;
            project.Save( );
        }

        private void tabcontrol_editor_SelectionChanged (object sender, SelectionChangedEventArgs e) {
            if (e.AddedItems.Count > 0) {
                if (e.AddedItems[0] is TabItem) {
                    TabItem selectedItem = (TabItem)e.AddedItems[0];
                    switch (selectedItem.Content) {
                        case MapEditor mapEditor:
                            SetTabPageMenu(mapEditor.Menu);
                            break;
                        case AnimationControl animationControl:
                            SetTabPageMenu(animationControl.Menu);
                            break;
                    }
                }
            } else {
                SetTabPageMenu(new List<FrameworkElement>( ));
            }
        }
    }
}
