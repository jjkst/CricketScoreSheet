using CricketScoreSheet.Shared.Models;

namespace CricketScoreSheet.Shared.Services
{
    public class BallService
    {
        public CalcBall CalcBall { get; set; }

        public BallService(Ball ball, int teamno)
        {
            CalcBall = new CalcBall(ball);
            if (teamno == 1)
            {
                Access.TeamOneBalls.Add(CalcBall);
            }
            else
            {
                Access.TeamTwoBalls.Add(CalcBall);
            }
        }
    }
}