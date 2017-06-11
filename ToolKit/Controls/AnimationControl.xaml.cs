using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;
using System.Windows.Media.Imaging;
using mapKnight.ToolKit.Data;
using mapKnight.ToolKit.Controls.Animation;

namespace mapKnight.ToolKit.Controls {
    public partial class AnimationControl : UserControl {
        private static readonly double[ ] scales = { 1d, 0.9d, 0.8d, 0.7d, 0.6d, 0.5d, 0.4d };

        public static readonly RoutedUICommand CopyFrameCommand = new RoutedUICommand(
            "Copy Frame", "CopyFrameCommand", typeof(AnimationControl),
            new InputGestureCollection( ) { new KeyGesture(Key.C, ModifierKeys.Control) });
        public static readonly RoutedUICommand PasteFrameCommand = new RoutedUICommand(
            "Paste Frame", "PasteFrameCommand", typeof(AnimationControl),
            new InputGestureCollection( ) { new KeyGesture(Key.V, ModifierKeys.Control) });

        private List<FrameworkElement> menu = new List<FrameworkElement>( );
        public List<FrameworkElement> Menu { get { return menu; } }

        private VertexAnimationData _Data;
        public VertexAnimationData Data {
            get { return _Data; }
            set {
                if (_Data != null) {
                    _Data.BoneCountChanged -= BonesChanged;
                    _Data.BoneZIndexChanged -= BonesZIndexChanged;
                    _Data.BoneScaleChanged -= BonesScaleChanged;
                    _Data.BoneImageChanged -= ResetEditor;
                }

                _Data = value;
                if (_Data != null) {
                    _Data.BoneCountChanged += BonesChanged;
                    _Data.BoneZIndexChanged += BonesZIndexChanged;
                    _Data.BoneScaleChanged += BonesScaleChanged;
                    _Data.BoneImageChanged += ResetEditor;
                    treeview_animations.DataContext = _Data.Animations;

                    try {
                        object animationView = contentpresenter.ContentTemplate.FindName("animationview", contentpresenter);
                        (animationView as AnimationView)?.Pause( );
                        (animationView as AnimationView)?.Update( );
                    } catch {
                    }

                    BonesChanged( );
                    ResetEditor( );
                }
            }
        }

        private List<BoneImage> boneImages = new List<BoneImage>( );
        private VertexAnimation currentAnimation = null;
        private VertexAnimationFrame currentFrame = null;
        private VertexAnimationFrame copiedFrame = null;

        public AnimationControl ( ) {
            InitializeComponent( );

            // init menu
            Style imageStyle = new Style(typeof(Image)) { Triggers = { new Trigger( ) { Property = Button.IsEnabledProperty, Value = false, Setters = { new Setter(Image.OpacityProperty, 0.5) } } } };
            Image settingButton = new Image( ) {
                Source = (BitmapImage)App.Current.FindResource("image_animationcomponent_settings"),
                Style = imageStyle
            };
            settingButton.MouseDown += (sender, e) => Data?.ShowEditBonesDialog( );
            menu.Add(settingButton);

            Image scissorButton = new Image( ) {
                Source = (BitmapImage)App.Current.FindResource("image_animationcomponent_scissors"),
                Style = imageStyle
            };
            scissorButton.MouseDown += (sender, e) => {
                if (Data?.ShowEntityResizeDialog(currentFrame ?? Data.Animations[0].Frames[0]) ?? false) {
                    ResetEditor( );
                }
            };
            menu.Add(scissorButton);

            Image selectBonesButton = new Image( ) {
                Source = (BitmapImage)App.Current.FindResource("image_animationcomponent_selectbones"),
                Style = imageStyle
            };
            selectBonesButton.MouseDown += (sender, e) => Data?.ShowSelectBonesDialog( );
            menu.Add(selectBonesButton);

            BoneImage.BackupChanges += ( ) => Data?.BackupChanges(currentAnimation, currentFrame);
            BoneImage.DumpChanges += ( ) => Data?.DumpChanges(currentAnimation, currentFrame);

            MLGCanvas.SelectedBoneImageChanged += MLGCanvas_SelectedBoneImageChanged;
        }

