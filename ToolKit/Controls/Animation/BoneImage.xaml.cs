using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using mapKnight.Core;
using mapKnight.ToolKit.Data;
using Newtonsoft.Json;
using Rectangle = System.Windows.Shapes.Rectangle;

namespace mapKnight.ToolKit.Controls.Components.Animation {
    public partial class BoneImage : ContentControl {
        public static Dictionary<AnimationControl2, Dictionary<string, ImageData>> Data = new Dictionary<AnimationControl2, Dictionary<string, ImageData>>( );

        public Visibility ResizerVisibility { get; set; } = Visibility.Visible;
        public ImageData Image { get; set; }
        public bool IsFlipped { set { if (value) image.RenderTransform = new ScaleTransform( ) { ScaleX = -1 }; else image.RenderTransform = new ScaleTransform( ) { ScaleX = 1 }; } }
        public float Rotation { get { return (float)((RotateTransform)RenderTransform).Angle; } set { ((RotateTransform)RenderTransform).Angle = value; } }
        public bool CanChangeRenderTransformOrigin { get { return rendertransformoriginthumb.Visibility == Visibility.Visible; } set { rendertransformoriginthumb.Visibility = value ? Visibility.Visible : Visibility.Hidden; } }

        public Rectangle RefRectangle { get; set; }
        public Border RefBorder { get; set; }

        private AnimationControl2 animControl;
        private VertexBone dataContextBone;

        private BoneImage ( ) {
            InitializeComponent( );

            // set references
            movethumb.DataContext = this;
            rotatethumb.DataContext = this;
            rendertransformoriginthumb.DataContext = this;
            image.DataContext = Image;

            // hook up events
            rendertransformoriginthumb.RenderTransformOriginChanged += BoneImage_RenderTransformOriginChanged;
            rotatethumb.Rotated += BoneImage_Rotated;
            DependencyPropertyDescriptor canvasLeftPropertyDesc = DependencyPropertyDescriptor.FromProperty(Canvas.LeftProperty, typeof(BoneImage));
            canvasLeftPropertyDesc.AddValueChanged(this, BoneImage_PositionChanged);
            DependencyPropertyDescriptor canvasTopPropertyDesc = DependencyPropertyDescriptor.FromProperty(Canvas.TopProperty, typeof(BoneImage));
            canvasTopPropertyDesc.AddValueChanged(this, BoneImage_PositionChanged);

            DataContextChanged += BoneImage_DataContextChanged;
        }

        public BoneImage (AnimationControl2 animationcontrol) : this( ) {
            if (!Data.ContainsKey(animationcontrol)) Data.Add(animationcontrol, new Dictionary<string, ImageData>( ));
            animControl = animationcontrol;
        }

        private void BoneImage_DataContextChanged (object sender, DependencyPropertyChangedEventArgs e) {
            dataContextBone = (VertexBone)DataContext;
            Update( );
        }

        private void BoneImage_Rotated ( ) {
            dataContextBone.Rotation = Rotation;
        }

        private void BoneImage_PositionChanged (object sender, EventArgs e) {
            RecalculatePosition( );
        }

        private void BoneImage_RenderTransformOriginChanged (Point origin) {
            Image.TransformOrigin = origin;
        }

        private void RecalculatePosition ( ) {
            if (RefRectangle.Width == 0 || RefRectangle.Height == 0) return;

            Vector position = new Vector(
                 ((Canvas.GetLeft(this) + Width / 2d) - (Canvas.GetLeft(RefBorder) + RefRectangle.Width / 2d)) / RefRectangle.Width,
                -((Canvas.GetTop(this) + Height / 2d) - (Canvas.GetTop(RefBorder) + RefRectangle.Height / 2d)) / RefRectangle.Height);

            if (double.IsNaN(position.X) || double.IsNaN(position.Y)) return;
            dataContextBone.Position = new Vector2((float)position.X, (float)position.Y);
        }

        public static void LoadImage (string name, Stream imageStream, Stream dataStream, AnimationControl2 animationControl, bool leaveOpen) {
            BitmapImage image = new BitmapImage( );
            image.BeginInit( );
            image.StreamSource = imageStream;
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.EndInit( );

            Point transformOrigin = JsonConvert.DeserializeObject<Point>(new StreamReader(dataStream).ReadToEnd( ));

            if (!Data.ContainsKey(animationControl)) Data.Add(animationControl, new Dictionary<string, ImageData>( ));

            if (Data[animationControl].ContainsKey(name)) {
                Data[animationControl][name] = new ImageData( ) { Image = image, TransformOrigin = transformOrigin };
            } else {
                Data[animationControl].Add(name, new ImageData( ) { Image = image, TransformOrigin = transformOrigin });
            }

            if (!leaveOpen) {
                imageStream.Close( );
                dataStream.Close( );
            }
        }

        public void Update ( ) {
            ApplyImage(dataContextBone.Image);
            if (RefRectangle.Width == 0 || RefRectangle.Height == 0) return;

            Image = Data[animControl][dataContextBone.Image];
            Width = RefRectangle.Width * dataContextBone.Scale * Image.Image.PixelWidth;
            Height = RefRectangle.Height * dataContextBone.Scale * Image.Image.PixelHeight;
            IsFlipped = dataContextBone.Mirrored;
            Rotation = dataContextBone.Rotation;

            double left = Canvas.GetLeft(RefBorder) + RefRectangle.Width / 2d + dataContextBone.Position.X * RefRectangle.Width - Width / 2d;
            double top = Canvas.GetTop(RefBorder) + RefRectangle.Height / 2d - dataContextBone.Position.Y * RefRectangle.Height - Height / 2d;
            Canvas.SetLeft(this, left);
            Canvas.SetTop(this, top);
        }

        private void BoneImage_DragEnter (object sender, DragEventArgs e) {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effects = DragDropEffects.Copy;
        }

        private void BoneImage_Drop (object sender, DragEventArgs e) {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) {
                string[ ] files = (string[ ])e.Data.GetData(DataFormats.FileDrop);
                foreach (string file in files) {
                    if (Path.GetExtension(file) == ".png") {
                        ApplyImage(file);
                        Update( );
                        break;
                    }
                }
            }
        }

        private void ApplyImage (string path) {
            LoadImage(path, animControl);

            string name = Path.GetFileNameWithoutExtension(path);
            Image = Data[animControl][name];
            dataContextBone.Image = name;
            image.Source = Image.Image;
            RenderTransformOrigin = Image.TransformOrigin;
        }

        public static void LoadImage (string path, AnimationControl2 animControl) {
            string name = Path.GetFileNameWithoutExtension(path);
            if (!Data.ContainsKey(animControl)) Data.Add(animControl, new Dictionary<string, ImageData>( ));
            if (!Data[animControl].ContainsKey(name)) {
                BitmapImage loadedImage = new BitmapImage( );
                loadedImage.BeginInit( );
                loadedImage.UriSource = new Uri(path);
                loadedImage.CacheOption = BitmapCacheOption.OnLoad;
                loadedImage.EndInit( );

                Data[animControl].Add(name, new ImageData( ) { TransformOrigin = new Point(0.5, 0.5), Image = loadedImage });
            }
        }

        public class ImageData {
            public BitmapImage Image;
            public Point TransformOrigin;
        }
    }
}
