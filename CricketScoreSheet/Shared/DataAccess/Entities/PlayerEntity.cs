using SQLite;

namespace CricketScoreSheet.Shared.DataAccess.Entities
{
    public class PlayerEntity
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Name { get; set; }

        public int TeamId { get; set; }

        public string TeamName { get; set; }

        public int MatchId { get; set; }

        public int RunsTaken { get; set; }

        public int BallsPlayed { get; set; }

        public int Fours { get; set; }

        public int Sixes { get; set; }

        public string HowOut { get; set; }

        public int RunsGiven { get; set; }

        public int BallsBowled { get; set; }

        public int Wickets { get; set; }

        public int Dots { get; set; }

        public int NoBalls { get; set; }

        public int Wides { get; set; }

        public int Catches { get; set; }

        public int Stumpings { get; set; }

    }
}