using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;
using System.Windows.Media.Imaging;
using mapKnight.ToolKit.Controls.Components.Animation;
using mapKnight.ToolKit.Data;
using mapKnight.ToolKit.Windows.Dialogs;
using Newtonsoft.Json;

namespace mapKnight.ToolKit.Controls {
    public partial class AnimationControl2 : UserControl {
        private static readonly double[ ] scales = { 1d, 0.9d, 0.8d, 0.7d, 0.6d, 0.5d, 0.4d };

        private ObservableCollection<VertexAnimation> animations = new ObservableCollection<VertexAnimation>( );
        private List<BoneImage> boneImages = new List<BoneImage>( );
        private ObservableCollection<VertexBone> bones = new ObservableCollection<VertexBone>( );
        private VertexAnimation currentAnimation = null;
        private VertexAnimationFrame currentFrame = null;
        private EditBonesDialog editBonesDialog;
        private List<FrameworkElement> menu = new List<FrameworkElement>( );
        private AnimationMetaData metaData = new AnimationMetaData( );

        public AnimationControl2 ( ) {
            InitializeComponent( );
            treeview_animations.DataContext = animations;

            // init menu
            Button settingButton = new Button( ) { Command = CustomCommands.Settings, Content = new Image( ) { Source = (BitmapImage)App.Current.FindResource("image_animationcomponent_settings"), Style = new Style(typeof(Image)) { Triggers = { new Trigger( ) { Property = Button.IsEnabledProperty, Value = false, Setters = { new Setter(Image.OpacityProperty, 0.5) } } } } } };
            menu.Add(settingButton);

            // init editbonesdialog
            editBonesDialog = new EditBonesDialog(bones);
            editBonesDialog.BoneAdded += EditBonesDialog_BoneAdded;
            editBonesDialog.BoneDeleted += EditBonesDialog_BoneDeleted;
            editBonesDialog.BonePositionChanged += EditBonesDialog_BonePositionChanged;
            editBonesDialog.ScaleChanged += EditBonesDialog_ScaleChanged;

            bones.CollectionChanged += Bones_CollectionChanged;
        }

        private void Bones_CollectionChanged (object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
            BonesChanged( );
        }

        public AnimationControl2 (string metafile) : this( ) {
            metaData = JsonConvert.DeserializeObject<AnimationMetaData>(File.ReadAllText(metafile));
        }

        public AnimationControl2 (double ratio, string entity) : this( ) {
            metaData.Entity = entity;
            metaData.Ratio = ratio;
        }

        public AnimationControl2 (Project project, string animationdirectory) : this( ) {
            foreach (string texturedir in project.GetAllEntries(animationdirectory, "textures")) {
                if (System.IO.Path.GetFileName(texturedir) == ".png") {
                    string name = new DirectoryInfo(System.IO.Path.GetDirectoryName(texturedir)).Name;
                    BoneImage.LoadImage(name, project.GetOrCreateStream(texturedir), project.GetOrCreateStream(System.IO.Path.ChangeExtension(texturedir, ".data")), this, false);
                }
            }

            using (Stream stream = project.GetOrCreateStream(animationdirectory, ".meta"))
                metaData = JsonConvert.DeserializeObject<AnimationMetaData>(new StreamReader(stream).ReadToEnd( ));
            using (Stream stream = project.GetOrCreateStream(animationdirectory, "bones.json"))
                bones.AddRange(JsonConvert.DeserializeObject<VertexBone[ ]>(new StreamReader(stream).ReadToEnd( )));
            using (Stream stream = project.GetOrCreateStream(animationdirectory, "animations.json"))
                animations.AddRange(JsonConvert.DeserializeObject<VertexAnimation[ ]>(new StreamReader(stream).ReadToEnd( )));
        }

        public List<FrameworkElement> Menu { get { return menu; } }

        public override string ToString ( ) {
            return metaData.Entity;
        }

        private void CommandEditorDelete_CanExecute (object sender, CanExecuteRoutedEventArgs e) {
            if (currentFrame != null) {
                e.CanExecute = currentAnimation.Frames.Count > 1;
            } else if (currentAnimation != null) {
                e.CanExecute = animations.Count > 1;
            }
        }

