using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using CricketScoreSheet.Shared.DataAccess.Entities;
using SQLite;

namespace CricketScoreSheet.Shared.DataAccess.Repository
{
    public class MatchesTable : MyRepository
    {
        public MatchesTable(string path) : base(path)
        {
            try
            {
                if (!TableExist("MatchEntity"))
                {
                    Connection.CreateTableAsync<MatchEntity>();
                }
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine("Matches Table is not created");
                Console.WriteLine(ex.Message);
            }
        }

        public int MatchesCount()
        {
            try
            {
                var query = Connection.ExecuteScalarAsync<int>("select count(*) from MatchEntity");
                return query.Result;
            }
            catch (AggregateException)
            {
               return 0;
            }
        }

        public int AddMatch(MatchEntity match)
        {
            Connection.InsertAsync(match);
            return Connection.Table<MatchEntity>().OrderByDescending(id => id.Id).FirstOrDefaultAsync().Result.Id;
        }

        public List<MatchEntity> GetMatches()
        {
            try
            {
                var query = Connection.Table<MatchEntity>().ToListAsync();
                return query.Result;
            }
            catch (AggregateException)
            {
                return new List<MatchEntity>();
            }
        }

        public MatchEntity GetMatch(int id)
        {
            var query = Connection.Table<MatchEntity>().Where(t => t.Id == id);
            var val = query.ToListAsync().Result;
            return val.FirstOrDefault();
        }

        public bool UpdateMatch(MatchEntity match)
        {
            var val = true;
            try
            {
                Connection.UpdateAsync(match);
            }
            catch (Exception)
            {
                val = false;
            }
            return val;
        }

        public bool DeleteMatch(MatchEntity match)
        {
            var val = true;
            try
            {
                Connection.DeleteAsync(match);
            }
            catch (Exception)
            {
                val = false;
            }
            return val;
        }

        public bool DropMatchesTable()
        {
            var val = true;
            try
            {
                Connection.DropTableAsync<MatchEntity>();
            }
            catch (Exception)
            {
                val = false;
            }
            return val;
        }
    }
}