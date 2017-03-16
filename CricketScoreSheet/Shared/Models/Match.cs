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

namespace CricketScoreSheet.Shared.Models
{
    public class Match
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int TotalOvers { get; set; }

        public string Location { get; set; }

        public string Date { get; set; }

        public string Comments { get; set; }

        public bool Complete { get; set; }

        public Team HomeTeam { get; set; }

        public Team AwayTeam { get; set; }

        public string WinningTeamName { get; set; }
    }
}