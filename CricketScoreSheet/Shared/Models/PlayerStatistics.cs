using CricketScoreSheet.Shared.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CricketScoreSheet.Shared.Models
{
    public class PlayerStatistics
    {
        public string PlayerName { get; set; }

        public string TeamName { get; set; }

        public int Matches { get; set; }

        public int Innings { get; set; }

        public int NotOuts { get; set; }

        public int Runs { get; set; }

        public int HS { get; set; }

        public int Balls { get; set; }

        public int Hundreds { get; set; }

        public int Fifties { get; set; }

        public decimal BattingAvg { get; set; }

        public decimal SR { get; set; }

        public int BallsBowled { get; set; }

        public int Maiden { get; set; }

        public int RunsGiven { get; set; }

        public int Wickets { get; set; }

        public string BB { get; set; }

        public int FWI { get; set; }

        public int TWI { get; set; }

        public decimal BowlingAvg { get; set; }

        public decimal Econ { get; set; }

        public decimal BowlingSR { get; set; }

        public int Catches { get; set; }

        public int Stumpings { get; set; }

        public PlayerStatistics(List<PlayerEntity> players)
        {
            PlayerName = players.First().Name;
            TeamName = players.First().TeamName;
            Matches = players.Count;

            Innings = players.Count(p => p.BallsPlayed > 0 || (p.HowOut != null && p.HowOut.ToLower() != "not out"));
            NotOuts = players.Count(p => p.BallsPlayed > 0 && (p.HowOut == null || p.HowOut.ToLower() == "not out"));
            Runs = players.Sum(r => r.RunsTaken);
            HS = players.Max(r => r.RunsTaken);
            Balls = players.Sum(r => r.BallsPlayed);
            Hundreds = players.Count(p => p.RunsTaken >= 100);
            Fifties = players.Count(p => p.RunsTaken >= 50 && p.RunsTaken < 100);
            var noOfOuts = (Matches - NotOuts) == 0 ? 1 : (Matches - NotOuts);
            BattingAvg = decimal.Round((decimal)Runs / noOfOuts, 2, MidpointRounding.AwayFromZero);
            SR = (Balls == 0) ? 0 : decimal.Round((decimal)Runs * 100 / Balls, 2, MidpointRounding.AwayFromZero);

            BallsBowled = players.Sum(p => p.BallsBowled);
            int ab; var ao = Math.DivRem(BallsBowled, 6, out ab);
            var overs = ao + "." + ab;

            Maiden = players.Sum(p => p.Maiden);
            RunsGiven = players.Sum(p => p.RunsGiven);
            Wickets = players.Sum(p => p.Wickets);
            var playerBb = players.Where(s => s.Wickets == players.Max(w => w.Wickets))
                                .OrderBy(m => m.RunsGiven).First();
            BB = $"{playerBb.Wickets}/{playerBb.RunsGiven}";
            FWI = players.Count(p => p.Wickets > 4);
            TWI = players.Count(p => p.Wickets > 9);
            BowlingAvg = (Wickets == 0) ? 0 : decimal.Round((decimal)RunsGiven / Wickets, 2, MidpointRounding.AwayFromZero);
            Econ = (BallsBowled == 0) ? 0 : decimal.Round((decimal)RunsGiven / Convert.ToDecimal(overs), 2, MidpointRounding.AwayFromZero);
            BowlingSR = (Wickets == 0) ? 0 : decimal.Round((decimal)BallsBowled / Wickets, 2, MidpointRounding.AwayFromZero);

            Catches = players.Sum(p => p.Catches);
            Stumpings = players.Sum(p => p.Stumpings);
        }
    }
}