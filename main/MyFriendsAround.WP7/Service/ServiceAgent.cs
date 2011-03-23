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
using Hammock;
using Hammock.Streaming;
using Hammock.Tasks;
using Hammock.Web;
using MyFriendsAround.Common.Entities;
using Hammock.Caching;
using CacheMode = Hammock.Caching.CacheMode;

namespace MyFriendsAround.WP7.Service
{
    public static class ServiceAgent
    {

        #region GetFriends

        private static EventHandler<FriendsListEventArgs> friendscallback;

        public static void GetFriends(EventHandler<FriendsListEventArgs> callback)
        {
            var serializer = new Hammock.Serialization.HammockDataContractJsonSerializer();
            RestClient client = new RestClient
            {
                Authority = "http://localhost.:55672/myfriends",
                Serializer = serializer,
                Deserializer = serializer
            };
            RestRequest request = new RestRequest
                                      {
                                          Path = "GetFriends" + "?timestamp=" + DateTime.Now.Ticks.ToString()
                                      };
            friendscallback = callback;
            client.BeginRequest(request, new RestCallback<List<Friend>>(GetFriendsCallback));
        }

        public static void GetFriendsCallback(RestRequest request, RestResponse<List<Friend>> response, object userState)
        {
            if (response.StatusCode == HttpStatusCode.OK)
            {
                List<Friend> list = response.ContentEntity;
                friendscallback.Invoke(null, new FriendsListEventArgs() { Friends = list });
            }
        }

        #endregion



        #region PublishLocation


        public static EventHandler<PublishLocationEventArgs> publishlocationcallback;
        public static void PublishLocation(Friend friend, EventHandler<PublishLocationEventArgs> callback)
        {
            var serializer = new Hammock.Serialization.HammockDataContractJsonSerializer();
            RestClient client = new RestClient
            {
                Authority = "http://localhost.:55672/myfriends",
                Serializer = serializer,
                Deserializer = serializer
            };
            RestRequest request = new RestRequest
            {
                Method = WebMethod.Post,
                Path = "PublishLocation",
                Entity = friend
            };
            publishlocationcallback = callback;
            client.BeginRequest(request, new RestCallback<bool>(PublishLocationCallback));
        }

        public static void PublishLocationCallback(RestRequest request, RestResponse<bool> response, object userState)
        {
            if (response.StatusCode == HttpStatusCode.OK)
            {
                bool success = response.ContentEntity;
                publishlocationcallback.Invoke(null, new PublishLocationEventArgs() { IsSuccess = success });
            }
        }

        #endregion
    }
}
