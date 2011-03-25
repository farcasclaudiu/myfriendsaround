using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using System.Windows.Navigation;

namespace MyFriendsAround.WP7.Helpers.Navigation
{
    public class PageNavigation : IPageNavigation
    {

        private PhoneApplicationFrame mainFrame;
        private bool EnsureMainFrame()
        {
            if (mainFrame != null)
            {
                return true;
            }
            mainFrame = Application.Current.RootVisual as PhoneApplicationFrame;
            if (mainFrame != null)
            {
                // Could be null if the app runs inside a design tool
                mainFrame.Navigating += (s, e) =>
                {
                    if (Navigating != null)
                    {
                        Navigating(s, e);
                    }
                };
                return true;
            }
            return false;
        }


        #region IPageNavigation implementation
        
        public event NavigatingCancelEventHandler Navigating;

        private object currentContext;
        public object CurrentContext
        {
            get { return this.currentContext; }
        }

        public void NavigateTo(Uri pageUri)
        {
            if (pageUri == null)
                throw new ArgumentNullException("uri");
            if (EnsureMainFrame())
                this.NavigateTo(pageUri, null);
        }

        public void NavigateTo(Uri pageUri, object context)
        {
            if (pageUri == null)
                throw new ArgumentNullException("uri");
            if (EnsureMainFrame())
            {
                this.currentContext = context;
                mainFrame.Navigate(pageUri);
            }
        }

        public void GoBack()
        {
            if (EnsureMainFrame() && mainFrame.CanGoBack)
            {
                mainFrame.GoBack();
            }
        }

        #endregion
    }
}
