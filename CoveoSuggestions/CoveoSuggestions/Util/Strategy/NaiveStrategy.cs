using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CoveoSuggestions.Models;

namespace CoveoSuggestions.Util.Strategy
{
    public class NaiveStrategy : IStrategy
    {
        private static readonly double PARTIAL_MATCH_WEIGHT = 0.9;
        private static readonly double NAME_WEIGHT = 0.7;
        private static readonly double DISTANCE_WEIGHT = 0.3;

        protected override double GetScoreOneCity(City suggestion, string q, double? lat, double? @long)
        {
            double nameMatch = suggestion.ascii.ToLower().Equals(q) ? 1 : PARTIAL_MATCH_WEIGHT;
            double? distance = GetDistance(suggestion.lat, suggestion.@long, lat, @long);
            double normalizedDistance;
            switch (distance)
            {
                case null:
                    normalizedDistance = 1.0;
                    break;
                case -1:
                    normalizedDistance = 0;
                    break;
                default:
                    normalizedDistance = 1 / (double)distance;
                    break;

            }

            return NAME_WEIGHT * nameMatch + DISTANCE_WEIGHT * normalizedDistance;
        }
    }
}