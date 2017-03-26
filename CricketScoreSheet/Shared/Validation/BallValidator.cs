using CricketScoreSheet.Shared.Models;
using System.Collections.Generic;

namespace CricketScoreSheet.Shared.Validation
{
    public class BallValidator
    {
        private Ball Ball { get; }

        public BallValidator(Ball ball)
        {
            Ball = ball;
        }

        public IList<string> Validate(string field)
        {
            var results = new List<string>();

            switch (field)
            {
                case "wide":
                    if (Ball.RunsScored > 0 && Ball.HowOut == "not out")
                    {
                        results.Add("Batsman can not score on wide ball.");
                    }
                    if (Ball.HowOut != "not out" && !Ball.HowOut.Contains("runout")
                        && !Ball.HowOut.Contains("st †") && !Ball.HowOut.Contains("hit wicket"))
                    {
                        results.Add("Batsman can not get out on wide ball.");
                    }
                    if (Ball.LegByes == 1)
                    {
                        results.Add("Wide and LegByes is not possible.");
                    }
                    break;
                case "noball":
                    if (Ball.HowOut != "not out" && !Ball.HowOut.Contains("runout"))
                        results.Add("No ball, Batsman can not get out like that.");
                    break;
                case "byes":
                    if (Ball.RunsScored > 0 && Ball.HowOut == "not out")
                        results.Add("Batsman can not score on byes.");
                    if (Ball.HowOut != "not out" && !Ball.HowOut.Contains("runout"))
                        results.Add("Batsman can get runout but not other way when byes.");
                    break;
                case "legbyes":
                    if (Ball.RunsScored > 0 && Ball.HowOut == "not out")
                        results.Add("Batsman can not score on legbyes.");
                    if (Ball.HowOut != "not out" && !Ball.HowOut.Contains("runout"))
                        results.Add("Batsman can get runout but not other way when legbyes.");
                    if (Ball.Wide == 1)
                        results.Add("Wide and LegByes is not possible.");
                    break;
                case "runs":
                    if (Ball.Wide == 1 || Ball.Byes == 1 || Ball.LegByes == 1)
                        results.Add("Batsman can not take credit for extras.");
                    break;
                case "out":
                    if (Ball.Wide == 1 || Ball.NoBall == 1 || Ball.Byes == 1 || Ball.LegByes == 1)
                        results.Add("Batsman can not get out and get extras");
                    break;
                case "xout":
                    if (Ball.NoBall == 1 || Ball.Byes == 1 || Ball.LegByes == 1)
                        results.Add("Batsman can not get out and get extras");
                    break;
                default:
                    break;
            }
            return results;
        }
    }
}