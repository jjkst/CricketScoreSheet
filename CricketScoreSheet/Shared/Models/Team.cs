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

namespace CricketScoreSheet.Shared.Models
{
    public class Team
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int Runs { get; set; }

        public int Balls { get; set; }

        public int Wickets { get; set; }

        public int NoBalls { get; set; }

        public int Wides { get; set; }

        public int Byes { get; set; }

        public int LegByes { get; set; }

        public bool InningsComplete { get; set; }

        public List<PlayerEntity> Players { get; set; }
    }
}