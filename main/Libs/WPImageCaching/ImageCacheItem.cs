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

namespace WPImageCaching
{
    //Beinhaltet die Informationen eines Bildes
    internal class ImageCacheItem
    {
        public string LocalFilename { get; set; }
        public string ImageID { get; set; }
        public DateTime Expiration { get; set; }
    }
}
