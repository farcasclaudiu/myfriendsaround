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
using System.Runtime.Serialization;

namespace WPImageCaching
{
    //Beinhaltet die Informationen eines Bildes
    [DataContract]
    public class ImageCacheItem
    {
        public ImageCacheItem()
        {
        }

        [DataMember]
        public string LocalFilename { get; set; }
        [DataMember]
        public string ImageID { get; set; }
        [DataMember]
        public DateTime Expiration { get; set; }
    }
}
