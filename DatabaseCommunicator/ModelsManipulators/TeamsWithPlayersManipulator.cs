using DatabaseCommunicator.Models;
using Neo4j.Driver;
using Neo4j.Driver.Preview.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseCommunicator.ModelsManipulators
{
    public class TeamsWithPlayersManipulator : DatabaseModelManipulator
    {
        public TeamsWithPlayersManipulator(DatabaseConnectorSettings databaseConnectorSettings) : base(databaseConnectorSettings) { }

        /// <summary>
        /// Returns information about selected team with its players
        /// </summary>
        /// <param name="teamId">Id of the team</param>
        /// <returns></returns>
        public async Task<TeamWithPlayers> GetTeamWithPlayers(string teamId)
        {
            List<Team> teams = [];
            using (var session = _driver.AsyncSession())
            {
                var query = "MATCH (t:Team {Id : $TeamId}) RETURN t";

                var result = await session.RunAsync(query, new { TeamId = teamId });

                await result.ForEachAsync(record =>
                {
                    teams.Add(MappingFromDatabase.MapTeam(record["t"].As<INode>()));
                });

                return new TeamWithPlayers(teams.First(), await GetPlayersInTeamAsync(teamId));
            }
        }

        /// <summary>
        /// Returns all players which play in selected team
        /// </summary>
        /// <param name="teamId">Id of the team</param>
        /// <returns></returns>
        public async Task<List<Player>> GetPlayersInTeamAsync(string teamId)
        {
            using (var session = _driver.AsyncSession())
            {
                var query = "MATCH (p:Player)-[relationship:PLAYS_FOR]->(t:Team {Id : $TeamId}) RETURN p ORDER BY relationship.OrderNumber;";

                var result = await session.RunAsync(query, new { TeamId = teamId });

                return (await result.ToListAsync<Player>()).ToList();
            }
        }

        /// <summary>
        /// Saves the roster of the selected team
        /// </summary>
        /// <param name="teamId">Id of the team</param>
        /// <param name="players">New roster ordered by players positions</param>
        /// <returns></returns>
        public async Task SaveTeamPlayers(string teamId, List<Player> players)
        {
            await DeleteAllPlayersFromTeam(teamId);

            using (var session = _driver.AsyncSession())
            {

                var properties = new
                {
                    TeamId = teamId,
                    PlayerIds = players.Select(player => player.Id).ToArray()
                };

                var query = "MATCH (team:Team {Id: $TeamId}) " +
                            "FOREACH(index IN range(0, size($PlayerIds) - 1) | " +
                            "MERGE (player:Player {Id: $PlayerIds[index]}) " +
                            "MERGE (player)-[:PLAYS_FOR {OrderNumber: index}]->(team))";

                IResultCursor result = await session.RunAsync(query, properties);
            }
        }

        /// <summary>
        /// Removes all players from selected team
        /// </summary>
        /// <param name="teamId">Id of the team</param>
        /// <returns></returns>
        public async Task DeleteAllPlayersFromTeam(string teamId)
        {
            using (var session = _driver.AsyncSession())
            {
                var properties = new
                {
                    TeamId = teamId
                };

                var query = "MATCH (:Player)-[relationship:PLAYS_FOR]->(:Team {Id : $TeamId}) " +
                            "DELETE relationship";

                IResultCursor result = await session.RunAsync(query, properties);
            }
        }
    }
}
