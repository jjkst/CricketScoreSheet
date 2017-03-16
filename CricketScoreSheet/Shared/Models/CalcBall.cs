namespace CricketScoreSheet.Shared.Models
{
    public class CalcBall
    {
        public string BatsmanName { get; set; }

        public string BatsmanHowOut { get; set; }

        public string RunnerBatsman { get; set; }

        public string RunnerHowOut { get; set; }

        public string BowlerName { get; set; }

        public string Fielder { get; set; }

        public int RunsTaken { get; set; }

        public int BallsPlayed { get; set; }

        public int RunsGiven { get; set; }

        public int BallBowled { get; set; }

        public int WicketsTaken { get; set; }

        public int Wides { get; set; }

        public int Noballs { get; set; }

        public int Byes { get; set; }

        public int LegByes { get; set; }

        public CalcBall(Ball ball)
        {
            BatsmanName = ball.ActiveBatsman;
            RunnerBatsman = ball.RunnerBatsman;
            RunnerHowOut = ball.RunnerHowOut;
            BowlerName = ball.ActiveBowler;
            Fielder = ball.Fielder;
            RunsTaken = ball.RunsScored;
            BallsPlayed = ((ball.Wide > 0) ? 0 : 1);
            BatsmanHowOut = ball.HowOut;
            RunsGiven = ball.RunsScored + ball.Wide + ball.NoBall + ball.Byes + ball.LegByes;
            BallBowled = ((ball.Wide > 0 || ball.NoBall > 0) ? 0 : 1);
            WicketsTaken = ((ball.HowOut == "not out" || ball.HowOut == "retired") ? 0 : 1) +
                    (!string.IsNullOrEmpty(ball.RunnerHowOut) && ball.RunnerHowOut.Contains("runout") ? 1 : 0);
            Wides = ((ball.Wide > 0) ? 1 : 0);
            Noballs = ((ball.NoBall > 0) ? 1 : 0);
            Byes = ball.Byes;
            LegByes = ball.LegByes;
        }
    }
}