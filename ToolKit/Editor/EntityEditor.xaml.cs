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

namespace mapKnight.ToolKit.Editor {
    /// <summary>
    /// Interaktionslogik für EntityEditor.xaml
    /// </summary>
    public partial class EntityEditor : UserControl {

        private List<UIElement> _Menu = new List<UIElement>( ) {
            new MenuItem( ) { Header = "ENTITY", Items = {
                    new MenuItem() { Header = "NEW", Height = 22, Icon = App.Current.FindResource("image_entity_newentity") },
                    new MenuItem() { Header = "LOAD", Height = 22 }
                } }
        };

        public List<UIElement> Menu { get { return _Menu; } }

        public EntityEditor ( ) {
            InitializeComponent( );
            listview_components.Items.Add("hallo welt");
            listview_components.Items.Add("hallo welt2");
        }

        private void DeleteComponentButton_Click (object sender, RoutedEventArgs e) {
            int index = listview_components.Items.IndexOf(((StackPanel)((Button)sender).Parent).DataContext);
            DeleteComponent(index);
        }

        private void DeleteComponent (int index) {
            listview_components.Items.RemoveAt(index);
        }
    }
}

