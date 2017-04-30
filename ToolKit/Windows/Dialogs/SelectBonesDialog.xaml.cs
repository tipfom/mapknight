using System.Collections.Generic;
using System.Windows;
using mapKnight.ToolKit.Data;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace mapKnight.ToolKit.Windows.Dialogs {
    /// <summary>
    /// Interaktionslogik für SelectBonesDialog.xaml
    /// </summary>
    public partial class SelectBonesDialog : Window {
        private SelectBonesDialog ( ) {
            InitializeComponent( );
            App.Current.MainWindow.Closed += (sender, e) => Close( );
            App.Current.MainWindow.Deactivated += (sender, e) => Topmost = false;
            App.Current.MainWindow.Activated += (sender, e) => Topmost = true;
            Owner = App.Current.MainWindow;
        }

        public SelectBonesDialog (ObservableCollection<VertexBone> Bones) : this( ) {
            listview_bones.ItemsSource = Bones;
        }

        public List<int> GetSelectedBones ( ) {
            List<int> result = new List<int>( );
            for (int i = 0; i < listview_bones.Items.Count; i++) {
                if (((VertexBone)listview_bones.Items[i]).Export) {
                    result.Add(i);
                }
            }
            return result;
        }

        private void ButtonInvert_Click (object sender, RoutedEventArgs e) {
            foreach (VertexBone bone in listview_bones.ItemsSource) {
                bone.Export = !bone.Export;
                bone.OnPropertyChanged("Export");
            }
        }

        protected override void OnClosing (CancelEventArgs e) {
            if (App.Current.MainWindow != null) {
                e.Cancel = true;
                Visibility = Visibility.Hidden;
            }
        }
    }
}
