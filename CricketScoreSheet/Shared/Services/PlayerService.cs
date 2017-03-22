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
using CricketScoreSheet.Shared.DataAccess.Repository;
using CricketScoreSheet.Shared.DataAccess.Entities;
using CricketScoreSheet.Shared.Models;

namespace CricketScoreSheet.Shared.Services
{
    public class PlayerService
    {
        private PlayersTable PlayersTable;
        private Access Access;

        public PlayerService()
        {
            Access = new Access();
            PlayersTable = new PlayersTable(Helper.DbPath);
        }

        public int PlayersCount() => PlayersTable.PlayersCount();

        public int AddPlayer(int teamId, int matchId, string name)
        {
            PlayerEntity player = new PlayerEntity
            {
                Name = name,
                TeamId = teamId,
                TeamName = Access.TeamService.GetTeam(teamId).Name,
                MatchId = matchId,
                RunsTaken = 0,
                BallsPlayed = 0,
                Fours = 0,
                Sixes = 0,
                HowOut = "not out",
                RunsGiven = 0,
                BallsBowled = 0,
                Wickets = 0,
                Maiden = 0,
                NoBalls = 0,
                Wides = 0,
                Catches = 0,
                Stumpings = 0
            };

            return PlayersTable.AddPlayer(player);
        }

        public List<PlayerEntity> GetPlayers() => PlayersTable.GetPlayers();

        public List<PlayerEntity> GetPlayersPerTeam(int teamId)
        {
            return PlayersTable.GetPlayers().Where(t => t.TeamId == teamId).ToList();
        }

        public List<PlayerEntity> GetPlayersPerTeamPerMatch(int teamId, int matchId)
        {
            return PlayersTable.GetPlayers().Where(t => t.TeamId == teamId && t.MatchId == matchId).ToList();
        }

        public PlayerEntity GetPlayer(int id)
        {
            return PlayersTable.GetPlayer(id);
        }

        public bool UpdatePlayer(PlayerEntity player)
        {
            return PlayersTable.UpdatePlayer(player);
        }

        public bool DeletePlayer(PlayerEntity player)
        {
            return PlayersTable.DeletePlayer(player);
        }

        public bool DropPlayersTable()
        {
            return PlayersTable.DropPlayersTable();
        }

        public bool UpdateBatsmanThisBall(int matchId, int battingTeamId, CalcBall value)
        {
            var val = false;

            // Handle when calc ball lost value
            if (value == null || string.IsNullOrEmpty(value.BatsmanName)) return val;

            // Handle when team innings complete
            var match = Access.MatchService.GetMatch(matchId);
            if ((match.HomeTeam.Id == battingTeamId && match.HomeTeam.InningsComplete)
                || (match.AwayTeam.Id == battingTeamId && match.AwayTeam.InningsComplete)) return val;

            var currentPlayers = GetPlayersPerTeamPerMatch(battingTeamId, matchId);

            // Active Batsman 
            var player = currentPlayers.Where(p => p.Name == value.BatsmanName).FirstOrDefault();
            player.RunsTaken = player.RunsTaken + value.RunsTaken;
            player.BallsPlayed = player.BallsPlayed + value.BallsPlayed;
            if (value.RunsTaken == 4) player.Fours = player.Fours + 1;
            if (value.RunsTaken == 6) player.Sixes = player.Sixes + 1;
            player.HowOut = value.BatsmanHowOut;

            val = PlayersTable.UpdatePlayer(player);

            // Update is runner out
            if (val && !string.IsNullOrEmpty(value.RunnerBatsman) && value.RunnerHowOut.Contains("runout"))
            {
                var runner = currentPlayers.Where(p => p.Name == value.RunnerBatsman).FirstOrDefault();
                runner.HowOut = value.RunnerHowOut;
                val = PlayersTable.UpdatePlayer(runner);
            }

            return val;
        }

        public bool UndoBatsmanLastBall(int matchId, int battingTeamId, CalcBall value)
        {
            var val = false;

            // Handle when calc ball lost value
            if (value == null || string.IsNullOrEmpty(value.BatsmanName)) return val;

            var currentPlayers = GetPlayersPerTeamPerMatch(battingTeamId, matchId);

            //Active Batsman
            var player = currentPlayers.Where(p => p.Name == value.BatsmanName).FirstOrDefault();
            if (player.BallsPlayed > 0)
            {
                player.RunsTaken = player.RunsTaken - value.RunsTaken;
                player.BallsPlayed = player.BallsPlayed - value.BallsPlayed;
                if (value.RunsTaken == 4) player.Fours = player.Fours - 1;
                if (value.RunsTaken == 6) player.Sixes = player.Sixes - 1;
                player.HowOut = "not out";
                val = PlayersTable.UpdatePlayer(player);
            }

            //Runner
            if (val && !string.IsNullOrEmpty(value.RunnerBatsman))
            {
                var runner = currentPlayers.Where(p => p.Name == value.RunnerBatsman).FirstOrDefault();
                runner.HowOut = "not out";
                val = PlayersTable.UpdatePlayer(runner);
            }

            return val;
        }

