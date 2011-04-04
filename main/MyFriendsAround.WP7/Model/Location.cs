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

namespace MyFriendsAround.WP7.Model
{
    public class Location
    {
        public static readonly Location Unknown;

        public DateTimeOffset Timestamp { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public Location(double latitude, double longitude, DateTimeOffset timestamp)
        {
            Latitude = latitude;
            Longitude = longitude;
            Timestamp = timestamp;
        }
    }
}
