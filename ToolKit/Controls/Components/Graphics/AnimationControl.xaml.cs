using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using mapKnight.Core;
using mapKnight.Core.Graphics;
using mapKnight.ToolKit.Controls.Components.Animation;
using Newtonsoft.Json;
using Size = System.Windows.Size;

namespace mapKnight.ToolKit.Controls.Components.Graphics {

    /// <summary>
    /// Interaktionslogik für AnimationControl.xaml
    /// </summary>
    public partial class AnimationControl : UserControl, IComponentControl {
        public readonly string EntityName;
        private static double[ ] greaterSizeEditorUsePercent = { 1d, 0.75f, 0.5d };

        private Dictionary<string, VertexBone> _Bones = new Dictionary<string, VertexBone>( );
        private Dictionary<string, BitmapImage> _Images = new Dictionary<string, BitmapImage>( );
        private double _TransformAspectRatio = 1.5d;
        private Dictionary<string, ResizableImage> boneImages = new Dictionary<string, ResizableImage>( );
        private VertexAnimation currentAnimation;
        private VertexAnimationFrame currentFrame;
        private HashSet<VertexAnimation> requiredAnimations = new HashSet<VertexAnimation>( );

        public AnimationControl ( ) {
            InitializeComponent( );
            treeview_animations.DataContext = Animations;
            rectangle_player.DataContext = TransformAspectRatio;
        }

        public AnimationControl (string pathtoload) : this( ) {
            if (File.Exists(Path.Combine(pathtoload, "animation.meta"))) {
                AnimationMetaData metaData = JsonConvert.DeserializeObject<AnimationMetaData>(File.ReadAllText(Path.Combine(pathtoload, "animation.meta")));
                EntityName = metaData.Name;
                foreach (string file in metaData.Bones) {
                    if (File.Exists(Path.Combine(pathtoload,file))) {
                        BoneListBoxItem item = new BoneListBoxItem(Path.Combine(pathtoload, file));
                        Bones.Add(item.Name, new VertexBone( ) { Mirrored = false, Position = new Vector2(0, 0), Rotation = 0, Size = new Vector2(0.25f, 0.25f) });
                        Images.Add(item.Name, item.Image);
                        listbox_bones.Items.Add(new BoneListBoxItem(Path.Combine(pathtoload, file)));
                    }
                }
                BonesChanged( );
                Animations = new ObservableCollection<VertexAnimation>(JsonConvert.DeserializeObject<List<VertexAnimation>>(File.ReadAllText(Path.Combine(pathtoload, "animation.json"))));
                treeview_animations.DataContext = Animations;
            }
        }

        public AnimationControl (float ratio, string text) : this( ) {
            TransformAspectRatio = ratio;
            EntityName = text;
        }

        public ObservableCollection<VertexAnimation> Animations { get; } = new ObservableCollection<VertexAnimation>( );

        public Dictionary<string, VertexBone> Bones { get { return _Bones; } set { _Bones = value; BonesChanged( ); } }

        public string Category { get { return "gfx"; } }

        public Dictionary<string, BitmapImage> Images { get { return _Images; } set { _Images = value; BonesChanged( ); } }

        public List<Control> Menu { get; } = new List<Control>( );

        public double TransformAspectRatio { get { return _TransformAspectRatio; } set { _TransformAspectRatio = value; AdjustEditor( ); } }

        public Dictionary<string, string> Compile ( ) {
            throw new NotImplementedException( );
        }

