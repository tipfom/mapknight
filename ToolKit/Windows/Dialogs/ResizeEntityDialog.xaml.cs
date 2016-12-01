using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using mapKnight.ToolKit.Controls;
using mapKnight.ToolKit.Controls.Components.Animation;
using mapKnight.ToolKit.Data;
using static mapKnight.ToolKit.Controls.Components.Animation.BoneImage;

namespace mapKnight.ToolKit.Windows.Dialogs {
    /// <summary>
    /// Interaktionslogik für ResizeEntityDialog.xaml
    /// </summary>
    public partial class ResizeEntityDialog : Window {
        public double TrimLeft, TrimRight, TrimTop, TrimBottom;

        public ResizeEntityDialog ( ) {
            InitializeComponent( );
        }

        public ResizeEntityDialog (double ratio, IList<VertexBone> bones, AnimationControl2 parent) : this( ) {
            if (ratio < 1) {
                // height > width
                rectangle_entity.Height = canvas.Height / 2d;
                rectangle_entity.Width = rectangle_entity.Height * ratio;
            } else {
                rectangle_entity.Width = canvas.Width / 2d;
                rectangle_entity.Height = rectangle_entity.Width / ratio;
            }
            Canvas.SetTop(rectangle_entity, (canvas.Height - rectangle_entity.Height) / 2d);
            Canvas.SetLeft(rectangle_entity, (canvas.Width - rectangle_entity.Width) / 2d);

            for (int i = 0; i < bones.Count; i++) {
                VertexBone bone = bones[i];
                Image image = new Image( );
                ImageData data = BoneImage.Data[parent][Path.GetFileNameWithoutExtension(bone.Image)];
                image.Source = data.Image;
                image.Width = data.Image.PixelWidth * bone.Scale * rectangle_entity.Width;
                image.Height = data.Image.PixelHeight * bone.Scale * rectangle_entity.Width;
                image.RenderTransform = new TransformGroup( ) { Children = { new RotateTransform( ) { Angle = bone.Rotation }, new ScaleTransform(bone.Mirrored ? -1 : 1, 1) } };
                image.RenderTransformOrigin = data.TransformOrigin;
                RenderOptions.SetBitmapScalingMode(this, BitmapScalingMode.NearestNeighbor);

                canvas.Children.Add(image);
                Canvas.SetLeft(image, Canvas.GetLeft(rectangle_entity) + rectangle_entity.Width * (0.5 + bone.Position.X) - image.Width * image.RenderTransformOrigin.X);
                Canvas.SetTop(image, Canvas.GetTop(rectangle_entity) + rectangle_entity.Height * (0.5 - bone.Position.Y) - image.Height * image.RenderTransformOrigin.Y);
                Canvas.SetZIndex(image, -i);
            }

            Canvas.SetLeft(thumb_left, Canvas.GetLeft(rectangle_entity));
            Canvas.SetLeft(thumb_right, Canvas.GetLeft(rectangle_entity) + rectangle_entity.Width);
            Canvas.SetTop(thumb_top, Canvas.GetTop(rectangle_entity));
            Canvas.SetTop(thumb_bottom, Canvas.GetTop(rectangle_entity) + rectangle_entity.Height);
        }

        private void thumb_left_DragDelta (object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e) {
            Canvas.SetLeft(thumb_left, Math.Max(0, Math.Min(canvas.ActualWidth, Canvas.GetLeft(thumb_left) + e.HorizontalChange)));
            App.Current.MainWindow.Title = Canvas.GetLeft(thumb_left) + " " + Canvas.GetLeft(rectangle_entity);
        }

        private void thumb_top_DragDelta (object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e) {
            Canvas.SetTop(thumb_top, Math.Max(0, Math.Min(canvas.ActualHeight, Canvas.GetTop(thumb_top) + e.VerticalChange)));
            App.Current.MainWindow.Title = Canvas.GetTop(thumb_top) + " " + Canvas.GetTop(rectangle_entity);
        }

        private void thumb_right_DragDelta (object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e) {
            Canvas.SetLeft(thumb_right, Math.Max(0, Math.Min(canvas.ActualWidth, Canvas.GetLeft(thumb_right) + e.HorizontalChange)));
            App.Current.MainWindow.Title = Canvas.GetLeft(thumb_right) + " " + (Canvas.GetLeft(rectangle_entity) + rectangle_entity.Width);
        }

        private void thumb_bottom_DragDelta (object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e) {
            Canvas.SetTop(thumb_bottom, Math.Max(0, Math.Min(canvas.ActualHeight, Canvas.GetTop(thumb_bottom) + e.VerticalChange)));
            App.Current.MainWindow.Title = Canvas.GetTop(thumb_bottom) + " " + (Canvas.GetTop(rectangle_entity) + rectangle_entity.Height);
        }

        private void Image_MouseDown (object sender, System.Windows.Input.MouseButtonEventArgs e) {
            DialogResult = true;
            // positive when expanded, and negative when area is removed
            TrimLeft = (Canvas.GetLeft(rectangle_entity) - Canvas.GetLeft(thumb_left)) / rectangle_entity.Width;
            TrimRight = (Canvas.GetLeft(thumb_right) - rectangle_entity.Width - Canvas.GetLeft(rectangle_entity)) / rectangle_entity.Width;
            TrimTop = (Canvas.GetTop(rectangle_entity) - Canvas.GetTop(thumb_top)) / rectangle_entity.Height;
            TrimBottom = (Canvas.GetTop(thumb_bottom) - rectangle_entity.Height - Canvas.GetTop(rectangle_entity)) / rectangle_entity.Height;
            Close( );
        }
    }
}
