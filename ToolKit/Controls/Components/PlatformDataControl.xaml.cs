using mapKnight.Core;
using mapKnight.ToolKit.Data.Components;
using System.Windows.Controls;

namespace mapKnight.ToolKit.Controls.Components {
    /// <summary>
    /// Interaktionslogik für PlatformDataControl.xaml
    /// </summary>
    public partial class PlatformDataControl : UserControl {
        private PlatformDataComponent referenceComponent;

        private PlatformDataControl( ) {
            InitializeComponent( );
        }

        public PlatformDataControl(PlatformDataComponent referenceComponent) : this() {
            this.referenceComponent = referenceComponent;
            textblock_count.DataContext = referenceComponent.Waypoints;
            listbox_waypoints.ItemsSource = referenceComponent.Waypoints;
        }

        private void button_reset_Click(object sender, System.Windows.RoutedEventArgs e) {
            referenceComponent.Waypoints.Clear( );
            referenceComponent.Waypoints.Add(referenceComponent.Owner.Transform.Center);
            referenceComponent.RequestMapVectorList(RequestMapVectorListCallback);
        }

        private bool RequestMapVectorListCallback(Vector2 input) {
            referenceComponent.Waypoints.Add(input);
            return true;
        }
    }
}
