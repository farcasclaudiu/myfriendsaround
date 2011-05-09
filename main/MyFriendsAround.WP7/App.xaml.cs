using System;
using System.Collections.Generic;
using System.Device.Location;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Devices;
using Microsoft.Phone;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Net.NetworkInformation;
using Microsoft.Phone.Shell;
using MyFriendsAround.WP7.Model;
using MyFriendsAround.WP7.ViewModel;
using MyFriendsAround.WP7.Utils;
using GalaSoft.MvvmLight.Threading;
using MyFriendsAround.WP7.Views;
using NetworkDetection;
using MyFriendsAround.WP7.Service;


namespace MyFriendsAround.WP7
{
    public partial class App : Application
    {

        // Easy access to the root frame
        public PhoneApplicationFrame RootFrame { get; private set; }

        // Constructor
        public App()
        {
            // Global handler for uncaught exceptions. 
            // Note that exceptions thrown by ApplicationBarItem.Click will not get caught here.
            UnhandledException += Application_UnhandledException;

            // Standard Silverlight initialization
            InitializeComponent();

            // Phone-specific initialization
            InitializePhoneApplication();

            //init NetworkDetector
            var dummy = NetworkDetector.Instance;

            //register ViewModelLocator
            Container.Instance.RegisterInstance(typeof(ViewModelLocator), "ViewModelLocator");
            Container.Instance.RegisterInstance<ILocationService>(new LocationService(), "LocationService");

            NetworkDetector.Instance.OnNetworkOFF += new EventHandler<NetworkAvailableEventArgs>(Instance_OnNetworkOFF);
            NetworkDetector.Instance.OnNetworkON += new EventHandler<NetworkAvailableEventArgs>(Instance_OnNetworkON);
        }

        void Instance_OnNetworkON(object sender, NetworkAvailableEventArgs e)
        {
            ViewModelLocator locator = Container.Instance.Resolve<ViewModelLocator>("ViewModelLocator");
            locator.Main.IsBusy = true;
        }

        void Instance_OnNetworkOFF(object sender, NetworkAvailableEventArgs e)
        {
            ViewModelLocator locator = Container.Instance.Resolve<ViewModelLocator>("ViewModelLocator");
            locator.Main.IsBusy = false;
        }


        public static ILocationService LocationService
        {
            get
            {
                return Container.Instance.Resolve<ILocationService>("LocationService");
            }
        }

        // Code to execute when the application is launching (eg, from Start)
        // This code will not execute when the application is reactivated
        private void Application_Launching(object sender, LaunchingEventArgs e)
        {
            if (Microsoft.Devices.Environment.DeviceType == DeviceType.Emulator)
            {
                IsolatedStorageExplorer.Explorer.Start("localhost");
            }

            DispatcherHelper.Initialize();
            LoadModel();
        }

        // Code to execute when the application is activated (brought to foreground)
        // This code will not execute when the application is first launched
        private void Application_Activated(object sender, ActivatedEventArgs e)
        {
            if (Microsoft.Devices.Environment.DeviceType == DeviceType.Emulator)
            {
                IsolatedStorageExplorer.Explorer.RestoreFromTombstone();
            }

            DispatcherHelper.Initialize();
            LoadModel();
        }

        // Code to execute when the application is deactivated (sent to background)
        // This code will not execute when the application is closing
        private void Application_Deactivated(object sender, DeactivatedEventArgs e)
        {
            SaveModel();
        }

        // Code to execute when the application is closing (eg, user hit Back)
        // This code will not execute when the application is deactivated
        private void Application_Closing(object sender, ClosingEventArgs e)
        {
            SaveModel();
            //
            ViewModelLocator locator = Container.Instance.Resolve<ViewModelLocator>("ViewModelLocator");
            locator.Cleanup();
        }


