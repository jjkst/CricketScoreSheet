using CricketScoreSheet.Shared.DataAccess.Entities;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CricketScoreSheet.Shared.DataAccess.Repository
{
    public class UmpiresTable : MyRepository
    {
        public UmpiresTable(string path) : base(path)
        {
            try
            {
                if (!TableExist("UmpireEntity"))
                {
                    Connection.CreateTableAsync<UmpireEntity>();
                }
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine("Umpire Table is not created");
                Console.WriteLine(ex.Message);
            }
        }

        public int UmpiresCount()
        {
            try
            {
                var query = Connection.ExecuteScalarAsync<int>("select count(*) from UmpireEntity");
                return query.Result;
            }
            catch (AggregateException)
            {
                return 0;

            }
        }

        public int AddUmpire(UmpireEntity umpire)
        {
            Connection.InsertAsync(umpire);
            return Connection.Table<UmpireEntity>().OrderByDescending(id => id.Id).FirstOrDefaultAsync().Result.Id;
        }

        public List<UmpireEntity> GetUmpires()
        {
            try
            {
                var query = Connection.Table<UmpireEntity>().ToListAsync();
                return query.Result;
            }
            catch (AggregateException)
            {
                return new List<UmpireEntity>();
            }
        }

        public UmpireEntity GetUmpire(int id)
        {
            var query = Connection.Table<UmpireEntity>().Where(t => t.Id == id);
            var val = query.ToListAsync().Result;
            return val.FirstOrDefault();
        }

        public bool UpdateUmpire(UmpireEntity umpire)
        {
            var val = true;
            try
            {
                Connection.UpdateAsync(umpire);
            }
            catch (Exception)
            {
                val = false;
            }
            return val;
        }

        public bool DeleteUmpire(UmpireEntity umpire)
        {
            var val = true;
            try
            {
                Connection.DeleteAsync(umpire);
            }
            catch (Exception)
            {
                val = false;
            }
            return val;
        }

        public bool DropUmpiresTable()
        {
            var val = true;
            try
            {
                Connection.DropTableAsync<UmpireEntity>();
            }
            catch (Exception)
            {
                val = false;
            }
            return val;
        }
    }
}