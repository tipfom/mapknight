using System;
using System.Windows;
using System.Windows.Controls;
using FolderBrowserDialog = System.Windows.Forms.FolderBrowserDialog;

namespace mapKnight.ToolKit {
    /// <summary>
    /// Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App : Application {
        public static event Action ProjectChanged;
        private static Project _Project;
        public static Project Project {
            get { return _Project; }
            set { _Project = value; ProjectChanged?.Invoke( ); }
        }

        public App ( ) {
            EmbeddedAssemblies.Serve( );
        }

        public static void CreateNewProject (Control renderParent) {
            if (Project?.HasChanged ?? false)
                ShowSaveDialog( );
            Project = new Project();
        }

        public static void ShowSaveDialog ( ) {
            if (MessageBox.Show("Don't you want to save the state of your current project?", "Save?", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                SaveProject( );
        }

        public static void SaveProject ( ) {
            if (Project.IsLocated) {
                Project.Save( );
            } else {
                FolderBrowserDialog projectsavedialog = new FolderBrowserDialog( );
                if (projectsavedialog.ShowDialog( ) == System.Windows.Forms.DialogResult.OK) {
                    App.Project.Save(projectsavedialog.SelectedPath);
                }
            }
        }
    }
}
