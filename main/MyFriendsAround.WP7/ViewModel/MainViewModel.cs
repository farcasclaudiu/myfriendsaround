using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Device.Location;
using System.Net;
using System.ServiceModel.Channels;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using Hammock;
using Hammock.Serialization;
using Microsoft.Phone.Controls;
using Microsoft.Silverlight.Testing;
using MyFriendsAround.Common.Entities;
using MyFriendsAround.WP7.Service;
using MyFriendsAround.WP7.Utils;
using Newtonsoft.Json;

namespace MyFriendsAround.WP7.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm/getstarted
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        public string ApplicationTitle
        {
            get
            {
                return "MVVM LIGHT";
            }
        }

        public string PageName
        {
            get
            {
                //myfriendsservice 
                return "My page:";
            }
        }

        public string Welcome
        {
            get
            {
                return "Welcome to MVVM Light";
            }
        }

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            MyName = "Guest";
            PublishLocationCommand = new RelayCommand(() => PublishLocationAction());
            if (IsInDesignMode)
            {
                // Code runs in Blend --> create design time data.
            }
            else
            {
                // Code runs "for real"
                ServiceAgent.GetFriends(this.GetFriendsResult);
            }

        }

        private void PopulatePushPins(List<Friend> list)
        {
            ObservableCollection<PushPinModel> result = new ObservableCollection<PushPinModel>();
            list.ForEach((f) =>
                             {
                                 //f.LocationStr
                                 result.Add(new PushPinModel()
                                                  {
                                                      PinSource = "ApplicationIcon.png",
                                                      Location = new GeoCoordinate(f.Latitude, f.Longitude)
                                                  });
                             });            
            PushPins = result;

        }


        private void PublishLocationAction()
        {
            Friend myInfo = new Friend();
            myInfo.Id = Identification.GetDeviceId();
            myInfo.FriendName = MyName;
            myInfo.LastUpdated = DateTime.UtcNow;
            myInfo.LocationStr = string.Format("POINT({0} {1})", MapCenter.Latitude, MapCenter.Longitude);
            ServiceAgent.PublishLocation(myInfo, new EventHandler<PublishLocationEventArgs>(PublishLocationResult));
        }

        public void GetFriendsResult(object sender, FriendsListEventArgs args)
        {
            List<Friend> list = args.Friends;
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
                                                      {
                                                          PopulatePushPins(list);
                                                      }
                );
        }

        public void PublishLocationResult(object sender, PublishLocationEventArgs args)
        {
            if (!args.IsSuccess)
            {
                var message = new DialogMessage("Communication error!", DialogMessageCallback)
                {
                    Button = MessageBoxButton.OK,
                    Caption = "Error!"
                };

                Messenger.Default.Send(message);
            }
            //
            //update
            ServiceAgent.GetFriends(this.GetFriendsResult);
        }

        private void DialogMessageCallback(MessageBoxResult result)
        {
            if (result == MessageBoxResult.OK)
            {
                //Message = "Continue";
            }
            else
            {
                //Message = "Stop";
            }
        }

        public RelayCommand PublishLocationCommand { get; set; }
        public string MyName { get; set; }

        ////public override void Cleanup()
        ////{
        ////    // Clean up if needed

        ////    base.Cleanup();
        ////}


        /// <summary>
        /// The <see cref="PushPins" /> property's name.
        /// </summary>
        public const string PushPinsPropertyName = "PushPins";
        private ObservableCollection<PushPinModel> _PushPins = new ObservableCollection<PushPinModel>();
        /// <summary>
        /// Gets the PushPins property.
        /// </summary>
        public ObservableCollection<PushPinModel> PushPins
        {
            get
            {
                return _PushPins;
            }

            set
            {
                if (_PushPins == value)
                {
                    return;
                }

                var oldValue = _PushPins;
                _PushPins = value;

                // Update bindings, no broadcast
                RaisePropertyChanged(PushPinsPropertyName);

                //// Update bindings and broadcast change using GalaSoft.MvvmLight.Messenging
                //RaisePropertyChanged(PushPinsPropertyName, oldValue, value, true);
            }
        }




        /// <summary>
        /// The <see cref="MapCenter" /> property's name.
        /// </summary>
        public const string MapCenterPropertyName = "MapCenter";
        private GeoCoordinate _mapCenter = new GeoCoordinate(0,0);

        /// <summary>
        /// Gets the MapCenter property.
        /// </summary>
        public GeoCoordinate MapCenter
        {
            get
            {
                return _mapCenter;
            }

            set
            {
                if (_mapCenter == value)
                {
                    return;
                }

                var oldValue = _mapCenter;
                _mapCenter = value;

                // Update bindings, no broadcast
                RaisePropertyChanged(MapCenterPropertyName);
                //// Update bindings and broadcast change using GalaSoft.MvvmLight.Messenging
                //RaisePropertyChanged(MapCenterPropertyName, oldValue, value, true);
            }
        }
    }
}