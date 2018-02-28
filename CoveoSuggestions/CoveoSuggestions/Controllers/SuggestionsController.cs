using CoveoSuggestions.Models;
using CoveoSuggestions.Util.Strategy;
using CsvHelper;
using Gma.DataStructures.StringSearch;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CoveoSuggestions.Controllers
{
    public class SuggestionsController : ApiController
    {

        static private IStrategy sStrategy;

        static SuggestionsController()
        {
            sStrategy = new BalancedStratedy();
        }
        // GET api/suggestions
        public List<ResponseCity> Get()
        {
            var requestCity = parseParams();
            return null != requestCity ? sStrategy.GetScore(requestCity) : new List<ResponseCity>();
        }

        // TODO make it prettier
        private RequestCity parseParams()
        {
            var requestParams = ControllerContext.Request.GetQueryNameValuePairs();
            if (requestParams.Count() < 1 ||
                "q" != requestParams.ElementAt(0).Key)
            {
                return null;
            }

            var requestCity = new RequestCity();
            requestCity.q = requestParams.ElementAt(0).Value;
            if (requestParams.Count() > 1 && Double.TryParse(requestParams.ElementAt(1).Value, out double value))
            {
                if ("lat" == requestParams.ElementAt(1).Key || "latitude" == requestParams.ElementAt(1).Key)
                {
                    requestCity.latitude = Double.Parse(requestParams.ElementAt(1).Value);
                }
                else if ("long" == requestParams.ElementAt(1).Key || "longitude" == requestParams.ElementAt(1).Key)
                {
                    requestCity.latitude = Double.Parse(requestParams.ElementAt(1).Value);
                }
            }
            if (requestParams.Count() > 2 && Double.TryParse(requestParams.ElementAt(2).Value, out value))
            {
                if ("lat" == requestParams.ElementAt(2).Key || "latitude" == requestParams.ElementAt(2).Key)
                {
                    requestCity.latitude = Double.Parse(requestParams.ElementAt(2).Value);
                }
                else if ("long" == requestParams.ElementAt(2).Key || "longitude" == requestParams.ElementAt(2).Key)
                {
                    requestCity.longitude = Double.Parse(requestParams.ElementAt(2).Value);
                }
            }

            return requestCity;
        }
    }
}