        public void Save (string path) {
            AnimationMetaData metaData = new AnimationMetaData( ) { Ratio = TransformAspectRatio, Name = EntityName, Bones = new List<string>( ) };
            if (Directory.Exists(Path.Combine(path, EntityName)))
                Directory.Delete(Path.Combine(path, EntityName));
            Directory.CreateDirectory(Path.Combine(path, EntityName));
            foreach (KeyValuePair<string, BitmapImage> kvpair in Images) {
                PngBitmapEncoder encoder = new PngBitmapEncoder( );
                encoder.Frames.Add(BitmapFrame.Create(kvpair.Value));
                using (Stream fileStream = File.Create(Path.Combine(path, EntityName, kvpair.Key + ".png")))
                    encoder.Save(fileStream);
                metaData.Bones.Add(kvpair.Key + ".png");
            }
            File.Create(Path.Combine(path, EntityName, "animation.meta")).Close();
            File.WriteAllText(Path.Combine(path, EntityName, "animation.meta"), JsonConvert.SerializeObject(metaData));
            File.Create(Path.Combine(path, EntityName, "animation.json")).Close();
            File.WriteAllText(Path.Combine(path, EntityName, "animation.json"), JsonConvert.SerializeObject(Animations));
        }

        public override string ToString ( ) {
            return EntityName;
        }

        private static TreeViewItem ContainerFromItem (ItemContainerGenerator containerGenerator, object item) {
            TreeViewItem container = (TreeViewItem)containerGenerator.ContainerFromItem(item);
            if (container != null)
                return container;

            foreach (object childItem in containerGenerator.Items) {
                TreeViewItem parent = containerGenerator.ContainerFromItem(childItem) as TreeViewItem;
                if (parent == null)
                    continue;

                container = parent.ItemContainerGenerator.ContainerFromItem(item) as TreeViewItem;
                if (container != null)
                    return container;

                container = ContainerFromItem(parent.ItemContainerGenerator, item);
                if (container != null)
                    return container;
            }
            return null;
        }

        private void AdjustEditor ( ) {
            Size oldRectangleSize = rectangle_player.DesiredSize;
            Point oldRectanglePos = new Point(Canvas.GetLeft(border_rectangle_player), Canvas.GetTop(border_rectangle_player));
            if (TransformAspectRatio > 1d) {
                // width > height
                rectangle_player.Width = greaterSizeEditorUsePercent[(int)slider_zoom.Value] * canvas_frame.RenderSize.Width;
                rectangle_player.Height = rectangle_player.Width / TransformAspectRatio;
            } else {
                // height > width
                rectangle_player.Height = greaterSizeEditorUsePercent[(int)slider_zoom.Value] * canvas_frame.RenderSize.Height;
                rectangle_player.Width = TransformAspectRatio * rectangle_player.Height;
            }
            Canvas.SetLeft(border_rectangle_player, (canvas_frame.RenderSize.Width - rectangle_player.Width) / 2d);
            Canvas.SetTop(border_rectangle_player, (canvas_frame.RenderSize.Height - rectangle_player.Height) / 2d);

            if (currentFrame == null)
                return;
            foreach (KeyValuePair<string, ResizableImage> kvpair in boneImages) {
                UpdateBoneImage(kvpair.Key);
            }
        }

        private void BoneImage_CanvasLeftChanged (object sender, EventArgs e) {
            UpdatePositionOfBone(boneImages.First(pair => pair.Value == sender).Key);
        }

        private void BoneImage_CanvasTopChanged (object sender, EventArgs e) {
            UpdatePositionOfBone(boneImages.First(pair => pair.Value == sender).Key);
        }

        private void BoneImage_Rotated (ResizableImage obj) {
            string key = boneImages.First(pair => pair.Value == obj).Key;
            if (currentFrame == null)
                return;
            VertexBone bone = currentFrame.State[key];
            bone.Rotation = obj.Rotation;
            currentFrame.State.Remove(key);
            currentFrame.State.Add(key, bone);
        }

        private void BoneImage_SizeChanged (object sender, SizeChangedEventArgs e) {
            if (currentFrame == null) return;
            ResizableImage image = (ResizableImage)sender;
            float newsizex = (float)(image.Width / rectangle_player.Width);
            float newsizey = (float)(image.Height / rectangle_player.Height);
            float newpercentx = (float)(((Canvas.GetLeft(image) + image.Width / 2d) - (Canvas.GetLeft(border_rectangle_player) + rectangle_player.Width / 2d)) / rectangle_player.Width);
            float newpercenty = (float)(((Canvas.GetTop(image) + image.Height / 2d) - (Canvas.GetTop(border_rectangle_player) + rectangle_player.Height / 2d)) / rectangle_player.Height);
            string key = boneImages.First(pair => pair.Value == sender).Key;
            VertexBone current = currentFrame.State[key];
            current.Size = new Vector2(newsizex, newsizey);
            current.Position = new Vector2(newpercentx, newpercenty);
            currentFrame.State.Remove(key);
            currentFrame.State.Add(key, current);
        }

