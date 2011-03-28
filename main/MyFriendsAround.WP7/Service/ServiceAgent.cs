using System;
using System.Collections.Generic;
using System.Net;
using Hammock;
using Hammock.Web;
using MyFriendsAround.Common.Entities;

namespace MyFriendsAround.WP7.Service
{
    public static class ServiceAgent
    {

        private static int _timeOut = 10;

        #region GetFriends

        private static EventHandler<FriendsListEventArgs> friendscallback;

        public static void GetFriends(EventHandler<FriendsListEventArgs> callback)
        {
            var serializer = new Hammock.Serialization.HammockDataContractJsonSerializer();
            RestClient client = new RestClient
            {
                Authority = "http://localhost.:55672/myfriends",
                Timeout = new TimeSpan(0, 0, 0, _timeOut),
                Serializer = serializer,
                Deserializer = serializer
            };
            RestRequest request = new RestRequest
                                      {
                                          Path = "GetFriends" + "?timestamp=" + DateTime.Now.Ticks.ToString(),
                                          Timeout = new TimeSpan(0, 0, 0, _timeOut)
                                      };
            friendscallback = callback;
            try
            {
                client.BeginRequest(request, new RestCallback<List<Friend>>(GetFriendsCallback));
            }
            catch (Exception ex)
            {
                friendscallback.Invoke(null, new FriendsListEventArgs() { Friends = null, Error = ex });
            }
            
        }

        public static void GetFriendsCallback(RestRequest request, RestResponse<List<Friend>> response, object userState)
        {
            if (response.StatusCode == HttpStatusCode.OK)
            {
                List<Friend> list = response.ContentEntity;
                friendscallback.Invoke(null, new FriendsListEventArgs() { Friends = list });
            }
            else
            {
                friendscallback.Invoke(null, new FriendsListEventArgs() { Friends = null, Error = new Exception("Communication Error!") });
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
                Timeout = new TimeSpan(0, 0, 0, _timeOut),
                Serializer = serializer,
                Deserializer = serializer
            };
            RestRequest request = new RestRequest
            {
                Timeout = new TimeSpan(0, 0, 0, _timeOut),
                Method = WebMethod.Post,
                Path = "PublishLocation",
                Entity = friend
            };
            publishlocationcallback = callback;
            try
            {
                client.BeginRequest(request, new RestCallback<bool>(PublishLocationCallback));
            }
            catch (Exception ex)
            {
                publishlocationcallback.Invoke(null, new PublishLocationEventArgs() { IsSuccess = false, Error = ex });
            }
            
        }

        public static void PublishLocationCallback(RestRequest request, RestResponse<bool> response, object userState)
        {
            if (response.StatusCode == HttpStatusCode.OK)
            {
                bool success = response.ContentEntity;
                publishlocationcallback.Invoke(null, new PublishLocationEventArgs() { IsSuccess = success });
            }
            else
            {
                publishlocationcallback.Invoke(null, new PublishLocationEventArgs() { IsSuccess = false, Error = new Exception("Communication Error!")});
            }
        }

        #endregion
    }
}
