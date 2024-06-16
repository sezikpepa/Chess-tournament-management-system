using DatabaseCommunicator.Models;
using Neo4j.Driver;
using Neo4j.Driver.Preview.Mapping;

namespace DatabaseCommunicator.ModelsManipulators.SettingsManipulators
{
    /// <summary>
    /// Loads, creates and deletes settings of the specification how to performs pairing of teams.
    /// </summary>
    public class TournamentTeamDrawSettingsManipulator : BasicSettingsManipulator
    {
        public TournamentTeamDrawSettingsManipulator(DatabaseConnectorSettings databaseConnectorSettings) : base(databaseConnectorSettings) { }


		private const string _settingsName = "TeamDrawSettings";
        private const string _connectionToTournamentNodeName = "HAS_TEAMDRAW_SETTINGS";

        public async Task SaveTournamentTeamDrawSettingsAsync(string tournamentId, TeamDrawSettings settings)
        {
            await SaveSettingsAsync(tournamentId, settings, _settingsName, _connectionToTournamentNodeName);
        }

        public async Task DeleteTournamentTeamDrawSettingsAsync(string tournamentId)
        {
            await DeleteSettingsAsync(tournamentId, _settingsName, _connectionToTournamentNodeName);
        }

        public async Task<TeamDrawSettings> GetTournamentTeamDrawSettingsAsync(string tournamentId)
        {
            using (var session = _driver.AsyncSession())
            {
                IResultCursor result = await GetSettingsAsync(session, tournamentId, _settingsName, _connectionToTournamentNodeName);
                return await FirstOrNewSettings<TeamDrawSettings>(result);
            }
        }
    }
}