        private void CommandEditorDelete_Executed (object sender, ExecutedRoutedEventArgs e) {
            if (currentFrame != null) {
                currentAnimation.Frames.Remove(currentFrame);
                currentFrame = null;
            } else if (currentAnimation != null) {
                animations.Remove(currentAnimation);
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
        }

        private void CommandEditorNew_CanExecute (object sender, CanExecuteRoutedEventArgs e) {
            e.CanExecute = true;
        }

        private void CommandEditorNew_Executed (object sender, ExecutedRoutedEventArgs e) {
            if (currentAnimation == null) {
                // add new animation
                ObservableCollection<VertexBone> firstFramesBones = (animations.Count == 0) ?
                    new ObservableCollection<VertexBone>(bones.Select(item => item.Clone( ))) : // set reference to the default bones
                    new ObservableCollection<VertexBone>(animations[0].Frames[0].Bones.Select(item => item.Clone( ))); // cheap clone :D

                animations.Add(new VertexAnimation( ) {
                    CanRepeat = true,
                    Frames = new ObservableCollection<VertexAnimationFrame>(new[ ] { new VertexAnimationFrame( ) { Bones = firstFramesBones, Time = 500 } }),
                    Name = "Default_" + animations.Where(anim => anim.Name.StartsWith("Default_")).Count( ).ToString( )
                });
            } else if (currentFrame == null) {
                // add new frame
                currentAnimation.Frames.Add(currentAnimation.Frames[0].Clone( ));
            } else {
                // copy frame
                currentAnimation.Frames.Add(currentFrame.Clone( ));
            }
        }

        private void CommandEditorUp_CanExecute (object sender, CanExecuteRoutedEventArgs e) {
            e.CanExecute = currentAnimation.Frames.IndexOf(currentFrame) > 0;
        }

        private void CommandEditorUp_Executed (object sender, ExecutedRoutedEventArgs e) {
            int index = currentAnimation.Frames.IndexOf(currentFrame);
            currentAnimation.Frames.Move(index, index - 1);
        }

        private void CommandSettings_CanExecute (object sender, CanExecuteRoutedEventArgs e) {
            e.CanExecute = true;
        }

        private void CommandSettings_Executed (object sender, ExecutedRoutedEventArgs e) {
            editBonesDialog.Show( );
        }

        private void EditBonesDialog_BoneAdded (VertexBone addedBone) {
            BoneImage.LoadImage(addedBone.Image, this);
            bones.Add(addedBone);

            string name = System.IO.Path.GetFileNameWithoutExtension(addedBone.Image);
            for (int i = 0; i < animations.Count; i++) {
                for (int j = 0; j < animations[i].Frames.Count; j++) {
                    VertexBone bone = addedBone.Clone( );
                    bone.Image = name;
                    animations[i].Frames[j].Bones.Add(bone);
                }
            }
        }

        private void EditBonesDialog_BoneDeleted (int deletedBoneIndex) {
            bones.RemoveAt(deletedBoneIndex);
            foreach (VertexAnimation animation in animations) {
                foreach (VertexAnimationFrame frame in animation.Frames) {
                    frame.Bones.RemoveAt(deletedBoneIndex);
                }
            }
        }

        private void EditBonesDialog_BonePositionChanged (int newz, int oldz) {
            BoneImage imageAtNewZ = boneImages.FirstOrDefault(image => Canvas.GetZIndex(image) == newz);
            BoneImage imageAtOldZ = boneImages.FirstOrDefault(image => Canvas.GetZIndex(image) == oldz);
            if (imageAtOldZ != null) Canvas.SetZIndex(imageAtOldZ, newz);
            if (imageAtNewZ != null) Canvas.SetZIndex(imageAtNewZ, oldz);

            bones.Move(oldz, newz);
            foreach (VertexAnimation animation in animations) {
                foreach (VertexAnimationFrame frame in animation.Frames) {
                    frame.Bones.Move(oldz, newz);
                }
            }
        }

        private void EditBonesDialog_ScaleChanged (VertexBone bone, double scale) {
            int index = bones.IndexOf(bone);
            foreach (VertexAnimation animation in animations) {
                foreach (VertexAnimationFrame frame in animation.Frames) {
                    frame.Bones[index].Scale = (float)scale;
                }
            }

            BoneImage image = boneImages.FirstOrDefault(item => Canvas.GetZIndex(item) == index);
            if (image != null && currentFrame != null) image.Update( );
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
                currentAnimation = animations.First(anim => anim.Frames.Contains(currentFrame));

                bool addBoneImagesToCanvas = (Canvas)contentpresenter.ContentTemplate.FindName("canvas_frame", contentpresenter) == null;
                contentpresenter.ContentTemplate = (DataTemplate)FindResource("edit");
                contentpresenter.ApplyTemplate( );

                Rectangle rect = (Rectangle)contentpresenter.ContentTemplate.FindName("rectangle_entity", contentpresenter);
                Border border = (Border)contentpresenter.ContentTemplate.FindName("border_rectangle_entity", contentpresenter);
                Canvas canvas = (Canvas)contentpresenter.ContentTemplate.FindName("canvas_frame", contentpresenter);
                for (int i = 0; i < boneImages.Count; i++) {
                    if (addBoneImagesToCanvas) canvas.Children.Add(boneImages[i]);
                    Canvas.SetZIndex(boneImages[i], i);
                    boneImages[i].RefBorder = border;
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

        private void BonesChanged ( ) {
            currentAnimation = null;
            currentFrame = null;

            if (bones.Count > boneImages.Count) {
                for (int i = 0; i < bones.Count - boneImages.Count; i++) {
                    boneImages.Add(new BoneImage(this) { });
                }
            } else {
                for (int i = 0; i < boneImages.Count - bones.Count; i++) {
                    boneImages.RemoveAt(i);
                }
            }

            treeview_animations.Items.Refresh( );
        }

        private void ButtonStartPlay_Click (object sender, RoutedEventArgs e) {
            AnimationView animationView = (AnimationView)contentpresenter.ContentTemplate.FindName("animationview", contentpresenter);
            animationView.Play(currentAnimation, (float)metaData.Ratio, BoneImage.Data[this]);
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

        public void Save (Project project) {
            using (Stream stream = project.GetOrCreateStream("animations", metaData.Entity, ".meta"))
            using (StreamWriter writer = new StreamWriter(stream))
                writer.WriteLine(JsonConvert.SerializeObject(metaData));

            using (Stream stream = project.GetOrCreateStream("animations", metaData.Entity, "animations.json"))
            using (StreamWriter writer = new StreamWriter(stream))
                writer.WriteLine(JsonConvert.SerializeObject(animations));

            using (Stream stream = project.GetOrCreateStream("animations", metaData.Entity, "bones.json"))
            using (StreamWriter writer = new StreamWriter(stream))
                writer.WriteLine(JsonConvert.SerializeObject(bones));

            foreach (KeyValuePair<string, BoneImage.ImageData> kvpair in BoneImage.Data[this]) {
                using (Stream stream = project.GetOrCreateStream("animations", metaData.Entity, "textures", kvpair.Key, ".png"))
                    kvpair.Value.Image.SaveToStream(stream);
                using (Stream stream = project.GetOrCreateStream("animations", metaData.Entity, "textures", kvpair.Key, ".data"))
                using (StreamWriter writer = new StreamWriter(stream))
                    writer.WriteLine(JsonConvert.SerializeObject(kvpair.Value.TransformOrigin));
            }
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
            Border border = (Border)contentpresenter.ContentTemplate.FindName("border_rectangle_entity", contentpresenter);
            Canvas canvas = (Canvas)contentpresenter.ContentTemplate.FindName("canvas_frame", contentpresenter);

            double ultrascale = scales[(int)((Slider)contentpresenter.ContentTemplate.FindName("slider_zoom", contentpresenter)).Value];
            double scale = Math.Min(canvas.RenderSize.Width / metaData.Ratio, canvas.RenderSize.Height * metaData.Ratio) * ultrascale;
            rect.Width = scale * metaData.Ratio;
            rect.Height = scale / metaData.Ratio;

            Canvas.SetLeft(border, (canvas.RenderSize.Width - rect.Width) / 2d);
            Canvas.SetTop(border, (canvas.RenderSize.Height - rect.Height) / 2d);

            foreach (BoneImage image in boneImages) {
                image.RefBorder = border;
                image.RefRectangle = rect;
                image.Update( );
            }
        }
    }
}