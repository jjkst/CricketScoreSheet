using CricketScoreSheet.Shared.DataAccess.Entities;
using System.Collections.Generic;
using System.Linq;

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

            if (string.IsNullOrEmpty(playername.TrimEnd('*', '†')))
            {
                results.Add("Player name cannot be blank.");
            }
            if (PlayersPerTeam.Any(t => t.Name.TrimEnd('*', '†') == playername.TrimEnd('*', '†')))
            {
                results.Add($"This player name {playername.TrimEnd('*', '†')} already exist.");
            }
            if (PlayersPerTeam.Any(t => playername.Contains("*") && t.Name.Contains("*")))
            {
                results.Add($"Captain already exist. If you want to add captain, please delete previous captain.");
            }
            if (PlayersPerTeam.Any(t => playername.Contains("†") && t.Name.Contains("†")))
            {
                results.Add($"Keeper already exist. If you want to add keeper, please delete previous keeper.");
            }
            return results;
        }
    }
}