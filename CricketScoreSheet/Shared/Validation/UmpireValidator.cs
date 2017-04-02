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

namespace CricketScoreSheet.Shared.Validation
{
    public class UmpireValidator
    {
        private List<UmpireEntity> Umpires { get; }

        public UmpireValidator(List<UmpireEntity> umpires)
        {
            Umpires = umpires;
        }

        public IList<string> Validate(string umpirename)
        {
            var results = new List<string>();

            if (string.IsNullOrEmpty(umpirename))
            {
                results.Add("Umpire name cannot be blank.");
            }
            if (Umpires.Any(t => t.Name == umpirename))
            {
                results.Add($"This Umpire name {umpirename} already exist.");
            }

            return results;
        }
   }
}