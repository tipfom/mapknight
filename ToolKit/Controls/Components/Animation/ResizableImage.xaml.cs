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
        public Visibility ResizerVisibility { get; set; } = Visibility.Visible;
        public bool ChangePositionOnResize { get; set; } = true;
        private BitmapImage _Image;
        private static BitmapImage defaultImage ;
        public BitmapImage Image { get { return _Image ?? defaultImage; } set { _Image = value; image.Source = Image; } }
        public bool IsFlipped { set { if (value) image.RenderTransform = new ScaleTransform( ) { ScaleX = -1 }; else image.RenderTransform = new ScaleTransform( ) { ScaleX = 1 }; } }
        public event Action<ResizableImage> Rotated;
        public float Rotation { get { return (float)((RotateTransform)RenderTransform).Angle; } set { ((RotateTransform)RenderTransform).Angle = value; } }

        public ResizableImage ( ) {
            this.RenderTransform = new RotateTransform( );
            if (defaultImage == null)
                defaultImage = (BitmapImage)App.Current.FindResource("image_resizableimage_error");
            InitializeComponent( );
            movethumb.DataContext = this;
            rotatethumb.DataContext = this;
            rotatethumb.Rotated += ( ) => Rotated?.Invoke(this);
            control_thumbcontainer.DataContext = this;
            image.Source = Image;
        }
    }
}
