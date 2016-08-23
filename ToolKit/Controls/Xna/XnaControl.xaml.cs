using Microsoft.Xna.Framework.Graphics;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Threading;
using Color = Microsoft.Xna.Framework.Color;

namespace mapKnight.ToolKit.Controls.Xna {
    /// <summary>
    /// Interaktionslogik für XnaControl.xaml
    /// </summary>
    public partial class XnaControl : UserControl {
        public GraphicsDeviceService GraphicsService { get; private set; }
        private ImageSource imageSource;

        /// <summary>
        /// Gets the GraphicsDevice behind the control.
        /// </summary>
        public GraphicsDevice GraphicsDevice {
            get { return GraphicsService.GraphicsDevice; }
        }

        public SpriteBatch SpriteBatch {
            get; private set;
        }

        public event Action DeviceInitialized;

        public new Color Background { get; protected set; } = Color.White;
        private DispatcherTimer resizeTimer = new DispatcherTimer( ) { Interval = new TimeSpan(100), IsEnabled = false };

        public XnaControl ( ) {
            InitializeComponent( );

            // hook up an event to fire when the control has finished loading
            Loaded += new RoutedEventHandler(XnaControl_Loaded);
            this.IsVisibleChanged += (sender, e) => { if ((bool)e.NewValue && Visibility == Visibility.Visible) DoStartUp( ); };
            resizeTimer.Tick += ResizeTimer_Tick;
        }

        ~XnaControl ( ) {
            imageSource?.Dispose( );

            // release on finalizer to clean up the graphics device
            if (GraphicsService != null)
                GraphicsService.Release( );
        }

        private void ResizeTimer_Tick (object sender, EventArgs e) {
            resizeTimer.IsEnabled = false;
            // if we're not in design mode, recreate the 
            // image source for the new size
            if (DesignerProperties.GetIsInDesignMode(this) == false &&
                GraphicsService != null) {
                // recreate the image source
                imageSource.Dispose( );
                imageSource = new ImageSource(
                    GraphicsDevice, (int)RenderSize.Width, (int)RenderSize.Height);
                rootImage.Source = imageSource.WriteableBitmap;
                Update( );
            }
        }

        void XnaControl_Loaded (object sender, RoutedEventArgs e) {
            // if we're not in design mode, initialize the graphics device
            DoStartUp( );
        }

        private void DoStartUp ( ) {
            if (DesignerProperties.GetIsInDesignMode(this) == false) {
                if (InitializeGraphicsDevice( )) {
                    SpriteBatch = new SpriteBatch(GraphicsDevice);
                    LoadContent( );
                }
            }
        }

        protected override void OnRenderSizeChanged (SizeChangedInfo sizeInfo) {
            resizeTimer.IsEnabled = true;
            resizeTimer.Stop( );
            resizeTimer.Start( );
            base.OnRenderSizeChanged(sizeInfo);
        }

        private bool InitializeGraphicsDevice ( ) {
            if (GraphicsService == null) {
                // add a reference to the graphics device
                PresentationSource presentationSource = PresentationSource.FromVisual(this);
                if (presentationSource == null)
                    return false;
                GraphicsService = GraphicsDeviceService.AddRef(((HwndSource)presentationSource).Handle);

                // create the image source
                if(RenderSize.Height == 0 || RenderSize.Width == 0) {
                    GraphicsService = null;
                    return false;
                }
                imageSource = new ImageSource(
                    GraphicsDevice, (int)RenderSize.Width, (int)RenderSize.Height);
                rootImage.Source = imageSource.WriteableBitmap;

                // hook the rendering event
                // CompositionTarget.Rendering += CompositionTarget_Rendering;
                DeviceInitialized?.Invoke( );
                return true;
            }
            return false;
        }

        /// <summary>
        /// Draws the control and allows subclasses to override 
        /// the default behavior of delegating the rendering.
        /// </summary>
        protected virtual void Render (SpriteBatch spriteBatch) {

        }

        protected virtual void LoadContent ( ) {

        }

        public void Update ( ) {
            // set the image source render target
            GraphicsDevice.SetRenderTarget(imageSource.RenderTarget);
            GraphicsDevice.Clear(Background);

            SpriteBatch.Begin(samplerState: SamplerState.PointClamp);
            // allow the control to draw
            Render(SpriteBatch);

            SpriteBatch.End( );

            // unset the render target
            GraphicsDevice.SetRenderTarget(null);

            // commit the changes to the image source
            imageSource.Commit( );
        }

    }
}
