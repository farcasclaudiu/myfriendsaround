using System;
using System.Device.Location;
using System.Security;
using System.Windows;
using System.Windows.Data;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Controls.Maps;
using Microsoft.Silverlight.Testing;
using MyFriendsAround.WP7.ViewModel;
using MyFriendsAround.WP7.Views;

namespace MyFriendsAround.WP7
{
    public partial class MainPage : PhoneApplicationPage
    {
        //private Map MyMap;

        // Constructor
        public MainPage()
        {
            InitializeComponent();

            Messenger.Default.Register<DialogMessage>(
               this,
               msg =>
               {
                   DispatcherHelper.CheckBeginInvokeOnUI(() =>
                                {

                                    var result = MessageBox.Show(
                                        msg.Content,
                                        msg.Caption,
                                        msg.Button);

                                    // Send callback
                                    msg.ProcessCallback(result);
                                });
               });

            this.Loaded += new RoutedEventHandler(MainPage_Loaded);

        }



        void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
#if TESTING
            DoTests();
#endif
        }

        private void DoTests()
        {
            var testPage = UnitTestSystem.CreateTestPage();
            IMobileTestPage imobileTPage = testPage as IMobileTestPage;
            BackKeyPress += (s, arg) =>
                                {
                                    bool navigateBackSuccessfull = imobileTPage.NavigateBack(); arg.Cancel = navigateBackSuccessfull;
                                }; (Application.Current.RootVisual as PhoneApplicationFrame).Content = testPage;

            map.SetView(LocationRect.CreateLocationRect());
        }

        private void map_ViewChangeEnd(object sender, MapEventArgs e)
        {
            //to workaround the issue
            // https://connect.microsoft.com/VisualStudio/feedback/details/643990/wp7-bing-maps-control-throwing-unspecified-error-with-mapitemscontrol
            //
            (DataContext as MainViewModel).MapViewChangedCommand.Execute(map.BoundingRectangle);
        }

    }
}
