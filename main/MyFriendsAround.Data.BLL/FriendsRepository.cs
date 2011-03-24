using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using Microsoft.SqlServer.Types;
using MyFriendsAround.Common.Entities;

namespace MyFriendsAround.Data.BLL
{
    public static class FriendsRepository
    {
        public static List<Friend> GetFriends()
        {            
            using (MyFriendsModelContainer ctx = new MyFriendsModelContainer())
            {
                ctx.ContextOptions.ProxyCreationEnabled = false;
                List<Friend> list = ctx.Friends.ToList();
                list.ForEach((f) =>
                                 {
                                     SqlGeometry geom = SqlGeometry.Parse(f.LocationStr);
                                     f.Latitude = geom.STX.Value;
                                     f.Longitude = geom.STY.Value;
                                 });
                return list;
            }
        }

        public static bool PublishLocation(Friend friend)
        {
            using (MyFriendsModelContainer ctx = new MyFriendsModelContainer())
            {
                Friend locateFriend = ctx.Friends.SingleOrDefault(f => f.Id == friend.Id);
                if (locateFriend == null)
                {
                    ctx.Friends.AddObject(friend);
                    ctx.ObjectStateManager.ChangeObjectState(friend, System.Data.EntityState.Added);
                }
                else
                { 
                    //update
                    locateFriend.FriendName = friend.FriendName;
                    locateFriend.FriendImageUrl = friend.FriendImageUrl;
                    locateFriend.LastUpdated = friend.LastUpdated;
                    locateFriend.LocationStr = friend.LocationStr;

                    ctx.ObjectStateManager.ChangeObjectState(locateFriend, System.Data.EntityState.Modified);
                }
                bool success = ctx.SaveChanges() > 0;
                if (success)
                {
                    //update gegraphy field
                    ctx.ExecuteFunction("UpdateFriendLocationById", new ObjectParameter[]
                                {
                                    new ObjectParameter("FriendID", friend.Id), 
                                });
                }
                return success;
            }
        }
    }
}
