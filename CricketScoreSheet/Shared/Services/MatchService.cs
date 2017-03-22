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

        public string CreateHtml(Match match)
        {
            string hometeamovers = Helper.ConvertBallstoOvers(match.HomeTeam.Balls);
            string awayteamovers = Helper.ConvertBallstoOvers(match.AwayTeam.Balls);

            var header = @"<head>
                                <style>
                                    body.ClassName{background-color: WhiteSmoke ;position:fixed; margin:0px; width:100%; height:100%;}
                                    div.ClassName{background-color: DimGray ;color: white;margin: 30px 30px 30px 30px;padding: 20px; border-style: solid;}
                                    th.header{background-color: DarkCyan;color: white;font-size:20px; text-align:center;}
									td.data{padding: 5px 5px 5px 10px;font-size:18px;}
                                    td.textcenter{text-align:center;}
                                </style>
                           </head>";
            var card = $@"<div class=""ClassName"">
                                <h3> {match.Date}, at {match.Location} </h3>
                                <label> {match.HomeTeam.Name} {match.HomeTeam.Runs}/{match.HomeTeam.Wickets} ({hometeamovers}/{match.TotalOvers}) </label></br>      
                                <label> {match.AwayTeam.Name} {match.AwayTeam.Runs}/{match.AwayTeam.Wickets} ({awayteamovers}/{match.TotalOvers}) </label></br>        
                                <label> {match.Comments} </label>            
                           </div>";

            var fi_BattingHeader = $@"<tr>
                                    <th colspan=""2"" class=""header""><center>{match.HomeTeam.Name} Innings ({match.TotalOvers} over(s) maximum)</center></th>
                                    <th class=""header"" align=""left"">R</th>
                                    <th class=""header"" align=""left"">B</th>
                                    <th class=""header"" align=""left"">4s</th>
                                    <th class=""header"" align=""left"">6s</th>
                                    <th class=""header"" align=""left"">SR</th>
                                   </tr>";

            string fi_Batsman = "";
            foreach(var batsman in match.HomeTeam.Players)
            {
                var sr = (batsman.BallsPlayed == 0) ? 0 : 
                    decimal.Round((decimal)batsman.RunsTaken * 100 / batsman.BallsPlayed, 2, MidpointRounding.AwayFromZero);
                fi_Batsman = fi_Batsman +
                    $@"<tr>
                        <td class=""data"">{batsman.Name}</td>
                        <td class=""data"">{batsman.HowOut}</td>
                        <td class=""data textcenter"">{batsman.RunsTaken}</td>
                        <td class=""data textcenter"">{batsman.BallsPlayed}</td>
                        <td class=""data textcenter"">{batsman.Fours}</td>
                        <td class=""data textcenter"">{batsman.Sixes}</td>
                        <td class=""data textcenter"">{sr}</td>
                     <tr>";
            }

            var fi_Extras = $@"<tr>
                                <td class=""data"">Extras</td>
                                <td class=""data"">(nb {match.HomeTeam.NoBalls}, w {match.HomeTeam.Wides}, b {match.HomeTeam.Byes},lb {match.HomeTeam.LegByes})</td>
                                <td class=""data textcenter"">{(match.HomeTeam.NoBalls + match.HomeTeam.Wides + match.HomeTeam.Byes + match.HomeTeam.LegByes)}</td>
                            </tr>";

            var fi_Total = $@"<tr>
                                <td class=""data"">Total</td>
                                <td class=""data"">({match.HomeTeam.Wickets} wickets; {hometeamovers} overs)</td>
                                <td class=""data textcenter"">{match.HomeTeam.Runs}</td>
                                <td class=""data"" colspan=""4"">({(decimal.Round(match.HomeTeam.Runs / Convert.ToDecimal(hometeamovers), 3, MidpointRounding.AwayFromZero))} runs per over)</td>
                           </tr>";

            var bowlingheader = $@"<tr>
                                       <th colspan=""2"" class=""header"" align=""left""><center>Bowling</center></th>
                                       <th class=""header"" align=""left"">O</th>
                                       <th class=""header"" align=""left"">Dots</th>
                                       <th class=""header"" align=""left"">R</th>
                                       <th class=""header"" align=""left"">W</th>
                                       <th class=""header"" align=""left"">Econ</th>
                                   </tr>";

            string fi_Bowlers = "";
            foreach (var bowler in match.AwayTeam.Players)
            {
                if (bowler.BallsBowled < 1) continue;
                var overs = Helper.ConvertBallstoOvers(bowler.BallsBowled);
                var econ = (bowler.BallsBowled == 0) ? 0 : 
                    decimal.Round((decimal)bowler.RunsGiven / Convert.ToDecimal(overs), 2, MidpointRounding.AwayFromZero);
                fi_Bowlers = fi_Bowlers +
                    $@"<tr>
                        <td colspan=""2"" class=""data"">{bowler.Name}</td>
                        <td class=""data textcenter"">{overs}</td>
                        <td class=""data textcenter"">{bowler.Maiden}</td>
                        <td class=""data textcenter"">{bowler.RunsGiven}</td>
                        <td class=""data textcenter"">{bowler.Wickets}</td>
                        <td class=""data textcenter"">{econ}</td>
                     <tr>";
            }
            // Second innings
            var si_BattingHeader = $@"<tr>
                                    <th colspan=""2"" class=""header""><center>{match.AwayTeam.Name} Innings ({match.TotalOvers} over(s) maximum)</center></th>
                                    <th class=""header"" align=""left"">R</th>
                                    <th class=""header"" align=""left"">B</th>
                                    <th class=""header"" align=""left"">4s</th>
                                    <th class=""header"" align=""left"">6s</th>
                                    <th class=""header"" align=""left"">SR</th>
                                   </tr>";

            string si_Batsman = "";
            foreach (var sbatsman in match.AwayTeam.Players)
            {
                var ssr = (sbatsman.BallsPlayed == 0) ? 0 :
                    decimal.Round((decimal)sbatsman.RunsTaken * 100 / sbatsman.BallsPlayed, 2, MidpointRounding.AwayFromZero);
                si_Batsman = si_Batsman +
                    $@"<tr>
                        <td class=""data"">{sbatsman.Name}</td>
                        <td class=""data"">{sbatsman.HowOut}</td>
                        <td class=""data textcenter"">{sbatsman.RunsTaken}</td>
                        <td class=""data textcenter"">{sbatsman.BallsPlayed}</td>
                        <td class=""data textcenter"">{sbatsman.Fours}</td>
                        <td class=""data textcenter"">{sbatsman.Sixes}</td>
                        <td class=""data textcenter"">{ssr}</td>
                     <tr>";
            }

            var si_Extras = $@"<tr>
                                <td class=""data"">Extras</td>
                                <td class=""data"">(nb {match.AwayTeam.NoBalls}, w {match.AwayTeam.Wides}, b {match.AwayTeam.Byes},lb {match.AwayTeam.LegByes})</td>
                                <td class=""data textcenter"">{(match.AwayTeam.NoBalls + match.AwayTeam.Wides + match.AwayTeam.Byes + match.AwayTeam.LegByes)}</td>
                            </tr>";

            var si_Total = $@"<tr>
                                <td class=""data"">Total</td>
                                <td class=""data"">({match.AwayTeam.Wickets} wickets; {awayteamovers} overs)</td>
                                <td class=""data textcenter"">{match.AwayTeam.Runs}</td>
                                <td class=""data"" colspan=""4"">({(decimal.Round(match.AwayTeam.Runs / Convert.ToDecimal(awayteamovers), 3, MidpointRounding.AwayFromZero))} runs per over)</td>
                           </tr>";

            string si_Bowlers = "";
            foreach (var sbowler in match.AwayTeam.Players)
            {
                if (sbowler.BallsBowled < 1) continue;
                var overs = Helper.ConvertBallstoOvers(sbowler.BallsBowled);
                var econ = (sbowler.BallsBowled == 0) ? 0 :
                    decimal.Round((decimal)sbowler.RunsGiven / Convert.ToDecimal(overs), 2, MidpointRounding.AwayFromZero);
                si_Bowlers = si_Bowlers +
                    $@"<tr>
                        <td colspan=""2"" class=""data"">{sbowler.Name}</td>
                        <td class=""data textcenter"">{overs}</td>
                        <td class=""data textcenter"">{sbowler.Maiden}</td>
                        <td class=""data textcenter"">{sbowler.RunsGiven}</td>
                        <td class=""data textcenter"">{sbowler.Wickets}</td>
                        <td class=""data textcenter"">{econ}</td>
                     <tr>";
            }

            var finalstring = $@"<html>
                                {header}
                                <body class=""ClassName"">
                                    {card}
                                    <table width=""100%""><tbody>{fi_BattingHeader}{fi_Batsman}{fi_Extras}{fi_Total}</tbody></table>
                                    <table width=""100%""><tbody>{bowlingheader}{fi_Bowlers}</tbody></table>
                                    <table width=""100%""><tbody>{si_BattingHeader}{si_Batsman}{si_Extras}{si_Total}</tbody></table>
                                    <table width=""100%""><tbody>{bowlingheader}{si_Bowlers}</tbody></table>
                                </body></html>";
            return finalstring;
        }
    }
}