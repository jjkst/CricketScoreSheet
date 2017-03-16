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
using CricketScoreSheet.Shared.DataAccess.Entities;

namespace CricketScoreSheet.Shared.Validation
{
    public class TeamValidator
    {
        private List<TeamEntity> Teams { get; }

        public TeamValidator(List<TeamEntity> teams)
        {
            Teams = teams;
        }

        public IList<string> Validate(string teamname)
        {
            var results = new List<string>();

            if (string.IsNullOrEmpty(teamname))
            {
                results.Add("Team name cannot be blank.");
            }
            if (Teams.Any(t => t.Name == teamname))
            {
                results.Add($"This team name {teamname} already exist.");
            }

            return results;
        }
    }
}