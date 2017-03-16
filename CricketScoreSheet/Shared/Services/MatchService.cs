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
using CricketScoreSheet.Shared.Models;
using CricketScoreSheet.Shared.DataAccess.Entities;

namespace CricketScoreSheet.Shared.Services
{
    public class MatchService
    {
        private MatchesTable MatchesTable;
        private Access Access;

        public MatchService()
        {
            Access = new Access();
            MatchesTable = new MatchesTable(Helper.DbPath);
        }

        public int MatchesCount() => MatchesTable.MatchesCount();

        public int AddMatch(Match match)
        {
            MatchEntity newMatch = new MatchEntity
            {                
                MatchDate = DateTime.Today.ToString("MMM dd, yyyy"),
                TotalOvers = match.TotalOvers,
                Location = match.Location,
                MatchComplete = false,
                WinningTeamName = string.Empty,
                Comments = string.Empty,

                HomeTeamId = match.HomeTeam.Id,
                HomeTeamName = match.HomeTeam.Name,
                HomeTeamRuns = 0,
                HomeTeamWickets = 0,
                HomeTeamBalls = 0,
                HomeTeamNoBalls = 0,
                HomeTeamWides = 0,
                HomeTeamByes = 0,
                HomeTeamLegByes = 0,
                HomeTeamInningsComplete = false,

                AwayTeamId = match.AwayTeam.Id,
                AwayTeamName = match.AwayTeam.Name,
                AwayTeamRuns = 0,
                AwayTeamWickets = 0,
                AwayTeamBalls = 0,
                AwayTeamNoBalls = 0,
                AwayTeamWides = 0,
                AwayTeamByes = 0,
                AwayTeamLegByes = 0,
                AwayTeamInningsComplete = false   
            };

            return MatchesTable.AddMatch(newMatch);
        }

        public List<Match> GetMatches()
        {
            var matchesentity = MatchesTable.GetMatches();
            var matches = new List<Match>();
            foreach(var matchentity in matchesentity)
            {
                matches.Add(MapMatchEntityToMatch(matchentity));
            }
            return matches;
        }

        public Match GetMatch(int id)
        {
            var matchentity = MatchesTable.GetMatch(id);
            return MapMatchEntityToMatch(matchentity);
        }

        public bool UpdateMatch(Match match)
        {
            return MatchesTable.UpdateMatch(MapMatchToMatchEntity(match));
        }

        public bool DeleteMatch(Match match)
        {
            return MatchesTable.DeleteMatch(MapMatchToMatchEntity(match));
        }

        public bool DropMatchesTable()
        {
            return MatchesTable.DropMatchesTable();
        }

