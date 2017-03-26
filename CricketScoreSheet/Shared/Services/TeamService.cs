using CricketScoreSheet.Shared.DataAccess.Entities;
using CricketScoreSheet.Shared.DataAccess.Repository;
using CricketScoreSheet.Shared.Models;
using System.Collections.Generic;

namespace CricketScoreSheet.Shared.Services
{
    public class TeamService
    {
        private TeamsTable TeamsTable;

        public TeamService()
        {
            TeamsTable = new TeamsTable(Helper.DbPath);
        }

        public int TeamsCount() => TeamsTable.TeamsCount();

        public int AddTeam(string teamName)
        {
            TeamEntity newTeam = new TeamEntity
            {
                Name = teamName,
                HighestScore = 0,
                LowestScore = 0,
                Wins = 0,
                Loss = 0,
            };
            return TeamsTable.AddTeam(newTeam);
        }

        public List<TeamEntity> GetTeams()
        {
            return TeamsTable.GetTeams();
        }

        public TeamEntity GetTeam(int id)
        {
            return TeamsTable.GetTeam(id);
        }

        public bool UpdateTeam(TeamEntity team)
        {
            return TeamsTable.UpdateTeam(team);
        }

        public bool DeleteTeam(TeamEntity team)
        {
            return TeamsTable.DeleteTeam(team);
        }

        public bool DropTeamsTable()
        {
            return TeamsTable.DropTeamsTable();
        }

        public TeamEntity MapTeamToTeamEntity(Team team)
        {
            return new TeamEntity
            {
                Name = team.Name,
                Id = team.Id
            };
        }

        public Team MapTeamEntityToTeam(TeamEntity teamEntity)
        {
            return new Team
            {
                Name = teamEntity.Name,
                Id = teamEntity.Id
            };
        }
    }
}   