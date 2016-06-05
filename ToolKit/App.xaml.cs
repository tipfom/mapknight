using System;
using System.Windows;

namespace mapKnight.ToolKit {
    /// <summary>
    /// Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App : Application {
        public static event Action ProjectChanged;
        private static Project _Project = new Project( );
        public static Project Project {
            get { return _Project; }
            set { _Project = value; ProjectChanged?.Invoke( ); }
        }
    }
}