        private void BonesChanged ( ) {
            currentAnimation = null;
            currentFrame = null;

            foreach (ResizableImage image in boneImages.Values) {
                canvas_frame.Children.Remove(image);
            }
            Dictionary<string, ResizableImage> newBoneImages = new Dictionary<string, ResizableImage>( );
            foreach (string boneKey in _Bones.Keys) {
                ResizableImage image;
                if (boneImages.ContainsKey(boneKey)) {
                    image = boneImages[boneKey];
                    image.Image = null;
                } else {
                    image = new ResizableImage( );
                    image.Rotated += BoneImage_Rotated;
                    image.Height = 100;
                    image.Width = 100;
                    Canvas.SetLeft(image, Canvas.GetLeft(border_rectangle_player) + image.Width / 2d);
                    Canvas.SetTop(image, Canvas.GetTop(border_rectangle_player) + image.Height / 2d);
                    image.SizeChanged += BoneImage_SizeChanged;
                    DependencyPropertyDescriptor canvasleftproperty = DependencyPropertyDescriptor.FromProperty(Canvas.LeftProperty, typeof(ResizableImage));
                    canvasleftproperty.AddValueChanged(image, BoneImage_CanvasLeftChanged);
                    DependencyPropertyDescriptor canvastopproperty = DependencyPropertyDescriptor.FromProperty(Canvas.TopProperty, typeof(ResizableImage));
                    canvastopproperty.AddValueChanged(image, BoneImage_CanvasTopChanged);
                }
                newBoneImages.Add(boneKey, image);
                if (_Images.ContainsKey(boneKey)) {
                    image.Image = _Images[boneKey];
                }
                canvas_frame.Children.Add(image);
            }
            boneImages = newBoneImages;

            foreach (VertexAnimation animation in Animations) {
                foreach (VertexAnimationFrame frame in animation.Frames) {
                    ObservableDictionary<string, VertexBone> newframestate = new ObservableDictionary<string, VertexBone>( );
                    foreach (KeyValuePair<string, VertexBone> bone in _Bones) {
                        if (frame.State.ContainsKey(bone.Key)) {
                            newframestate.Add(bone.Key, frame.State[bone.Key]);
                        } else {
                            newframestate.Add(bone.Key, new VertexBone( ) { Mirrored = bone.Value.Mirrored, Size = new Vector2(bone.Value.Size), Rotation = bone.Value.Rotation, Position = new Vector2(bone.Value.Position) });
                        }
                    }
                    frame.State = newframestate;
                }
            }

            treeview_animations.Items.Refresh( );
        }

        private void ButtonBoneFlipped_Click (object sender, RoutedEventArgs e) {
            KeyValuePair<string, VertexBone> kvpair = (KeyValuePair<string, VertexBone>)(((Control)sender).DataContext);
            VertexBone bone = kvpair.Value;
            VertexAnimationFrame frame = FindFrame(bone);
            bone.Mirrored = !bone.Mirrored;
            frame.State.Remove(kvpair.Key);
            frame.State.Add(kvpair.Key, bone);
            if (frame == currentFrame)
                UpdateBoneImage(kvpair.Key);
        }

        private void ButtonPausePlay_Click (object sender, RoutedEventArgs e) {
            animationview.Pause( );
        }

