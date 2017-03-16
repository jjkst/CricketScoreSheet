using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using CricketScoreSheet.Shared.Models;
using CricketScoreSheet.Shared.DataAccess.Entities;

namespace CricketScoreSheet.Shared.Validation
{
    public class MatchValidator
    {
        private List<Match> Matches { get; }

        public MatchValidator() { }

        public MatchValidator(List<Match> matches)
        {
            Matches = matches;
        }

        public IList<string> Validate(Match match)
        {
            var results = new List<string>();

            if (match == null)
            {
                results.Add("Error creating Match");
            }
            else
            {
                if (match.HomeTeam == null)
                    results.Add(@"Please select/add valid Home Team.");
                if (match.AwayTeam == null)
                    results.Add(@"Please select/add valid Away Team.");
                if (match.Location == @"Add Ground/Location.")
                    results.Add(@"Please select/add valid location.");
                if (match.TotalOvers == 0)
                    results.Add("Please select valid total overs.");
            }
            return results;
        }

        public IList<string> ValidateLocation(string location)
        {
            var results = new List<string>();
            if (string.IsNullOrEmpty(location))
            {
                results.Add("Location cannot be blank.");
            }
            if (Matches.Where(m => m.Location.ToLower() == location.ToLower()).Any())
            {
                results.Add($"This location {location} already exist.");
            }

            return results;
        }
    }
}