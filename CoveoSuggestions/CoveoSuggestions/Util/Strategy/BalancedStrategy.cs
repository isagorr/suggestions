using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CoveoSuggestions.Models;
using MoreLinq;

namespace CoveoSuggestions.Util.Strategy
{
    public class BalancedStratedy : IStrategy
    {
        private static readonly double DISTANCE_OFFSET = 5000;
        private static readonly double WORD_COEFF = 0.5;
        private static readonly double DISTANCE_COEFF = 0.4;
        private static readonly double POP_COEFF = 0.1;

        protected override void GetScoreInternal(RequestCity requestCity, IEnumerable<City> suggestions)
        {
            GetPopulationNorm(suggestions, out double min, out double diff);
            foreach (City city in suggestions)
            {
                double popScore = POP_COEFF * (city.population - min) / diff;
                var suggestedCity = new ResponseCity
                {
                    id = city.id,
                    name = city.ascii + ", " + city.country + " (" + city.tz + ")",
                    latitude = city.lat,
                    longitude = city.@long,
                    score = popScore + GetScoreOneCity(city, requestCity.q, requestCity.latitude, requestCity.longitude)
                };

                mSuggestedCities.Add(suggestedCity);
            }
        }

        private void GetPopulationNorm(IEnumerable<City> suggestions, out double min, out double diff)
        {
            if (null == suggestions || 0 == suggestions.Count())
            {
                min = 0;
                diff = 1;

                return;
            }

            double max = suggestions.MaxBy(city => city.population).population;
            min = suggestions.MinBy(city => city.population).population;
            diff = max - min;
        }

        protected override double GetScoreOneCity(City suggestion, string q, double? lat, double? @long)
        {
            double locationDistance = GetNormalizedDistance(suggestion.lat, suggestion.@long, lat, @long);
            // TODO consider the alternative names
            double wordDistance = GetLCS(q, suggestion.ascii);

            return WORD_COEFF * wordDistance + locationDistance * DISTANCE_COEFF;
        }

        private double GetNormalizedDistance(double? lat1, double? long1, double? lat2, double? long2)
        {
            double distance = GetDistance(lat1, long1, lat2, long2) ?? 0;
            return 1 - distance / DISTANCE_OFFSET;
        }

        private static double GetLCS(string str1, string str2)
        {
            int[,] table;
            double wordDistance = GetLCSInternal(str1, str2, out table);
            return 0 != wordDistance ? 1 / wordDistance : 0;
        }

        private static int GetLCSInternal(string str1, string str2, out int[,] matrix)
        {
            matrix = null;

            if (string.IsNullOrEmpty(str1) || string.IsNullOrEmpty(str2))
            {
                return 0;
            }

            int[,] table = new int[str1.Length + 1, str2.Length + 1];
            for (int i = 0; i < table.GetLength(0); i++)
            {
                table[i, 0] = 0;
            }
            for (int j = 0; j < table.GetLength(1); j++)
            {
                table[0, j] = 0;
            }

            for (int i = 1; i < table.GetLength(0); i++)
            {
                for (int j = 1; j < table.GetLength(1); j++)
                {
                    if (str1[i - 1] == str2[j - 1])
                        table[i, j] = table[i - 1, j - 1] + 1;
                    else
                    {
                        if (table[i, j - 1] > table[i - 1, j])
                            table[i, j] = table[i, j - 1];
                        else
                            table[i, j] = table[i - 1, j];
                    }
                }
            }

            matrix = table;
            return table[str1.Length, str2.Length];
        }
    }
}