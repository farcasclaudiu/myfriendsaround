using System;
using System.Windows;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using Microsoft.Phone.Controls;
using Microsoft.Silverlight.Testing;

namespace MyFriendsAround.WP7
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {

            DispatcherHelper.Initialize();
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
        }

    }
}
