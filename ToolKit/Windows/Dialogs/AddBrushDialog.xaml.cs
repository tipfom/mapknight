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

        public static readonly RoutedUICommand FinishCommand = new RoutedUICommand("Finish", "Finish", typeof(AddBrushDialog));

        private EditorMap map;
        private int indexCTL, indexCTR, indexCBR, indexCBL, indexCentre, indexIL, indexIT, indexIR, indexIB, indexLBL, indexLBR, indexLTL, indexLTR;

        private BitmapImage image_d = (BitmapImage)App.Current.FindResource("image_map_autotile_d");
        private BitmapImage image_I = (BitmapImage)App.Current.FindResource("image_map_autotile_I");
        private BitmapImage image_O = (BitmapImage)App.Current.FindResource("image_map_autotile_O");
        private BitmapImage image_L = (BitmapImage)App.Current.FindResource("image_map_autotile_L");

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
            SetTile(ref DialogResultBrush.CTL, ref indexCTL, image_cornertl);
            ImageClick(image_cornertl, image_d, (DialogResultBrush.CTL.Length > indexCTL) ? DialogResultBrush.CTL[indexCTL].tile : (Tile?)null, 90, e.RightButton == MouseButtonState.Pressed);
        }
        
        private void Image_CornerTR_MouseDown (object sender, MouseButtonEventArgs e) {
            SetTile(ref DialogResultBrush.CTR, ref indexCTR, image_cornertr);
            ImageClick(image_cornertr, image_d, (DialogResultBrush.CTR.Length > indexCTR) ? DialogResultBrush.CTR[indexCTR].tile : (Tile?)null, 180, e.RightButton == MouseButtonState.Pressed);
        }

        private void Image_CornerBR_MouseDown (object sender, MouseButtonEventArgs e) {
            SetTile(ref DialogResultBrush.CBR, ref indexCBR, image_cornerbr);
            ImageClick(image_cornerbr, image_d, (DialogResultBrush.CBR.Length > indexCBR) ? DialogResultBrush.CBR[indexCBR].tile : (Tile?)null, 270, e.RightButton == MouseButtonState.Pressed);
        }

        private void Image_CornerBL_MouseDown (object sender, MouseButtonEventArgs e) {
            SetTile(ref DialogResultBrush.CBL, ref indexCBL, image_cornerbl);
            ImageClick(image_cornerbl, image_d, (DialogResultBrush.CBL.Length > indexCBL) ? DialogResultBrush.CBL[indexCBL].tile : (Tile?)null, 0, e.RightButton == MouseButtonState.Pressed);
        }

        private void Image_Centre_MouseDown (object sender, MouseButtonEventArgs e) {
            SetTile(ref DialogResultBrush.Centre, ref indexCentre, image_centre);
            ImageClick(image_centre, image_O, (DialogResultBrush.Centre.Length > indexCentre) ? DialogResultBrush.Centre[indexCentre].tile : (Tile?)null, 0, e.RightButton == MouseButtonState.Pressed);
        }

        private void Image_IL_MouseDown (object sender, MouseButtonEventArgs e) {
            SetTile(ref DialogResultBrush.IL, ref indexIL, image_Il);
            ImageClick(image_Il, image_I, (DialogResultBrush.IL.Length > indexIL) ? DialogResultBrush.IL[indexIL].tile : (Tile?)null, 90, e.RightButton == MouseButtonState.Pressed);
        }

        private void Image_IT_MouseDown (object sender, MouseButtonEventArgs e) {
            SetTile(ref DialogResultBrush.IT, ref indexIT, image_It);
            ImageClick(image_It, image_I, (DialogResultBrush.IT.Length > indexIT) ? DialogResultBrush.IT[indexIT].tile : (Tile?)null, 180, e.RightButton == MouseButtonState.Pressed);
        }

        private void Image_IR_MouseDown (object sender, MouseButtonEventArgs e) {
            SetTile(ref DialogResultBrush.IR, ref indexIR, image_Ir);
            ImageClick(image_Ir, image_I, (DialogResultBrush.IR.Length > indexIR) ? DialogResultBrush.IR[indexIR].tile : (Tile?)null, 270, e.RightButton == MouseButtonState.Pressed);
        }

        private void Image_IB_MouseDown (object sender, MouseButtonEventArgs e) {
            SetTile(ref DialogResultBrush.IB, ref indexIB, image_Ib);
            ImageClick(image_Ib, image_I, (DialogResultBrush.IB.Length > indexIB) ? DialogResultBrush.IB[indexIB].tile : (Tile?)null, 0, e.RightButton == MouseButtonState.Pressed);
        }

        private void Image_LBL_MouseDown (object sender, MouseButtonEventArgs e) {
            SetTile(ref DialogResultBrush.LBL, ref indexLBL, image_Lbl);
            ImageClick(image_Lbl, image_L, (DialogResultBrush.LBL.Length > indexLBL) ? DialogResultBrush.LBL[indexLBL].tile : (Tile?)null, 0, e.RightButton == MouseButtonState.Pressed);
        }

        private void Image_LBR_MouseDown (object sender, MouseButtonEventArgs e) {
            SetTile(ref DialogResultBrush.LBR, ref indexLBR, image_Lbr);
            ImageClick(image_Lbr, image_L, (DialogResultBrush.LBR.Length > indexLBR) ? DialogResultBrush.LBR[indexLBR].tile : (Tile?)null, 270, e.RightButton == MouseButtonState.Pressed);
        }

        private void Image_LTL_MouseDown (object sender, MouseButtonEventArgs e) {
            SetTile(ref DialogResultBrush.LTL, ref indexLTL, image_Ltl);
            ImageClick(image_Ltl, image_L, (DialogResultBrush.LTL.Length > indexLTL) ? DialogResultBrush.LTL[indexLTL].tile : (Tile?)null, 90, e.RightButton == MouseButtonState.Pressed);
        }

        private void Image_LTR_MouseDown (object sender, MouseButtonEventArgs e) {
            SetTile(ref DialogResultBrush.LTR, ref indexLTR, image_Ltr);
            ImageClick(image_Ltr, image_L, (DialogResultBrush.LTR.Length > indexLTR) ? DialogResultBrush.LTR[indexLTR].tile : (Tile?)null, 180, e.RightButton == MouseButtonState.Pressed);
        }

        private void Button_NextCentre_Click (object sender, RoutedEventArgs e) {
            ButtonNextClick(ref indexCentre, ref DialogResultBrush.Centre, image_centre, image_O, 0f);
        }

        private void Button_NextCTL_Click (object sender, RoutedEventArgs e) {
            ButtonNextClick(ref indexCTL, ref DialogResultBrush.CTL, image_cornertl, image_d, 90f);
        }

        private void Button_NextCTR_Click (object sender, RoutedEventArgs e) {
            ButtonNextClick(ref indexCTR, ref DialogResultBrush.CTR, image_cornertr, image_d, 180f);
        }

        private void Button_NextCBR_Click (object sender, RoutedEventArgs e) {
            ButtonNextClick(ref indexCBR, ref DialogResultBrush.CBR, image_cornerbr, image_d, 270f);
        }

        private void Button_NextCBL_Click (object sender, RoutedEventArgs e) {
            ButtonNextClick(ref indexCBL, ref DialogResultBrush.CBL, image_cornerbl, image_d, 0f);
        }

        private void Button_NextIL_Click (object sender, RoutedEventArgs e) {
            ButtonNextClick(ref indexIL, ref DialogResultBrush.IL, image_Il, image_I, 90f);
        }

        private void Button_NextIT_Click (object sender, RoutedEventArgs e) {
            ButtonNextClick(ref indexIT, ref DialogResultBrush.IT, image_It, image_I, 180f);
        }

        private void Button_NextIR_Click (object sender, RoutedEventArgs e) {
            ButtonNextClick(ref indexIR, ref DialogResultBrush.IR, image_Ir, image_I, 270f);
        }

        private void Button_NextIB_Click (object sender, RoutedEventArgs e) {
            ButtonNextClick(ref indexIB, ref DialogResultBrush.IB, image_Ib, image_I, 0f);
        }

        private void Button_NextLBL_Click (object sender, RoutedEventArgs e) {
            ButtonNextClick(ref indexLBL, ref DialogResultBrush.LBL, image_Lbl, image_L, 0f);
        }

        private void Button_NextLBR_Click (object sender, RoutedEventArgs e) {
            ButtonNextClick(ref indexLBR, ref DialogResultBrush.LBR, image_Lbr, image_L, 270f);
        }

        private void Button_NextLTL_Click (object sender, RoutedEventArgs e) {
            ButtonNextClick(ref indexLTL, ref DialogResultBrush.LTL, image_Ltl, image_L, 90f);
        }

        private void Button_NextLTR_Click (object sender, RoutedEventArgs e) {
            ButtonNextClick(ref indexLTR, ref DialogResultBrush.LTR, image_Ltr, image_L, 180f);
        }

        private void ButtonNextClick (ref int index, ref (Tile tile, float rotation, int possibility)[ ] brushRef, Image image, BitmapImage defaultImage, float initialRotation) {
            index++;
            if (index > brushRef.Length) index = 0;
            ImageClick(image, defaultImage, (brushRef.Length > index) ? brushRef[index].tile : (Tile?)null, initialRotation, false);
        }

        private void SetTile (ref (Tile tile, float rotation, int possibility)[ ] brushRef, ref int index, Image image) {
            if (listview_tiles.SelectedItem != null) {
                if (brushRef.Length <= index) {
                    Array.Resize(ref brushRef, index + 1);
                }
                brushRef[index] = (map.Tiles[listview_tiles.SelectedIndex], (float)(((RotateTransform)image.RenderTransform).Angle / 180), 1);
            }
        }

        private void ImageClick (Image image, BitmapImage defaultImage, Tile? currentTile, double initialRotation, bool rotate) {
            if (!rotate) {
                if (currentTile != null) {
                    image.Source = ((ListViewEntry)listview_tiles.Items[Array.FindIndex(map.Tiles, tile => tile.Name == currentTile?.Name)]).Image;
                } else {
                    image.Source = defaultImage;
                }
                ((RotateTransform)image.RenderTransform).Angle = initialRotation;
            } else {
                ((RotateTransform)image.RenderTransform).Angle = (((RotateTransform)image.RenderTransform).Angle + 90) % 360;
            }
        }

        private void CommandBinding_Finish_CanExecute (object sender, CanExecuteRoutedEventArgs e) {
            e.CanExecute = DialogResultBrush.IsValid(map);
        }

        private void CommandBinding_Finish_Executed (object sender, ExecutedRoutedEventArgs e) {
            DialogResult = true;
            DialogResultBrush.GeneratePreviewImages(map);
            Close( );
        }
    }
}
