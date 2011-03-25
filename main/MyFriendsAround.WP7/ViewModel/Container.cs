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
using MicroIoc;

namespace MyFriendsAround.WP7.ViewModel
{
    public sealed class Container
    {
        Container()
        {
        }

        public static IMicroIocContainer Instance
        {
            get
            {
                return Nested.instance;
            }
        }

        class Nested
        {
            // Explicit static constructor to tell C# compiler
            // not to mark type as before field init
            static Nested()
            {
            }
            internal static readonly IMicroIocContainer instance =
                new MicroIocContainer();
        }
    }
}
