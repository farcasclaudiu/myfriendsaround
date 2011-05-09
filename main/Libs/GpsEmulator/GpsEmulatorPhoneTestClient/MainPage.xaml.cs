using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Devices;
using Microsoft.Phone.Controls;
using GpsEmulatorClient;
using System.Device.Location;
using System.Threading;



namespace GpsEmulatorPhoneTestClient
{
    public partial class MainPage : PhoneApplicationPage
    {
        // the wathcer through which we'll get the device location
        private IGeoPositionWatcher<GeoCoordinate> _Watcher;
        private static int counter = 0;

        // Constructor
        public MainPage()
        {
            InitializeComponent();

            if (Microsoft.Devices.Environment.DeviceType == DeviceType.Emulator)
            {
                _Watcher = new GpsEmulatorClient.GeoCoordinateWatcher();
            }
            else
            {
                _Watcher = new System.Device.Location.GeoCoordinateWatcher();
            }

            _Watcher.PositionChanged += new EventHandler<GeoPositionChangedEventArgs<GeoCoordinate>>(watcher_PositionChanged);
            _Watcher.StatusChanged += new EventHandler<GeoPositionStatusChangedEventArgs>(watcher_StatusChanged);
        }

        void watcher_StatusChanged(object sender, GeoPositionStatusChangedEventArgs e)
        {
            tbDeviceStatus.Text = e.Status.ToString();
        }

        void watcher_PositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            tbTimeAcquired.Text = e.Position.Timestamp.ToString();
            tbLatitude.Text = e.Position.Location.Latitude.ToString();
            tbLongtitude.Text = e.Position.Location.Longitude.ToString();

            System.Diagnostics.Debug.WriteLine("watcher_PositionChanged + " + DateTime.Now.Second);
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            _Watcher.Start();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            _Watcher.Stop();
        }

        private void btnTicks_Click(object sender, EventArgs e)
        {
            PageTitle.Text = counter.ToString();
        }
    }
}