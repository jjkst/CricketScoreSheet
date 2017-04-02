using SQLite;

namespace CricketScoreSheet.Shared.DataAccess.Entities
{
    public class UmpireEntity
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public int MatchId { get; set; }

        public string Name { get; set; }

        public bool Primary { get; set; }
    }
}