        private void ButtonRemoveBoneItem_Click (object sender, RoutedEventArgs e) {
            string boneName = ((TextBlock)((StackPanel)((Button)sender).Parent).Children[2]).Text;
            BoneListBoxItem item = (BoneListBoxItem)listbox_bones.Items[new List<string>(Bones.Keys.AsEnumerable( )).IndexOf(boneName)];
            listbox_bones.Items.Remove(item);
            Bones.Remove(item.Name);
            Images.Remove(item.Name);
            BonesChanged( );
        }

        private void ButtonResetPlay_Click (object sender, RoutedEventArgs e) {
            animationview.Reset( );
        }

        private void ButtonStartPlay_Click (object sender, RoutedEventArgs e) {
            if (currentAnimation == null || currentAnimation.Frames.Count < 0) return;
            animationview.Play(currentAnimation, (float)_TransformAspectRatio, boneImages.ToDictionary(entry => entry.Key, entry => entry.Value.Image));
        }

        private void ButtonStopPlay_Click (object sender, RoutedEventArgs e) {
            animationview.Stop( );
        }

        private void canvas_frame_SizeChanged (object sender, SizeChangedEventArgs e) {
            AdjustEditor( );
        }

        private void CommandEditorDelete_CanExecute (object sender, CanExecuteRoutedEventArgs e) {
            if (treeview_animations.SelectedItem is VertexAnimation) {
                e.CanExecute = !requiredAnimations.Contains((VertexAnimation)treeview_animations.SelectedItem);
            } else if (treeview_animations.SelectedItem is VertexAnimationFrame) {
                e.CanExecute = Animations.First(a => a.Frames.Contains((VertexAnimationFrame)treeview_animations.SelectedItem)).Frames.Count > 2;
            }
        }

        private void CommandEditorDelete_Executed (object sender, ExecutedRoutedEventArgs e) {
            if (treeview_animations.SelectedItem is VertexAnimation) {
                Animations.Remove((VertexAnimation)treeview_animations.SelectedItem);
            } else if (treeview_animations.SelectedItem is VertexAnimationFrame) {
                VertexAnimation anim = Animations.First(a => a.Frames.Contains((VertexAnimationFrame)treeview_animations.SelectedItem));
                anim.Frames.Remove((VertexAnimationFrame)treeview_animations.SelectedItem);
            }
        }

        private void CommandEditorDown_CanExecute (object sender, CanExecuteRoutedEventArgs e) {
            e.CanExecute = true;
            if (treeview_animations.SelectedItem is VertexAnimationFrame) {
                VertexAnimation animation = Animations.FirstOrDefault(anim => anim.Frames.Contains(treeview_animations.SelectedItem));
                if (animation != default(VertexAnimation))
                    e.CanExecute = animation.Frames.IndexOf((VertexAnimationFrame)treeview_animations.SelectedItem) < animation.Frames.Count - 1;
            }
        }

        private void CommandEditorDown_Executed (object sender, ExecutedRoutedEventArgs e) {
            if (treeview_animations.SelectedItem is VertexAnimationFrame) {
                VertexAnimation animation = Animations.FirstOrDefault(anim => anim.Frames.Contains(treeview_animations.SelectedItem));
                if (animation != default(VertexAnimation)) {
                    int index = animation.Frames.IndexOf((VertexAnimationFrame)treeview_animations.SelectedItem);
                    animation.Frames.Move(index, index + 1);
                }
            }
        }

        private void CommandEditorNew_CanExecute (object sender, CanExecuteRoutedEventArgs e) {
            e.CanExecute = true;
        }

