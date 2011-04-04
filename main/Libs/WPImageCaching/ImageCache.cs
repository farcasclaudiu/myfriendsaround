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
using System.Collections.Generic;
using System.Windows.Media.Imaging;
using System.IO;
using System.IO.IsolatedStorage;
using System.Runtime.Serialization;

namespace WPImageCaching
{
    internal static class ImageCache
    {
        private const string IMAGECACHEFILE = "imagecachefile.nfo";
        internal static Dictionary<string, ImageCacheItem> imageCache;


        public static BitmapImage GetImage(BitmapImage image)
        {
            string url = image.UriSource.ToString();

            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                //Wenn im Designmodus von Blend oder Visual Studio
                image.UriSource = new Uri(url);
                return image;
            }

            if (imageCache==null)
            {
                LoadCachedImageInfo();
            }
            
            //Prüfen auf ein vorhandenes gespeichertes Bild
            if (imageCache.ContainsKey(url))
            {
                //Prüfen auf Gültigkeit des Bildes
                if (DateTime.Compare(DateTime.Now, imageCache[url].Expiration) >= 0)
                {
                    ImageDownloadHelper.DownloadImage(url,image,imageCache[url]);
                }
                else
                {
                    //Bild ist noch gültig
                    image.SetSource(IsolatedStorageFile.GetUserStoreForApplication().OpenFile(imageCache[url].LocalFilename, FileMode.Open));
                    return image;
                }
            }
            else
            {
                //Bild noch nicht gespeichert
                ImageCacheItem item = new ImageCacheItem();
                ImageDownloadHelper.DownloadImage(url, image, item);
            }
            return image;
        }

        //Laden der Bildinformationen
        private static void LoadCachedImageInfo()
        {
            if (IsolatedStorageFile.GetUserStoreForApplication().FileExists(IMAGECACHEFILE))
            {
                IsolatedStorageFileStream fs =
                    IsolatedStorageFile.GetUserStoreForApplication().OpenFile(IMAGECACHEFILE,FileMode.Open);

                DataContractSerializer dcs = new DataContractSerializer(typeof(Dictionary<string, ImageCacheItem>));
                imageCache = (Dictionary<string, ImageCacheItem>)dcs.ReadObject(fs);
                fs.Close();
            }
            else
            {
                imageCache = new Dictionary<string, ImageCacheItem>();
            }
        }

        //Speichern der Bildinformationen
        internal static void SaveCachedImageInfo()
        {
            IsolatedStorageFileStream fs =
                IsolatedStorageFile.GetUserStoreForApplication().CreateFile(IMAGECACHEFILE);

            DataContractSerializer dcs = new DataContractSerializer(typeof(Dictionary<string, ImageCacheItem>));
            dcs.WriteObject(fs, imageCache);
            fs.Flush();
            fs.Close();
        }
    }
}
