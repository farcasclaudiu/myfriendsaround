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
using System.Text;
using System.Security.Cryptography;
using System.Threading;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.IO;
using System.Windows.Media.Imaging;
using System.Diagnostics;

namespace WPImageCaching
{
    internal static class ImageDownloadHelper
    {
        private const double EXPIRATIONDAYS = 1.0;

        //Hilfsmethode zum Laden des Bildes
        public static void DownloadImage(string url, BitmapImage image, ImageCacheItem item)
        {
            string filename = CreateUniqueFilename(url);
            item.LocalFilename = filename;

            //Erstellen des Hilfsobjektes zur Übergabe an den asynchronen Aufruf
            AsyncDataTransfer transfer = new AsyncDataTransfer();
            transfer.Item = item;
            transfer.Image = image;

            //Erstellen der Abfrage
            var wc = (HttpWebRequest)HttpWebRequest.Create(url);
            if (item.ImageID != null)
            {
                //Prüfen, ob das Bild im Web immer noch aktuell ist
                wc.Headers["If-None-Match"] = item.ImageID;
            }
            transfer.WebRequest = wc;

            wc.BeginGetResponse(RequestCallback, transfer);
        }

        private static void RequestCallback(IAsyncResult result)
        {
            //War die Abfrage erfolgreich
            if (!result.IsCompleted)
            {
                return;
            }

            //Herstellen des Hilfsobjektes
            AsyncDataTransfer transfer = (AsyncDataTransfer)result.AsyncState;
            try
            {
                var response = (HttpWebResponse)transfer.WebRequest.EndGetResponse(result);

                //Bild wurde nicht geändert seit dem letzten Aufruf
                if (response.StatusCode == HttpStatusCode.NotModified)
                {
                    Deployment.Current.Dispatcher.BeginInvoke(() =>
                                                                  {
                                                                      using (IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication())
                                                                      {
                                                                          lock (ImageCache._lock)
                                                                          {
                                                                              using (IsolatedStorageFileStream fs =isf.OpenFile(transfer.Item.LocalFilename,FileMode.Open))
                                                                              {
                                                                                  transfer.Image.SetSource(fs);
                                                                              }
                                                                          }
                                                                      }
                                                                  });
                    return;
                }
                lock (ImageCache._lock)
                {
                    string newKey = transfer.WebRequest.RequestUri.ToString();
                    if (ImageCache.imageCache.ContainsKey(newKey))
                    {
                        //Setzen des Bildes
                        Deployment.Current.Dispatcher.BeginInvoke(
                            () =>
                            {
                                using (IsolatedStorageFile ifs = IsolatedStorageFile.GetUserStoreForApplication())
                                {
                                    using (IsolatedStorageFileStream fs = ifs.OpenFile(transfer.Item.LocalFilename, FileMode.Open))
                                    {
                                        transfer.Image.SetSource(fs);
                                    }
                                }
                            });
                        return;
                    }
                    //Hat das Bild eine neue ID?
                    if (response.Headers["ETag"] != null)
                    {
                        transfer.Item.ImageID = response.Headers["ETag"];
                    }
                    else
                    {
                        transfer.Item.ImageID = null;
                    }
                    //Gibt es ein Ablaufdatum?
                    if (response.Headers["Expires"] != null)
                    {
                        transfer.Item.Expiration = DateTime.Parse(response.Headers["Expires"]);
                    }
                    else
                    {
                        transfer.Item.Expiration = DateTime.Now.AddDays(EXPIRATIONDAYS);
                    }

                    var responseStream = response.GetResponseStream();

                    //Schreiben der Bilddatei
                    using ( var bw = new BinaryWriter( IsolatedStorageFile.GetUserStoreForApplication().CreateFile(transfer.Item.LocalFilename)) )
                    {
                        byte[] b = new byte[4096];
                        int read = 0;
                        while ((read = responseStream.Read(b, 0, b.Length)) > 0)
                        {
                            bw.Write(b, 0, read);
                        }
                        bw.Flush();
                        bw.Close();
                    }
                    //Setzen des Bildes
                    Deployment.Current.Dispatcher.BeginInvoke(
                        () =>
                            {
                                using (IsolatedStorageFile ifs = IsolatedStorageFile.GetUserStoreForApplication())
                                {
                                    using (IsolatedStorageFileStream fs = ifs.OpenFile( transfer.Item.LocalFilename,FileMode.Open))
                                    {
                                        transfer.Image.SetSource(fs);
                                    }
                                }
                            });
                    //Hinzufügen der Bildinformationen
                    if (!ImageCache.imageCache.ContainsKey(newKey))
                    {
                        ImageCache.imageCache.Add(newKey, transfer.Item);
                        //Speichern der Bildinformationen
                        ImageCache.SaveCachedImageInfo();
                    }
                }
            }
            catch (WebException webException)
            {
            }
            catch (Exception ex)
            {
                //Nichts machen, da Bild nicht heruntergeladen werden konnte
                Debug.WriteLine(ex.ToString());
            }

        }

        //Erstellt einen eindeutigen Namen für das zu ladende Bild
        private static string CreateUniqueFilename(string url)
        {
            string extension = System.IO.Path.GetExtension(url);
            byte[] textToHash = Encoding.UTF8.GetBytes(url);
            SHA1Managed sa = new SHA1Managed();
            byte[] hash = sa.ComputeHash(textToHash);
            return BitConverter.ToString(hash) + extension;
        }
    }
    //Hilfsklasse für den asynchronen Aufruf
    internal class AsyncDataTransfer
    {
        public ManualResetEvent ResetEvent { get; set; }
        public HttpWebRequest WebRequest { get; set; }
        public BitmapImage Image { get; set; }
        public ImageCacheItem Item { get; set; }
    }
}
