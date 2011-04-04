using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Device.Location;
using MyFriendsAround.WP7.Model;

namespace MyFriendsAround.WP7.Service
{
    public interface ILocationService
    {
        event EventHandler<LocationChangedEventArgs> LocationChanged;
        event EventHandler<LocationStatusEventArgs> StatusChanged;
        Location CurrentLocation { get; }
        Location GetCurrentLocation();
        void Start();
        void Stop();
    }
}
