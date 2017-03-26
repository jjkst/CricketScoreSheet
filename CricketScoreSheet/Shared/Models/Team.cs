using CricketScoreSheet.Shared.DataAccess.Entities;
using System.Collections.Generic;

namespace CricketScoreSheet.Shared.Models
{
    public class Team
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int Runs { get; set; }

        public int Balls { get; set; }

        public int Wickets { get; set; }

        public int NoBalls { get; set; }

        public int Wides { get; set; }

        public int Byes { get; set; }

        public int LegByes { get; set; }

        public bool InningsComplete { get; set; }

        public List<PlayerEntity> Players { get; set; }
    }
}