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
using SQLite;

namespace CricketScoreSheet.Shared.DataAccess.Repository
{
    public abstract class MyRepository
    {
        private const string Db = "cricketscoresheetSQLite.db3";

        public SQLiteAsyncConnection Connection;

        public MyRepository(string path)
        {
            string dbPath = System.IO.Path.Combine(path, Db);
            Connection = new SQLiteAsyncConnection(dbPath);
        }

        public bool TableExist(string tableName)
        {
            string cmdText = $"SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name='{tableName}'";
            return (Connection.ExecuteScalarAsync<int>(cmdText).Result > 0); ;
        }

    }
}