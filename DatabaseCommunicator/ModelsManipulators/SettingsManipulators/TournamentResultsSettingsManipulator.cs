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
    /// Loads, saves and creates settings how to calculate results of the tournament.
    /// </summary>
    public class TournamentResultsSettingsManipulator : BasicSettingsManipulator
    {
        public TournamentResultsSettingsManipulator(DatabaseConnectorSettings databaseConnectorSettings) : base(databaseConnectorSettings) { }

		private const string _settingsName = "ResultsSettings";
        private const string _connectionToTournamentNodeName = "HAS_RESULTS_SETTINGS";

        public async Task SaveTournamentResultsSettingsAsync(string tournamentId, ResultsSettings settings)
        {
            await SaveSettingsAsync(tournamentId, settings, _settingsName, _connectionToTournamentNodeName);
        }

        public async Task DeleteTournamentResultsSettingsAsync(string tournamentId)
        {
            await DeleteSettingsAsync(tournamentId, _settingsName, _connectionToTournamentNodeName);
        }

        public async Task<ResultsSettings> GetTournamentResultSettingsAsync(string tournamentId)
        {
            using (var session = _driver.AsyncSession())
            {
                IResultCursor result = await GetSettingsAsync(session, tournamentId, _settingsName, _connectionToTournamentNodeName);
                return await FirstOrNewSettings<ResultsSettings>(result);
            }
        }

    }
}
