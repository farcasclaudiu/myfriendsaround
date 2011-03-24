using System.Text;
using System.Web.Mvc;

namespace GoogleMaps.Models
{
    public static class MapsHelper
    {
        /// <summary>
        /// Draws the map.
        /// </summary>
        /// <param name="helper">The helper.</param>
        /// <param name="key">The key.</param>
        /// <param name="jsonUrl">The json URL.</param>
        /// <param name="mapWidth">Width of the map.</param>
        /// <param name="mapHeight">Height of the map.</param>
        /// <returns>The html of the map.</returns>
        public static string DrawMap(this HtmlHelper helper, string key, string jsonUrl, string mapWidth, string mapHeight)
        {
            StringBuilder mapHtml = new StringBuilder();

            // Google Maps API Link
            mapHtml.Append("<script src=\"http://maps.google.com/maps?file=api&amp;v=2&amp;sensor=false&amp;key=");
            mapHtml.Append(key);
            mapHtml.Append("\" type=\"text/javascript\"></script>");

            // Hidden value
            mapHtml.Append("<input type=\"hidden\" id=\"MarkerUrl\" value=\"");
            mapHtml.Append(jsonUrl);
            mapHtml.Append("\" />");

            // Map Div
            mapHtml.Append("<div id=\"map\" style=\"width: ");
            mapHtml.Append(mapWidth);
            mapHtml.Append("px; height: ");
            mapHtml.Append(mapHeight);
            mapHtml.Append("px\"></div>");

            // Maps javascript file
            mapHtml.Append("<script type=\"text/javascript\" src=\"/Scripts/GoogleMapping.js\"></script>");

            return mapHtml.ToString();
        }
    }
}
