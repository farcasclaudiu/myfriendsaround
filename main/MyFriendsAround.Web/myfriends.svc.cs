using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;
using MyFriendsAround.Common.Entities;
using MyFriendsAround.Data;
using MyFriendsAround.Data.BLL;

namespace MyFriendsAround.Web
{
    [ServiceContract(Namespace = "http://myfriendsaround.com/")]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class myfriends
    {

        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json)]
        public List<Friend> GetFriends()
        {
            return FriendsRepository.GetFriends();
        }


        [OperationContract]
        [WebInvoke(ResponseFormat = WebMessageFormat.Json, Method = "POST")]
        public bool PublishLocation(Friend friend)
        {
            return FriendsRepository.PublishLocation(friend);
        }
        
    }
}