        private void CommandEditorNew_Executed (object sender, ExecutedRoutedEventArgs e) {
            if (treeview_animations.SelectedItem == null) {
                Animations.Add(new VertexAnimation( ) { Name = "Default" + Animations.Where(a => a.Name.StartsWith("Default")).Count( ), Frames = new ObservableCollection<VertexAnimationFrame>( ), CanRepeat = false });
            } else if (treeview_animations.SelectedItem is VertexAnimation) {
                ((VertexAnimation)treeview_animations.SelectedItem).Frames.Add(new VertexAnimationFrame( ) { Time = 500, State = new ObservableDictionary<string, VertexBone>(_Bones.ToDictionary(entry => entry.Key, entry => entry.Value)) });
            } else if (treeview_animations.SelectedItem is VertexAnimationFrame) {
                VertexAnimation animation = Animations.FirstOrDefault(anim => anim.Frames.Contains(treeview_animations.SelectedItem));
                if (animation != default(VertexAnimation)) {
                    int index = animation.Frames.IndexOf((VertexAnimationFrame)treeview_animations.SelectedItem);
                    animation.Frames.Add(new VertexAnimationFrame( ) { State = new ObservableDictionary<string, VertexBone>(animation.Frames[index].State.ToDictionary(entry => entry.Key, entry => new VertexBone( ) { Mirrored = entry.Value.Mirrored, Position = new Vector2(entry.Value.Position), Rotation = entry.Value.Rotation, Size = new Vector2(entry.Value.Size) })), Time = animation.Frames[index].Time });
                }
            }
        }

        private void CommandEditorR_CanExecute (object sender, CanExecuteRoutedEventArgs e) {
            e.CanExecute = treeview_animations.SelectedItem is VertexAnimationFrame;
        }

        private void CommandEditorR_Executed (object sender, ExecutedRoutedEventArgs e) {
        }

        private void CommandEditorUp_CanExecute (object sender, CanExecuteRoutedEventArgs e) {
            e.CanExecute = true;
            if (treeview_animations.SelectedItem is VertexAnimationFrame) {
                VertexAnimation animation = Animations.FirstOrDefault(anim => anim.Frames.Contains(treeview_animations.SelectedItem));
                if (animation != default(VertexAnimation))
                    e.CanExecute = animation.Frames.IndexOf((VertexAnimationFrame)treeview_animations.SelectedItem) > 0;
            }
        }

        private void CommandEditorUp_Executed (object sender, ExecutedRoutedEventArgs e) {
            if (treeview_animations.SelectedItem is VertexAnimationFrame) {
                VertexAnimation animation = Animations.FirstOrDefault(anim => anim.Frames.Contains(treeview_animations.SelectedItem));
                if (animation != default(VertexAnimation)) {
                    int index = animation.Frames.IndexOf((VertexAnimationFrame)treeview_animations.SelectedItem);
                    animation.Frames.Move(index, index - 1);
                }
            }
        }

        private VertexAnimationFrame FindFrame (VertexBone bone) {
            foreach (VertexAnimation anim in Animations) {
                foreach (VertexAnimationFrame frame in anim.Frames) {
                    if (frame.State.Any(pair => pair.Value.Equals(bone)))
                        return frame;
                }
            }
            return null;
        }

        private void listbox_bones_DragEnter (object sender, DragEventArgs e) {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) {
                e.Effects = DragDropEffects.Copy;
            }
        }

