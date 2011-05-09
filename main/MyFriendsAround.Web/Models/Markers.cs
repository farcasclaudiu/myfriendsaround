using System.Collections.Generic;

namespace GoogleMaps.Models
{
    public class MarkerList
    {
       public List<Marker> markers { get; set; }
    }
    
    public class Marker
    {
        public string lat { get; set; }
        public string lng { get; set; }
        public string html { get; set; }
        public string label { get; set; }
        public string icon { get; set; }
        public string lastUpdated { get; set; }
    }
}
