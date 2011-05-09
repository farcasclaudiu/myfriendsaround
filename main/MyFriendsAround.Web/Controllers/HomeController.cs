using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GoogleMaps.Models;
using MyFriendsAround.Data.BLL;

namespace MyFriendsAround.Web.Controllers
{
    //[HandleError]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Message = "Welcome to My Friends Around community site.";

            return View();
        }

        public ActionResult About()
        {
            return View();
        }

        [OutputCache(NoStore = true, Duration = 0)]
        public ActionResult GetMarkers()
        {
            // This would normally be our call to the Db, else we could populate this
            // with some data as Ive done here.
            MarkerList markers = GetMarkersObjects();

            return Json(markers, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Gets the markers. This data could be filled with whatever you 
        /// set from your DB.
        /// </summary>
        /// <returns></returns>
        public static MarkerList GetMarkersObjects()
        {
            MarkerList list = new MarkerList();
            list.markers = new List<Marker>();

            FriendsRepository.GetFriends().ForEach(f =>
            {
                string imageUrl = string.Format("https://myfriendsaround.blob.core.windows.net/profiles/profile_{0}.jpg", f.Id);
                Marker marker = new Marker
                                    {
                                        html = f.FriendName,
                                        icon = imageUrl,
                                        lat = f.Latitude.ToString(),
                                        lng = f.Longitude.ToString(),
                                        label = f.FriendName,
                                        lastUpdated = f.LastUpdated.ToString()
                                    };
                list.markers.Add(marker);
            });

            return list;
        }
    }
}
