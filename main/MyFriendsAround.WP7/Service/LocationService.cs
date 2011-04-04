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
using MyFriendsAround.WP7.Model;
using System.Device.Location;

namespace MyFriendsAround.WP7.Service
{
    public class LocationService : ILocationService
    {

        private IGeoPositionWatcher<GeoCoordinate> _gpsWatcher;
        public event EventHandler<LocationChangedEventArgs> LocationChanged;
        public event EventHandler<LocationStatusEventArgs> StatusChanged;
        public Location CurrentLocation { get; private set; }


        public LocationService()
        {
#if GPS_EMULATOR
            _gpsWatcher = new GpsEmulatorClient.GeoCoordinateWatcher();
#else
            _gpsWatcher = new GeoCoordinateWatcher(GeoPositionAccuracy.High)
                              {
                                  MovementThreshold = 10
                              };
#endif
            _gpsWatcher.PositionChanged += new EventHandler<GeoPositionChangedEventArgs<GeoCoordinate>>(watcher_PositionChanged);
            _gpsWatcher.StatusChanged += new EventHandler<GeoPositionStatusChangedEventArgs>(watcher_StatusChanged);
        }

        private void watcher_StatusChanged(object sender, GeoPositionStatusChangedEventArgs args)
        {
            //
            if (StatusChanged != null)
                StatusChanged(sender, new LocationStatusEventArgs(args.Status));
        }

        private void watcher_PositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> args)
        {
            if (LocationChanged != null)
                LocationChanged(sender, new LocationChangedEventArgs(
                    new Location(args.Position.Location.Latitude, args.Position.Location.Longitude, args.Position.Timestamp)
                    ));
        }

        public Location GetCurrentLocation()
        {
            return CurrentLocation;
        }

        public void Start()
        {
            _gpsWatcher.Start();
        }

        public void Stop()
        {
            _gpsWatcher.Stop();
        }

    }
}
