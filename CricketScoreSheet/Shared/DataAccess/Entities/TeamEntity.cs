using SQLite;

namespace CricketScoreSheet.Shared.DataAccess.Entities
{
    public class TeamEntity
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Name { get; set; }

        public int HighestScore { get; set; }

        public int LowestScore { get; set; }

        public int Wins { get; set; }

        public int Loss { get; set; }
    }
}