using System;
using System.IO;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Newtonsoft.Json;

namespace MyFriendsAround.WP7.Utils
{
    public static class SerializationHelper
    {
        public static T Deserialize<T>(string serialized)
        {
            if (string.IsNullOrEmpty(serialized))
                return default(T);

            return JsonConvert.DeserializeObject<T>(serialized);
        }

        public static string Serialize<T>(T obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

    }
}
