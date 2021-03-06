﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Device.Location;
using System.IO;
using System.Net;
using System.Security;
using System.ServiceModel.Channels;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Coding4Fun.Phone.Controls;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using Hammock;
using Hammock.Serialization;
using Microsoft.Phone;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Controls.Maps;
using Microsoft.Phone.Reactive;
using Microsoft.Silverlight.Testing;
using MyFriendsAround.Common.Entities;
using MyFriendsAround.WP7.Service;
using MyFriendsAround.WP7.Utils;
using MyFriendsAround.WP7.ViewModel;
using MyFriendsAround.WP7.Views;
using NetworkDetection;
using Newtonsoft.Json;
using Microsoft.Phone.Tasks;
using MyFriendsAround.WP7.Model;

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

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            GpsLocation = Location.Unknown;
            _SelectedFriend = null;

            //
            MainLoadCommand = new RelayCommand(() => MainLoad());
            BackKeyPressCommand = new RelayCommand<CancelEventArgs>((args) =>
                                                                        {
                                                                            if (IsSelectFriend)
                                                                            {
                                                                                IsSelectFriend = !IsSelectFriend;
                                                                                args.Cancel = true;
                                                                            }
                                                                        });
            PublishLocationCommand = new RelayCommand(() => PublishLocationAction());
            NavigateToSettingsCommand = new RelayCommand(() => NavigateToSettings());
            RefreshFriendsCommand = new RelayCommand(() => RefreshFriends());
            ShowAboutCommand = new RelayCommand(() => ShowAbout());
            SaveMySettingsCommand = new RelayCommand(() => SaveMySettings());
            CancelMySettingsCommand = new RelayCommand(() => CancelMySettings());
            ChoosePhotoCommand = new RelayCommand(() => ChoosePhoto());
            CropSaveCommand = new RelayCommand(() => CropSave());
            CropCancelCommand = new RelayCommand(() => CropCancel());
            MapViewChangedCommand = new RelayCommand<LocationRect>(boundRectangle => MapViewChanged(boundRectangle));
            ShowMyLocationCommand = new RelayCommand(() => ShowMyLocation());
            MapZoomInCommand = new RelayCommand(() => MapZoomIn());
            MapZoomOutCommand = new RelayCommand(() => MapZoomOut());
            ShowSelectFriendCommand = new RelayCommand(() => ShowSelectFriend());

            if (IsInDesignMode)
            {
                // Code runs in Blend --> create design time data.
            }
            else
            {
                // Code runs "for real"
            }

            photoChooserTask = new PhotoChooserTask();
            photoChooserTask.Completed += cameraTask_Completed;
            //photoChooserTask.PixelHeight = 100;
            //photoChooserTask.PixelWidth = 100;
            photoChooserTask.ShowCamera = true;

            //init GPS
            InitGps();
        }



        private void ShowSelectFriend()
        {
            IsSelectFriend = true;
        }






        #region Properties & Fields

        private IDisposable rxGpsLocation;
        private IDisposable rxGpsStatus;

        private LocationRect LastBoundRect = null;

        private PhotoChooserTask photoChooserTask;

        public string ApplicationTitle
        {
            get
            {
                return "MyFriendsAround";
            }
        }


        #region PageTitles

        public string PageName
        {
            get
            {
                return "Friends Map";
            }
        }

        public string PageNameSettings
        {
            get
            {
                return "Settings";
            }
        }

        public string PageNameCropping
        {
            get
            {
                return "Crop";
            }
        }

        #endregion

        #region AppBarText

        public string AppBarTextAbout
        {
            get { return "About"; }
        }

        public string AppBarTextSettings
        {
            get { return "Settings"; }
        }

        public string AppBarTextPublish
        {
            get { return "Publish"; }
        }

        public string AppBarTextRefresh
        {
            get { return "Refresh"; }
        }

        public string AppBarTextMyLocation
        {
            get { return "My Location"; }
        }

        public string AppBarTextSaveSettings
        {
            get { return "Save"; }
        }

        public string AppBarTextCancelSettings
        {
            get { return "Cancel"; }
        }

        #endregion



        /// <summary>
        /// The <see cref="SelectedFriend" /> property's name.
        /// </summary>
        public const string SelectedFriendPropertyName = "SelectedFriend";

        private PushPinModel _SelectedFriend;


        /// <summary>
        /// Gets the SelectedFriend property.
        /// </summary>
        public PushPinModel SelectedFriend
        {
            get
            {
                return _SelectedFriend;
            }

            set
            {
                IsSelectFriend = false;
                if (_SelectedFriend == value)
                {
                    return;
                }
                _SelectedFriend = value;

                if (_SelectedFriend != null && _SelectedFriend.Location != GeoCoordinate.Unknown)
                    MapCenter = _SelectedFriend.Location;

                // Update bindings, no broadcast
                RaisePropertyChanged(SelectedFriendPropertyName);
            }
        }



        /// <summary>
        /// The <see cref="IsSelectFriend" /> property's name.
        /// </summary>
        public const string IsSelectFriendPropertyName = "IsSelectFriend";

        private bool _isSelectFriend = false;

        /// <summary>
        /// Gets the IsSelectFriend property.
        /// </summary>
        public bool IsSelectFriend
        {
            get
            {
                return _isSelectFriend;
            }

            set
            {
                if (_isSelectFriend == value)
                {
                    return;
                }

                _isSelectFriend = value;

                // Update bindings, no broadcast
                RaisePropertyChanged(IsSelectFriendPropertyName);
            }
        }


        /// <summary>
        /// The <see cref="GpsLocation" /> property's name.
        /// </summary>
        public const string GpsLocationPropertyName = "GpsLocation";

        private Location _gpsLocation = Location.Unknown;

        /// <summary>
        /// Gets the GpsLocation property.
        /// </summary>
        public Location GpsLocation
        {
            get
            {
                return _gpsLocation;
            }

            set
            {
                if (_gpsLocation == value)
                {
                    return;
                }

                _gpsLocation = value;

                // Update bindings, no broadcast
                RaisePropertyChanged(GpsLocationPropertyName);

                UpdateDistances();
            }
        }

        private void UpdateDistances()
        {
            foreach (var pushPin in PushPins)
            {
                pushPin.PinDistance =
                    (GpsLocation != Location.Unknown)
                        ? Haversine.Distance(
                            new Position() { Latitude = pushPin.Location.Latitude, Longitude = pushPin.Location.Longitude },
                            new Position() { Latitude = GpsLocation.Latitude, Longitude = GpsLocation.Longitude },
                            DistanceType.Kilometers)
                        : 0;
            }
        }


        /// <summary>
        /// The <see cref="GpsStatus" /> property's name.
        /// </summary>
        public const string GpsStatusPropertyName = "GpsStatus";

        private GeoPositionStatus _gpsStatus = GeoPositionStatus.Disabled;

        /// <summary>
        /// Gets the GpsStatus property.
        /// </summary>
        public GeoPositionStatus GpsStatus
        {
            get
            {
                return _gpsStatus;
            }

            set
            {
                if (_gpsStatus == value)
                {
                    return;
                }

                _gpsStatus = value;

                // Update bindings, no broadcast
                RaisePropertyChanged(GpsStatusPropertyName);
            }
        }


        /// <summary>
        /// The <see cref="MyPicture" /> property's name.
        /// </summary>
        public const string MyPicturePropertyName = "MyPicture";
        private string _myPicture = Constants.MYPICTURE_FILE_NAME;

        /// <summary>
        /// Gets the MyPicture property.
        /// </summary>
        public string MyPicture
        {
            get
            {
                return _myPicture;
            }

            set
            {
                if (_myPicture == value)
                {
                    return;
                }
                _myPicture = value;

                // Update bindings, no broadcast
                RaisePropertyChanged(MyPicturePropertyName);
            }
        }



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
                _myName = value;
                // Update bindings, no broadcast
                RaisePropertyChanged(MyNamePropertyName);
            }
        }


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
                if (IsInDesignMode)
                {
                    ObservableCollection<PushPinModel> _testlist = new ObservableCollection<PushPinModel>();
                    _testlist.Add(new PushPinModel()
                                      {
                                          Location = new GeoCoordinate(10, 10),
                                          PinUserName = "User1",
                                          PinImageUrl = "http://www.clker.com/cliparts/b/1/f/a/1195445301811339265dagobert83_female_user_icon.svg.thumb.png"
                                      });
                    _testlist.Add(new PushPinModel()
                    {
                        Location = new GeoCoordinate(20, 20),
                        PinUserName = "User2",
                        PinImageUrl = "http://www.spanishseo.org/files/avatar_selection/user.png"
                    });
                    return _testlist;
                }
                return _PushPins;
            }

            set
            {
                if (_PushPins == value)
                {
                    return;
                }

                _PushPins = value;

                // Update bindings, no broadcast
                RaisePropertyChanged(PushPinsPropertyName);
                //
                RefreshVisiblePushPins();
            }
        }


        /// <summary>
        /// The <see cref="VisiblePushPins" /> property's name.
        /// </summary>
        public const string VisiblePushPinsPropertyName = "VisiblePushPins";
        private ObservableCollection<PushPinModel> _VisiblePushPins = new ObservableCollection<PushPinModel>();
        /// <summary>
        /// Gets the VisiblePushPins property.
        /// </summary>
        [JsonIgnore]
        public ObservableCollection<PushPinModel> VisiblePushPins
        {
            get
            {
                return _VisiblePushPins;
            }

            set
            {
                if (_VisiblePushPins == value)
                {
                    return;
                }

                _VisiblePushPins = value;

                // Update bindings, no broadcast
                RaisePropertyChanged(VisiblePushPinsPropertyName);
            }
        }


        /// <summary>
        /// The <see cref="MyLocationPushPins" /> property's name.
        /// </summary>
        public const string MyLocationPushPinsPropertyName = "MyLocationPushPins";
        private ObservableCollection<PushPinModel> _MyLocationPushPins = new ObservableCollection<PushPinModel>();
        /// <summary>
        /// Gets the MyLocationPushPins property.
        /// </summary>
        public ObservableCollection<PushPinModel> MyLocationPushPins
        {
            get
            {
                return _MyLocationPushPins;
            }

            set
            {
                if (_MyLocationPushPins == value)
                {
                    return;
                }

                _MyLocationPushPins = value;

                // Update bindings, no broadcast
                RaisePropertyChanged(MyLocationPushPinsPropertyName);
            }
        }



        /// <summary>
        /// The <see cref="MapZoom" /> property's name.
        /// </summary>
        public const string MapZoomPropertyName = "MapZoom";

        private int _mapZoom = 1;

        /// <summary>
        /// Gets the MapZoom property.
        /// </summary>
        public int MapZoom
        {
            get
            {
                return _mapZoom;
            }

            set
            {
                if (_mapZoom == value)
                {
                    return;
                }
                _mapZoom = value;

                // Update bindings, no broadcast
                RaisePropertyChanged(MapZoomPropertyName);
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
                _isBusy = value;

                // Update bindings, no broadcast
                RaisePropertyChanged(IsBusyPropertyName);
            }
        }



        /// <summary>
        /// The <see cref="MapCenter" /> property's name.
        /// </summary>
        public const string MapCenterPropertyName = "MapCenter";

        private GeoCoordinate _mapCenter = null;

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
            }
        }

        #endregion



        #region Commands

        public ICommand MainLoadCommand { get; set; }
        public ICommand BackKeyPressCommand { get; set; }
        public ICommand PublishLocationCommand { get; set; }
        public ICommand NavigateToSettingsCommand { get; set; }
        public ICommand ShowMyLocationCommand { get; set; }
        public ICommand RefreshFriendsCommand { get; set; }
        public ICommand ShowAboutCommand { get; set; }
        public ICommand SaveMySettingsCommand { get; set; }
        public ICommand CancelMySettingsCommand { get; set; }
        public ICommand ChoosePhotoCommand { get; set; }
        public ICommand CropSaveCommand { get; set; }
        public ICommand CropCancelCommand { get; set; }
        public ICommand MapViewChangedCommand { get; set; }
        public ICommand MapZoomInCommand { get; set; }
        public ICommand MapZoomOutCommand { get; set; }
        public ICommand ShowSelectFriendCommand { get; set; }

        #endregion




        #region Implemented Commands & Methods


        private void MapZoomOut()
        {
            //
            if (MapZoom < 22)
            {
                MapZoom++;
            }
        }

        private void MapZoomIn()
        {
            //
            if (MapZoom > 2)
            {
                MapZoom--;
            }
        }


        private void InitGps()
        {
            var geoLocationObs = Observable.FromEvent<LocationChangedEventArgs>(App.LocationService, "LocationChanged").Select(r => r.EventArgs.Location);
            rxGpsLocation = geoLocationObs.Subscribe(res =>
                {
                    if (res != Location.Unknown)
                    {
                        GpsLocation = res;
                        RefreshMyLocationPushPins();
                    }

                    System.Diagnostics.Debug.WriteLine(
                        "watcher_PositionChanged + " + res.ToString());
                });

            var geoStatusObs = Observable.FromEvent<LocationStatusEventArgs>(App.LocationService, "StatusChanged").Select(r => r.EventArgs.Status);
            rxGpsStatus = geoStatusObs.Subscribe(res =>
                {
                    GpsStatus = res;
                });


            App.LocationService.Start();
        }


        public void RefreshMyLocationPushPins()
        {
            if (LastBoundRect != null && LastBoundRect.Intersects(new LocationRect(
                    new GeoCoordinate(GpsLocation.Latitude, GpsLocation.Longitude),
                    .5,
                    .5)))
            {
                ObservableCollection<PushPinModel> _mynewlocation = new ObservableCollection<PushPinModel>();
                _mynewlocation.Add(new PushPinModel()
                {
                    Location = new GeoCoordinate(GpsLocation.Latitude, GpsLocation.Longitude),
                    PinUserName = "Me"
                });
                MyLocationPushPins = _mynewlocation;
            }
            else
            {
                MyLocationPushPins = new ObservableCollection<PushPinModel>();
            }
        }


        private void ShowMyLocation()
        {
            //
            if (GpsLocation != Location.Unknown &&
                GpsStatus == GeoPositionStatus.Ready
                )
            {
                MapCenter = new GeoCoordinate(GpsLocation.Latitude, GpsLocation.Longitude);
            }
        }


        private void MapViewChanged(LocationRect boundRectangle)
        {
            LastBoundRect = boundRectangle;
            //
            RefreshVisiblePushPins();
            //
            RefreshMyLocation();
        }

        private void RefreshMyLocation()
        {
            if (GpsLocation == Location.Unknown ||
                LastBoundRect == null ||
                !LastBoundRect.Intersects(new LocationRect(
                                              new GeoCoordinate(GpsLocation.Latitude, GpsLocation.Longitude),
                                              .5, .5)))
            {
                MyLocationPushPins = new ObservableCollection<PushPinModel>();
            }
            else
            {
                ObservableCollection<PushPinModel> _mynewlocation = new ObservableCollection<PushPinModel>();
                _mynewlocation.Add(new PushPinModel()
                {
                    Location = new GeoCoordinate(GpsLocation.Latitude, GpsLocation.Longitude),
                    PinUserName = "Me"
                });
                MyLocationPushPins = _mynewlocation;
            }
        }

        private void RefreshVisiblePushPins()
        {
            ObservableCollection<PushPinModel> _newVisiblePushPins = new ObservableCollection<PushPinModel>();
            //filter visible pushpins
            foreach (PushPinModel pushPin in PushPins)
            {
                if (LastBoundRect != null && LastBoundRect.Intersects(new LocationRect(
                                                                        new GeoCoordinate(pushPin.Location.Latitude, pushPin.Location.Longitude),
                                                                        .5, .5)))
                {
                    _newVisiblePushPins.Add(pushPin);
                }
            }
            VisiblePushPins = _newVisiblePushPins;
        }

        public void CropCancel()
        {
            //
            this.PageNav.GoBack();
        }

        public void CropSave()
        {
            //
        }


        private void MainLoad()
        {
            //this.MyPicture = Constants.MYPICTURE_FILE_NAME + "?" + DateTime.UtcNow.Ticks;
            RaisePropertyChanged(MyPicturePropertyName);
            RaisePropertyChanged(MyNamePropertyName);
            //
            RefreshMyLocation();
        }


        private void ChoosePhoto()
        {
            //choose photo
            //ShowCameraCaptureTask();
            //ShowPhotoChooserTask();
            if (!NetworkDetector.Instance.GetZuneStatus())
            {
                this.PageNav.NavigateTo(new Uri("/Views/CropPage.xaml", UriKind.RelativeOrAbsolute));
            }
            else
            {
                MessageBox.Show("Please disconnect from Zune!");
            }
        }

        private void ShowPhotoChooserTask()
        {
            photoChooserTask.Show();
        }

        private void ShowCameraCaptureTask()
        {
            var cameraTask = new CameraCaptureTask();
            cameraTask.Completed += cameraTask_Completed;
            cameraTask.Show();
        }

        private void cameraTask_Completed(object sender, PhotoResult e)
        {
            if (e.ChosenPhoto != null && e.ChosenPhoto.Length > 0) // e.TaskResult == TaskResult.OK)
            {
                // Get the image temp file from e.OriginalFileName.
                // Get the image temp stream from e.ChosenPhoto.
                // Don't keep either the stream or rely on the temp
                // file name as they may be vanished!

                // Store the image bytes.
                byte[] _imageBytes = new byte[e.ChosenPhoto.Length];
                e.ChosenPhoto.Read(_imageBytes, 0, _imageBytes.Length);
                //save
                IsolatedStorageHelper.SaveToLocalStorage(Constants.MYPICTURE_FILE_NAME, "profiles", _imageBytes);

                // Seek back so we can create an image.
                e.ChosenPhoto.Seek(0, SeekOrigin.Begin);
                // Create an image from the stream.
                //var imageSource = PictureDecoder.DecodeJpeg(e.ChosenPhoto);
                BitmapImage bi = new BitmapImage();
                bi.SetSource(e.ChosenPhoto);
                //MyPicture = bi;// imageSource;
            }
        }



        private void CancelMySettings()
        {
            //navigate back
            this.PageNav.GoBack();
        }

        private void SaveMySettings()
        {
            //save settings locally and on the server
        }

        private void ShowAbout()
        {
            //
            var aboutPrompt = new AboutPrompt();
            aboutPrompt.Title = "About";
            aboutPrompt.Body = Environment.NewLine + "Created by Claudiu Farcas" + Environment.NewLine + Environment.NewLine + "@claudiufarcas" + Environment.NewLine + Environment.NewLine + "Please visit" + Environment.NewLine + Environment.NewLine + "http://www.vorienteering.com";
            aboutPrompt.VersionNumber = "1.0";
            aboutPrompt.Show();
        }

        private void RefreshFriends()
        {
            IsBusy = true;
            ServiceAgent.GetFriends(this.GetFriendsResult);
        }

        private void NavigateToSettings()
        {
            //
            this.PageNav.NavigateTo(new Uri("/Views/SettingsPage.xaml", UriKind.Relative));
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
                                   Location = new GeoCoordinate(f.Latitude, f.Longitude, 0),
                                   PinUserName = f.FriendName,
                                   PinImageUrl =
                                       string.Format(
                                           "https://myfriendsaround.blob.core.windows.net/profiles/profile_{0}.jpg",
                                           f.Id),
                                   PinLastUpdated = f.LastUpdated,
                                   PinDistance =
                                       (GpsLocation != Location.Unknown)
                                           ? Haversine.Distance(
                                               new Position() { Latitude = f.Latitude, Longitude = f.Longitude },
                                               new Position() { Latitude = GpsLocation.Latitude, Longitude = GpsLocation.Longitude },
                                               DistanceType.Kilometers)
                                           : 0
                               });
            });
            PushPins = result;
            if (result.Count > 0)
                SelectedFriend = PushPins[0];
        }


        private void PublishLocationAction()
        {
            if (GpsLocation != Location.Unknown)
            {
                Friend myInfo = new Friend();
                myInfo.Id = Identification.GetDeviceId();
                myInfo.FriendName = MyName;
                myInfo.LastUpdated = DateTime.UtcNow;
                myInfo.LocationStr = string.Format("POINT({0} {1})", GpsLocation.Latitude, GpsLocation.Longitude);
                IsBusy = true;
                ServiceAgent.PublishLocation(myInfo, new EventHandler<PublishLocationEventArgs>(PublishLocationResult));
            }
            else
            {
                MessageBox.Show("GPS position not aquired yet!");
            }
        }

        public void GetFriendsResult(object sender, FriendsListEventArgs args)
        {
            if (args.Error == null)
            {
                List<Friend> list = args.Friends;
                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                    {
                        IsBusy = false;
                        PopulatePushPins(list);
                    }
                );
            }
            else
            {
                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                    {
                        IsBusy = false;
                        if (args.Error is WebException)
                        {
                            MessageBox.Show(args.Error.Message);
                        }
                        else
                        {
                            var exception = new ExceptionPrompt();
                            exception.Show(args.Error);
                        }
                    }
                );
            }
        }

        public void PublishLocationResult(object sender, PublishLocationEventArgs args)
        {
            if (args.Error != null)
            {
                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    IsBusy = false;
                    if (args.Error is WebException)
                    {
                        MessageBox.Show(args.Error.Message);
                    }
                    else
                    {
                        var exception = new ExceptionPrompt();
                        exception.Show(args.Error);
                    }
                });
            }
            else
            {
                if (!args.IsSuccess)
                {
                    DispatcherHelper.CheckBeginInvokeOnUI(() =>
                    {
                        IsBusy = false;
                        var message = new DialogMessage(
                            "Communication error!", DialogMessageCallback)
                        {
                            Button = MessageBoxButton.OK,
                            Caption = "Error!"
                        };

                        Messenger.Default.Send(message);
                    });
                }
                else
                {
                    //update also the picture
                    byte[] img = IsolatedStorageHelper.LoadFromLocalStorageArray(Constants.MYPICTURE_FILE_NAME, "profiles");
                    if (img != null)
                    {
                        ServiceAgent.PublishMyPicture(Identification.GetDeviceId(), img, new EventHandler<PublishLocationEventArgs>(PublishMyPictureResult));
                    }
                    else
                    {
                        //update friends list
                        ServiceAgent.GetFriends(this.GetFriendsResult);
                    }
                }
            }

        }

        public void PublishMyPictureResult(object sender, PublishLocationEventArgs args)
        {
            //
            if (args.Error != null)
            {
                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    IsBusy = false;
                    if (args.Error is WebException)
                    {
                        MessageBox.Show(args.Error.Message);
                    }
                    else
                    {
                        var exception = new ExceptionPrompt();
                        exception.Show(args.Error);
                    }
                });
            }
            else
            {
                if (!args.IsSuccess)
                {
                    DispatcherHelper.CheckBeginInvokeOnUI(() =>
                    {
                        IsBusy = false;
                        var message = new DialogMessage(
                            "Communication error!", DialogMessageCallback)
                        {
                            Button = MessageBoxButton.OK,
                            Caption = "Error!"
                        };

                        Messenger.Default.Send(message);
                    });
                }
                else
                {
                    //update friends list
                    ServiceAgent.GetFriends(this.GetFriendsResult);
                }
            }
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

        public override void Cleanup()
        {
            // Clean up if needed
            if (rxGpsLocation != null)
                rxGpsLocation.Dispose();
            if (rxGpsStatus != null)
                rxGpsStatus.Dispose();

            base.Cleanup();
            App.LocationService.Stop();
        }

        #endregion

    }

}