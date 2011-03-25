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
    public class AboutViewModel : ViewModelBase
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
                return "About";
            }
        }


        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public AboutViewModel()
        {
            if (IsInDesignMode)
            {
                // Code runs in Blend --> create design time data.
            }
            else
            {
                // Code runs "for real"
            }

        }




        public string AppBarTextAbout {
            get { return "About"; }
        }

        public string AppBarTextPublish
        {
            get { return "Publish"; }
        }

    }
}