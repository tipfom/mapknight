using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using mapKnight.Core;
using mapKnight.ToolKit.Data;
using Newtonsoft.Json;
using Rectangle = System.Windows.Shapes.Rectangle;

namespace mapKnight.ToolKit.Controls.Animation {
    public partial class BoneImage : ContentControl {
        public enum HitResult {
            NoHit,
            VisualChildHit,
            Hit,
        }

        private static readonly Effect MOVE_EFFECT = new DropShadowEffect( ) { Color = Colors.Cyan, ShadowDepth = 0, Opacity = 1, BlurRadius = 100, RenderingBias = RenderingBias.Performance };
        private static readonly Effect ROTATE_EFFECT = new DropShadowEffect( ) { Color = Colors.Red, ShadowDepth = 0, Opacity = 1, BlurRadius = 100, RenderingBias = RenderingBias.Performance };
        private static readonly Effect NO_EFFECT = new DropShadowEffect( ) { Color = Colors.White, ShadowDepth = 0, Opacity = 0, BlurRadius = 0, RenderingBias = RenderingBias.Performance };

        public static event Action BackupChanges;
        public static event Action DumpChanges;
        
        public ImageData Image { get; set; }
        public bool IsFlipped { set { if (value) image.RenderTransform = new ScaleTransform( ) { ScaleX = -1 }; else image.RenderTransform = new ScaleTransform( ) { ScaleX = 1 }; } }
        public float Rotation { get { return (float)((RotateTransform)RenderTransform).Angle; } set { ((RotateTransform)RenderTransform).Angle = value; } }

        public Rectangle RefRectangle { get; set; }

        private VertexAnimationData data;
        private VertexBone dataContextBone;

        private BoneImage ( ) {
            InitializeComponent( );

            image.Effect = NO_EFFECT;

            // set references
            rendertransformoriginthumb.DataContext = this;
            image.DataContext = Image;

            // hook up events
            rendertransformoriginthumb.RenderTransformOriginChanged += (origin) => Image.TransformOrigin = origin;

            rendertransformoriginthumb.DragStarted += (sender, e) => BackupChanges?.Invoke( ); ;

            DataContextChanged += BoneImage_DataContextChanged;
        }

        public BoneImage (VertexAnimationData data) : this( ) {
            this.data = data;
        }

        private void BoneImage_DataContextChanged (object sender, DependencyPropertyChangedEventArgs e) {
            dataContextBone = (VertexBone)DataContext;
            Update( );
        }

        public void PositionOrRotationChangeBegan ( ) {
            BackupChanges?.Invoke( );
        }

        public void PositionChangeCompleted ( ) {
            if (RefRectangle.Width == 0 || RefRectangle.Height == 0) return;

            Vector position = new Vector(
                 ((Canvas.GetLeft(this) + Width * RenderTransformOrigin.X) - (Canvas.GetLeft(RefRectangle) + RefRectangle.Width / 2d)) / RefRectangle.Width,
                -((Canvas.GetTop(this) + Height * RenderTransformOrigin.Y) - (Canvas.GetTop(RefRectangle) + RefRectangle.Height / 2d)) / RefRectangle.Height);

            if (double.IsNaN(position.X) || double.IsNaN(position.Y)) return;
            if((float)position.X != dataContextBone.Position.X ||(float)position.Y != dataContextBone.Position.Y) {
                dataContextBone.Position = new Vector2((float)position.X, (float)position.Y);
            } else {
                DumpChanges?.Invoke( );
            }
        }

        public void RotationChangeCompleted (double rotation) {
            dataContextBone.Rotation = (float)rotation;
        }

        public void Update ( ) {
            ApplyImage(dataContextBone.Image);
            if (RefRectangle.Width == 0 || RefRectangle.Height == 0) return;

            Image = data.Images[dataContextBone.Image];
            Width = RefRectangle.Width * dataContextBone.Scale * Image.Image.PixelWidth;
            Height = RefRectangle.Width * dataContextBone.Scale * Image.Image.PixelHeight;
            IsFlipped = dataContextBone.Mirrored;
            Rotation = dataContextBone.Rotation;

            double left = Canvas.GetLeft(RefRectangle) + RefRectangle.Width / 2d + dataContextBone.Position.X * RefRectangle.Width - Width * RenderTransformOrigin.X;
            double top = Canvas.GetTop(RefRectangle) + RefRectangle.Height / 2d - dataContextBone.Position.Y * RefRectangle.Height - Height * RenderTransformOrigin.Y;
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
            string name = Path.GetFileNameWithoutExtension(path);
            Image = data.Images[name];
            dataContextBone.Image = name;
            image.Source = Image.Image;
            RenderTransformOrigin = Image.TransformOrigin;
        }

        public void SetMoveEffect ( ) {
            image.Effect = MOVE_EFFECT;
        }

        public void SetRotateEffect ( ) {
            image.Effect = ROTATE_EFFECT;
        }

        public void SetNoEffect ( ) {
            image.Effect = NO_EFFECT;
        }

        public HitResult EvaluateHit (Point position) {
            Point rendertransformoriginthumbposition = rendertransformoriginthumb.PointFromScreen(this.PointToScreen(position));
            if (Math.Abs(rendertransformoriginthumbposition.X) < rendertransformoriginthumb.RenderSize.Width &&
                Math.Abs(rendertransformoriginthumbposition.Y) < rendertransformoriginthumb.RenderSize.Height) return HitResult.VisualChildHit;

            if (image.Source == null) return HitResult.NoHit;
             var source = (BitmapSource)image.Source;

            // Get the pixel of the source that was hit
            double x = position.X / ActualWidth * source.PixelWidth;
            double y = position.Y / ActualHeight * source.PixelHeight;

            if (x < 0 || y < 0 || x >= source.PixelWidth || y >= source.PixelHeight) return HitResult.NoHit;

            // Copy the single pixel into a new byte array representing RGBA
            var pixel = new byte[4];
            source.CopyPixels(new Int32Rect((int)Math.Floor(x), (int)Math.Floor(y), 1, 1), pixel, 4, 0);

            // Check the alpha (transparency) of the pixel
            // - threshold can be adjusted from 0 to 255
            return pixel[3] > 10 ? HitResult.Hit : HitResult.NoHit;
        }
    }
}
