using CricketScoreSheet.Shared.DataAccess.Entities;
using CricketScoreSheet.Shared.DataAccess.Repository;
using System.Collections.Generic;

namespace CricketScoreSheet.Shared.Services
{
    public class UmpireService
    {
        private UmpiresTable UmpiresTable;

        public UmpireService()
        {
            UmpiresTable = new UmpiresTable(Helper.DbPath);
        }

        public int UmpiresCount() => UmpiresTable.UmpiresCount();

        public int AddUmpire(int matchId, string umpirename, bool primary)
        {
            UmpireEntity newUmpire = new UmpireEntity
            {
                Name = umpirename,
                MatchId = matchId,
                Primary = primary
            };
            return UmpiresTable.AddUmpire(newUmpire);
        }

        public List<UmpireEntity> GetUmpires()
        {
            return UmpiresTable.GetUmpires();
        }

        public UmpireEntity GetUmpire(int id)
        {
            return UmpiresTable.GetUmpire(id);
        }

        public bool UpdateUmpire(UmpireEntity umpire)
        {
            return UmpiresTable.UpdateUmpire(umpire);
        }

        public bool DeleteUmpire(UmpireEntity umpire)
        {
            return UmpiresTable.DeleteUmpire(umpire);
        }

        public bool DropUmpiresTable()
        {
            return UmpiresTable.DropUmpiresTable();
        }
    }
}