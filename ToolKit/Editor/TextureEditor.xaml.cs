using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Path = System.IO.Path;
using Microsoft.Win32;

namespace mapKnight.ToolKit.Editor {
    public partial class TextureEditor : UserControl {
        private List<FrameworkElement> _Menu = new List<FrameworkElement>( ) {
            new MenuItem () {Header = "CLEAR" }
        };

        public List<FrameworkElement> Menu { get { return _Menu; } }

        public TextureEditor ( ) {
            InitializeComponent( );
            ((MenuItem)_Menu[0]).Click += MenuItemClear_Click;
        }

        private void MenuItemClear_Click (object sender, RoutedEventArgs e) {
            texturecreationcontrol.Clear( );
        }

        private void ButtonBuild_Click (object sender, RoutedEventArgs e) {
            SaveFileDialog dialog = new SaveFileDialog( );
            dialog.OverwritePrompt = true;
            if (dialog.ShowDialog( ) ?? false) {
                texturecreationcontrol.Build(dialog.FileName);
            }
        }
    }
}
