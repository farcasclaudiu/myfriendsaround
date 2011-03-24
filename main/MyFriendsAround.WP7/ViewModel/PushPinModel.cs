using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Device.Location;
using System.Linq;
using System.Text;

namespace MyFriendsAround.WP7.ViewModel
{
    public class PushPinModel : INotifyPropertyChanged
    {
        public PushPinModel() { ; }
        private GeoCoordinate _location;

        private string _pinSource;

        public string PinSource
        {
            get { return _pinSource; }
            set
            {
                if (_pinSource != value)
                {
                    _pinSource = value;
                    OnPropertyChanged("PinSource");
                }
            }
        }

        public GeoCoordinate Location
        {
            get { return _location; }
            set
            {
                if (_location != value)
                {
                    _location = value;
                    OnPropertyChanged("Location");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }

}
