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
    /// Loads, creates and deletes time control settings of the tournament.
    /// </summary>
    public class TournamentTimeControlManipulator : DatabaseModelManipulator
    {

        public TournamentTimeControlManipulator(DatabaseConnectorSettings databaseConnectorSettings) : base(databaseConnectorSettings) { }


		public async Task SaveTournamentTimeSettingsAsync(string tournamentId, TimeControlSettings settings)
        {
            using (var session = _driver.AsyncSession())
            {
                foreach (var element in settings.Elements)
                {
                    var properties = new
                    {
                        TournamentId = tournamentId,
                        element.Increment,
                        element.UntilMove,

                        element.AvailableTimeHours,
                        element.AvailableTimeMinutes,
                        element.AvailableTimeSeconds
                    };

                    var query = "MATCH (tournament: Tournament { Id: $TournamentId}) " +
                                "MERGE (tournament)-[:HAS_TIME_SETTINGS]->(timeSettings:TimeSettings) " +
                                "MERGE (timeSettingsPiece:TimeSettingsPiece {Increment : $Increment, UntilMove : $UntilMove, AvailableTimeHours : $AvailableTimeHours, AvailableTimeMinutes : $AvailableTimeMinutes, AvailableTimeSeconds : $AvailableTimeSeconds }) " +
                                "MERGE (timeSettingsPiece)-[:BELONGS_TO]->(timeSettings)";

                    IResultCursor result = await session.RunAsync(query, properties);
                }
            }
        }

        public async Task DeleteTournamentTimeSettingsAsync(string tournamentId)
        {
            using (var session = _driver.AsyncSession())
            {
                var properties = new
                {
                    TournamentId = tournamentId
                };

                var query = "MATCH (tournament:Tournament {Id : $TournamentId})-[:HAS_TIME_SETTINGS]->(timeSettings:TimeSettings)<-[:BELONGS_TO]-(piece:TimeSettingsPiece) " +
							"DETACH DELETE piece, timeSettings";

                await session.RunAsync(query, properties);
            }
        }

        public async Task<TimeControlSettings> GetTournamentTimeControl(string tournamentId)
        {
            using (var session = _driver.AsyncSession())
            {
                var properties = new
                {
                    TournamentId = tournamentId
                };

                var query = "MATCH (tournament:Tournament {Id : $TournamentId})-[:HAS_TIME_SETTINGS]->(timeSettings:TimeSettings)<-[:BELONGS_TO]-(piece:TimeSettingsPiece)" +
                            "RETURN piece";

                IResultCursor result = await session.RunAsync(query, properties);

                var settings = await result.ToListAsync<TimeControlSettingsPiece>();

                if (settings.Count == 0)
                {
                    return new TimeControlSettings();
                }

                return new TimeControlSettings(settings.ToList());
            }
        }

    }
}
