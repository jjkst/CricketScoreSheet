namespace CricketScoreSheet.Shared.Models
{
    public class Match
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int TotalOvers { get; set; }

        public string Location { get; set; }

        public string Date { get; set; }

        public string Comments { get; set; }

        public bool Complete { get; set; }

        public Team HomeTeam { get; set; }

        public Team AwayTeam { get; set; }

        public string WinningTeamName { get; set; }

        public string UmpireOne { get; set; }

        public string UmpireTwo { get; set; }

    }
}