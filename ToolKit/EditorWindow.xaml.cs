using System.Collections.Generic;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;

namespace mapKnight.ToolKit {
    /// <summary>
    /// Interaktionslogik für EditorWindow.xaml
    /// </summary>
    public partial class EditorWindow : Window {
        public const string PROJECT_FILTER = "PROJECT-Files|*.mkproj";

        public EditorWindow ( ) {
            InitializeComponent( );
            LoadConfig( );
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

        private void SetTabPageMenu (List<UIElement> items) {
            while (menu_editor.Items.Count > 2)
                menu_editor.Items.RemoveAt(2);

            foreach (UIElement item in items)
                menu_editor.Items.Add(item);
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

        private void Map_Selected (object sender, RoutedEventArgs e) {
            SetTabPageMenu(mapeditor.Menu);
        }

        private void Entity_Selected (object sender, RoutedEventArgs e) {
            SetTabPageMenu(new List<UIElement>( ));
        }

        private void NewCommand_CanExecute (object sender, CanExecuteRoutedEventArgs e) {
            e.CanExecute = true;
        }

        private void NewCommand_Executed (object sender, ExecutedRoutedEventArgs e) {
            App.Project = new Project(this);
        }

        private void OpenCommand_CanExecute (object sender, CanExecuteRoutedEventArgs e) {
            e.CanExecute = true;
        }

        private void OpenCommand_Executed (object sender, ExecutedRoutedEventArgs e) {
            FolderBrowserDialog projectopendialog = new FolderBrowserDialog( );
            if (projectopendialog.ShowDialog( ) == System.Windows.Forms.DialogResult.OK) {
                App.Project = new Project(this, projectopendialog.SelectedPath);
            }
        }

        private void SaveCommand_CanExecute (object sender, CanExecuteRoutedEventArgs e) {
            e.CanExecute = true;
        }

        private void SaveCommand_Executed (object sender, ExecutedRoutedEventArgs e) {
            if (!App.Project.IsLocated) {
                FolderBrowserDialog projectsavedialog = new FolderBrowserDialog( );
                if (projectsavedialog.ShowDialog( ) == System.Windows.Forms.DialogResult.OK) {
                    App.Project.Save(projectsavedialog.SelectedPath);
                }
            } else {
                App.Project.Save( );
            }
        }

        private void Window_Loaded (object sender, RoutedEventArgs e) {
            App.Project = new Project(this);
        }
    }
}
