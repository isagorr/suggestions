using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CoveoSuggestions.Models
{
    public class ResponseCity:Geolocation
    {
        public int id { get; set; }
        public string name { get; set; }
        public double score { get; set; }
    }
}