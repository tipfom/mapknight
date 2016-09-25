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
        private string defaultSizeBone = null;

        public AnimationControl ( ) {
            InitializeComponent( );
            treeview_animations.DataContext = Animations;
        }

        public AnimationControl (string metafile) : this( ) {
            if (File.Exists(metafile)) {
                string pathtoload = Path.GetDirectoryName(metafile);
                AnimationMetaData metaData = JsonConvert.DeserializeObject<AnimationMetaData>(File.ReadAllText(metafile));
                EntityName = metaData.Name;
                defaultSizeBone = metaData.DefaultBoneName;
                TransformAspectRatio = metaData.Ratio;
                Bones = metaData.Bones;
                foreach (KeyValuePair<string, VertexBone> kvpair in metaData.Bones) {
                    if (File.Exists(Path.Combine(pathtoload, kvpair.Key + ".png"))) {
                        BitmapImage image = new BitmapImage(new Uri(Path.Combine(pathtoload, kvpair.Key + ".png")));

                        ResizableImage defaultBoneImage = new ResizableImage( ) { Image = image, ContextMenu = new ContextMenu( ), CanChangeRenderTransformOrigin = true };
                        defaultBoneImage.ContextMenu = new ContextMenu( ) {
                            DataContext = defaultBoneImage,
                            Items = {
                                new MenuItem() { Header = "Delete", Icon = new Image() { Source = (BitmapImage)App.Current.FindResource("image_animationcomponent_delete") } }
                            }
                        };
                        defaultBoneImage.RenderTransformOriginChanged += (origin) => UpdateOrigin(boneImages[kvpair.Key], origin);
                        UpdateBoneImage(defaultBoneImage, kvpair.Value, rectangle_entity_default, border_rectangle_entity_default);

                        ((MenuItem)defaultBoneImage.ContextMenu.Items[0]).Click += MenuItemDelete_Click;
                        defaultBoneImage.MouseDoubleClick += DefaultBoneImage_MouseDoubleClick;

                        defaultBoneImage.SizeChanged += DefaultBoneImage_SizeChanged;
                        DependencyPropertyDescriptor canvasleftproperty = DependencyPropertyDescriptor.FromProperty(Canvas.LeftProperty, typeof(ResizableImage));
                        canvasleftproperty.AddValueChanged(defaultBoneImage, DefaultBoneImage_CanvasLeftChanged);
                        DependencyPropertyDescriptor canvastopproperty = DependencyPropertyDescriptor.FromProperty(Canvas.TopProperty, typeof(ResizableImage));
                        canvastopproperty.AddValueChanged(defaultBoneImage, DefaultBoneImage_CanvasTopChanged);

                        canvas_bones.Children.Add(defaultBoneImage);
                        Images.Add(kvpair.Key, image);
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
            AnimationMetaData metaData = new AnimationMetaData( ) { Ratio = TransformAspectRatio, Name = EntityName, Bones = Bones, DefaultBoneName = defaultSizeBone };
            if (Directory.Exists(Path.Combine(path, EntityName)))
                Directory.Delete(Path.Combine(path, EntityName));
            Directory.CreateDirectory(Path.Combine(path, EntityName));
            foreach (KeyValuePair<string, BitmapImage> kvpair in Images) {
                PngBitmapEncoder encoder = new PngBitmapEncoder( );
                encoder.Frames.Add(BitmapFrame.Create(kvpair.Value));
                using (Stream fileStream = File.Create(Path.Combine(path, EntityName, kvpair.Key + ".png")))
                    encoder.Save(fileStream);
            }
            File.Create(Path.Combine(path, EntityName, "animation.meta")).Close( );
            File.WriteAllText(Path.Combine(path, EntityName, "animation.meta"), JsonConvert.SerializeObject(metaData));
            File.Create(Path.Combine(path, EntityName, "animation.json")).Close( );
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
                if (rectangle_player.Height > canvas_frame.RenderSize.Height) {
                    rectangle_player.Height = canvas_frame.RenderSize.Height;
                    rectangle_player.Width = canvas_frame.RenderSize.Height * TransformAspectRatio;
                }
                rectangle_entity_default.Width = canvas_bones.RenderSize.Width;
                rectangle_entity_default.Height = canvas_bones.RenderSize.Width / TransformAspectRatio;
                if (rectangle_entity_default.Height > canvas_bones.RenderSize.Height) {
                    rectangle_entity_default.Height = canvas_bones.RenderSize.Height;
                    rectangle_entity_default.Width = canvas_bones.RenderSize.Height * TransformAspectRatio;
                }
            } else {
                // height > width
                rectangle_player.Height = greaterSizeEditorUsePercent[(int)slider_zoom.Value] * canvas_frame.RenderSize.Height;
                rectangle_player.Width = TransformAspectRatio * rectangle_player.Height;
                if (rectangle_player.Width > canvas_frame.RenderSize.Width) {
                    rectangle_player.Width = canvas_frame.RenderSize.Width;
                    rectangle_player.Height = canvas_frame.RenderSize.Width / TransformAspectRatio;
                }
                rectangle_entity_default.Height = canvas_bones.RenderSize.Height;
                rectangle_entity_default.Width = canvas_bones.RenderSize.Height * TransformAspectRatio;
                if (rectangle_entity_default.Width > canvas_bones.RenderSize.Width) {
                    rectangle_entity_default.Width = canvas_bones.RenderSize.Width;
                    rectangle_entity_default.Height = canvas_bones.RenderSize.Width / TransformAspectRatio;
                }
            }
            Canvas.SetLeft(border_rectangle_player, (canvas_frame.RenderSize.Width - rectangle_player.Width) / 2d);
            Canvas.SetTop(border_rectangle_player, (canvas_frame.RenderSize.Height - rectangle_player.Height) / 2d);
            Canvas.SetLeft(border_rectangle_entity_default, (canvas_bones.RenderSize.Width - rectangle_entity_default.Width) / 2d);
            Canvas.SetTop(border_rectangle_entity_default, (canvas_bones.RenderSize.Height - rectangle_entity_default.Height) / 2d);

            foreach (UIElement element in canvas_bones.Children) {
                ResizableImage image = element as ResizableImage;
                if (image != null) {
                    UpdateBoneImage(image, Bones[Path.GetFileNameWithoutExtension(image.Image.UriSource.AbsolutePath)], rectangle_entity_default, border_rectangle_entity_default);
                }
            }

            if (currentFrame == null)
                return;
            foreach (KeyValuePair<string, ResizableImage> kvpair in boneImages) {
                UpdateBoneImage(kvpair.Value, currentFrame.State[kvpair.Key], rectangle_player, border_rectangle_player);
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
            currentFrame.State[key] = bone;
        }

        private void BoneImage_SizeChanged (object sender, SizeChangedEventArgs e) {
            if (currentFrame == null) return;
            ResizableImage image = (ResizableImage)sender;
            float newsizex = (float)(image.Width / rectangle_player.Width);
            float newsizey = (float)(image.Height / rectangle_player.Height);
            float newpercentx = (float)(((Canvas.GetLeft(image) + image.Width / 2d) - (Canvas.GetLeft(border_rectangle_player) + rectangle_player.Width / 2d)) / rectangle_player.Width);
            float newpercenty = -(float)(((Canvas.GetTop(image) + image.Height / 2d) - (Canvas.GetTop(border_rectangle_player) + rectangle_player.Height / 2d)) / rectangle_player.Height);
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
                    image = new ResizableImage( ) { CanChangeRenderTransformOrigin = false };
                    image.Rotated += BoneImage_Rotated;
                    image.MouseDoubleClick += BoneImage_MouseDoubleClick;
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

        private void BoneImage_MouseDoubleClick (object sender, MouseButtonEventArgs e) {
            string key = boneImages.First(pair => pair.Value == (ResizableImage)sender).Key;
            if (currentFrame == null) return;
            VertexBone bone = currentFrame.State[key];
            bone.Mirrored = !bone.Mirrored;
            currentFrame.State[key] = bone;
            UpdateBoneImage(boneImages[key], currentFrame.State[key], rectangle_player, border_rectangle_player);
        }

        private void ButtonPausePlay_Click (object sender, RoutedEventArgs e) {
            animationview.Pause( );
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
            if (currentFrame == null) return;
            string[ ] keys = Bones.Keys.ToArray( );
            for (int i = 0; i < keys.Length; i++) {
                string bone = keys[i];
                currentFrame.State[bone] = new VertexBone( ) { Mirrored = Bones[bone].Mirrored, Position = Bones[bone].Position, Rotation = Bones[bone].Rotation, Size = Bones[bone].Size };
                UpdateBoneImage(boneImages[bone], currentFrame.State[bone], rectangle_player, border_rectangle_player);
            }
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
                    UpdateBoneImage(boneImages[bone], currentFrame.State[bone], rectangle_player, border_rectangle_player);
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

        private void UpdateBoneImage (ResizableImage image, VertexBone bone, System.Windows.Shapes.Rectangle rect, Border border) {
            image.Width = rect.Width * bone.Size.X;
            image.Height = rect.Height * bone.Size.Y;
            image.IsFlipped = bone.Mirrored;
            image.Rotation = bone.Rotation;
            double newleft = Canvas.GetLeft(border) + rect.Width / 2d + bone.Position.X * rect.Width - image.Width / 2d;
            double newtop = Canvas.GetTop(border) + rect.Height / 2d - bone.Position.Y * rect.Height - image.Height / 2d;
            Canvas.SetLeft(image, newleft);
            Canvas.SetTop(image, newtop);
        }

        private void UpdatePositionOfBone (string key) {
            if (currentFrame == null) return;
            ResizableImage image = boneImages[key];
            float newpercentx = (float)(((Canvas.GetLeft(image) + image.Width / 2d) - (Canvas.GetLeft(border_rectangle_player) + rectangle_player.Width / 2d)) / rectangle_player.Width);
            float newpercenty = -(float)(((Canvas.GetTop(image) + image.Height / 2d) - (Canvas.GetTop(border_rectangle_player) + rectangle_player.Height / 2d)) / rectangle_player.Height);
            VertexBone current = currentFrame.State[key];
            current.Position = new Vector2(newpercentx, newpercenty);
            currentFrame.State.Remove(key);
            currentFrame.State.Add(key, current);
        }

        private struct AnimationMetaData {
            public string Name;
            public string DefaultAnimation;
            public double Ratio;
            public Dictionary<string, VertexBone> Bones;
            public string DefaultBoneName;
        }

        private void canvas_bones_DragEnter (object sender, DragEventArgs e) {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) {
                e.Effects = DragDropEffects.Copy;
            }
        }

        private void canvas_bones_Drop (object sender, DragEventArgs e) {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) {
                string[ ] files = (string[ ])e.Data.GetData(DataFormats.FileDrop);
                foreach (string file in files) {
                    if (file.EndsWith("png") && !Bones.ContainsKey(Path.GetFileNameWithoutExtension(file))) {
                        string name = Path.GetFileNameWithoutExtension(file);
                        BitmapImage image = new BitmapImage(new Uri(file));

                        ResizableImage defaultBoneImage = new ResizableImage( ) { Image = image, ContextMenu = new ContextMenu( ), CanChangeRenderTransformOrigin = true };
                        defaultBoneImage.ContextMenu = new ContextMenu( ) {
                            DataContext = defaultBoneImage,
                            Items = {
                                new MenuItem() { Header = "Delete", Icon = new Image() { Source = (BitmapImage)App.Current.FindResource("image_animationcomponent_delete") } }
                            }
                        };
                        defaultBoneImage.RenderTransformOriginChanged += (origin) => UpdateOrigin(boneImages[name], origin);
                        ((MenuItem)defaultBoneImage.ContextMenu.Items[0]).Click += MenuItemDelete_Click;
                        defaultBoneImage.MouseDoubleClick += DefaultBoneImage_MouseDoubleClick;

                        defaultBoneImage.SizeChanged += DefaultBoneImage_SizeChanged;
                        DependencyPropertyDescriptor canvasleftproperty = DependencyPropertyDescriptor.FromProperty(Canvas.LeftProperty, typeof(ResizableImage));
                        canvasleftproperty.AddValueChanged(defaultBoneImage, DefaultBoneImage_CanvasLeftChanged);
                        DependencyPropertyDescriptor canvastopproperty = DependencyPropertyDescriptor.FromProperty(Canvas.TopProperty, typeof(ResizableImage));
                        canvastopproperty.AddValueChanged(defaultBoneImage, DefaultBoneImage_CanvasTopChanged);

                        canvas_bones.Children.Add(defaultBoneImage);
                        Bones.Add(name, new VertexBone( ) { Mirrored = false, Position = new Vector2(0, 0), Rotation = 0, Size = new Vector2(0.25f, 0.25f) });
                        _Images.Add(name, image);

                        Canvas.SetTop(defaultBoneImage, 0);
                        Canvas.SetLeft(defaultBoneImage, 0);
                        if (!string.IsNullOrEmpty(defaultSizeBone)) {
                            Vector2 percentPerPixel = new Vector2((float)(Bones[defaultSizeBone].Size.X / Images[defaultSizeBone].Width), (float)(Bones[defaultSizeBone].Size.Y / Images[defaultSizeBone].Height));
                            defaultBoneImage.Width = percentPerPixel.X * image.Width * rectangle_entity_default.Width;
                            defaultBoneImage.Height = percentPerPixel.Y * image.Height * rectangle_entity_default.Height;
                        } else {
                            defaultBoneImage.Width = 100;
                            defaultBoneImage.Height = 100 * image.Height / image.Width;
                        }
                        Canvas.SetLeft(defaultBoneImage, (canvas_bones.RenderSize.Width - defaultBoneImage.Width) / 2d);
                        Canvas.SetTop(defaultBoneImage, (canvas_bones.RenderSize.Height - defaultBoneImage.Width) / 2d);
                        BonesChanged( );
                    }
                }
            }
        }

        private void DefaultBoneImage_MouseDoubleClick (object sender, MouseButtonEventArgs e) {
            defaultSizeBone = Path.GetFileNameWithoutExtension(((ResizableImage)sender).Image.UriSource.AbsolutePath);
            UpdateDefaultSizes( );
        }

        private void UpdateDefaultSizes ( ) {
            if (defaultSizeBone == null) return;

            // rescale bones
            Vector2 percentPerPixel = new Vector2((float)(Bones[defaultSizeBone].Size.X / Images[defaultSizeBone].Width), (float)(Bones[defaultSizeBone].Size.Y / Images[defaultSizeBone].Height));
            foreach (string bone in Images.Keys) {
                if (bone == defaultSizeBone) continue;
                Bones[bone] = new VertexBone( ) {
                    Position = Bones[bone].Position,
                    Mirrored = Bones[bone].Mirrored,
                    Rotation = Bones[bone].Rotation,
                    Size = new Vector2(
                        (float)Images[bone].Width * percentPerPixel.X,
                        (float)Images[bone].Height * percentPerPixel.Y)
                };
            }

            // apply changes to images
            foreach (UIElement element in canvas_bones.Children) {
                ResizableImage image = element as ResizableImage;
                if (image != null) {
                    string bone = Path.GetFileNameWithoutExtension(image.Image.UriSource.AbsolutePath);
                    if (bone != defaultSizeBone) {
                        image.Width = rectangle_entity_default.Width * Bones[bone].Size.X;
                        image.Height = rectangle_entity_default.Height * Bones[bone].Size.Y;
                        // Canvas.SetLeft(image, Canvas.GetLeft(border_rectangle_entity_default) + rectangle_entity_default.Width * (Bones[bone].Position.X + 0.5f) - image.Width / 2f);
                        // Canvas.SetTop(image, Canvas.GetTop(border_rectangle_entity_default) + rectangle_entity_default.Height * (Bones[bone].Position.Y + 0.5f) - image.Height / 2f);
                    }
                }
            }
        }

        private void DefaultBoneImage_CanvasLeftChanged (object sender, EventArgs e) {
            UpdatePositionOfDefaultBone((ResizableImage)sender);
        }

        private void DefaultBoneImage_CanvasTopChanged (object sender, EventArgs e) {
            UpdatePositionOfDefaultBone((ResizableImage)sender);
        }

        private void UpdatePositionOfDefaultBone (ResizableImage image) {
            if (double.IsNaN(Canvas.GetLeft(image)) || double.IsNaN(Canvas.GetTop(image))) return;
            string bone = Path.GetFileNameWithoutExtension(image.Image.UriSource.AbsolutePath);
            float newpercentx = (float)(((Canvas.GetLeft(image) + image.Width / 2d) - (Canvas.GetLeft(border_rectangle_entity_default) + rectangle_entity_default.Width / 2d)) / rectangle_entity_default.Width);
            float newpercenty = -(float)(((Canvas.GetTop(image) + image.Height / 2d) - (Canvas.GetTop(border_rectangle_entity_default) + rectangle_entity_default.Height / 2d)) / rectangle_entity_default.Height);
            VertexBone current = Bones[bone];
            current.Position = new Vector2(newpercentx, newpercenty);
            Bones[bone] = current;
        }

        private void DefaultBoneImage_SizeChanged (object sender, SizeChangedEventArgs e) {
            ResizableImage image = (ResizableImage)sender;
            if (double.IsNaN(Canvas.GetLeft(image)) || double.IsNaN(Canvas.GetTop(image))) return;

            string bone = Path.GetFileNameWithoutExtension(image.Image.UriSource.AbsolutePath);
            float newsizex = (float)(image.Width / rectangle_entity_default.Width);
            float newsizey = (float)(image.Height / rectangle_entity_default.Height);
            float newpercentx = (float)(((Canvas.GetLeft(image) + image.Width / 2d) - (Canvas.GetLeft(border_rectangle_entity_default) + rectangle_entity_default.Width / 2d)) / rectangle_entity_default.Width);
            float newpercenty = -(float)(((Canvas.GetTop(image) + image.Height / 2d) - (Canvas.GetTop(border_rectangle_entity_default) + rectangle_entity_default.Height / 2d)) / rectangle_entity_default.Height);

            VertexBone current = Bones[bone];
            current.Size = new Vector2(newsizex, newsizey);
            current.Position = new Vector2(newpercentx, newpercenty);
            Bones[bone] = current;
        }

        private void MenuItemDelete_Click (object sender, RoutedEventArgs e) {
            ResizableImage image = ((sender as Control).Parent as Control).DataContext as ResizableImage;
            if (image != null) {
                canvas_bones.Children.Remove(image);
                string bonename = Path.GetFileNameWithoutExtension(image.Image.UriSource.AbsolutePath);
                _Images.Remove(bonename);
                Bones.Remove(bonename);
                BonesChanged( );
            }
        }

        private void canvas_bones_SizeChanged (object sender, SizeChangedEventArgs e) {
            AdjustEditor( );
        }

        private void treeview_animations_MouseRightButtonDown (object sender, MouseButtonEventArgs e) {
            treeview_animations.Focus( );
        }

        private void UpdateOrigin (ResizableImage image, Point origin) {
            image.RenderTransformOrigin = origin;
        }
    }
}