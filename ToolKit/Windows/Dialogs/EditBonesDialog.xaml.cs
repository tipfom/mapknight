using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using mapKnight.ToolKit.Data;
using Microsoft.Win32;

namespace mapKnight.ToolKit.Windows.Dialogs {
    public partial class EditBonesDialog : Window {
        private static readonly double[ ] CLAMP_VALUES = { 0.01, 0.02, 0.03, 0.04, 0.05, 0.06, 0.07, 0.08, 0.09, 0.1 };
        private static readonly OpenFileDialog OPEN_IMAGE_DIALOG = new OpenFileDialog( ) { Multiselect = true, ValidateNames = true, CheckFileExists = true, Filter = "Images|*.png;*.jpg;*.jpeg" };

        public static readonly RoutedUICommand UpCommand = new RoutedUICommand(
            "Up", "UpCommand", typeof(EditBonesDialog),
            new InputGestureCollection( ) { new KeyGesture(Key.U, ModifierKeys.Control) });
        public static readonly RoutedUICommand DownCommand = new RoutedUICommand(
            "Down", "DownCommand", typeof(EditBonesDialog),
            new InputGestureCollection( ) { new KeyGesture(Key.D, ModifierKeys.Control) });
        public static readonly RoutedUICommand AdoptScaleCommand = new RoutedUICommand(
            "Adopt Scale", "AdoptScaleCommand", typeof(EditBonesDialog));
        public static readonly RoutedUICommand ChangeTextureCommand = new RoutedUICommand(
            "Change Texture", "ChangeTextureCommand", typeof(EditBonesDialog));

        public event Action<VertexBone, double> ScaleChanged;
        public event Action<VertexBone> BoneAdded;
        public event Action<int> BoneDeleted;
        public event Action<int, int> BonePositionChanged;
        public event Action<VertexBone, string> BoneTextureChanged;


        private EditBonesDialog ( ) {
            InitializeComponent( );
            App.Current.MainWindow.Closed += (sender, e) => Close( );
            App.Current.MainWindow.Deactivated += (sender, e) => Topmost = false;
            App.Current.MainWindow.Activated += (sender, e) => Topmost = true;
            Owner = App.Current.MainWindow;
        }

        public EditBonesDialog (ObservableCollection<VertexBone> bonelist) : this( ) {
            listbox_bones.ItemsSource = bonelist;
        }

        private void Slider_ValueChanged (object sender, RoutedPropertyChangedEventArgs<double> e) {
            VertexBone bone = (VertexBone)((Control)sender).DataContext;
            if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift)) {
                // clamp slider value
                Slider slider = (Slider)sender;
                double clampedValue = new List<double>(CLAMP_VALUES).Concat(new[ ] { 1d / BitmapFrame.Create(new Uri(bone.Image), BitmapCreateOptions.DelayCreation, BitmapCacheOption.None).PixelWidth }).Min(value => Math.Abs(value - slider.Value)) + slider.Value;
                if (slider.Value != clampedValue) slider.Value = clampedValue;
            }
            ScaleChanged?.Invoke(bone, ((Slider)sender).Value);
        }

        protected override void OnClosing (CancelEventArgs e) {
            if (App.Current.MainWindow != null) {
                e.Cancel = true;
                Visibility = Visibility.Hidden;
            }
        }

        #region CommandBindings
        private void CommandBinding_New_CanExecute (object sender, CanExecuteRoutedEventArgs e) {
            e.CanExecute = true;
        }

        private void CommandBinding_New_Executed (object sender, ExecutedRoutedEventArgs e) {
            if (OPEN_IMAGE_DIALOG.ShowDialog( ) ?? false) {
                foreach (string file in OPEN_IMAGE_DIALOG.FileNames) {
                    VertexBone bone = new VertexBone( ) { Image = file, Scale = 0.05f, Position = new Core.Vector2(0, 0), Mirrored = false, Rotation = 0 };
                    BoneAdded?.Invoke(bone);

                }
            }
        }

        private void CommandBinding_Delete_CanExecute (object sender, CanExecuteRoutedEventArgs e) {
            e.CanExecute = listbox_bones != null && listbox_bones.SelectedIndex > -1;
        }

        private void CommandBinding_Delete_Executed (object sender, ExecutedRoutedEventArgs e) {
            BoneDeleted?.Invoke(listbox_bones.SelectedIndex);
        }

        private void CommandBinding_Up_CanExecute (object sender, CanExecuteRoutedEventArgs e) {
            e.CanExecute = listbox_bones != null && listbox_bones.SelectedIndex > 0 && listbox_bones.SelectedIndex < listbox_bones.Items.Count;
        }

        private void CommandBinding_Up_Executed (object sender, ExecutedRoutedEventArgs e) {
            BonePositionChanged?.Invoke(listbox_bones.SelectedIndex - 1, listbox_bones.SelectedIndex);
        }

        private void CommandBinding_Down_CanExecute (object sender, CanExecuteRoutedEventArgs e) {
            e.CanExecute = listbox_bones != null && listbox_bones.HasItems && listbox_bones.SelectedIndex < listbox_bones.Items.Count - 1 && listbox_bones.Items.Count > 1 && listbox_bones.SelectedIndex > -1;
        }

        private void CommandBinding_Down_Executed (object sender, ExecutedRoutedEventArgs e) {
            BonePositionChanged?.Invoke(listbox_bones.SelectedIndex + 1, listbox_bones.SelectedIndex);
        }

        private void CommandBinding_AdoptScale_CanExecute (object sender, CanExecuteRoutedEventArgs e) {
            e.CanExecute = listbox_bones != null && listbox_bones.SelectedItem != null;
        }

        private void CommandBinding_AdoptScale_Executed (object sender, ExecutedRoutedEventArgs e) {
            float scale = ((VertexBone)listbox_bones.SelectedItem).Scale;
            for (int i = 0; i < listbox_bones.Items.Count; i++) {
                ScaleChanged?.Invoke((VertexBone)listbox_bones.Items[i], scale);
                ((VertexBone)listbox_bones.Items[i]).Scale = scale;
            }
            listbox_bones.Items.Refresh( );
        }

        private void CommandBinding_ChangeTexture_CanExecute (object sender, CanExecuteRoutedEventArgs e) {
            e.CanExecute = listbox_bones != null && listbox_bones.SelectedItem != null;
        }

        private void CommandBinding_ChangeTexture_Executed (object sender, ExecutedRoutedEventArgs e) {
            if (OPEN_IMAGE_DIALOG.ShowDialog( ) ?? false) {
                BoneTextureChanged?.Invoke((VertexBone)listbox_bones.SelectedItem, OPEN_IMAGE_DIALOG.FileName);
            }
        }
        #endregion
    }
}