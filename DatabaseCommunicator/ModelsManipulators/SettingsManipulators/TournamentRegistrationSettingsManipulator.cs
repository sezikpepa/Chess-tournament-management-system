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
    /// Loads, saves and creates the specification of the registration period of the tournament.
    /// </summary>
    public class TournamentRegistrationSettingsManipulator : BasicSettingsManipulator
    {

        public TournamentRegistrationSettingsManipulator(DatabaseConnectorSettings databaseConnectorSettings) : base(databaseConnectorSettings) { }


		private const string _settingsName = "TournamentRegistrationSettings";
        private const string _connectionToTournamentNodeName = "HAS_REGISTRATION_SETTINGS";

        public async Task SaveTournamentRegistrationSettingsAsync(string tournamentId, RegistrationSettings settings)
        {
            await SaveSettingsAsync(tournamentId, settings, _settingsName, _connectionToTournamentNodeName);
        }

        public async Task DeleteTournamentRegistrationSettingsAsync(string tournamentId)
        {
            await DeleteSettingsAsync(tournamentId, _settingsName, _connectionToTournamentNodeName);
        }

        public async Task<RegistrationSettings> GetTournamentRegistrationSettingsAsync(string tournamentId)
        {
            using (var session = _driver.AsyncSession())
            {
                IResultCursor result = await GetSettingsAsync(session, tournamentId, _settingsName, _connectionToTournamentNodeName);
                return await FirstOrNewSettings<RegistrationSettings>(result);
            }
        }


    }
}