        private void MLGCanvas_SelectedBoneImageChanged (BoneImage obj) {
            if (boneImages.Contains(obj) && Data != null) {
                Data.EditBonesDialog.listbox_bones.SelectedIndex = currentFrame.Bones.IndexOf((VertexBone)obj.DataContext);
            }
        }

        private void CommandEditorDelete_CanExecute (object sender, CanExecuteRoutedEventArgs e) {
            if (currentFrame != null) {
                e.CanExecute = currentAnimation != null && currentAnimation.Frames.Count > 1;
            } else if (currentAnimation != null) {
                e.CanExecute = Data.Animations.Count > 1;
            }
        }

        private void CommandEditorDelete_Executed (object sender, ExecutedRoutedEventArgs e) {
            if (currentFrame != null) {
                currentAnimation.Frames.Remove(currentFrame);
                foreach (VertexAnimationFrame frame in currentAnimation.Frames)
                    frame.OnPropertyChanged("Index");

                currentFrame = null;
            } else if (currentAnimation != null) {
                Data.Animations.Remove(currentAnimation);
                currentAnimation = null;
                currentFrame = null;
            }
        }

        private void CommandEditorDown_CanExecute (object sender, CanExecuteRoutedEventArgs e) {
            e.CanExecute = currentAnimation.Frames.IndexOf(currentFrame) < currentAnimation.Frames.Count - 1;
        }

        private void CommandEditorDown_Executed (object sender, ExecutedRoutedEventArgs e) {
            int index = currentAnimation.Frames.IndexOf(currentFrame);
            currentAnimation.Frames.Move(index, index + 1);
            foreach (VertexAnimationFrame frame in currentAnimation.Frames)
                frame.OnPropertyChanged("Index");
        }

        private void CommandEditorNew_CanExecute (object sender, CanExecuteRoutedEventArgs e) {
            e.CanExecute = true;
        }

        private void CommandEditorNew_Executed (object sender, ExecutedRoutedEventArgs e) {
            if (currentAnimation == null) {
                // add new animation
                ObservableCollection<VertexBone> firstFramesBones = (Data.Animations.Count == 0) ?
                    new ObservableCollection<VertexBone>(Data.Bones.Select(item => item.Clone( ))) : // set reference to the default bones
                    new ObservableCollection<VertexBone>(copiedFrame?.Bones ?? Data.Animations[0].Frames[0].Bones.Select(item => item.Clone( ))); // cheap clone :D

                Data.Animations.Add(new VertexAnimation( ) {
                    CanRepeat = true,
                    Frames = new ObservableCollection<VertexAnimationFrame>(new[ ] { new VertexAnimationFrame( ) { Bones = firstFramesBones, Time = 500 } }),
                    Name = "Default_" + Data.Animations.Where(anim => anim.Name.StartsWith("Default_")).Count( ).ToString( ),
                    IsDefault = Data.Animations.Count == 0
                });
            } else {
                currentAnimation.Frames.Add(Data.Animations[0].Frames[0].Clone( ));
            }
        }

        private void CommandEditorUp_CanExecute (object sender, CanExecuteRoutedEventArgs e) {
            e.CanExecute = currentAnimation.Frames.IndexOf(currentFrame) > 0;
        }

        private void CommandEditorUp_Executed (object sender, ExecutedRoutedEventArgs e) {
            int index = currentAnimation.Frames.IndexOf(currentFrame);
            currentAnimation.Frames.Move(index, index - 1);
            foreach (VertexAnimationFrame frame in currentAnimation.Frames)
                frame.OnPropertyChanged("Index");
        }

        private void treeview_animations_MouseDown (object sender, MouseButtonEventArgs e) {
            if (e.RightButton == MouseButtonState.Pressed) {
                // deselect item on button press
                if (treeview_animations.SelectedItem != null)
                    treeview_animations.FindContainer(treeview_animations.SelectedItem).IsSelected = false;
                treeview_animations.Focus( );
            }
        }

