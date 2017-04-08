using mapKnight.Core;
using mapKnight.ToolKit.Data.Components;
using System.Collections.Generic;
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
            referenceComponent.RequestMapVectorList(RequestMapVectorListCallback);
        }

        private void RequestMapVectorListCallback(List<Vector2> result) {
            referenceComponent.Waypoints.Clear();
            referenceComponent.Waypoints.AddRange(result);
        }
    }
}
