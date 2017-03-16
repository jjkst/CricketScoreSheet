using SQLite;

namespace CricketScoreSheet.Shared.DataAccess.Entities
{
    public class MatchEntity
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string MatchDate { get; set; }

        public int TotalOvers { get; set; }

        public string Location { get; set; }

        public bool MatchComplete { get; set; }

        public string WinningTeamName { get; set; }

        public string Comments { get; set; }

        public int HomeTeamId { get; set; }

        public string HomeTeamName { get; set; }

        public int HomeTeamRuns { get; set; }

        public int HomeTeamWickets { get; set; }

        public int HomeTeamBalls { get; set; }

        public int HomeTeamNoBalls { get; set; }

        public int HomeTeamWides { get; set; }

        public int HomeTeamByes { get; set; }

        public int HomeTeamLegByes { get; set; }

        public bool HomeTeamInningsComplete { get; set; }

        public int AwayTeamId { get; set; }

        public string AwayTeamName { get; set; }

        public int AwayTeamRuns { get; set; }

        public int AwayTeamWickets { get; set; }

        public int AwayTeamBalls { get; set; }

        public int AwayTeamNoBalls { get; set; }

        public int AwayTeamWides { get; set; }

        public int AwayTeamByes { get; set; }

        public int AwayTeamLegByes { get; set; }

        public bool AwayTeamInningsComplete { get; set; }

    }
}