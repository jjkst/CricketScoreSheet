namespace CricketScoreSheet.Shared.Models
{
    public class Ball
    {
        public string ActiveBatsman { get; set; }

        public string RunnerBatsman { get; set; }

        public string RunnerHowOut { get; set; }

        public string ActiveBowler { get; set; }

        public string Fielder { get; set; }

        public int RunsScored { get; set; }

        public string HowOut { get; set; } = "not out";

        public int Wide { get; set; }

        public int NoBall { get; set; }

        public int Byes { get; set; }

        public int LegByes { get; set; }

    }
}