        public bool UpdateTeamScore(int matchId, int battingTeamId, CalcBall value)
        {
            var match = MatchesTable.GetMatch(matchId);

            if (match.HomeTeamId == battingTeamId && !match.HomeTeamInningsComplete)
            {
                match.HomeTeamRuns = match.HomeTeamRuns + value.RunsGiven;
                match.HomeTeamWickets = match.HomeTeamWickets + value.WicketsTaken;
                match.HomeTeamBalls = match.HomeTeamBalls + value.BallBowled;
                match.HomeTeamWides = match.HomeTeamWides + value.Wides;
                match.HomeTeamNoBalls = match.HomeTeamNoBalls + value.Noballs;
                match.HomeTeamByes = match.HomeTeamByes + value.Byes;
                match.HomeTeamLegByes = match.HomeTeamLegByes + value.LegByes;

                // Home team completed innings
                if (match.HomeTeamBalls >= match.TotalOvers * 6)
                {
                    match.HomeTeamInningsComplete = true;
                }

                //Match complete
                if (match.AwayTeamInningsComplete && (match.HomeTeamInningsComplete || match.HomeTeamRuns > match.AwayTeamRuns))
                {
                    match.MatchComplete = true;
                    match.HomeTeamInningsComplete = true;

                    // Home team successfully chased
                    if (match.HomeTeamRuns > match.AwayTeamRuns)
                    {
                        match.WinningTeamName = match.HomeTeamName;
                        match.Comments = match.WinningTeamName + " won by " +
                            (Access.PlayerService.GetPlayersPerTeamPerMatch(match.HomeTeamId, match.Id).Count - match.HomeTeamWickets) + " wickets";
                    }
                    // Home team lost
                    else if (match.HomeTeamRuns < match.AwayTeamRuns)
                    {
                        var diffruns = match.AwayTeamRuns - match.HomeTeamRuns;
                        match.Comments = match.AwayTeamName + " won by " + diffruns + " runs";
                    }
                    // Game Tie
                    else if (match.HomeTeamRuns == match.AwayTeamRuns)
                    {
                        match.Comments = "Game is tie";
                    }
                }
            }
            else if (match.AwayTeamId == battingTeamId && !match.AwayTeamInningsComplete)
            {
                match.AwayTeamRuns = match.AwayTeamRuns + value.RunsGiven;
                match.AwayTeamWickets = match.AwayTeamWickets + value.WicketsTaken;
                match.AwayTeamBalls = match.AwayTeamBalls + value.BallBowled;
                match.AwayTeamWides = match.AwayTeamWides + value.Wides;
                match.AwayTeamNoBalls = match.AwayTeamNoBalls + value.Noballs;
                match.AwayTeamByes = match.AwayTeamByes + value.Byes;
                match.AwayTeamLegByes = match.AwayTeamLegByes + value.LegByes;

                // Away team completed innings
                if (match.AwayTeamBalls >= match.TotalOvers * 6)
                {
                    match.AwayTeamInningsComplete = true;
                }

                //Match complete
                if (match.HomeTeamInningsComplete && (match.AwayTeamInningsComplete || match.HomeTeamRuns < match.AwayTeamRuns))
                {
                    match.MatchComplete = true;
                    match.AwayTeamInningsComplete = true;

                    // Away team successfully chased
                    if (match.HomeTeamRuns < match.AwayTeamRuns)
                    {
                        match.WinningTeamName = match.AwayTeamName;
                        match.Comments = match.WinningTeamName + " won by " +
                            (Access.PlayerService.GetPlayersPerTeamPerMatch(match.AwayTeamId, match.Id).Count - match.AwayTeamWickets) + " wickets";
                    }
                    // Away team lost
                    else if (match.HomeTeamRuns > match.AwayTeamRuns)
                    {
                        var diffruns = match.HomeTeamRuns - match.AwayTeamRuns;
                        match.Comments = match.HomeTeamName + " won by " + diffruns + " runs";
                    }
                    // Game Tie
                    else if (match.HomeTeamRuns == match.AwayTeamRuns)
                    {
                        match.Comments = "Game is tie";
                    }
                }
            }

            return MatchesTable.UpdateMatch(match);
        }

        public bool UndoTeamScore(int matchId, int battingTeamId, CalcBall value)
        {
            if (matchId == 0 || battingTeamId == 0 || value == null) return false;
            var match = MatchesTable.GetMatch(matchId);
            match.MatchComplete = false;
            match.WinningTeamName = null;
            match.Comments = null;
            if (match.HomeTeamId == battingTeamId && match.HomeTeamBalls > 0)
            {
                match.HomeTeamRuns = match.HomeTeamRuns - value.RunsGiven;
                match.HomeTeamWickets = match.HomeTeamWickets - value.WicketsTaken;
                match.HomeTeamBalls = match.HomeTeamBalls - value.BallBowled;
                match.HomeTeamWides = match.HomeTeamWides - value.Wides;
                match.HomeTeamNoBalls = match.HomeTeamNoBalls - value.Noballs;
                match.HomeTeamByes = match.HomeTeamByes - value.Byes;
                match.HomeTeamLegByes = match.HomeTeamLegByes - value.LegByes;
            }
            else if (match.AwayTeamId == battingTeamId && match.AwayTeamBalls > 0)
            {
                match.AwayTeamRuns = match.AwayTeamRuns - value.RunsGiven;
                match.AwayTeamWickets = match.AwayTeamWickets - value.WicketsTaken;
                match.AwayTeamBalls = match.AwayTeamBalls - value.BallBowled;
                match.AwayTeamWides = match.AwayTeamWides - value.Wides;
                match.AwayTeamNoBalls = match.AwayTeamNoBalls - value.Noballs;
                match.AwayTeamByes = match.AwayTeamByes - value.Byes;
                match.AwayTeamLegByes = match.AwayTeamLegByes - value.LegByes;
                match.AwayTeamInningsComplete = false;
            }

            return MatchesTable.UpdateMatch(match); ;
        }

