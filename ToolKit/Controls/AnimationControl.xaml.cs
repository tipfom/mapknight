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
using mapKnight.ToolKit.Data;
using mapKnight.ToolKit.Windows.Dialogs;
using Newtonsoft.Json;
using Path = System.IO.Path;
using mapKnight.ToolKit.Serializer;
using mapKnight.ToolKit.Controls.Animation;

namespace mapKnight.ToolKit.Controls {
    public partial class AnimationControl : UserControl {
        private static readonly double[ ] scales = { 1d, 0.9d, 0.8d, 0.7d, 0.6d, 0.5d, 0.4d };

        private ObservableCollection<VertexAnimation> animations = new ObservableCollection<VertexAnimation>( );
        private List<BoneImage> boneImages = new List<BoneImage>( );
        private ObservableCollection<VertexBone> bones = new ObservableCollection<VertexBone>( );
        private VertexAnimation currentAnimation = null;
        private VertexAnimationFrame currentFrame = null;
        private VertexAnimationFrame featuredFrame = null;
        private EditBonesDialog editBonesDialog;
        private List<FrameworkElement> menu = new List<FrameworkElement>( );
        private Dictionary<VertexAnimation, Dictionary<VertexAnimationFrame, Stack<ObservableCollection<VertexBone>>>> undoStack = new Dictionary<VertexAnimation, Dictionary<VertexAnimationFrame, Stack<ObservableCollection<VertexBone>>>>( );
        public AnimationMetaData MetaData = new AnimationMetaData( );

        public AnimationControl ( ) {
            InitializeComponent( );
            treeview_animations.DataContext = animations;

            // init menu
            Style imageStyle = new Style(typeof(Image)) { Triggers = { new Trigger( ) { Property = Button.IsEnabledProperty, Value = false, Setters = { new Setter(Image.OpacityProperty, 0.5) } } } };
            Image settingButton = new Image( ) {
                Source = (BitmapImage)App.Current.FindResource("image_animationcomponent_settings"),
                Style = imageStyle
            };
            settingButton.MouseDown += SettingButton_MouseDown;
            menu.Add(settingButton);
            Image scissorButton = new Image( ) {
                Source = (BitmapImage)App.Current.FindResource("image_animationcomponent_scissors"),
                Style = imageStyle
            };
            scissorButton.MouseDown += ScissorButton_MouseDown;
            menu.Add(scissorButton);

            // init editbonesdialog
            editBonesDialog = new EditBonesDialog(bones);
            editBonesDialog.BoneAdded += EditBonesDialog_BoneAdded;
            editBonesDialog.BoneDeleted += EditBonesDialog_BoneDeleted;
            editBonesDialog.BonePositionChanged += EditBonesDialog_BonePositionChanged;
            editBonesDialog.ScaleChanged += EditBonesDialog_ScaleChanged;

            BoneImage.BackupChanges += BoneImage_BackupChanges;
            BoneImage.DumpChanges += BoneImage_DumpChanges;

            MLGCanvas.SelectedBoneImageChanged += MLGCanvas_SelectedBoneImageChanged;

            VertexAnimationFrame.GetIndex = (frame) => {
                return animations.FirstOrDefault(anim => anim.Frames.Contains(frame))?.Frames.IndexOf(frame) ?? -1;
            };
        }

        public AnimationControl (string metafile) : this( ) {
            MetaData = JsonConvert.DeserializeObject<AnimationMetaData>(File.ReadAllText(metafile));
        }

        public AnimationControl (double ratio, string entity) : this( ) {
            MetaData.Entity = entity;
            MetaData.Ratio = ratio;
        }

        public AnimationControl (Project project, string animationdirectory) : this( ) {
            foreach (string texturedir in project.GetAllEntries(animationdirectory, "textures")) {
                if (Path.GetFileName(texturedir) == ".png") {
                    string name = new DirectoryInfo(Path.GetDirectoryName(texturedir)).Name;
                    BoneImage.LoadImage(name, project.GetOrCreateStream(false, texturedir), project.GetOrCreateStream(false, Path.ChangeExtension(texturedir, ".data")), this, false);
                }
            }

            using (Stream stream = project.GetOrCreateStream(false, animationdirectory, ".meta"))
                MetaData = JsonConvert.DeserializeObject<AnimationMetaData>(new StreamReader(stream).ReadToEnd( ));
            using (Stream stream = project.GetOrCreateStream(false, animationdirectory, "bones.json"))
                bones.AddRange(JsonConvert.DeserializeObject<VertexBone[ ]>(new StreamReader(stream).ReadToEnd( )));
            using (Stream stream = project.GetOrCreateStream(false, animationdirectory, "animations.json"))
                animations.AddRange(JsonConvert.DeserializeObject<VertexAnimation[ ]>(new StreamReader(stream).ReadToEnd( )));
            featuredFrame = animations.FirstOrDefault(anim => anim.Frames.Any(frame => frame.Featured))?.Frames.FirstOrDefault(frame => frame.Featured);
            BonesChanged( );

            foreach (VertexBone bone in bones) {
                bone.SetBitmapImage(this);
            }
        }

