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
                return "MyFriendsAround";
            }
        }

        public string PageName
        {
            get
            {
                return "Friends Map";
            }
        }

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            //
            PublishLocationCommand = new RelayCommand(() => PublishLocationAction());
            DisplayAboutCommand = new RelayCommand(() => DisplayAbout());
            InputBoxCommand = new RelayCommand(() => InputBox());
            NavigateToAboutCommand = new RelayCommand(() => NavigateToAbout());
            RefreshFriendsCommand = new RelayCommand(() => RefreshFriends());

            if (IsInDesignMode)
            {
                // Code runs in Blend --> create design time data.
            }
            else
            {
                // Code runs "for real"
            }

        }

        private void RefreshFriends()
        {
            IsBusy = true;
            ServiceAgent.GetFriends(this.GetFriendsResult);
        }

        private void  NavigateToAbout()
        {
            //
            this.PageNav.NavigateTo(new Uri("/Views/AboutPage.xaml", UriKind.Relative));
        }

        private void InputBox()
        {
            MessageBox.Show("Input box");
        }


        private void DisplayAbout()
        {
            MessageBox.Show("About");
        }


        private void PopulatePushPins(List<Friend> list)
        {
            ObservableCollection<PushPinModel> result = new ObservableCollection<PushPinModel>();
            list.ForEach((f) =>
                             {
                                 //f.LocationStr
                                 result.Add(new PushPinModel()
                                                  {
                                                      PinSource = "/ApplicationIcon.png",
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
            IsBusy = true;
            ServiceAgent.PublishLocation(myInfo, new EventHandler<PublishLocationEventArgs>(PublishLocationResult));
        }

        public void GetFriendsResult(object sender, FriendsListEventArgs args)
        {
            List<Friend> list = args.Friends;
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
                                                      {
                                                          PopulatePushPins(list);
                                                          IsBusy = false;
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

        public ICommand PublishLocationCommand { get; set; }
        public ICommand DisplayAboutCommand { get; set; }
        public ICommand InputBoxCommand { get; set; }
        public ICommand NavigateToAboutCommand { get; set; }
        public ICommand RefreshFriendsCommand { get; set; }


        /// <summary>
        /// The <see cref="MyName" /> property's name.
        /// </summary>
        public const string MyNamePropertyName = "MyName";
        private string _myName = "Guest";

        /// <summary>
        /// Gets the MyName property.
        /// </summary>
        public string MyName
        {
            get { return _myName; }
            set
            {
                if (_myName == value)
                {
                    return;
                }
                var oldValue = _myName;
                _myName = value;
                // Update bindings, no broadcast
                RaisePropertyChanged(MyNamePropertyName);
            }
        }


        public string AppBarTextAbout {
            get { return "About"; }
        }

        public string AppBarTextPublish
        {
            get { return "Publish"; }
        }

        public string AppBarTextRefresh
        {
            get { return "Refresh"; }
        }

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


        /// <summary>
        /// The <see cref="IsBusy" /> property's name.
        /// </summary>
        public const string IsBusyPropertyName = "IsBusy";

        private bool _isBusy = false;

        /// <summary>
        /// Gets the IsBusy property.
        /// </summary>
        public bool IsBusy
        {
            get
            {
                return _isBusy;
            }

            set
            {
                if (_isBusy == value)
                {
                    return;
                }

                var oldValue = _isBusy;
                _isBusy = value;

                // Update bindings, no broadcast
                RaisePropertyChanged(IsBusyPropertyName);
            }
        }
    }
}