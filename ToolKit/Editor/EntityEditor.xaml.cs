using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using mapKnight.ToolKit.Controls.Components;
using mapKnight.ToolKit.Controls.Components.Graphics;

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

        private ObservableCollection<IComponentControl> availableComponents { get; set; } = new ObservableCollection<IComponentControl>( ) {
            new AnimationControl()
        };
        private ObservableCollection<IComponentControl> components { get; set; } = new ObservableCollection<IComponentControl>( );


        public EntityEditor ( ) {
            InitializeComponent( );
            listbox_components.DataContext = components;
            listbox_components.ContextMenu = (ContextMenu)listbox_components.Resources["contextmenu_default"];
            listbox_components.ContextMenu.DataContext = CreateMenu(availableComponents);
        }

        private void MenuItemAddComponent_Click (object sender, RoutedEventArgs e) {
            int index = listbox_components.ContextMenu.Items.IndexOf((MenuItem)sender);
            IComponentControl control = availableComponents[index];
            availableComponents.RemoveAt(index);
            listbox_components.ContextMenu.DataContext = CreateMenu(availableComponents);
            components.Add(control);
            components = new ObservableCollection<IComponentControl>(components.OrderBy(item => item.ToString( )));
            listbox_components.DataContext = components;
        }

        private void listbox_components_SelectionChanged (object sender, SelectionChangedEventArgs e) {
            if (listbox_components.SelectedItem == null) {
                listbox_components.ContextMenu = (ContextMenu)listbox_components.Resources["contextmenu_default"];
            } else {
                listbox_components.ContextMenu = (ContextMenu)listbox_components.Resources["contextmenu_component"];
                contentpresenter_component.Content = listbox_components.SelectedItem;
            }
        }

        private void listbox_components_MouseDown (object sender, MouseButtonEventArgs e) {
            listbox_components.SelectedIndex = -1;
        }

        private void MenuItemRemoveComponent_Click (object sender, RoutedEventArgs e) {
            int index = components.IndexOf((IComponentControl)listbox_components.SelectedItem);
            IComponentControl control = components[index];
            components.RemoveAt(index);
            availableComponents.Add(control);
            availableComponents = new ObservableCollection<IComponentControl>(availableComponents.OrderBy(item => item.Category).ThenBy(item => item.ToString( )));
            listbox_components.ContextMenu.DataContext = CreateMenu(availableComponents);
        }

        private MenuItem CreateMenuItemForComponent (IComponentControl component) {
            MenuItem item = new MenuItem( );
            item.Header = component.ToString( );
            item.Click += MenuItemAddComponent_Click;
            return item;
        }

        private ObservableCollection<MenuItem> CreateMenu (IEnumerable<IComponentControl> controls) {
            ObservableCollection<MenuItem> result = new ObservableCollection<MenuItem>( );
            foreach (IComponentControl control in controls) {
                result.Add(CreateMenuItemForComponent(control));
            }
            return result;
        }
    }
}

