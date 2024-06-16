using DatabaseCommunicator.Models;
using Microsoft.Identity.Client;
using Neo4j.Driver;
using Neo4j.Driver.Preview.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseCommunicator.ModelsManipulators
{
    public class TournamentTeamsManipulator : DatabaseModelManipulator
    {

        public TournamentTeamsManipulator(DatabaseConnectorSettings databaseConnectorSettings) : base(databaseConnectorSettings) { }


		public async Task<List<TeamWithPlayers>> GetRegisteredTeamsWithPlayersAsync(string tournamentId)
        {
            using (var session = _driver.AsyncSession())
            {

                var properties = new
                {
                    TournamentId = tournamentId,
                };

                var query = "MATCH (player:Player)-[:PLAYS_FOR]->(team:Team)-[:PLAYS_IN]->(tournament:Tournament {Id : $TournamentId}) " +
                            "RETURN team, COLLECT(player) AS players";

                IResultCursor result = await session.RunAsync(query, properties);

                var teamsWithPlayers = await result.ToListAsync<TeamWithPlayers>();

                return teamsWithPlayers.ToList();
            }
        }

        public async Task<List<Team>> GetRegisteredTeamsAsync(string tournamentId)
        {
            using (var session = _driver.AsyncSession())
            {

                var properties = new
                {
                    TournamentId = tournamentId,
                };

                var query = "MATCH (team:Team)-[:PLAYS_IN]->(tournament:Tournament {Id : $TournamentId}) " +
                            "RETURN team";

                IResultCursor result = await session.RunAsync(query, properties);

                var teams = await result.ToListAsync<Team>();

                return teams.ToList();
            }
        }

        public async Task RegisterTeamInTournament(string tournamentId, string teamId)
        {
            using (var session = _driver.AsyncSession())
            {
                var properties = new
                {
                    TournamentId = tournamentId,
                    TeamId = teamId
                };

                var query = "MERGE (team:Team {Id : $TeamId}) " +
                            "MERGE (tournament:Tournament {Id : $TournamentId}) " +
                            "MERGE (team)-[:PLAYS_IN]->(tournament)";

                await session.RunAsync(query, properties);
            }
        }


        public async Task<IEnumerable<Tournament>> GetTournamentsOfTeam(string teamId)
        {
            List<Tournament> tournaments = [];

			using (var session = _driver.AsyncSession())
			{

				var properties = new
				{
					TeamId = teamId,
				};

				var query = "MATCH (team:Team {Id : $TeamId})-[:PLAYS_IN]->(tournament:Tournament) " +
							"RETURN tournament";

				IResultCursor result = await session.RunAsync(query, properties);

				await result.ForEachAsync(record =>
				{
					tournaments.Add(MappingFromDatabase.MapTournament(record["tournament"].As<INode>()));
				});

                return tournaments;
			}
		}

		public async Task<Player> GetManagerOfTeam(string teamId)
		{
			var properties = new
			{
				TeamId = teamId
			};

			List<Team> teams = [];

			using (var session = _driver.AsyncSession())
			{
				var query = "MATCH (t:Team {Id : $TeamId})-[:MANAGED_BY]->(manager:Player) RETURN manager";

				var result = await session.RunAsync(query, properties);

                return (await result.ToListAsync<Player>())[0];
			}
		}

        public async Task DeleteTeam(string teamId)
        {
            var properties = new
            {
                TeamId = teamId
            };

            using (var session = _driver.AsyncSession())
            {
                var query = "MATCH (t:Team {Id : $TeamId}) DETACH DELETE t";

                await session.RunAsync(query, properties);
            }
        }
    }
}
