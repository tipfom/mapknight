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

        public static Dictionary<AnimationControl, Dictionary<string, ImageData>> Data = new Dictionary<AnimationControl, Dictionary<string, ImageData>>( );
        public static event Action BackupChanges;
        public static event Action DumpChanges;

        private static readonly Effect MOVE_EFFECT = new DropShadowEffect( ) { Color = Colors.Cyan, ShadowDepth = 0, Opacity = 1, BlurRadius = 100, RenderingBias = RenderingBias.Performance };
        private static readonly Effect ROTATE_EFFECT = new DropShadowEffect( ) { Color = Colors.Red, ShadowDepth = 0, Opacity = 1, BlurRadius = 100, RenderingBias = RenderingBias.Performance };
        private static readonly Effect NONE_EFFECT = new DropShadowEffect( ) { Color = Colors.White, ShadowDepth = 0, Opacity = 0, BlurRadius = 0, RenderingBias = RenderingBias.Performance };

        public Visibility ResizerVisibility { get; set; } = Visibility.Visible;
        public ImageData Image { get; set; }
        public bool IsFlipped { set { if (value) image.RenderTransform = new ScaleTransform( ) { ScaleX = -1 }; else image.RenderTransform = new ScaleTransform( ) { ScaleX = 1 }; } }
        public float Rotation { get { return (float)((RotateTransform)RenderTransform).Angle; } set { ((RotateTransform)RenderTransform).Angle = value; } }
        public bool CanChangeRenderTransformOrigin { get { return rendertransformoriginthumb.Visibility == Visibility.Visible; } set { rendertransformoriginthumb.Visibility = value ? Visibility.Visible : Visibility.Hidden; } }
        public bool AllowInput = false;

        public Rectangle RefRectangle { get; set; }

        private AnimationControl animControl;
        private VertexBone dataContextBone;

        private BoneImage ( ) {
            InitializeComponent( );

            image.Effect = NONE_EFFECT;

            // set references
            rendertransformoriginthumb.DataContext = this;
            image.DataContext = Image;

            // hook up events
            rendertransformoriginthumb.RenderTransformOriginChanged += BoneImage_RenderTransformOriginChanged;

            rendertransformoriginthumb.DragStarted += Rendertransformoriginthumb_DragStarted;

            DataContextChanged += BoneImage_DataContextChanged;
        }

        private void Rendertransformoriginthumb_DragStarted (object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e) {
            BackupChanges?.Invoke( );
        }

        public BoneImage (AnimationControl animationcontrol) : this( ) {
            if (!Data.ContainsKey(animationcontrol)) Data.Add(animationcontrol, new Dictionary<string, ImageData>( ));
            animControl = animationcontrol;
        }

        private void BoneImage_DataContextChanged (object sender, DependencyPropertyChangedEventArgs e) {
            dataContextBone = (VertexBone)DataContext;
            Update( );
        }

        private void BoneImage_RenderTransformOriginChanged (Point origin) {
            Image.TransformOrigin = origin;
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

        public static void LoadImage (string name, Stream imageStream, Stream dataStream, AnimationControl animationControl, bool leaveOpen) {
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
            LoadImage(path, animControl);

            string name = Path.GetFileNameWithoutExtension(path);
            Image = Data[animControl][name];
            dataContextBone.Image = name;
            image.Source = Image.Image;
            RenderTransformOrigin = Image.TransformOrigin;
        }

        public static void LoadImage (string path, AnimationControl animControl) {
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

        public void SetMoveEffect ( ) {
            image.Effect = MOVE_EFFECT;
        }

        public void SetRotateEffect ( ) {
            image.Effect = ROTATE_EFFECT;
        }

        public void SetNoEffect ( ) {
            image.Effect = NONE_EFFECT;
        }

        public class ImageData {
            public BitmapImage Image;
            public Point TransformOrigin;
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