        private void treeview_animations_SelectedItemChanged (object sender, RoutedPropertyChangedEventArgs<object> e) {
            currentAnimation = null;
            currentFrame = null;

            if (treeview_animations.SelectedItem == null) {
                treeview_animations.ContextMenu = (ContextMenu)treeview_animations.Resources["contextmenu_default"];
                return;
            }

            Type selectedType = treeview_animations.SelectedItem.GetType( );
            if (selectedType == typeof(VertexAnimation)) {
                treeview_animations.ContextMenu = (ContextMenu)treeview_animations.Resources["contextmenu_animation"];

                currentAnimation = (VertexAnimation)treeview_animations.SelectedItem;

                contentpresenter.ApplyTemplate( );
                ((Canvas)contentpresenter.ContentTemplate.FindName("canvas_frame", contentpresenter))?.Children.Clear( );

                contentpresenter.ContentTemplate = (DataTemplate)FindResource("preview");
            } else if (selectedType == typeof(VertexAnimationFrame)) {
                treeview_animations.ContextMenu = (ContextMenu)treeview_animations.Resources["contextmenu_frame"];

                currentFrame = (VertexAnimationFrame)treeview_animations.SelectedItem;
                currentAnimation = Data.Animations.First(anim => anim.Frames.Contains(currentFrame));

                bool addBoneImagesToCanvas = (Canvas)contentpresenter.ContentTemplate.FindName("canvas_frame", contentpresenter) == null;
                contentpresenter.ContentTemplate = (DataTemplate)FindResource("edit");
                contentpresenter.ApplyTemplate( );

                Rectangle rect = (Rectangle)contentpresenter.ContentTemplate.FindName("rectangle_entity", contentpresenter);
                Canvas canvas = (Canvas)contentpresenter.ContentTemplate.FindName("canvas_frame", contentpresenter);
                for (int i = 0; i < boneImages.Count; i++) {
                    if (addBoneImagesToCanvas) canvas.Children.Add(boneImages[i]);
                    Canvas.SetZIndex(boneImages[i], -i);
                    boneImages[i].RefRectangle = rect;
                    boneImages[i].DataContext = currentFrame.Bones[i];
                }
            }
        }

        private void treeview_animations_TreeViewItemRightMouseButtonDown (object sender, MouseButtonEventArgs e) {
            TreeViewItem item = sender as TreeViewItem;
            if (item != null) {
                item.Focus( );
                e.Handled = true;
            }
        }

        private void BonesZIndexChanged (int newz, int oldz) {
            BoneImage imageAtNewZ = boneImages.FirstOrDefault(image => Canvas.GetZIndex(image) == -newz);
            BoneImage imageAtOldZ = boneImages.FirstOrDefault(image => Canvas.GetZIndex(image) == -oldz);
            if (imageAtOldZ != null) Canvas.SetZIndex(imageAtOldZ, -newz);
            if (imageAtNewZ != null) Canvas.SetZIndex(imageAtNewZ, -oldz);
        }

        private void BonesScaleChanged (int index) {
            BoneImage image = boneImages.FirstOrDefault(item => Canvas.GetZIndex(item) == -index);
            if (image != null && currentFrame != null) image.Update( );
        }

        private void BonesChanged ( ) {
            // to prevent crashing
            try {
                object animationView = contentpresenter.ContentTemplate.FindName("animationview", contentpresenter);
                (animationView as AnimationView)?.Stop( );
            } catch { }

            if (Data.Bones.Count != (copiedFrame?.Bones.Count ?? -1)) copiedFrame = null;

            if (Data.Bones.Count > boneImages.Count) {
                int bonesToAdd = Data.Bones.Count - boneImages.Count;
                for (int i = 0; i < bonesToAdd; i++) {
                    boneImages.Add(new BoneImage(Data) { });
                }
            } else {
                while (boneImages.Count > Data.Bones.Count) {
                    boneImages.RemoveAt(0);
                }
            }

            if (currentFrame != null) {
                ResetEditor( );
            }
        }

        private void ButtonStartPlay_Click (object sender, RoutedEventArgs e) {
            AnimationView animationView = (AnimationView)contentpresenter.ContentTemplate.FindName("animationview", contentpresenter);
            animationView.Play(currentAnimation, (float)Data.Meta.Ratio, Data.Images);
        }

