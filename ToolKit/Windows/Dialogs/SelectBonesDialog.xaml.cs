using System.Collections.Generic;
using System.Windows;
using mapKnight.ToolKit.Data;

namespace mapKnight.ToolKit.Windows.Dialogs {
    /// <summary>
    /// Interaktionslogik für SelectBonesDialog.xaml
    /// </summary>
    public partial class SelectBonesDialog : Window {
        public List<int> SelectedIndices;

        private SelectBonesDialog ( ) {
            InitializeComponent( );
        }

        public SelectBonesDialog (IEnumerable<VertexBone> Bones) : this( ) {
            foreach (VertexBone bone in Bones)
                listview_bones.Items.Add(bone);
        }

        private void Button_Click (object sender, RoutedEventArgs e) {
            DialogResult = true;
            SelectedIndices = new List<int>( );
            for (int i = 0; i < listview_bones.Items.Count; i++) {
                if (((VertexBone)listview_bones.Items[i]).Export) {
                    SelectedIndices.Add(i);
                }
            }
            Close( );
        }
    }
}
