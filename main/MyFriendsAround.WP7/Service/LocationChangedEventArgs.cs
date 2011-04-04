using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyFriendsAround.WP7.Model;

namespace MyFriendsAround.WP7.Service
{
    public class LocationChangedEventArgs: EventArgs
    {
        public Location Location { get; private set; }

        public LocationChangedEventArgs(Location location)
        {
            Location = location;
        }
    }
}
