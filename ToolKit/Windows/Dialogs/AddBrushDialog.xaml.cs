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
        public static readonly RoutedUICommand FinishCommand = new RoutedUICommand("Finish", "FinishCommand", typeof(AddBrushDialog));
        public static readonly RoutedUICommand EditPossibilitiesCommand = new RoutedUICommand("Edit Possibilities", "EditPossibilitiesCommand", typeof(AddBrushDialog));

        public static readonly RoutedUICommand NextCornerTLCommand = new RoutedUICommand("Next Corner TL", "NextCornerTLCommand", typeof(AddBrushDialog));
        public static readonly RoutedUICommand NextCornerTRCommand = new RoutedUICommand("Next Corner TR", "NextCornerTRCommand", typeof(AddBrushDialog));
        public static readonly RoutedUICommand NextCornerBLCommand = new RoutedUICommand("Next Corner BL", "NextCornerBLCommand", typeof(AddBrushDialog));
        public static readonly RoutedUICommand NextCornerBRCommand = new RoutedUICommand("Next Corner BR", "NextCornerBRCommand", typeof(AddBrushDialog));
        public static readonly RoutedUICommand NextCentreCommand = new RoutedUICommand("Next Centre", "NextCentreCommand", typeof(AddBrushDialog));
        public static readonly RoutedUICommand NextILCommand = new RoutedUICommand("Next IL", "NextILCommand", typeof(AddBrushDialog));
        public static readonly RoutedUICommand NextITCommand = new RoutedUICommand("Next IT", "NextITCommand", typeof(AddBrushDialog));
        public static readonly RoutedUICommand NextIRCommand = new RoutedUICommand("Next IR", "NextIRCommand", typeof(AddBrushDialog));
        public static readonly RoutedUICommand NextIBCommand = new RoutedUICommand("Next IB", "NextIBCommand", typeof(AddBrushDialog));
        public static readonly RoutedUICommand NextLTLCommand = new RoutedUICommand("Next LTL", "NextLTLCommand", typeof(AddBrushDialog));
        public static readonly RoutedUICommand NextLTRCommand = new RoutedUICommand("Next LTR", "NextLTRCommand", typeof(AddBrushDialog));
        public static readonly RoutedUICommand NextLBLCommand = new RoutedUICommand("Next LBL", "NextLBLCommand", typeof(AddBrushDialog));
        public static readonly RoutedUICommand NextLBRCommand = new RoutedUICommand("Next LBR", "NextLBRCommand", typeof(AddBrushDialog));

        public Data.TileBrush DialogResultBrush = new Data.TileBrush( );

        private EditorMap map;
        private int indexCTL, indexCTR, indexCBR, indexCBL, indexCentre, indexIL, indexIT, indexIR, indexIB, indexLBL, indexLBR, indexLTL, indexLTR;

        private BitmapImage image_d = (BitmapImage)App.Current.FindResource("img_autotile_d");
        private BitmapImage image_I = (BitmapImage)App.Current.FindResource("img_autotile_I");
        private BitmapImage image_O = (BitmapImage)App.Current.FindResource("img_autotile_O");
        private BitmapImage image_L = (BitmapImage)App.Current.FindResource("img_autotile_L");

        private AddBrushDialog ( ) {
            InitializeComponent( );
        }

        public AddBrushDialog (EditorMap map) {
            InitializeComponent( );
            this.map = map;
            foreach (Tile tile in map.Tiles)
                listview_tiles.Items.Add(new ListViewEntry(map.WpfTextures[tile.Name]));
        }

        private void SetTile (ref TileBrushStrokeCollection brushRef, ref int index, Image image) {
            if (listview_tiles.SelectedItem != null) {
                if (brushRef.Count <= index) {
                    brushRef.Add(new TileBrushStroke(map.Tiles[listview_tiles.SelectedIndex], (float)(((RotateTransform)image.RenderTransform).Angle / 180), 1));
                } else {
                    brushRef[index] = new TileBrushStroke(map.Tiles[listview_tiles.SelectedIndex], (float)(((RotateTransform)image.RenderTransform).Angle / 180), 1);
                }
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

        private void CommandBinding_Next_Executed (ref int index, ref TileBrushStrokeCollection brushRef, Image image, BitmapImage defaultImage, float initialRotation) {
            index++;
            if (index > brushRef.Count) index = 0;
            ImageClick(image, defaultImage, (brushRef.Count > index) ? brushRef[index].Tile : (Tile?)null, initialRotation, false);
        }

        #region Image_MouseDown
        private void Image_CornerTL_MouseDown (object sender, MouseButtonEventArgs e) {
            SetTile(ref DialogResultBrush.CTL, ref indexCTL, image_cornertl);
            ImageClick(image_cornertl, image_d, (DialogResultBrush.CTL.Count > indexCTL) ? DialogResultBrush.CTL[indexCTL].Tile : (Tile?)null, 90, e.RightButton == MouseButtonState.Pressed);
        }
        
        private void Image_CornerTR_MouseDown (object sender, MouseButtonEventArgs e) {
            SetTile(ref DialogResultBrush.CTR, ref indexCTR, image_cornertr);
            ImageClick(image_cornertr, image_d, (DialogResultBrush.CTR.Count > indexCTR) ? DialogResultBrush.CTR[indexCTR].Tile : (Tile?)null, 180, e.RightButton == MouseButtonState.Pressed);
        }

        private void Image_CornerBR_MouseDown (object sender, MouseButtonEventArgs e) {
            SetTile(ref DialogResultBrush.CBR, ref indexCBR, image_cornerbr);
            ImageClick(image_cornerbr, image_d, (DialogResultBrush.CBR.Count > indexCBR) ? DialogResultBrush.CBR[indexCBR].Tile : (Tile?)null, 270, e.RightButton == MouseButtonState.Pressed);
        }

        private void Image_CornerBL_MouseDown (object sender, MouseButtonEventArgs e) {
            SetTile(ref DialogResultBrush.CBL, ref indexCBL, image_cornerbl);
            ImageClick(image_cornerbl, image_d, (DialogResultBrush.CBL.Count > indexCBL) ? DialogResultBrush.CBL[indexCBL].Tile : (Tile?)null, 0, e.RightButton == MouseButtonState.Pressed);
        }

        private void Image_Centre_MouseDown (object sender, MouseButtonEventArgs e) {
            SetTile(ref DialogResultBrush.Centre, ref indexCentre, image_centre);
            ImageClick(image_centre, image_O, (DialogResultBrush.Centre.Count > indexCentre) ? DialogResultBrush.Centre[indexCentre].Tile : (Tile?)null, 0, e.RightButton == MouseButtonState.Pressed);
        }

        private void Image_IL_MouseDown (object sender, MouseButtonEventArgs e) {
            SetTile(ref DialogResultBrush.IL, ref indexIL, image_Il);
            ImageClick(image_Il, image_I, (DialogResultBrush.IL.Count > indexIL) ? DialogResultBrush.IL[indexIL].Tile : (Tile?)null, 90, e.RightButton == MouseButtonState.Pressed);
        }

        private void Image_IT_MouseDown (object sender, MouseButtonEventArgs e) {
            SetTile(ref DialogResultBrush.IT, ref indexIT, image_It);
            ImageClick(image_It, image_I, (DialogResultBrush.IT.Count > indexIT) ? DialogResultBrush.IT[indexIT].Tile : (Tile?)null, 180, e.RightButton == MouseButtonState.Pressed);
        }

        private void Image_IR_MouseDown (object sender, MouseButtonEventArgs e) {
            SetTile(ref DialogResultBrush.IR, ref indexIR, image_Ir);
            ImageClick(image_Ir, image_I, (DialogResultBrush.IR.Count > indexIR) ? DialogResultBrush.IR[indexIR].Tile : (Tile?)null, 270, e.RightButton == MouseButtonState.Pressed);
        }

        private void Image_IB_MouseDown (object sender, MouseButtonEventArgs e) {
            SetTile(ref DialogResultBrush.IB, ref indexIB, image_Ib);
            ImageClick(image_Ib, image_I, (DialogResultBrush.IB.Count > indexIB) ? DialogResultBrush.IB[indexIB].Tile : (Tile?)null, 0, e.RightButton == MouseButtonState.Pressed);
        }

        private void Image_LBL_MouseDown (object sender, MouseButtonEventArgs e) {
            SetTile(ref DialogResultBrush.LBL, ref indexLBL, image_Lbl);
            ImageClick(image_Lbl, image_L, (DialogResultBrush.LBL.Count > indexLBL) ? DialogResultBrush.LBL[indexLBL].Tile : (Tile?)null, 0, e.RightButton == MouseButtonState.Pressed);
        }

        private void Image_LBR_MouseDown (object sender, MouseButtonEventArgs e) {
            SetTile(ref DialogResultBrush.LBR, ref indexLBR, image_Lbr);
            ImageClick(image_Lbr, image_L, (DialogResultBrush.LBR.Count > indexLBR) ? DialogResultBrush.LBR[indexLBR].Tile : (Tile?)null, 270, e.RightButton == MouseButtonState.Pressed);
        }

        private void Image_LTL_MouseDown (object sender, MouseButtonEventArgs e) {
            SetTile(ref DialogResultBrush.LTL, ref indexLTL, image_Ltl);
            ImageClick(image_Ltl, image_L, (DialogResultBrush.LTL.Count > indexLTL) ? DialogResultBrush.LTL[indexLTL].Tile : (Tile?)null, 90, e.RightButton == MouseButtonState.Pressed);
        }

        private void Image_LTR_MouseDown (object sender, MouseButtonEventArgs e) {
            SetTile(ref DialogResultBrush.LTR, ref indexLTR, image_Ltr);
            ImageClick(image_Ltr, image_L, (DialogResultBrush.LTR.Count > indexLTR) ? DialogResultBrush.LTR[indexLTR].Tile : (Tile?)null, 180, e.RightButton == MouseButtonState.Pressed);
        }
        #endregion

        #region CommandBindings
        #region Next* CommandBindings
        private void CommandBinding_NextCornerTL_CanExecute (object sender, CanExecuteRoutedEventArgs e) {
            e.CanExecute = DialogResultBrush.CTL.Count > 0;
        }

        private void CommandBinding_NextCornerTL_Executed (object sender, ExecutedRoutedEventArgs e) {
            CommandBinding_Next_Executed(ref indexCTL, ref DialogResultBrush.CTL, image_cornertl, image_d, 90f);
        }

        private void CommandBinding_NextCornerTR_CanExecute (object sender, CanExecuteRoutedEventArgs e) {
            e.CanExecute = DialogResultBrush.CTR.Count > 0;
        }

        private void CommandBinding_NextCornerTR_Executed (object sender, ExecutedRoutedEventArgs e) {
            CommandBinding_Next_Executed(ref indexCTR, ref DialogResultBrush.CTR, image_cornertr, image_d, 180f);
        }

        private void CommandBinding_NextCornerBL_CanExecute (object sender, CanExecuteRoutedEventArgs e) {
            e.CanExecute = DialogResultBrush.CBL.Count > 0;
        }

        private void CommandBinding_NextCornerBL_Executed (object sender, ExecutedRoutedEventArgs e) {
            CommandBinding_Next_Executed(ref indexCBL, ref DialogResultBrush.CBL, image_cornerbl, image_d, 0f);
        }

        private void CommandBinding_NextCornerBR_CanExecute (object sender, CanExecuteRoutedEventArgs e) {
            e.CanExecute = DialogResultBrush.CBR.Count > 0;
        }

        private void CommandBinding_NextCornerBR_Executed (object sender, ExecutedRoutedEventArgs e) {
            CommandBinding_Next_Executed(ref indexCBR, ref DialogResultBrush.CBR, image_cornerbr, image_d, 270f);
        }

        private void CommandBinding_NextCentre_CanExecute (object sender, CanExecuteRoutedEventArgs e) {
            e.CanExecute = DialogResultBrush.Centre.Count > 0;
        }

        private void CommandBinding_NextCentre_Executed (object sender, ExecutedRoutedEventArgs e) {
            CommandBinding_Next_Executed(ref indexCentre, ref DialogResultBrush.Centre, image_centre, image_O, 0f);
        }

        private void CommandBinding_NextIL_CanExecute (object sender, CanExecuteRoutedEventArgs e) {
            e.CanExecute = DialogResultBrush.IL.Count > 0;
        }

        private void CommandBinding_NextIL_Executed (object sender, ExecutedRoutedEventArgs e) {
            CommandBinding_Next_Executed(ref indexIL, ref DialogResultBrush.IL, image_Il, image_I, 90f);
        }

        private void CommandBinding_NextIT_CanExecute (object sender, CanExecuteRoutedEventArgs e) {
            e.CanExecute = DialogResultBrush.IT.Count > 0;
        }

        private void CommandBinding_NextIT_Executed (object sender, ExecutedRoutedEventArgs e) {
            CommandBinding_Next_Executed(ref indexIT, ref DialogResultBrush.IT, image_It, image_I, 180f);
        }

        private void CommandBinding_NextIR_CanExecute (object sender, CanExecuteRoutedEventArgs e) {
            e.CanExecute = DialogResultBrush.IR.Count > 0;
        }

        private void CommandBinding_NextIR_Executed (object sender, ExecutedRoutedEventArgs e) {
            CommandBinding_Next_Executed(ref indexIR, ref DialogResultBrush.IR, image_Ir, image_I, 270f);
        }

        private void CommandBinding_NextIB_CanExecute (object sender, CanExecuteRoutedEventArgs e) {
            e.CanExecute = DialogResultBrush.IB.Count > 0;
        }

        private void CommandBinding_NextIB_Executed (object sender, ExecutedRoutedEventArgs e) {
            CommandBinding_Next_Executed(ref indexIB, ref DialogResultBrush.IB, image_Ib, image_I, 0f);
        }

        private void CommandBinding_NextLTL_CanExecute (object sender, CanExecuteRoutedEventArgs e) {
            e.CanExecute = DialogResultBrush.LTL.Count > 0;
        }

        private void CommandBinding_NextLTL_Executed (object sender, ExecutedRoutedEventArgs e) {
            CommandBinding_Next_Executed(ref indexLTL, ref DialogResultBrush.LTL, image_Ltl, image_L, 90f);
        }

        private void CommandBinding_NextLTR_CanExecute (object sender, CanExecuteRoutedEventArgs e) {
            e.CanExecute = DialogResultBrush.LTR.Count > 0;
        }

        private void CommandBinding_NextLTR_Executed (object sender, ExecutedRoutedEventArgs e) {
            CommandBinding_Next_Executed(ref indexLTR, ref DialogResultBrush.LTR, image_Ltr, image_L, 180f);
        }

        private void CommandBinding_NextLBL_CanExecute (object sender, CanExecuteRoutedEventArgs e) {
            e.CanExecute = DialogResultBrush.LBL.Count > 0;
        }

        private void CommandBinding_NextLBL_Executed (object sender, ExecutedRoutedEventArgs e) {
            CommandBinding_Next_Executed(ref indexLBL, ref DialogResultBrush.LBL, image_Lbl, image_L, 0f);
        }

        private void CommandBinding_NextLBR_CanExecute (object sender, CanExecuteRoutedEventArgs e) {
            e.CanExecute = DialogResultBrush.LBR.Count > 0;
        }

        private void CommandBinding_NextLBR_Executed (object sender, ExecutedRoutedEventArgs e) {
            CommandBinding_Next_Executed(ref indexLBR, ref DialogResultBrush.LBR, image_Lbr, image_L, 270f);
        }
        #endregion
        
        private void CommandBinding_Finish_CanExecute (object sender, CanExecuteRoutedEventArgs e) {
            e.CanExecute = DialogResultBrush.IsValid(map);
        }

        private void CommandBinding_Finish_Executed (object sender, ExecutedRoutedEventArgs e) {
            DialogResult = true;
            DialogResultBrush.GeneratePreviewImages(map);
            Close( );
        }

        private void CommandBinding_EditPossibilities_CanExecute (object sender, CanExecuteRoutedEventArgs e) {
            e.CanExecute = DialogResultBrush.IsValid(map);
        }

        private void CommandBinding_EditPossibilities_Executed (object sender, ExecutedRoutedEventArgs e) {
            DialogResultBrush.GeneratePreviewImages(map);
            new EditBrushPossibilitiesDialog(DialogResultBrush).ShowDialog( );
        }
        #endregion

        private struct ListViewEntry {
            public ListViewEntry (BitmapImage image) {
                Image = image;
            }

            public BitmapImage Image { get; private set; }
        }
    }
}
