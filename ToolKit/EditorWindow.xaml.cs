using Microsoft.Win32;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace mapKnight.ToolKit {
    /// <summary>
    /// Interaktionslogik für EditorWindow.xaml
    /// </summary>
    public partial class EditorWindow : Window {
        public const string PROJECT_FILTER = "PROJECT-Files|*.mkproj";

        public EditorWindow( ) {
            InitializeComponent( );
            LoadConfig( );
        }

        private void LoadConfig( ) {
            this.Top = Properties.Settings.Default.Top;
            this.Left = Properties.Settings.Default.Left;
            this.Height = Properties.Settings.Default.Height;
            this.Width = Properties.Settings.Default.Width;
            // Very quick and dirty - but it does the job
            if (Properties.Settings.Default.Maximized) {
                WindowState = WindowState.Maximized;
            }
        }

        private void SetTabPageMenu(List<UIElement> items) {
            while (menu_editor.Items.Count > 2)
                menu_editor.Items.RemoveAt(2);

            foreach (UIElement item in items)
                menu_editor.Items.Add(item);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
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

        private void Map_Selected(object sender, RoutedEventArgs e) {
            SetTabPageMenu(mapeditor.Menu);
        }

        private void NewCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e) {
            e.CanExecute = true;
        }

        private void NewCommand_Executed(object sender, ExecutedRoutedEventArgs e) {
            App.Project = new Project( );
        }

        private void OpenCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e) {
            e.CanExecute = true;
        }

        private void OpenCommand_Executed(object sender, ExecutedRoutedEventArgs e) {
            OpenFileDialog projectopendialog = new OpenFileDialog( );
            projectopendialog.CheckFileExists = true;
            projectopendialog.Multiselect = false;
            projectopendialog.Filter = PROJECT_FILTER;
            if (projectopendialog.ShowDialog( ) ?? false) {
                App.Project = new Project(projectopendialog.FileName);
            }
        }

        private void SaveCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e) {
            e.CanExecute = true;
        }

        private void SaveCommand_Executed(object sender, ExecutedRoutedEventArgs e) {
            if (!App.Project.IsLocated) {
                SaveFileDialog projectsavedialog = new SaveFileDialog( );
                projectsavedialog.OverwritePrompt = true;
                projectsavedialog.Filter = PROJECT_FILTER;
                if (projectsavedialog.ShowDialog( ) ?? false) {
                    App.Project.Save(projectsavedialog.FileName);
                }
            } else {
                App.Project.Save( );
            }
        }
    }
}
