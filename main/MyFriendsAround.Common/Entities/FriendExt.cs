using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyFriendsAround.Common.Entities
{
    public partial class Friend
    {
        public virtual double Latitude
        {
            get;
            set;
        }

        public virtual double Longitude
        {
            get;
            set;
        }
    }
}
