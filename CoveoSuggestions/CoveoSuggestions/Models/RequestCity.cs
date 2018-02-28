using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CoveoSuggestions.Models
{
    public class RequestCity:Geolocation
    {
        public string q { get; set; }
    }
}