        private void ButtonStopPlay_Click (object sender, RoutedEventArgs e) {
            AnimationView animationView = (AnimationView)contentpresenter.ContentTemplate.FindName("animationview", contentpresenter);
            animationView.Stop( );
        }

        private void ButtonPausePlay_Click (object sender, RoutedEventArgs e) {
            AnimationView animationView = (AnimationView)contentpresenter.ContentTemplate.FindName("animationview", contentpresenter);
            animationView.Pause( );
        }

        private void ButtonResetPlay_Click (object sender, RoutedEventArgs e) {
            AnimationView animationView = (AnimationView)contentpresenter.ContentTemplate.FindName("animationview", contentpresenter);
            animationView.Reset( );
        }

        private void slider_zoom_ValueChanged (object sender, RoutedPropertyChangedEventArgs<double> e) {
            ResetEditor( );
        }

        private void canvas_frame_SizeChanged (object sender, SizeChangedEventArgs e) {
            ResetEditor( );
        }

        private void ResetEditor ( ) {
            if (currentFrame == null) return;
            Rectangle rect = (Rectangle)contentpresenter.ContentTemplate.FindName("rectangle_entity", contentpresenter);
            Canvas canvas = (Canvas)contentpresenter.ContentTemplate.FindName("canvas_frame", contentpresenter);

            double ultrascale = scales[(int)((Slider)contentpresenter.ContentTemplate.FindName("slider_zoom", contentpresenter)).Value];
            if (canvas.RenderSize.Width / canvas.RenderSize.Height > Data.Meta.Ratio) {
                rect.Width = canvas.RenderSize.Height * Data.Meta.Ratio * ultrascale;
                rect.Height = canvas.RenderSize.Height * ultrascale;
            } else {
                rect.Width = canvas.RenderSize.Width * ultrascale;
                rect.Height = canvas.RenderSize.Width / Data.Meta.Ratio * ultrascale;
            }
            rect.Height -= rect.StrokeThickness * 2;
            rect.Width -= rect.StrokeThickness * 2;
            Canvas.SetLeft(rect, (canvas.RenderSize.Width - rect.Width - (rect.StrokeThickness * 2)) / 2d);
            Canvas.SetTop(rect, (canvas.RenderSize.Height - rect.Height - (rect.StrokeThickness * 2)) / 2d);

            for (int i = 0; i < boneImages.Count; i++) {
                if (!canvas.Children.Contains(boneImages[i])) canvas.Children.Add(boneImages[i]);
                Canvas.SetZIndex(boneImages[i], -i);
                boneImages[i].RefRectangle = rect;
                boneImages[i].DataContext = currentFrame.Bones[i];
                boneImages[i].Update( );
            }
        }

        private void CommandEditorUndo_Executed (object sender, ExecutedRoutedEventArgs e) {
            currentFrame.Bones = Data.Undo(currentAnimation, currentFrame);

            for (int i = 0; i < boneImages.Count; i++) {
                Canvas.SetZIndex(boneImages[i], -i);
                boneImages[i].DataContext = currentFrame.Bones[i];
            }
        }

        private void CommandEditorUndo_CanExecute (object sender, CanExecuteRoutedEventArgs e) {
            e.CanExecute = currentAnimation != null && Data.CanUndo(currentAnimation, currentFrame);
        }

        private void CommandBinding_CopyFrame_CanExecute (object sender, CanExecuteRoutedEventArgs e) {
            e.CanExecute = currentFrame != null;
        }

        private void CommandBinding_CopyFrame_Executed (object sender, ExecutedRoutedEventArgs e) {
            copiedFrame = currentFrame.Clone( );
        }

        private void CommandBinding_PasteFrame_Executed (object sender, ExecutedRoutedEventArgs e) {
            if (currentFrame != null) {
                currentAnimation.Frames.Insert(currentAnimation.Frames.IndexOf(currentFrame) + 1, copiedFrame.Clone( ));
            } else {
                currentAnimation.Frames.Add(copiedFrame.Clone( ));
            }
        }

        private void CommandBinding_PasteFrame_CanExecute (object sender, CanExecuteRoutedEventArgs e) {
            e.CanExecute = copiedFrame != null;
        }
    }
}