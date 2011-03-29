﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Device.Location;
using System.IO;
using System.Net;
using System.Security;
using System.ServiceModel.Channels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Coding4Fun.Phone.Controls;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using Hammock;
using Hammock.Serialization;
using Microsoft.Phone;
using Microsoft.Phone.Controls;
using Microsoft.Silverlight.Testing;
using MyFriendsAround.Common.Entities;
using MyFriendsAround.WP7.Service;
using MyFriendsAround.WP7.Utils;
using MyFriendsAround.WP7.Views;
using Newtonsoft.Json;
using Microsoft.Phone.Tasks;

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

        public string PageNameSettings
        {
            get
            {
                return "Settings";
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
            NavigateToSettingsCommand = new RelayCommand(() => NavigateToSettings());
            RefreshFriendsCommand = new RelayCommand(() => RefreshFriends());
            ShowAboutCommand = new RelayCommand(() => ShowAbout());
            SaveMySettingsCommand = new RelayCommand(() => SaveMySettings());
            CancelMySettingsCommand = new RelayCommand(() => CancelMySettings());
            ChoosePhotoCommand = new RelayCommand(() => ChoosePhoto());

            if (IsInDesignMode)
            {
                // Code runs in Blend --> create design time data.
            }
            else
            {
                // Code runs "for real"
            }

        }

        private void ChoosePhoto()
        {
            //choose photo
            ShowCameraCaptureTask();
            //ShowPhotoChooserTask();
        }

        private void ShowPhotoChooserTask()
        {
            var photoChooserTask = new PhotoChooserTask();
            photoChooserTask.Completed += cameraTask_Completed;
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
            if (e.TaskResult == TaskResult.OK)
            {
                // Get the image temp file from e.OriginalFileName.
                // Get the image temp stream from e.ChosenPhoto.
                // Don't keep either the stream or rely on the temp
                // file name as they may be vanished!

                // Store the image bytes.
                byte[] _imageBytes = new byte[e.ChosenPhoto.Length];
                e.ChosenPhoto.Read(_imageBytes, 0, _imageBytes.Length);

                // Seek back so we can create an image.
                e.ChosenPhoto.Seek(0, SeekOrigin.Begin);

                // Create an image from the stream.
                var imageSource = PictureDecoder.DecodeJpeg(e.ChosenPhoto);
                MyPicture = imageSource;
            }
        }

        /// <summary>
        /// The <see cref="MyPicture" /> property's name.
        /// </summary>
        public const string MyPicturePropertyName = "MyPicture";
        private BitmapSource _myPicture = new BitmapImage(new Uri("/icons/anonymousIcon.png", UriKind.RelativeOrAbsolute));

        /// <summary>
        /// Gets the MyPicture property.
        /// </summary>
        public BitmapSource MyPicture
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

                var oldValue = _myPicture;
                _myPicture = value;

                // Update bindings, no broadcast
                RaisePropertyChanged(MyPicturePropertyName);

                // Update bindings and broadcast change using GalaSoft.MvvmLight.Messenging
                RaisePropertyChanged(MyPicturePropertyName, oldValue, value, true);
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
            if (args.Error == null)
            {
                List<Friend> list = args.Friends;
                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                                                          {
                                                              PopulatePushPins(list);
                                                              IsBusy = false;
                                                          }
                    );
            }
            else
            {
                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                                                          {
                                                              IsBusy = false;
                                                              var exception = new ExceptionPrompt();
                                                              exception.Show(args.Error);
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
                                                              var exception = new ExceptionPrompt();
                                                              exception.Show(args.Error);
                                                          });
            }
            else
            {
                if (!args.IsSuccess)
                {
                    DispatcherHelper.CheckBeginInvokeOnUI(() =>
                                                              {
                                                                  var message = new DialogMessage(
                                                                      "Communication error!", DialogMessageCallback)
                                                                                    {
                                                                                        Button = MessageBoxButton.OK,
                                                                                        Caption = "Error!"
                                                                                    };

                                                                  Messenger.Default.Send(message);
                                                              });
                }
                //
                //update
                ServiceAgent.GetFriends(this.GetFriendsResult);
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

        public ICommand PublishLocationCommand { get; set; }
        public ICommand DisplayAboutCommand { get; set; }
        public ICommand NavigateToSettingsCommand { get; set; }
        public ICommand RefreshFriendsCommand { get; set; }
        public ICommand ShowAboutCommand { get; set; }
        public ICommand SaveMySettingsCommand { get; set; }
        public ICommand CancelMySettingsCommand { get; set; }
        public ICommand ChoosePhotoCommand { get; set; }
        


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

        public string AppBarTextSaveSettings
        {
            get { return "Save"; }
        }

        public string AppBarTextCancelSettings
        {
            get { return "Cancel"; }
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
        private GeoCoordinate _mapCenter = new GeoCoordinate(0, 0);

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