        private MatchEntity MapMatchToMatchEntity(Match match)
        {
            return new MatchEntity
            {
                Id = match.Id,
                TotalOvers = match.TotalOvers,
                Location = match.Location,
                MatchDate = match.Date,
                Comments = match.Comments,
                MatchComplete = match.Complete,
                HomeTeamId = match.HomeTeam.Id,
                HomeTeamName = match.HomeTeam.Name,
                HomeTeamRuns = match.HomeTeam.Runs,
                HomeTeamBalls = match.HomeTeam.Balls,
                HomeTeamWickets = match.HomeTeam.Wickets,
                HomeTeamNoBalls = match.HomeTeam.NoBalls,
                HomeTeamWides = match.HomeTeam.Wides,
                HomeTeamByes = match.HomeTeam.Byes,
                HomeTeamLegByes = match.HomeTeam.LegByes,
                HomeTeamInningsComplete = match.HomeTeam.InningsComplete,
                AwayTeamId = match.AwayTeam.Id,
                AwayTeamName = match.AwayTeam.Name,
                AwayTeamRuns = match.AwayTeam.Runs,
                AwayTeamBalls = match.AwayTeam.Balls,
                AwayTeamWickets = match.AwayTeam.Wickets,
                AwayTeamNoBalls = match.AwayTeam.NoBalls,
                AwayTeamWides = match.AwayTeam.Wides,
                AwayTeamByes = match.AwayTeam.Byes,
                AwayTeamLegByes = match.AwayTeam.LegByes,
                AwayTeamInningsComplete = match.AwayTeam.InningsComplete,
                WinningTeamName = match.WinningTeamName
            };
        }

        private Match MapMatchEntityToMatch(MatchEntity matchentity)
        {
            return new Match
            {
                Id = matchentity.Id,
                Name = $"{matchentity.HomeTeamName}_{matchentity.AwayTeamName}_{matchentity.Id}",
                TotalOvers = matchentity.TotalOvers,
                Location = matchentity.Location,
                Date = matchentity.MatchDate,
                Comments = matchentity.Comments,
                Complete = matchentity.MatchComplete,
                HomeTeam = new Team
                {
                    Id = matchentity.HomeTeamId,
                    Name = matchentity.HomeTeamName,
                    Runs = matchentity.HomeTeamRuns,
                    Balls = matchentity.HomeTeamBalls,
                    Wickets = matchentity.HomeTeamWickets,
                    NoBalls = matchentity.HomeTeamNoBalls,
                    Wides = matchentity.HomeTeamWides,
                    Byes = matchentity.HomeTeamByes,
                    LegByes = matchentity.HomeTeamLegByes,
                    InningsComplete = matchentity.HomeTeamInningsComplete,
                    Players = Access.PlayerService.GetPlayersPerTeamPerMatch(matchentity.HomeTeamId, matchentity.Id)
                },
                AwayTeam = new Team
                {
                    Id = matchentity.AwayTeamId,
                    Name = matchentity.AwayTeamName,
                    Runs = matchentity.AwayTeamRuns,
                    Balls = matchentity.AwayTeamBalls,
                    Wickets = matchentity.AwayTeamWickets,
                    NoBalls = matchentity.AwayTeamNoBalls,
                    Wides = matchentity.AwayTeamWides,
                    Byes = matchentity.AwayTeamByes,
                    LegByes = matchentity.AwayTeamLegByes,
                    InningsComplete = matchentity.AwayTeamInningsComplete,
                    Players = Access.PlayerService.GetPlayersPerTeamPerMatch(matchentity.AwayTeamId, matchentity.Id)
                },
                WinningTeamName = matchentity.WinningTeamName
            };
        }
    }
}