using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace mapKnight.ToolKit.Controls.Components.Animation {
    /// <summary>
    /// Interaktionslogik für ResizableImage.xaml
    /// </summary>
    public partial class ResizableImage : ContentControl {
        public event Action<Point> RenderTransformOriginChanged;

        public Visibility ResizerVisibility { get; set; } = Visibility.Visible;
        public bool ChangePositionOnResize { get; set; } = true;
        private BitmapImage _Image;
        private static BitmapImage defaultImage;
        public BitmapImage Image { get { return _Image ?? defaultImage; } set { _Image = value; image.Source = Image; } }
        public bool IsFlipped { set { if (value) image.RenderTransform = new ScaleTransform( ) { ScaleX = -1 }; else image.RenderTransform = new ScaleTransform( ) { ScaleX = 1 }; } }
        public event Action<ResizableImage> Rotated;
        public float Rotation { get { return (float)((RotateTransform)RenderTransform).Angle; } set { ((RotateTransform)RenderTransform).Angle = value; } }
        public bool CanChangeRenderTransformOrigin { get { return rendertransformoriginthumb.Visibility == Visibility.Visible; } set { rendertransformoriginthumb.Visibility = value ? Visibility.Visible : Visibility.Hidden; } }

        public ResizableImage ( ) {
            RenderTransform = new RotateTransform( );
            if (defaultImage == null)
                defaultImage = (BitmapImage)App.Current.FindResource("image_resizableimage_error");
            InitializeComponent( );
            movethumb.DataContext = this;
            rotatethumb.DataContext = this;
            rotatethumb.Rotated += ( ) => Rotated?.Invoke(this);
            rendertransformoriginthumb.DataContext = this;
            rendertransformoriginthumb.RenderTransformOriginChanged += (origin) => RenderTransformOriginChanged?.Invoke(origin);
            control_thumbcontainer.DataContext = this;
            image.Source = Image;
        }
    }
}
