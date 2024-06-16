using DatabaseCommunicator.Models;
using Neo4j.Driver;
using Neo4j.Driver.Preview.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseCommunicator.ModelsManipulators.SettingsManipulators
{
    /// <summary>
    /// Provides methods for manipulation of settings of the tournament.
    /// </summary>
    public abstract class BasicSettingsManipulator : DatabaseModelManipulator
    {


        public BasicSettingsManipulator(DatabaseConnectorSettings databaseConnectorSettings) : base(databaseConnectorSettings) { }


        protected async Task SaveSettingsAsync<T>(string tournamentId, T settings, string settingsName, string connectionToTournamentNodeName)
        {
            using (var session = _driver.AsyncSession())
            {
                var properties = new
                {
                    TournamentId = tournamentId,

                    Settings = settings
                };

                var query = "MATCH (tournament: Tournament { Id: $TournamentId}) " +
                            "CREATE (settings:" + settingsName + " $Settings) " +
                            "CREATE (tournament)-[:" + connectionToTournamentNodeName + "]->(settings) ";

                await session.RunAsync(query, properties);
            }
        }



        protected async Task DeleteSettingsAsync(string tournamentId, string settingsName, string connectionToTournamentNodeName)
        {
            using (var session = _driver.AsyncSession())
            {
                var properties = new
                {
                    TournamentId = tournamentId,

                    SettingsName = settingsName,
                    ConnectionToTournamentNodeName = connectionToTournamentNodeName
                };

                var query = "MATCH (tournament:Tournament {Id : $TournamentId})-[:" + connectionToTournamentNodeName + "]->(settings:" + settingsName + ") " +
                            "DETACH DELETE settings";

                await session.RunAsync(query, properties);
            }
        }

        

        protected async Task<IResultCursor> GetSettingsAsync(IAsyncSession session, string tournamentId, string settingsName, string connectionToTournamentNodeName)
        {
            var properties = new
            {
                TournamentId = tournamentId
            };

            var query = "MATCH (tournament:Tournament {Id : $TournamentId})-[:" + connectionToTournamentNodeName + "]->(settings:" + settingsName + ") " +
                        "RETURN settings";

            return await session.RunAsync(query, properties);       
        }

        
        protected async Task<T> FirstOrNewSettings<T>(IResultCursor result) where T : class, new()
        {
            IReadOnlyList<T> parsed = await result.ToListAsync<T>();

            if (parsed.Count == 0)
            {
                return new T();
            }

            return parsed[0];        
        }
    }
}
