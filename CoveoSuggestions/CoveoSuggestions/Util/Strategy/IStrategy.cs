using CoveoSuggestions.Models;
using CsvHelper;
using Gma.DataStructures.StringSearch;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoveoSuggestions.Util.Strategy
{
    public abstract class IStrategy
    {
        private static readonly double EARTH_RAD = 6378.16;
        private static readonly string CSV_PATH = "cities.csv";

        private static Trie<City> sTrie;
        protected List<ResponseCity> mSuggestedCities;

        static IStrategy()
        {
            using (var fileReader = System.IO.File.OpenText(CSV_PATH))
            using (var csv = new CsvReader(fileReader))
            {
                csv.Configuration.Delimiter = $"\t";
                csv.Configuration.HasHeaderRecord = true;
                csv.Configuration.BadDataFound = null;

                var result = csv.GetRecords<City>().ToList();
                sTrie = new Trie<City>();

                foreach (City city in result)
                {
                    sTrie.Add(city.ascii.ToLower(), city);
                }
            }
        }

        public List<ResponseCity> GetScore(RequestCity requestCity)
        {
            mSuggestedCities = new List<ResponseCity>();

            IEnumerable<City> suggestions = sTrie.Retrieve(requestCity.q.ToLower());
            GetScoreInternal(requestCity, suggestions);
            Normalize();

            return mSuggestedCities;
        }

        protected virtual void GetScoreInternal(RequestCity requestCity, IEnumerable<City> suggestions)
        {
            foreach (City city in suggestions)
            {
                var suggestedCity = new ResponseCity
                {
                    name = city.ascii,
                    latitude = city.lat,
                    longitude = city.@long,
                    score = GetScoreOneCity(city, requestCity.q, requestCity.latitude, requestCity.longitude)
                };

                mSuggestedCities.Add(suggestedCity);
            }
        }

        protected abstract double GetScoreOneCity(City suggestions, string q, double? lat, double? @long);

        protected static double? GetDistance(double? lat1, double? long1, double? lat2, double? long2)
        {
            if (null == lat1 || null == lat2 || null == long1 || null == long2)
            {
                return null;
            }
            double newlong1 = long1 ?? default(int);
            double newlong2 = long2 ?? default(int);
            double newlat1 = lat1 ?? default(int);
            double newlat2 = lat2 ?? default(int);

            double dlon = Radians(newlong2 - newlong1);
            double dlat = Radians(newlat2 - newlat1);

            double a = (Math.Sin(dlat / 2) * Math.Sin(dlat / 2))
                + Math.Cos(Radians(newlat1)) * Math.Cos(Radians(newlat2)) * (Math.Sin(dlon / 2) * Math.Sin(dlon / 2));
            double angle = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return angle * EARTH_RAD;
        }

        private static double Radians(double x)
        {
            return x * Math.PI / 180;
        }

        private void Normalize()
        {
            if (0 == mSuggestedCities.Count)
            {
                return;
            }
            double max = mSuggestedCities.MaxBy(city => city.score).score;
            double min = mSuggestedCities.MinBy(city => city.score).score;
            double diff = max - min;

            foreach (ResponseCity city in mSuggestedCities)
            {
                city.score = 0 == diff ? 1.0 : (city.score - min) / diff;
                city.score = Math.Round(city.score, 3);
            }


            mSuggestedCities.Sort((city1, city2) => city2.score.CompareTo(city1.score));
        }

    }
}
