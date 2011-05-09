using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Data.SqlTypes;
using System.Linq;
using System.Net;
using System.Text;
using Microsoft.SqlServer.Types;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;
using MyFriendsAround.Common.Entities;
using System.IO;
using System.Configuration;

namespace MyFriendsAround.Data.BLL
{
    public static class FriendsRepository
    {
        private static string profilesContainerName = string.Empty;
        private static string profileImageFormat = string.Empty;

        static FriendsRepository()
        {
            profilesContainerName = ConfigurationManager.AppSettings["azureProfilesContainer"];
            profileImageFormat = ConfigurationManager.AppSettings["profileImageFormat"];
        }

        public static List<Friend> GetFriends()
        {
            return GetFriends(0, 50);
        }

        public static List<Friend> GetFriends(int skip, int take)
        {
            using (MyFriendsModelContainer ctx = new MyFriendsModelContainer())
            {
                ctx.ContextOptions.ProxyCreationEnabled = false;
                List<Friend> list = ctx.Friends.OrderByDescending(f=> f.LastUpdated).Skip(skip).Take(take).ToList();
                list.ForEach((f) =>
                                 {
                                     SqlGeometry geom = SqlGeometry.Parse(f.LocationStr);
                                     f.Latitude = geom.STX.Value;
                                     f.Longitude = geom.STY.Value;
                                 });
                return list;
            }
        }

        /// <summary>
        /// Publish user info
        /// </summary>
        /// <param name="friend">friend obj</param>
        /// <returns>true if success</returns>
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
                    locateFriend.LocationStr = string.Format("POINT({0} {1})", friend.Latitude, friend.Longitude);

                    ctx.ObjectStateManager.ChangeObjectState(locateFriend, System.Data.EntityState.Modified);
                }
                bool success = ctx.SaveChanges() > 0;
                if (success)
                {
                    //update geography field
                    ctx.ExecuteFunction("UpdateFriendLocationById", new ObjectParameter[]
                                {
                                    new ObjectParameter("FriendID", friend.Id), 
                                });
                }
                return success;
            }
        }

        private static bool storageInitialized = false;
        private static object gate = new Object();
        private static CloudBlobClient blobStorage;

        public static bool UpdatePicture(string userId, byte[] userPicture)
        {
            bool success = false;
            try
            {
                InitializeStorage();

                // upload the image to blob storage
                CloudBlobContainer container = blobStorage.GetContainerReference(profilesContainerName);
                string uniqueBlobName = string.Format(profileImageFormat, userId);
                CloudBlockBlob blob = container.GetBlockBlobReference(uniqueBlobName);
                blob.DeleteIfExists();
                blob.Properties.ContentType = "image/jpeg";
                using (MemoryStream ms = new MemoryStream(userPicture))
                {
                    blob.UploadFromStream(ms);
                }
                //
                System.Diagnostics.Trace.TraceInformation("Uploaded image '{0}' to blob storage as '{1}'", userId, uniqueBlobName);
                //
                success = true;
            }
            catch (Exception)
            {
                throw;
            }

            return success;
        }

        private static void InitializeStorage()
        {
            if (storageInitialized)
            {
                return;
            }

            lock (gate)
            {
                if (storageInitialized)
                {
                    return;
                }

                try
                {
                    // read account configuration settings
                    var storageAccount = CloudStorageAccount.FromConfigurationSetting("DataConnectionString");

                    // create blob container for images
                    blobStorage = storageAccount.CreateCloudBlobClient();
                    CloudBlobContainer container = blobStorage.GetContainerReference(profilesContainerName);
                    container.CreateIfNotExist();

                    // configure container for public access
                    var permissions = container.GetPermissions();
                    permissions.PublicAccess = BlobContainerPublicAccessType.Container;
                    container.SetPermissions(permissions);
                }
                catch (WebException)
                {
                    throw new WebException("Storage services initialization failure. "
                        + "Check your storage account configuration settings. If running locally, "
                        + "ensure that the Development Storage service is running.");
                }

                storageInitialized = true;
            }
        }

    }
}
