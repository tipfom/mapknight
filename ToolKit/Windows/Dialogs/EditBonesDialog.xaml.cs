using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using mapKnight.ToolKit.Data;
using Microsoft.Win32;

namespace mapKnight.ToolKit.Windows.Dialogs {
    public partial class EditBonesDialog : Window {
        public event Action<VertexBone, double> ScaleChanged;
        public event Action<VertexBone> BoneAdded;
        public event Action<int> BoneDeleted;
        public event Action<int, int> BonePositionChanged;

        private OpenFileDialog openImageDialog = new OpenFileDialog( ) { Multiselect = false, ValidateNames = true, CheckFileExists = true, Filter = "Images|*.png;*.jpg;*.jpeg" };

        public EditBonesDialog ( ) {
            InitializeComponent( );
            App.Current.MainWindow.Closed += (sender, e) => Close( );
        }

        public EditBonesDialog (ObservableCollection<VertexBone> bonelist) : this( ) {
            bonelist.CollectionChanged += (sender, e) => {
                listbox_bones.Items.Clear( );
                foreach (VertexBone bone in bonelist)
                    listbox_bones.Items.Add(bone);
            };
        }

        private void CommandDelete_CanExecute (object sender, CanExecuteRoutedEventArgs e) {
            e.CanExecute = listbox_bones != null && listbox_bones.SelectedIndex > -1;
        }

        private void CommandDelete_Executed (object sender, ExecutedRoutedEventArgs e) {
            BoneDeleted?.Invoke(listbox_bones.SelectedIndex);
        }

        private void CommandDown_CanExecute (object sender, CanExecuteRoutedEventArgs e) {
            e.CanExecute = listbox_bones != null && listbox_bones.HasItems && listbox_bones.SelectedIndex < listbox_bones.Items.Count - 1 && listbox_bones.Items.Count > 1 && listbox_bones.SelectedIndex > -1;
        }

        private void CommandDown_Executed (object sender, ExecutedRoutedEventArgs e) {
            BonePositionChanged?.Invoke(listbox_bones.SelectedIndex + 1, listbox_bones.SelectedIndex);
        }

        private void CommandNew_CanExecute (object sender, CanExecuteRoutedEventArgs e) {
            e.CanExecute = true;
        }

        private void CommandNew_Executed (object sender, ExecutedRoutedEventArgs e) {
            if (openImageDialog.ShowDialog( ) ?? false) {
                VertexBone bone = new VertexBone( ) { Image = openImageDialog.FileName, Scale = 0.05f, Position = new Core.Vector2(0, 0), Mirrored = false, Rotation = 0 };
                BoneAdded?.Invoke(bone);
            }
        }

        private void CommandUp_CanExecute (object sender, CanExecuteRoutedEventArgs e) {
            e.CanExecute = listbox_bones != null && listbox_bones.SelectedIndex > 0 && listbox_bones.SelectedIndex < listbox_bones.Items.Count;
        }

        private void CommandUp_Executed (object sender, ExecutedRoutedEventArgs e) {
            BonePositionChanged?.Invoke(listbox_bones.SelectedIndex - 1, listbox_bones.SelectedIndex);
        }

        private void Slider_ValueChanged (object sender, RoutedPropertyChangedEventArgs<double> e) {
            if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift)) {
                // clamp slider value
                Slider slider = (Slider)sender;
                double clampedValue = Math.Round(slider.Value * 2, 1) / 2d;
                if (slider.Value != clampedValue) slider.Value = clampedValue;
            }
            ScaleChanged?.Invoke((VertexBone)((Control)sender).DataContext, ((Slider)sender).Value);
        }

        protected override void OnClosing (CancelEventArgs e) {
            if (Visibility != Visibility.Hidden) {
                e.Cancel = true;
                Visibility = Visibility.Hidden;
            }
        }
    }
}