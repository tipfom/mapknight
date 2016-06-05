using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace mapKnight.ToolKit {
    public class ImageConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            if (value is Bitmap) {
                var stream = new MemoryStream( );
                ((Bitmap)value).Save(stream, ImageFormat.Png);

                BitmapImage bitmap = new BitmapImage( );
                bitmap.BeginInit( );
                bitmap.StreamSource = stream;
                bitmap.EndInit( );

                return bitmap;
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            throw new NotImplementedException( );
        }
    }
}
