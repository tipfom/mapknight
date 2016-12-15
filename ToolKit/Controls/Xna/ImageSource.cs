using Microsoft.Xna.Framework.Graphics;
using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace mapKnight.ToolKit.Controls.Xna {
    /// <summary>
    /// A wrapper for a RenderTarget2D and WriteableBitmap 
    /// that handles taking the XNA rendering and moving it 
    /// into the WriteableBitmap which is consumed as the
    /// ImageSource for an Image control.
    /// </summary>
    public class ImageSource : IDisposable {
        // the render target we draw to
        private RenderTarget2D renderTarget;

        // a WriteableBitmap we copy the pixels into for 
        // display into the Image
        private WriteableBitmap writeableBitmap;

        // a buffer array that gets the data from the render target
        private byte[ ] buffer;

        /// <summary>
        /// Gets the render target used for this image source.
        /// </summary>
        public RenderTarget2D RenderTarget {
            get { return renderTarget; }
        }

        /// <summary>
        /// Gets the underlying WriteableBitmap that can 
        /// be bound as an ImageSource.
        /// </summary>
        public WriteableBitmap WriteableBitmap {
            get { return writeableBitmap; }
        }

        /// <summary>
        /// Creates a new XnaImageSource.
        /// </summary>
        /// <param name="graphics">The GraphicsDevice to use.</param>
        /// <param name="width">The width of the image source.</param>
        /// <param name="height">The height of the image source.</param>
        public ImageSource (GraphicsDevice graphics, int width, int height) {
            // create the render target and buffer to hold the data
            renderTarget = new RenderTarget2D(
                graphics, width, height, false,
                SurfaceFormat.Bgra32,
                DepthFormat.None);
            buffer = new byte[width * height * 4];
            writeableBitmap = new WriteableBitmap(
                width, height, 96, 96,
                PixelFormats.Bgra32, null);
        }

        ~ImageSource ( ) {
            Dispose(false);
        }

        public void Dispose ( ) {
            Dispose(true);
        }

        protected virtual void Dispose (bool disposing) {
            renderTarget.Dispose( );

            if (disposing)
                GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Commits the render target data into our underlying bitmap source.
        /// </summary>
        public void Commit ( ) {
            // get the data from the render target
            renderTarget.GetData(buffer);
            
            // write our pixels into the bitmap source
            writeableBitmap.Lock( );
            Marshal.Copy(buffer, 0, writeableBitmap.BackBuffer, buffer.Length);
            writeableBitmap.AddDirtyRect(
                new Int32Rect(0, 0, renderTarget.Width, renderTarget.Height));
            writeableBitmap.Unlock( );
        }
    }
}