        private void BoneImage_DumpChanges ( ) {
            if (undoStack.ContainsKey(currentAnimation) && undoStack[currentAnimation].ContainsKey(currentFrame)) {
                undoStack[currentAnimation][currentFrame].Pop( );
            }
        }

        private void SettingButton_MouseDown (object sender, MouseButtonEventArgs e) {
            foreach (VertexBone bone in bones) {
                bone.SetBitmapImage(this);
            }
            editBonesDialog.Show( );
        }

        private void ScissorButton_MouseDown (object sender, RoutedEventArgs e) {
            ResizeEntityDialog dialog = new ResizeEntityDialog(MetaData.Ratio, currentFrame?.Bones ?? (featuredFrame.Bones ?? bones), this);
            if (dialog.ShowDialog( ) ?? false) {
                double centerShiftXReal = 0.5d + dialog.TrimRight - (1 + dialog.TrimLeft + dialog.TrimRight) / 2d;
                double centerShiftYReal = 0.5d + dialog.TrimTop - (1 + dialog.TrimTop + dialog.TrimBottom) / 2d;
                double scaleX = (1 + dialog.TrimLeft + dialog.TrimRight);
                double scaleY = (1 + dialog.TrimTop + dialog.TrimBottom);

                foreach (VertexBone bone in bones) {
                    bone.Position = new Core.Vector2(
                        (float)((bone.Position.X - centerShiftXReal) / scaleX),
                        (float)((bone.Position.Y - centerShiftYReal) / scaleY));
                    bone.Scale /= (float)(scaleX);
                }
                foreach (VertexAnimation animation in animations) {
                    foreach (VertexAnimationFrame frame in animation.Frames) {
                        foreach (VertexBone bone in frame.Bones) {
                            bone.Position = new Core.Vector2(
                                (float)((bone.Position.X - centerShiftXReal) / scaleX),
                                (float)((bone.Position.Y - centerShiftYReal) / scaleY));
                            bone.Scale /= (float)(scaleX);
                        }
                    }
                }
                MetaData.Ratio *= scaleX / scaleY;
                ResetEditor( );
            }
        }

        private void MLGCanvas_SelectedBoneImageChanged (BoneImage obj) {
            if (boneImages.Contains(obj)) {
                editBonesDialog.listbox_bones.SelectedIndex = currentFrame.Bones.IndexOf((VertexBone)obj.DataContext);
            }
        }

        private void BoneImage_BackupChanges ( ) {
            if (!undoStack.ContainsKey(currentAnimation)) {
                undoStack.Add(currentAnimation, new Dictionary<VertexAnimationFrame, Stack<ObservableCollection<VertexBone>>>( ));
            }
            if (!undoStack[currentAnimation].ContainsKey(currentFrame)) {
                undoStack[currentAnimation].Add(currentFrame, new Stack<ObservableCollection<VertexBone>>( ));
            }
            undoStack[currentAnimation][currentFrame].Push(new ObservableCollection<VertexBone>(currentFrame.Bones.Select(bone => bone.Clone( ))));
        }
        
        public List<FrameworkElement> Menu { get { return menu; } }

        public override string ToString ( ) {
            return MetaData.Entity;
        }

        private void CommandEditorDelete_CanExecute (object sender, CanExecuteRoutedEventArgs e) {
            if (currentFrame != null) {
                e.CanExecute = currentAnimation != null && currentAnimation.Frames.Count > 1;
            } else if (currentAnimation != null) {
                e.CanExecute = animations.Count > 1;
            }
        }