        private void LoadModel()
        {
            //
            MainViewModel mainModel = this.RetrieveFromIsolatedStorage<MainViewModel>();
            if (mainModel != null)
            {
                Container.Instance.RegisterInstance<MainViewModel>(mainModel, Constants.VM_MAIN);
            }
            else
            {
                Container.Instance.RegisterInstance<MainViewModel>(new MainViewModel(), Constants.VM_MAIN);
            }
            Container.Instance.Resolve<ViewModelLocator>("ViewModelLocator").Main.GpsLocation = Location.Unknown;
            Container.Instance.Resolve<ViewModelLocator>("ViewModelLocator").Main.GpsStatus = GeoPositionStatus.NoData;
            Container.Instance.Resolve<ViewModelLocator>("ViewModelLocator").Main.RefreshMyLocationPushPins();
            Container.Instance.Resolve<ViewModelLocator>("ViewModelLocator").Main.IsBusy = false; //(NetworkDetector.Instance.GetCurrentNetworkType() == NetworkInterfaceType.None) || !NetworkDetector.Instance.GetZuneStatus();
            //
            SettingsViewModel settingsModel = this.RetrieveFromIsolatedStorage<SettingsViewModel>();
            if (settingsModel != null)
            {
                Container.Instance.RegisterInstance<SettingsViewModel>(settingsModel, Constants.VM_SETTINGS);
            }
            else
            {
                Container.Instance.RegisterInstance<SettingsViewModel>(new SettingsViewModel(), Constants.VM_SETTINGS);
            }

        }


        private void SaveModel()
        {
            this.SaveToIsolatedStorage<MainViewModel>(Container.Instance.Resolve<MainViewModel>(Constants.VM_MAIN));
            this.SaveToIsolatedStorage<SettingsViewModel>(Container.Instance.Resolve<SettingsViewModel>(Constants.VM_SETTINGS));
        }


        // Code to execute if a navigation fails
        void RootFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            //LittleWatson.ReportException(e.Exception, string.Empty);
            if (e.Exception is WebException)
            {
                MessageBox.Show(e.Exception.Message);
            }
            else
            {
                ShowException(e.Exception);
            }

            if (System.Diagnostics.Debugger.IsAttached)
            {
                // A navigation has failed; break into the debugger
                System.Diagnostics.Debugger.Break();
            }
        }

        // Code to execute on Unhandled Exceptions
        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            #region Error Logging
            
            //LittleWatson.ReportException(e.ExceptionObject, string.Empty);
            if (e.ExceptionObject is WebException)
            {
                MessageBox.Show(e.ExceptionObject.Message);
            }
            else
            {
                ShowException(e.ExceptionObject);
            }

            #endregion


            if (System.Diagnostics.Debugger.IsAttached)
            {
                // An unhandled exception has occurred; break into the debugger
                System.Diagnostics.Debugger.Break();
            }

            e.Handled = true;
        }

        private void ShowException(Exception ex)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                Container.Instance.Resolve<ViewModelLocator>("ViewModelLocator").Main.IsBusy = false;
                var exception = new ExceptionPrompt();
                exception.Show(ex);
            }
            );
        }

        #region Phone application initialization

        // Avoid double-initialization
        private bool phoneApplicationInitialized = false;

        // Do not add any additional code to this method
        private void InitializePhoneApplication()
        {
            if (phoneApplicationInitialized)
                return;

            // Create the frame but don't set it as RootVisual yet; this allows the splash
            // screen to remain active until the application is ready to render.
            RootFrame = new PhoneApplicationFrame();
            RootFrame.Navigated += CompleteInitializePhoneApplication;

            // Handle navigation failures
            RootFrame.NavigationFailed += RootFrame_NavigationFailed;

            // Ensure we don't initialize again
            phoneApplicationInitialized = true;
        }

        // Do not add any additional code to this method
        private void CompleteInitializePhoneApplication(object sender, NavigationEventArgs e)
        {
            // Set the root visual to allow the application to render
            if (RootVisual != RootFrame)
                RootVisual = RootFrame;

            // Remove this handler since it is no longer needed
            RootFrame.Navigated -= CompleteInitializePhoneApplication;
            //
            //LittleWatson.CheckForPreviousException();
        }

        #endregion
    }
}
