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
    /// Loads, deletes, creates settings of the swiss tournaments.
    /// </summary>
    public class SwissSettingsManipulator : BasicSettingsManipulator
    {

        public SwissSettingsManipulator(DatabaseConnectorSettings databaseConnectorSettings) : base(databaseConnectorSettings) { }



		private const string _settingsName = "SwissTournamentSettings";
        private const string _connectionToTournamentNodeName = "HAS_SWISS_SETTINGS";

        public async Task SaveSwissSettingsAsync(string tournamentId, SwissTournamentSettings settings)
        {
            await SaveSettingsAsync(tournamentId, settings, _settingsName, _connectionToTournamentNodeName);
        }

        public async Task DeleteSwissSettingsAsync(string tournamentId)
        {
            await DeleteSettingsAsync(tournamentId, _settingsName, _connectionToTournamentNodeName);
        }

        public async Task<SwissTournamentSettings> GetSwissSettingsAsync(string tournamentId)
        {
            using (var session = _driver.AsyncSession())
            {
                IResultCursor result = await GetSettingsAsync(session, tournamentId, _settingsName, _connectionToTournamentNodeName);
                return await FirstOrNewSettings<SwissTournamentSettings>(result);
            }
        }

    }
}
