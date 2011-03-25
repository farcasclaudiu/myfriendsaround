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
using System.IO.IsolatedStorage;
using System.IO;
using MyFriendsAround.WP7.ViewModel;
using System.Text;

namespace MyFriendsAround.WP7.Utils
{
    public static class ApplicationExtensions
    {

        private static string GetIsFile(Type t)
        {
            return string.Concat(t.Name, ".dat");
        }


        public static T RetrieveFromIsolatedStorage<T>(this Application app) where T : class
        {
            using (var userAppStore = IsolatedStorageFile.GetUserStoreForApplication())
            {
                var dataFileName = GetIsFile(typeof(T));
                if (userAppStore.FileExists(dataFileName))
                {
                    using(StreamReader sr =new StreamReader(userAppStore.OpenFile(dataFileName, FileMode.Open), Encoding.UTF8))
                    {
                        return SerializationHelper.Deserialize<T>(sr.ReadToEnd());    
                    }
                }
            }
            return null;
        }

        public static void SaveToIsolatedStorage<T>(this Application app, T model) where T : class
        {
            var dataFileName = GetIsFile((model.GetType()));
            using (var userAppStore = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (userAppStore.FileExists(dataFileName))
                {
                    userAppStore.DeleteFile(dataFileName);
                }
                using (StreamWriter sw = new StreamWriter(userAppStore.CreateFile(dataFileName), Encoding.UTF8))
                {
                    sw.Write(SerializationHelper.Serialize<T>(model));
                }
            }
        }

    }
}