        public void Compile (string animationpath) {
            SelectBonesDialog selectBonesDialog = new SelectBonesDialog(bones);
            if(selectBonesDialog.ShowDialog() ?? false) {
                string basedirectory = Path.Combine(animationpath, MetaData.Entity);
                if (!Directory.Exists(basedirectory)) Directory.CreateDirectory(basedirectory);

                using (Stream stream = File.Open(Path.Combine(basedirectory, "animation.json"), FileMode.Create))
                    AnimationSerizalizer.Compile(animations.ToArray(), stream, selectBonesDialog.SelectedIndices);
            }
        }

        private void CommandEditorDelete_Executed (object sender, ExecutedRoutedEventArgs e) {
            if (currentFrame != null) {
                currentAnimation.Frames.Remove(currentFrame);
                foreach (VertexAnimationFrame frame in currentAnimation.Frames)
                    frame.OnPropertyChanged("Index");

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
            foreach (VertexAnimationFrame frame in currentAnimation.Frames)
                frame.OnPropertyChanged("Index");
        }

        private void CommandEditorNew_CanExecute (object sender, CanExecuteRoutedEventArgs e) {
            e.CanExecute = true;
        }

        private void CommandEditorNew_Executed (object sender, ExecutedRoutedEventArgs e) {
            if (currentAnimation == null) {
                // add new animation
                ObservableCollection<VertexBone> firstFramesBones = (animations.Count == 0) ?
                    new ObservableCollection<VertexBone>(bones.Select(item => item.Clone( ))) : // set reference to the default bones
                    new ObservableCollection<VertexBone>(featuredFrame?.Bones ?? animations[0].Frames[0].Bones.Select(item => item.Clone( ))); // cheap clone :D

                animations.Add(new VertexAnimation( ) {
                    CanRepeat = true,
                    Frames = new ObservableCollection<VertexAnimationFrame>(new[ ] { new VertexAnimationFrame( ) { Bones = firstFramesBones, Time = 500} }),
                    Name = "Default_" + animations.Where(anim => anim.Name.StartsWith("Default_")).Count( ).ToString( ),
                    IsDefault = animations.Count == 0
                });
                if(animations.Count == 1) {
                    featuredFrame = animations[0].Frames[0];
                    featuredFrame.Featured = true;
                }
            } else if (currentFrame == null) {
                // add new frame
                currentAnimation.Frames.Add(featuredFrame.Clone( ));
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
            foreach (VertexAnimationFrame frame in currentAnimation.Frames)
                frame.OnPropertyChanged("Index");
        }

        private void EditBonesDialog_BoneAdded (VertexBone addedBone) {
            BoneImage.LoadImage(addedBone.Image, this);
            bones.Add(addedBone);
            addedBone.SetBitmapImage(this);

            string name = System.IO.Path.GetFileNameWithoutExtension(addedBone.Image);
            for (int i = 0; i < animations.Count; i++) {
                for (int j = 0; j < animations[i].Frames.Count; j++) {
                    VertexBone bone = addedBone.Clone( );
                    bone.Image = name;
                    animations[i].Frames[j].Bones.Add(bone);
                    if (undoStack.ContainsKey(animations[i])) {
                        if (undoStack[animations[i]].ContainsKey(animations[i].Frames[j])) {
                            foreach (ObservableCollection<VertexBone> collection in undoStack[animations[i]][animations[i].Frames[j]]) {
                                collection.Add(bone);
                            }
                        }
                    }
                }
            }

            BonesChanged( );
        }

        private void EditBonesDialog_BoneDeleted (int deletedBoneIndex) {
            bones.RemoveAt(deletedBoneIndex);
            foreach (VertexAnimation animation in animations) {
                foreach (VertexAnimationFrame frame in animation.Frames) {
                    frame.Bones.RemoveAt(deletedBoneIndex);
                }
            }

            BonesChanged( );
        }

        private void EditBonesDialog_BonePositionChanged (int newz, int oldz) {
            BoneImage imageAtNewZ = boneImages.FirstOrDefault(image => Canvas.GetZIndex(image) == -newz);
            BoneImage imageAtOldZ = boneImages.FirstOrDefault(image => Canvas.GetZIndex(image) == -oldz);
            if (imageAtOldZ != null) Canvas.SetZIndex(imageAtOldZ, -newz);
            if (imageAtNewZ != null) Canvas.SetZIndex(imageAtNewZ, -oldz);

            bones.Move(oldz, newz);
            foreach (VertexAnimation animation in animations) {
                foreach (VertexAnimationFrame frame in animation.Frames) {
                    frame.Bones.Move(oldz, newz);
                    if (undoStack.ContainsKey(animation) && undoStack[animation].ContainsKey(frame)) {
                        foreach (ObservableCollection<VertexBone> collection in undoStack[animation][frame]) {
                            collection.Move(oldz, newz);
                        }
                    }
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

            BoneImage image = boneImages.FirstOrDefault(item => Canvas.GetZIndex(item) == -index);
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

        private void BonesChanged ( ) {
            // to prevent crashing
            try {
                object animationView = contentpresenter.ContentTemplate.FindName("animationview", contentpresenter);
                (animationView as AnimationView)?.Stop( );
            } catch {

            }

            if (bones.Count > boneImages.Count) {
                int bonesToAdd = bones.Count - boneImages.Count;
                for (int i = 0; i < bonesToAdd; i++) {
                    boneImages.Add(new BoneImage(this) { });
                }
            } else {
                for (int i = 0; i < boneImages.Count - bones.Count; i++) {
                    boneImages.RemoveAt(i);
                }
            }

            if (currentFrame != null) {
                ResetEditor( );
            }
        }

        private void ButtonStartPlay_Click (object sender, RoutedEventArgs e) {
            if (BoneImage.Data.ContainsKey(this)) {
                AnimationView animationView = (AnimationView)contentpresenter.ContentTemplate.FindName("animationview", contentpresenter);
                animationView.Play(currentAnimation, (float)MetaData.Ratio, BoneImage.Data[this]);
            }
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
            using (Stream stream = project.GetOrCreateStream(true, "animations", MetaData.Entity, ".meta"))
            using (StreamWriter writer = new StreamWriter(stream))
                writer.WriteLine(JsonConvert.SerializeObject(MetaData));

            using (Stream stream = project.GetOrCreateStream(true, "animations", MetaData.Entity, "animations.json"))
            using (StreamWriter writer = new StreamWriter(stream))
                writer.WriteLine(JsonConvert.SerializeObject(animations));

            using (Stream stream = project.GetOrCreateStream(true, "animations", MetaData.Entity, "bones.json"))
            using (StreamWriter writer = new StreamWriter(stream))
                writer.WriteLine(JsonConvert.SerializeObject(bones));

            if (!BoneImage.Data.ContainsKey(this)) return;
            foreach (KeyValuePair<string, BoneImage.ImageData> kvpair in BoneImage.Data[this]) {
                using (Stream stream = project.GetOrCreateStream(true, "animations", MetaData.Entity, "textures", kvpair.Key, ".png"))
                    kvpair.Value.Image.SaveToStream(stream);
                using (Stream stream = project.GetOrCreateStream(true, "animations", MetaData.Entity, "textures", kvpair.Key, ".data"))
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
            Canvas canvas = (Canvas)contentpresenter.ContentTemplate.FindName("canvas_frame", contentpresenter);

            double ultrascale = scales[(int)((Slider)contentpresenter.ContentTemplate.FindName("slider_zoom", contentpresenter)).Value];
            if (canvas.RenderSize.Width / canvas.RenderSize.Height > MetaData.Ratio) {
                rect.Width = canvas.RenderSize.Height * MetaData.Ratio * ultrascale;
                rect.Height = canvas.RenderSize.Height * ultrascale;
            } else {
                rect.Width = canvas.RenderSize.Width * ultrascale;
                rect.Height = canvas.RenderSize.Width / MetaData.Ratio * ultrascale;
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
            currentFrame.Bones = undoStack[currentAnimation][currentFrame].Pop( );

            for (int i = 0; i < boneImages.Count; i++) {
                Canvas.SetZIndex(boneImages[i], -i);
                boneImages[i].DataContext = currentFrame.Bones[i];
            }
        }

        private void CommandEditorUndo_CanExecute (object sender, CanExecuteRoutedEventArgs e) {
            e.CanExecute = currentAnimation != null && undoStack.ContainsKey(currentAnimation) && undoStack[currentAnimation].ContainsKey(currentFrame) && undoStack[currentAnimation][currentFrame].Count > 0;
        }

        private void MenuItemStarFrame_Click (object sender, RoutedEventArgs e) {
            if (featuredFrame != null) {
                featuredFrame.Featured = false;
                featuredFrame.OnPropertyChanged("Featured");
            }
            currentFrame.Featured = true;
            currentFrame.OnPropertyChanged("Featured");
            featuredFrame = currentFrame;
        }
    }
}