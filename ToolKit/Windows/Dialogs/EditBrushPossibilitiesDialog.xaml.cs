using System.Windows;
using mapKnight.ToolKit.Data;

namespace mapKnight.ToolKit.Windows.Dialogs {
    /// <summary>
    /// Interaktionslogik für EditBrushPossibilitiesDialog.xaml
    /// </summary>
    public partial class EditBrushPossibilitiesDialog : Window {
        private EditBrushPossibilitiesDialog ( ) {
            InitializeComponent( );
        }

        public EditBrushPossibilitiesDialog (TileBrush brush) : this( ) {
            expander_centre.Content = brush.Centre.Strokes;
            expander_ctr.Content = brush.CTR.Strokes;
            expander_ctl.Content = brush.CTL.Strokes;
            expander_cbr.Content = brush.CBR.Strokes;
            expander_cbl.Content = brush.CBL.Strokes;
            expander_It.Content = brush.IT.Strokes;
            expander_Ib.Content = brush.IB.Strokes;
            expander_Ir.Content = brush.IR.Strokes;
            expander_Il.Content = brush.IL.Strokes;
            expander_Ltr.Content = brush.LTR.Strokes;
            expander_Ltl.Content = brush.LTL.Strokes;
            expander_Lbr.Content = brush.LBR.Strokes;
            expander_Lbl.Content = brush.LBL.Strokes;
        }
    }
}
