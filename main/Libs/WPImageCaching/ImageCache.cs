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
using Microsoft.Phone;

namespace WPImageCaching
{
    internal static class ImageCache
    {
        private const string IMAGECACHEFILE = "imagecachefile.nfo";
        internal static Dictionary<string, ImageCacheItem> imageCache;
        internal static object _lock = new object();


        public static BitmapImage GetImage(string url)
        {
            BitmapImage image = new BitmapImage();
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                //Wenn im Designmodus von Blend oder Visual Studio
                image.UriSource = new Uri(url);
                return image;
            }

            if (imageCache == null)
            {
                LoadCachedImageInfo();
            }

            //Prüfen auf ein vorhandenes gespeichertes Bild
            if (imageCache.ContainsKey(url))
            {
                //Prüfen auf Gültigkeit des Bildes
                if (DateTime.Compare(DateTime.Now, imageCache[url].Expiration) >= 0)
                {
                    ImageDownloadHelper.DownloadImage(url, image, imageCache[url]);
                }
                else
                {
                    if (IsolatedStorageFile.GetUserStoreForApplication().FileExists(imageCache[url].LocalFilename))
                    {
                        //Bild ist noch gültig
                        using (IsolatedStorageFile isf =  IsolatedStorageFile.GetUserStoreForApplication())
                        {
                            lock (_lock)
                            {
                                using (
                                    IsolatedStorageFileStream fs = isf.OpenFile(imageCache[url].LocalFilename,
                                                                                FileMode.Open))
                                {
                                    image.SetSource(fs);
                                    return image;
                                }
                            }
                        }
                    }
                    else
                    {
                        ImageDownloadHelper.DownloadImage(url, image, imageCache[url]);
                    }
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
            using (IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication())
            {
                lock (_lock)
                {
                    if (isf.FileExists(IMAGECACHEFILE))
                    {
                        using (IsolatedStorageFileStream fs = isf.OpenFile(IMAGECACHEFILE, FileMode.Open))
                        {
                            using (StreamReader sr = new StreamReader(fs))
                            {
                                string serObj = sr.ReadToEnd();
                                imageCache = SerializationHelper.Deserialize<Dictionary<string, ImageCacheItem>>(serObj);
                            }

                        }
                    }
                }
                if (imageCache == null)
                {
                    imageCache = new Dictionary<string, ImageCacheItem>();
                }
            }
        }

        //Speichern der Bildinformationen
        internal static void SaveCachedImageInfo()
        {
            using (IsolatedStorageFile sf = IsolatedStorageFile.GetUserStoreForApplication())
            {
                lock (_lock)
                {
                    if (sf.FileExists(IMAGECACHEFILE))
                        sf.DeleteFile(IMAGECACHEFILE);
                    using (IsolatedStorageFileStream fs = sf.CreateFile(IMAGECACHEFILE))
                    {
                        using (StreamWriter sw = new StreamWriter(fs))
                        {
                            string serObj = SerializationHelper.Serialize(imageCache);
                            sw.Write(serObj);
                        }
                    }
                }
            }
        }
    }
}
