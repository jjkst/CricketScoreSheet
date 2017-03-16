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
    public class PlayersTable : MyRepository
    {
        public PlayersTable(string path) : base(path)
        {
            try
            {
                if (!TableExist("PlayerEntity"))
                {
                    Connection.CreateTableAsync<PlayerEntity>();
                }
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine("Players Table is not created");
                Console.WriteLine(ex.Message);
            }
        }

        public int PlayersCount()
        {
            try
            {
                var count = Connection.ExecuteScalarAsync<int>("select count(*) from PlayerEntity").Result;
                return count;
            }
            catch(SQLiteException)
            {
                return 0;

            }
        }

        public int AddPlayer(PlayerEntity player)
        {
            Connection.InsertAsync(player);
            return Connection.Table<PlayerEntity>().OrderByDescending(id => id.Id).FirstOrDefaultAsync().Result.Id;
        }

        public List<PlayerEntity> GetPlayers()
        {
            var query = Connection.Table<PlayerEntity>().ToListAsync();
            return query.Result;
        }

        public List<PlayerEntity> GetPlayersPerTeam(int teamId)
        {
            var query = Connection.Table<PlayerEntity>().Where(t => t.TeamId == teamId).ToListAsync();
            return query.Result;
        }

        public List<PlayerEntity> GetPlayersPerTeamPerMatch(int teamId, int matchId)
        {
            var query = Connection.Table<PlayerEntity>().Where(t => t.TeamId == teamId && t.MatchId == matchId).ToListAsync();
            return query.Result;
        }

        public PlayerEntity GetPlayer(int id)
        {
            var query = Connection.Table<PlayerEntity>().Where(t => t.Id == id);
            var val = query.ToListAsync().Result;
            return val.FirstOrDefault();
        }

        public bool UpdatePlayer(PlayerEntity player)
        {
            var val = true;
            try
            {
                Connection.UpdateAsync(player);
            }
            catch (Exception)
            {
                val = false;
            }
            return val;
        }

        public bool DeletePlayer(PlayerEntity player)
        {
            var val = true;
            try
            {
                Connection.DeleteAsync(player);
            }
            catch (Exception)
            {
                val = false;
            }
            return val;
        }

        public bool DropPlayersTable()
        {
            var val = true;
            try
            {
                Connection.DropTableAsync<PlayerEntity>();
            }
            catch (Exception)
            {
                val = false;
            }
            return val;
        }
    }
}