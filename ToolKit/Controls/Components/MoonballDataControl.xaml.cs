using mapKnight.Core;
using mapKnight.ToolKit.Data.Components;
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

namespace mapKnight.ToolKit.Controls.Components {
    /// <summary>
    /// Interaktionslogik für MoonballDataControl.xaml
    /// </summary>
    public partial class MoonballDataControl : UserControl {
        private MoonballDataComponent referenceComponent;

        private MoonballDataControl( ) {
            InitializeComponent( );
        }

        public MoonballDataControl(MoonballDataComponent referenceComponent) {
            InitializeComponent();
            this.referenceComponent = referenceComponent;
        }

        private void btn_reset_Click(object sender, RoutedEventArgs e) {
            referenceComponent.RequestMapVectorList(MapVectorRequestCallback);
        }

        private bool MapVectorRequestCallback(Vector2 input) {
            referenceComponent.MoonballSpawnOffset = input - referenceComponent.Owner.Transform.Center;
            return false;
        }
    }
}
