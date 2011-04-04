using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Media.Imaging;
using MyFriendsAround.WP7.Utils;
using System.IO;

namespace MyFriendsAround.WP7.Helpers.Converters
{
    public class MyImageConverter : IValueConverter
    {
        private static BitmapImage anonymousBitmap =
            new BitmapImage(new Uri("/icons/anonymousIcon.png", UriKind.RelativeOrAbsolute));

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string imageName = value.ToString();
            if (!string.IsNullOrEmpty(imageName))
            {
                string[] res = imageName.Split('?');
                byte[] img = IsolatedStorageHelper.LoadFromLocalStorageArray(res[0], "profiles");
                if (img != null)
                {
                    BitmapImage bi = new BitmapImage();
                    using (MemoryStream ms = new MemoryStream(img))
                    {                            
                        bi.SetSource(ms);
                    }
                    return bi;
                }
            }
            return anonymousBitmap;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
