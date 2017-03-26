using CricketScoreSheet.Shared.Models;
using System.Collections.Generic;

namespace CricketScoreSheet.Shared.Services
{
    public class Access
    {
        private MatchService matchService;
        public MatchService MatchService
        {
            get
            {
                if (matchService == null)
                    matchService = new MatchService();
                return matchService;
            }
        }

        private TeamService teamService;
        public TeamService TeamService
        {
            get
            {
                if (teamService == null)
                    teamService = new TeamService();
                return teamService;
            }
        }

        private PlayerService playerService;
        public PlayerService PlayerService
        {
            get
            {
                if (playerService == null)
                    playerService = new PlayerService();
                return playerService;
            }
        }

        private static List<CalcBall> teamOneBalls;
        public static List<CalcBall> TeamOneBalls
        {
            get
            {
                if (teamOneBalls == null)
                    teamOneBalls = new List<CalcBall>();
                return teamOneBalls;
            }
        }

        private static List<CalcBall> teamTwoBalls;
        public static List<CalcBall> TeamTwoBalls
        {
            get
            {
                if (teamTwoBalls == null)
                    teamTwoBalls = new List<CalcBall>();
                return teamTwoBalls;
            }
        }
    }
}