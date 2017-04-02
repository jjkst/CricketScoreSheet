using CricketScoreSheet.Shared.DataAccess.Entities;
using System.Collections.Generic;
using System.Linq;

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