using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Data;
using System.Globalization;
using System.Windows.Media.Imaging;

namespace WPImageCaching
{
    public class ImageCacheConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is BitmapImage)
            {
                return ImageCache.GetImage((BitmapImage)value);    
            }
            if (value is string && !string.IsNullOrEmpty(value as string))
            {
                return ImageCache.GetImage((string)value);
            }
            else
            {
                return value;
            }
            
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
