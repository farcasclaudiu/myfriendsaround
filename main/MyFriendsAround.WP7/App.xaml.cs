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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using MyFriendsAround.WP7.ViewModel;
using MyFriendsAround.WP7.Utils;
using GalaSoft.MvvmLight.Threading;
using MyFriendsAround.WP7.Views;


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

            //register ViewModelLocator
            Container.Instance.RegisterInstance(typeof(ViewModelLocator), "ViewModelLocator");
        }

        // Code to execute when the application is launching (eg, from Start)
        // This code will not execute when the application is reactivated
        private void Application_Launching(object sender, LaunchingEventArgs e)
        {
            DispatcherHelper.Initialize();
            LoadModel();
        }

        // Code to execute when the application is activated (brought to foreground)
        // This code will not execute when the application is first launched
        private void Application_Activated(object sender, ActivatedEventArgs e)
        {
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
            ViewModelLocator locator = Container.Instance.Resolve<ViewModelLocator>();
            locator.Cleanup();
        }


        private void LoadModel()
        {
            //
            MainViewModel mainModel = this.RetrieveFromIsolatedStorage<MainViewModel>();
            if (mainModel != null)
            {
                Container.Instance.RegisterInstance<MainViewModel>(mainModel, "MainViewModel");
            }
            else
            {
                Container.Instance.RegisterInstance<MainViewModel>(new MainViewModel(), "MainViewModel");
            }
            AboutViewModel aboutModel = this.RetrieveFromIsolatedStorage<AboutViewModel>();
            if (aboutModel != null)
            {
                Container.Instance.RegisterInstance<AboutViewModel>(aboutModel, "AboutViewModel");
            }
            else
            {
                Container.Instance.RegisterInstance<AboutViewModel>(new AboutViewModel(), "AboutViewModel");
            }
        }

        private void SaveModel()
        {
            this.SaveToIsolatedStorage<MainViewModel>(Container.Instance.Resolve<MainViewModel>("MainViewModel"));
            this.SaveToIsolatedStorage<AboutViewModel>(Container.Instance.Resolve<AboutViewModel>("AboutViewModel"));
        }


        // Code to execute if a navigation fails
        void RootFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            //LittleWatson.ReportException(e.Exception, string.Empty);
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    var exception = new ExceptionPrompt();
                    exception.Show(e.Exception);
                }
            );

            if (System.Diagnostics.Debugger.IsAttached)
            {
                // A navigation has failed; break into the debugger
                System.Diagnostics.Debugger.Break();
            }
        }

        // Code to execute on Unhandled Exceptions
        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            //LittleWatson.ReportException(e.ExceptionObject, string.Empty);

            DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    var exception = new ExceptionPrompt();
                    exception.Show(e.ExceptionObject);
                }
            );

            if (System.Diagnostics.Debugger.IsAttached)
            {
                // An unhandled exception has occurred; break into the debugger
                System.Diagnostics.Debugger.Break();
            }

            e.Handled = true;
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
