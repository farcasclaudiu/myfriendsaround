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
using Microsoft.Phone.Info;

namespace MyFriendsAround.WP7.Utils
{
    public class Identification
    {
        public static String GetDeviceId()
        {
            byte[] id = (byte[])DeviceExtendedProperties.GetValue("DeviceUniqueId");
            return Convert.ToBase64String(id);
        }


        private static readonly int ANIDLength = 32;
        private static readonly int ANIDOffset = 2;
        public static string GetManufacturer()
        {
            string result = string.Empty;
            object manufacturer;
            if (DeviceExtendedProperties.TryGetValue("DeviceManufacturer", out manufacturer))
                result = manufacturer.ToString();

            return result;
        }

        //Note: to get a result requires ID_CAP_IDENTITY_DEVICE
        // to be added to the capabilities of the WMAppManifest
        // this will then warn users in marketplace
        public static byte[] GetDeviceUniqueID()
        {
            byte[] result = null;
            object uniqueId;
            if (DeviceExtendedProperties.TryGetValue("DeviceUniqueId", out uniqueId))
                result = (byte[])uniqueId;

            return result;
        }

        // NOTE: to get a result requires ID_CAP_IDENTITY_USER
        //  to be added to the capabilities of the WMAppManifest
        // this will then warn users in marketplace
        public static string GetWindowsLiveAnonymousID()
        {
            string result = string.Empty;
            object anid;
            if (UserExtendedProperties.TryGetValue("ANID", out anid))
            {
                if (anid != null && anid.ToString().Length >= (ANIDLength + ANIDOffset))
                {
                    result = anid.ToString().Substring(ANIDOffset, ANIDLength);
                }
            }

            return result;
        }

    }
}
