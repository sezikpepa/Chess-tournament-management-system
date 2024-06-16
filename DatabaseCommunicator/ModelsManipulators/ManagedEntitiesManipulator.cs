using DatabaseCommunicator.Models;
using Neo4j.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseCommunicator.ModelsManipulators
{
    public class ManagedEntitiesManipulator : DatabaseModelManipulator
    {
        public ManagedEntitiesManipulator(DatabaseConnectorSettings databaseConnectorSettings) : base(databaseConnectorSettings) { }

        public async Task<IEnumerable<Team>> GetManagedTeams(string accountId)
        {

            var properties = new
            {
                AccountId = accountId
            };

            List<Team> teams = [];

            using (var session = _driver.AsyncSession())
            {
                var query = "MATCH (t:Team)-[:MANAGED_BY]->(:Player {AccountId : $AccountId}) RETURN t";

                var result = await session.RunAsync(query, properties);

                await result.ForEachAsync(record =>
                {
                    teams.Add(MappingFromDatabase.MapTeam(record["t"].As<INode>()));
                });

                return teams;
            }
        }

        public async Task<IEnumerable<Tournament>> GetManagedTournaments(string accountId)
        {
            List<Tournament> tournaments = [];

            using (var session = _driver.AsyncSession())
            {
                var query = "MATCH (t:Tournament)-[:MANAGED_BY]->(:Player {AccountId : $AccountId}) RETURN t";

                var result = await session.RunAsync(query, new { AccountId = accountId });

                await result.ForEachAsync(record =>
                {
                    tournaments.Add(MappingFromDatabase.MapTournament(record["t"].As<INode>()));
                });

                return tournaments;
            }
        }

    }
}
