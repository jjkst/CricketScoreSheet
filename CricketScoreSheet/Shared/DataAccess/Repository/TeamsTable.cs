using CricketScoreSheet.Shared.DataAccess.Entities;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CricketScoreSheet.Shared.DataAccess.Repository
{
    public class TeamsTable : MyRepository
    {
        public TeamsTable(string path) : base(path)
        {
            try
            {
                if (!TableExist("TeamEntity"))
                {
                    Connection.CreateTableAsync<TeamEntity>();
                }
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine("Teams Table is not created");
                Console.WriteLine(ex.Message);
            }
        }

        public int TeamsCount()
        {
            try
            {
                var query = Connection.ExecuteScalarAsync<int>("select count(*) from TeamEntity");
                return query.Result;
            }
            catch (AggregateException)
            {
                return 0;

            }
        }

        public int AddTeam(TeamEntity team)
        {
            Connection.InsertAsync(team);
            return Connection.Table<TeamEntity>().OrderByDescending(id => id.Id).FirstOrDefaultAsync().Result.Id;
        }

        public List<TeamEntity> GetTeams()
        {
            try
            {
                var query = Connection.Table<TeamEntity>().ToListAsync();
                return query.Result;
            }
            catch (AggregateException)
            {
                return new List<TeamEntity>();
            }
        }

        public TeamEntity GetTeam(int id)
        {
            var query = Connection.Table<TeamEntity>().Where(t => t.Id == id);
            var val = query.ToListAsync().Result;
            return val.FirstOrDefault();
        }

        public bool UpdateTeam(TeamEntity team)
        {
            var val = true;
            try
            {
                Connection.UpdateAsync(team);
            }
            catch (Exception)
            {
                val = false;
            }
            return val;
        }

        public bool DeleteTeam(TeamEntity team)
        {
            var val = true;
            try
            {
                Connection.DeleteAsync(team);
            }
            catch (Exception)
            {
                val = false;
            }
            return val;
        }

        public bool DropTeamsTable()
        {
            var val = true;
            try
            {
                Connection.DropTableAsync<TeamEntity>();
            }
            catch (Exception)
            {
                val = false;
            }
            return val;
        }
    }
}