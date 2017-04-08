using mapKnight.ToolKit.Data.Components;
using System.Windows;
using System.Windows.Controls;

namespace mapKnight.ToolKit.Controls.Components {
    /// <summary>
    /// Interaktionslogik für SlimeDataControl.xaml
    /// </summary>
    public partial class SlimeDataControl : UserControl {
        private SlimeDataComponent referenceComponent;

        private SlimeDataControl( ) {
            InitializeComponent( );
        }

        public SlimeDataControl(SlimeDataComponent referenceComponent) : this( ) {
            this.referenceComponent = referenceComponent;
            UpdateButtons( );
        }

        private void btn_imove_top_Click(object sender, System.Windows.RoutedEventArgs e) {
            referenceComponent.InitialMoveDirection = SlimeDataComponent.Direction.Top;
            if (referenceComponent.InitialWallDirection != SlimeDataComponent.Direction.Left && referenceComponent.InitialWallDirection != SlimeDataComponent.Direction.Right) {
                referenceComponent.InitialWallDirection = SlimeDataComponent.Direction.Left;
            }
            UpdateButtons( );
        }

        private void btn_imove_left_Click(object sender, System.Windows.RoutedEventArgs e) {
            referenceComponent.InitialMoveDirection = SlimeDataComponent.Direction.Left;
            if (referenceComponent.InitialWallDirection != SlimeDataComponent.Direction.Top && referenceComponent.InitialWallDirection != SlimeDataComponent.Direction.Down) {
                referenceComponent.InitialWallDirection = SlimeDataComponent.Direction.Down;
            }
            UpdateButtons( );
        }

        private void btn_imove_down_Click(object sender, System.Windows.RoutedEventArgs e) {
            referenceComponent.InitialMoveDirection = SlimeDataComponent.Direction.Down;
            if (referenceComponent.InitialWallDirection != SlimeDataComponent.Direction.Left && referenceComponent.InitialWallDirection != SlimeDataComponent.Direction.Right) {
                referenceComponent.InitialWallDirection = SlimeDataComponent.Direction.Left;
            }
            UpdateButtons( );
        }

        private void btn_imove_right_Click(object sender, System.Windows.RoutedEventArgs e) {
            referenceComponent.InitialMoveDirection = SlimeDataComponent.Direction.Right;
            if (referenceComponent.InitialWallDirection != SlimeDataComponent.Direction.Top && referenceComponent.InitialWallDirection != SlimeDataComponent.Direction.Down) {
                referenceComponent.InitialWallDirection = SlimeDataComponent.Direction.Down;
            }
            UpdateButtons( );
        }

        private void btn_iwall_top_Click(object sender, System.Windows.RoutedEventArgs e) {
            referenceComponent.InitialWallDirection = SlimeDataComponent.Direction.Top;
            UpdateButtons( );
        }

        private void btn_iwall_left_Click(object sender, System.Windows.RoutedEventArgs e) {
            referenceComponent.InitialWallDirection = SlimeDataComponent.Direction.Left;
            UpdateButtons( );
        }

        private void btn_iwall_down_Click(object sender, System.Windows.RoutedEventArgs e) {
            referenceComponent.InitialWallDirection = SlimeDataComponent.Direction.Down;
            UpdateButtons( );
        }

        private void btn_iwall_right_Click(object sender, System.Windows.RoutedEventArgs e) {
            referenceComponent.InitialWallDirection = SlimeDataComponent.Direction.Right;
            UpdateButtons( );
        }

        private void UpdateButtons( ) {
            btn_iwall_top.Style = referenceComponent.InitialMoveDirection == SlimeDataComponent.Direction.Left || referenceComponent.InitialMoveDirection == SlimeDataComponent.Direction.Right ? null : (Style)FindResource("btn_style_inactive");
            btn_iwall_down.Style = referenceComponent.InitialMoveDirection == SlimeDataComponent.Direction.Left || referenceComponent.InitialMoveDirection == SlimeDataComponent.Direction.Right ? null : (Style)FindResource("btn_style_inactive");
            btn_iwall_left.Style = referenceComponent.InitialMoveDirection == SlimeDataComponent.Direction.Top || referenceComponent.InitialMoveDirection == SlimeDataComponent.Direction.Down ? null : (Style)FindResource("btn_style_inactive");
            btn_iwall_right.Style = referenceComponent.InitialMoveDirection == SlimeDataComponent.Direction.Top || referenceComponent.InitialMoveDirection == SlimeDataComponent.Direction.Down ? null : (Style)FindResource("btn_style_inactive");

            btn_imove_top.Style = null;
            btn_imove_down.Style = null;
            btn_imove_left.Style = null;
            btn_imove_right.Style = null;

            switch (referenceComponent.InitialMoveDirection) {
                case SlimeDataComponent.Direction.Left:
                    btn_imove_left.Style = (Style)FindResource("btn_style_active");
                    break;
                case SlimeDataComponent.Direction.Right:
                    btn_imove_right.Style = (Style)FindResource("btn_style_active");
                    break;
                case SlimeDataComponent.Direction.Top:
                    btn_imove_top.Style = (Style)FindResource("btn_style_active");
                    break;
                case SlimeDataComponent.Direction.Down:
                    btn_imove_down.Style = (Style)FindResource("btn_style_active");
                    break;
            }

            switch (referenceComponent.InitialWallDirection) {
                case SlimeDataComponent.Direction.Left:
                    btn_iwall_left.Style = (Style)FindResource("btn_style_active");
                    break;
                case SlimeDataComponent.Direction.Right:
                    btn_iwall_right.Style = (Style)FindResource("btn_style_active");
                    break;
                case SlimeDataComponent.Direction.Top:
                    btn_iwall_top.Style = (Style)FindResource("btn_style_active");
                    break;
                case SlimeDataComponent.Direction.Down:
                    btn_iwall_down.Style = (Style)FindResource("btn_style_active");
                    break;
            }
        }
    }
}
