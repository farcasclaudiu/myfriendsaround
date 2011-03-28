using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using MyFriendsAround.Common.Entities;

namespace MyFriendsAround.WP7.Service
{
    public class FriendsListEventArgs: EventArgs
    {
        public List<Friend> Friends { get; set; }
        public Exception Error { get; set; }
    }
}
