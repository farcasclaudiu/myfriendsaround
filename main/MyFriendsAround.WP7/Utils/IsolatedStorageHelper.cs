using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.IO;
using System.IO.IsolatedStorage;
using Microsoft.Phone;
using System.Windows.Media.Imaging;

namespace MyFriendsAround.WP7.Utils
{
    public class IsolatedStorageHelper
    {

        public static void SaveToLocalStorage(string imageFileName, string imageFolder, byte[] content)
        {
            if (content == null)
            {
                return;
            }

            var isoFile = IsolatedStorageFile.GetUserStoreForApplication();
            if (!isoFile.DirectoryExists(imageFolder))
            {
                isoFile.CreateDirectory(imageFolder);
            }

            string filePath = Path.Combine(imageFolder, imageFileName);
            using (var stream = isoFile.CreateFile(filePath))
            {
                stream.Write(content, 0, content.Length);
            }
        }

        public static WriteableBitmap LoadFromLocalStorage(string imageFileName, string imageFolder)
        {
            var isoFile = IsolatedStorageFile.GetUserStoreForApplication();
            if (!isoFile.DirectoryExists(imageFolder))
            {
                isoFile.CreateDirectory(imageFolder);
            }
            string filePath = Path.Combine(imageFolder, imageFileName);
            if (!isoFile.FileExists(filePath))
            {
                return null;
            }
            using (var imageStream = isoFile.OpenFile(filePath, FileMode.Open, FileAccess.Read))
            {
                var imageSource = PictureDecoder.DecodeJpeg(imageStream);
                return imageSource;
            }
        }

        public static byte[] LoadFromLocalStorageArray(string imageFileName, string imageFolder)
        {
            var isoFile = IsolatedStorageFile.GetUserStoreForApplication();
            if (!isoFile.DirectoryExists(imageFolder))
            {
                isoFile.CreateDirectory(imageFolder);
            }
            string filePath = Path.Combine(imageFolder, imageFileName);
            if (!isoFile.FileExists(filePath))
            {
                return null;
            }
            using (var imageStream = isoFile.OpenFile(filePath, FileMode.Open, FileAccess.Read))
            {
                byte[] buffer = new byte[imageStream.Length];
                imageStream.Read(buffer, 0, buffer.Length);
                return buffer;
            }
            return null;
        }
    }
}
