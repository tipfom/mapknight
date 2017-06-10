using mapKnight.Core;
using mapKnight.ToolKit.Data;
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
using System.Windows.Shapes;

namespace mapKnight.ToolKit.Windows.Dialogs {
    /// <summary>
    /// Interaktionslogik für AddBrushWindow.xaml
    /// </summary>
    public partial class AddBrushDialog : Window {
        public Data.TileBrush DialogResultBrush = new Data.TileBrush( );

        private EditorMap map;

        private AddBrushDialog ( ) {
            InitializeComponent( );
        }

        public AddBrushDialog (EditorMap map) {
            InitializeComponent( );
            this.map = map;
            foreach (Tile tile in map.Tiles)
                listview_tiles.Items.Add(new ListViewEntry(map.WpfTextures[tile.Name]));
        }

        private struct ListViewEntry {
            public ListViewEntry (BitmapImage image) {
                Image = image;
            }

            public BitmapImage Image { get; private set; }
        }

        private void Image_CornerTL_MouseDown (object sender, MouseButtonEventArgs e) {
            ImageClick(image_cornertl, 90, e);
            if (listview_tiles.SelectedItem != null)
                DialogResultBrush.CTL = (map.Tiles[listview_tiles.SelectedIndex], (float)(((RotateTransform)image_cornertl.RenderTransform).Angle / 180));
        }

        private void Image_CornerTR_MouseDown (object sender, MouseButtonEventArgs e) {
            ImageClick(image_cornertr, 180, e);
            if (listview_tiles.SelectedItem != null)
                DialogResultBrush.CTR = (map.Tiles[listview_tiles.SelectedIndex], (float)(((RotateTransform)image_cornertr.RenderTransform).Angle / 180));
        }

        private void Image_CornerBR_MouseDown (object sender, MouseButtonEventArgs e) {
            ImageClick(image_cornerbr, 270, e);
            if (listview_tiles.SelectedItem != null)
                DialogResultBrush.CBR = (map.Tiles[listview_tiles.SelectedIndex], (float)(((RotateTransform)image_cornerbr.RenderTransform).Angle / 180));
        }

        private void Image_CornerBL_MouseDown (object sender, MouseButtonEventArgs e) {
            ImageClick(image_cornerbl, 0, e);
            if (listview_tiles.SelectedItem != null)
                DialogResultBrush.CBL = (map.Tiles[listview_tiles.SelectedIndex], (float)(((RotateTransform)image_cornerbl.RenderTransform).Angle / 180));
        }

        private void Image_Centre_MouseDown (object sender, MouseButtonEventArgs e) {
            ImageClick(image_centre, 0, e);
            if (listview_tiles.SelectedItem != null)
                DialogResultBrush.Centre = (map.Tiles[listview_tiles.SelectedIndex], (float)(((RotateTransform)image_centre.RenderTransform).Angle / 180));
        }

        private void Image_IL_MouseDown (object sender, MouseButtonEventArgs e) {
            ImageClick(image_Il, 90, e);
            if (listview_tiles.SelectedItem != null)
                DialogResultBrush.IL = (map.Tiles[listview_tiles.SelectedIndex], (float)(((RotateTransform)image_Il.RenderTransform).Angle / 180));
        }

        private void Image_IT_MouseDown (object sender, MouseButtonEventArgs e) {
            ImageClick(image_It, 180, e);
            if (listview_tiles.SelectedItem != null)
                DialogResultBrush.IT = (map.Tiles[listview_tiles.SelectedIndex], (float)(((RotateTransform)image_It.RenderTransform).Angle / 180));
        }

        private void Image_IR_MouseDown (object sender, MouseButtonEventArgs e) {
            ImageClick(image_Ir, 270, e);
            if (listview_tiles.SelectedItem != null)
                DialogResultBrush.IR = (map.Tiles[listview_tiles.SelectedIndex], (float)(((RotateTransform)image_Ir.RenderTransform).Angle / 180));
        }

        private void Image_IB_MouseDown (object sender, MouseButtonEventArgs e) {
            ImageClick(image_Ib, 0, e);
            if (listview_tiles.SelectedItem != null)
                DialogResultBrush.IB = (map.Tiles[listview_tiles.SelectedIndex], (float)(((RotateTransform)image_Ib.RenderTransform).Angle / 180));
        }

        private void Image_LBL_MouseDown (object sender, MouseButtonEventArgs e) {
            ImageClick(image_Lbl, 0, e);
            if (listview_tiles.SelectedItem != null)
                DialogResultBrush.LBL = (map.Tiles[listview_tiles.SelectedIndex], (float)(((RotateTransform)image_Lbl.RenderTransform).Angle / 180));
        }

        private void Image_LBR_MouseDown (object sender, MouseButtonEventArgs e) {
            ImageClick(image_Lbr, 270, e);
            if (listview_tiles.SelectedItem != null)
                DialogResultBrush.LBR = (map.Tiles[listview_tiles.SelectedIndex], (float)(((RotateTransform)image_Lbr.RenderTransform).Angle / 180));
        }

        private void Image_LTL_MouseDown (object sender, MouseButtonEventArgs e) {
            ImageClick(image_Ltl, 90, e);
            if (listview_tiles.SelectedItem != null)
                DialogResultBrush.LTL = (map.Tiles[listview_tiles.SelectedIndex], (float)(((RotateTransform)image_Ltl.RenderTransform).Angle / 180));
        }

        private void Image_LTR_MouseDown (object sender, MouseButtonEventArgs e) {
            ImageClick(image_Ltr, 180, e);
            if (listview_tiles.SelectedItem != null)
                DialogResultBrush.LTR = (map.Tiles[listview_tiles.SelectedIndex], (float)(((RotateTransform)image_Ltr.RenderTransform).Angle / 180));
        }

        private void ImageClick (Image image, double initialRotation, MouseButtonEventArgs e) {
            if (e.LeftButton == MouseButtonState.Pressed) {
                if (listview_tiles.SelectedItem != null) {
                    image.Source = ((ListViewEntry)listview_tiles.SelectedItem).Image;
                    ((RotateTransform)image.RenderTransform).Angle = initialRotation;
                }
            } else if (e.RightButton == MouseButtonState.Pressed) {
                ((RotateTransform)image.RenderTransform).Angle = (((RotateTransform)image.RenderTransform).Angle + 90) % 360;
            }
        }

        private void Window_Closing (object sender, System.ComponentModel.CancelEventArgs e) {
            DialogResult = DialogResultBrush.IsValid(map);
        }
    }
}
