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
    public class PlayerValidator
    {
        private List<PlayerEntity> PlayersPerTeam { get; }

        public PlayerValidator(List<PlayerEntity> players)
        {
            PlayersPerTeam = players;
        }

        public IList<string> Validate(string playername)
        {
            var results = new List<string>();

            if (string.IsNullOrEmpty(playername))
            {
                results.Add("Player name cannot be blank.");
            }
            if (PlayersPerTeam.Any(t => t.Name == playername))
            {
                results.Add($"This player name {playername} already exist.");
            }

            return results;
        }
    }
}