        private void listbox_bones_Drop (object sender, DragEventArgs e) {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) {
                string[ ] files = (string[ ])e.Data.GetData(DataFormats.FileDrop);
                foreach (string file in files) {
                    if (file.EndsWith("png") && !Bones.ContainsKey(Path.GetFileNameWithoutExtension(file))) {
                        BoneListBoxItem item = new BoneListBoxItem(file);
                        listbox_bones.Items.Add(item);
                        Bones.Add(item.Name, new VertexBone( ) { Mirrored = false, Position = new Vector2(0, 0), Rotation = 0, Size = new Vector2(0.25f, 0.25f) });
                        _Images.Add(item.Name, item.Image);
                        BonesChanged( );
                    }
                }
            }
        }

        private void slider_zoom_ValueChanged (object sender, RoutedPropertyChangedEventArgs<double> e) {
            AdjustEditor( );
        }

        private void TextBox_IntegerTextInput (object sender, TextCompositionEventArgs e) {
            int i;
            if (!int.TryParse(((TextBox)sender).Text + e.Text, out i) || i < 0)
                e.Handled = true;
        }

        private void treeview_animations_MouseDown (object sender, MouseButtonEventArgs e) {
            if (treeview_animations.SelectedItem != null && e.LeftButton == MouseButtonState.Pressed) {
                ContainerFromItem(treeview_animations.ItemContainerGenerator, treeview_animations.SelectedItem).IsSelected = false;
                treeview_animations.Focus( );
            }
        }

        private void treeview_animations_SelectedItemChanged (object sender, RoutedPropertyChangedEventArgs<object> e) {
            if (e.NewValue == null) {
                treeview_animations.ContextMenu = (ContextMenu)treeview_animations.Resources["contextmenu_default"];
                currentFrame = null;
                currentAnimation = null;
                dockpanel_edit.Visibility = Visibility.Hidden;
                dockpanel_preview.Visibility = Visibility.Hidden;
            } else if (e.NewValue is VertexAnimation) {
                treeview_animations.ContextMenu = (ContextMenu)treeview_animations.Resources["contextmenu_animation"];
                currentFrame = null;
                currentAnimation = (VertexAnimation)e.NewValue;
                dockpanel_edit.Visibility = Visibility.Hidden;
                dockpanel_preview.Visibility = Visibility.Visible;
            } else if (e.NewValue is VertexAnimationFrame) {
                treeview_animations.ContextMenu = (ContextMenu)treeview_animations.Resources["contextmenu_frame"];
                currentFrame = (VertexAnimationFrame)e.NewValue;
                dockpanel_edit.Visibility = Visibility.Visible;
                dockpanel_preview.Visibility = Visibility.Hidden;
                foreach (string bone in boneImages.Keys) {
                    UpdateBoneImage(bone);
                }
            } else {
                treeview_animations.ContextMenu = null;
            }
        }

        private void treeview_animations_TreeViewItemRightMouseButtonDown (object sender, MouseButtonEventArgs e) {
            TreeViewItem item = sender as TreeViewItem;
            if (item != null) {
                item.Focus( );
                e.Handled = true;
            }
        }

        private void UpdateBoneImage (string key) {
            ResizableImage image = boneImages[key];
            VertexBone bone = currentFrame.State[key];
            image.Width = rectangle_player.Width * bone.Size.X;
            image.Height = rectangle_player.Height * bone.Size.Y;
            image.IsFlipped = bone.Mirrored;
            image.Rotation = bone.Rotation;
            double newleft = Canvas.GetLeft(border_rectangle_player) + rectangle_player.Width / 2d + bone.Position.X * rectangle_player.Width - image.Width / 2d;
            double newtop = Canvas.GetTop(border_rectangle_player) + rectangle_player.Height / 2d + bone.Position.Y * rectangle_player.Height - image.Height / 2d;
            Canvas.SetLeft(image, newleft);
            Canvas.SetTop(image, newtop);
        }

        private void UpdatePositionOfBone (string key) {
            if (currentFrame == null) return;
            ResizableImage image = boneImages[key];
            float newpercentx = (float)(((Canvas.GetLeft(image) + image.Width / 2d) - (Canvas.GetLeft(border_rectangle_player) + rectangle_player.Width / 2d)) / rectangle_player.Width);
            float newpercenty = (float)(((Canvas.GetTop(image) + image.Height / 2d) - (Canvas.GetTop(border_rectangle_player) + rectangle_player.Height / 2d)) / rectangle_player.Height);
            VertexBone current = currentFrame.State[key];
            current.Position = new Vector2(newpercentx, newpercenty);
            currentFrame.State.Remove(key);
            currentFrame.State.Add(key, current);
        }

        private struct AnimationMetaData {
            public List<string> Bones;
            public string Name;
            public double Ratio;
        }

        private struct BoneListBoxItem {

            public BoneListBoxItem (string file) : this( ) {
                if (file.EndsWith("png")) {
                    Image = new BitmapImage(new Uri(file));
                    Name = Path.GetFileNameWithoutExtension(file);
                }
            }

            public BitmapImage Image { get; set; }
            public string Name { get; set; }
        }
    }
}