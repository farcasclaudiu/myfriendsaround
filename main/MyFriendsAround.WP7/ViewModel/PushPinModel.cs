using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Device.Location;
using System.Linq;
using System.Text;
using MyFriendsAround.WP7.Model;

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


        private string _pinUserName;
        public string PinUserName
        {
            get { return _pinUserName; }
            set
            {
                if (_pinUserName != value)
                {
                    _pinUserName = value;
                    OnPropertyChanged("PinUserName");
                }
            }
        }


        private string _pinImageUrl;
        public string PinImageUrl
        {
            get { return _pinImageUrl; }
            set
            {
                if (_pinImageUrl != value)
                {
                    _pinImageUrl = value;
                    OnPropertyChanged("PinImageUrl");
                }
            }
        }

        private DateTime _pinLastUpdated;
        public DateTime PinLastUpdated
        {
            get { return _pinLastUpdated; }
            set
            {
                if (_pinLastUpdated != value)
                {
                    _pinLastUpdated = value;
                    OnPropertyChanged("PinLastUpdated");
                }
            }
        }

        private double _pinDistance;
        public double PinDistance
        {
            get { return Math.Round(_pinDistance, 2); }
            set
            {
                if (_pinDistance != value)
                {
                    _pinDistance = value;
                    OnPropertyChanged("PinDistance");
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
