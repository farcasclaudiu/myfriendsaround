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
    using MyFriendsAround.WP7.Helpers.Navigation;

namespace MyFriendsAround.WP7.ViewModel
{
    public class ViewModelBase : GalaSoft.MvvmLight.ViewModelBase
    {
        private object context;
        public object Context
        {
            get { return context; }
            set
            {
                if (context == value)
                    return;
                context = value;
                RaisePropertyChanged("Context");
            }
        }

        /// <summary>
        /// Gets PageNavigation from Container
        /// </summary>
        public IPageNavigation PageNav
        {
            get
            {
                IPageNavigation pageNav =
                    (IPageNavigation)Container.Instance.Resolve(typeof(PageNavigation), "PageNavigation");
                return pageNav;
            }
        }
    }
}
