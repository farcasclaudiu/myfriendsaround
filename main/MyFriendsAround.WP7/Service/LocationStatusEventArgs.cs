using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Device.Location;

namespace MyFriendsAround.WP7.Service
{
    public class LocationStatusEventArgs : EventArgs
    {
        public GeoPositionStatus Status { get; private set; }
        public LocationStatusEventArgs(GeoPositionStatus status)
        {
            Status = status;
        }
    }
}