        public bool UpdateBowlerThisBall(int matchId, int bowlingTeamId, CalcBall value)
        {
            var val = false;

            // Handle when calc ball lost value
            if (value == null || string.IsNullOrEmpty(value.BowlerName)) return val;

            // Handle when team innings complete
            var match = Access.MatchService.GetMatch(matchId);
            if ((match.AwayTeam.Id == bowlingTeamId && match.HomeTeam.InningsComplete)
                    || (match.HomeTeam.Id == bowlingTeamId && match.AwayTeam.InningsComplete)) return val;

            var currentPlayers = GetPlayersPerTeamPerMatch(bowlingTeamId, matchId);

            var player = currentPlayers.Where(p => p.Name == value.BowlerName).FirstOrDefault();
            player.RunsGiven = player.RunsGiven + value.RunsGiven;
            player.BallsBowled = player.BallsBowled + value.BallBowled;
            player.Wickets = player.Wickets + value.WicketsTaken;

            player.Thisoverballs = (player.Thisoverballs < 6 ? player.Thisoverballs : 0) + 
                (player.BallsBowled == 0 ? 0 : 1);
            player.Thisoverruns = (player.RunsGiven == 0) ? false : true;
            player.Maiden = player.Maiden + ((player.Thisoverballs == 6 && !player.Thisoverruns) ? 1 : 0);

            player.Wides = player.Wides + value.Wides;
            player.NoBalls = player.NoBalls + value.Noballs;

            val = PlayersTable.UpdatePlayer(player);

            //Catch or Stumping
            if (val && !string.IsNullOrEmpty(value.Fielder))
            {
                var fielder = currentPlayers.Where(p => p.Name == value.Fielder).FirstOrDefault();

                fielder.Catches = fielder.Catches + (value.BatsmanHowOut.Contains($"c {value.Fielder}") ? 1 : 0);
                fielder.Stumpings = fielder.Stumpings + (value.BatsmanHowOut.Contains($"st †{value.Fielder}") ? 1 : 0);
                val = PlayersTable.UpdatePlayer(fielder);
            }

            return val;
        }

        public bool UndoBowlerLastBall(int matchId, int bowlingTeamId, CalcBall value)
        {
            var val = false;

            // Handle when calc ball lost value
            if (value == null || string.IsNullOrEmpty(value.BowlerName)) return val;

            var currentPlayers = GetPlayersPerTeamPerMatch(bowlingTeamId, matchId);
            var player = currentPlayers.Where(p => p.Name == value.BowlerName).FirstOrDefault();
            if (player.BallsBowled > 0)
            {
                if(player.Maiden > 0)
                {
                    player.Thisoverballs = (player.Thisoverballs < 6 ? player.Thisoverballs : 0) +
                            (player.BallsBowled == 0 ? 0 : player.BallsBowled);
                    player.Thisoverruns = (player.RunsGiven == 0) ? false : true;
                    player.Maiden = player.Maiden - ((player.Thisoverballs == 6 && !player.Thisoverruns) ? 1 : 0);
                }

                player.RunsGiven = player.RunsGiven - value.RunsGiven;
                player.BallsBowled = player.BallsBowled - value.BallBowled;
                player.Wickets = player.Wickets - value.WicketsTaken;
                player.Wides = player.Wides - value.Wides;
                player.NoBalls = player.NoBalls - value.Noballs;
                val = PlayersTable.UpdatePlayer(player);
            }

            //Catch or Stumping
            if (val && !string.IsNullOrEmpty(value.Fielder))
            {
                var fielder = currentPlayers.Where(p => p.Name == value.Fielder).FirstOrDefault();

                fielder.Catches = fielder.Catches - (value.BatsmanHowOut.Contains($"c {value.Fielder}") ? 1 : 0);
                fielder.Stumpings = fielder.Stumpings - (value.BatsmanHowOut.Contains($"st †{value.Fielder}") ? 1 : 0);
                val = PlayersTable.UpdatePlayer(fielder);
            }

            return val;
        }
    }
}