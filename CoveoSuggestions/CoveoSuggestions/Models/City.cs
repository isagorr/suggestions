using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CoveoSuggestions.Models
{
    public class City
    {
        public int id { set; get; }
        public string name { set; get; }
        public string ascii { set; get; }
        public string alt_name { set; get; }
        public double lat { set; get; }
        public double @long { set; get; }
        public int population { set; get; }
        public string country { set; get; }
        public string tz { set; get; }
    }
}