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
	/// Loads, deletes and save the location of the tournament
	/// </summary>
	public class AddressSettingsManipulator : BasicSettingsManipulator
	{
		public AddressSettingsManipulator(DatabaseConnectorSettings databaseConnectorSettings) : base(databaseConnectorSettings) { }

		private const string _settingsName = "Address";
		private const string _connectionToTournamentNodeName = "HAS_ADDRESS";

		public async Task SaveTournamenAddressAsync(string tournamentId, Address settings)
		{
			await SaveSettingsAsync(tournamentId, settings, _settingsName, _connectionToTournamentNodeName);
		}

		public async Task DeleteTournamentAddressAsync(string tournamentId)
		{
			await DeleteSettingsAsync(tournamentId, _settingsName, _connectionToTournamentNodeName);
		}

		public async Task<Address> GetTournamentAddressAsync(string tournamentId)
		{
			using (var session = _driver.AsyncSession())
			{
				IResultCursor result = await GetSettingsAsync(session, tournamentId, _settingsName, _connectionToTournamentNodeName);
				return await FirstOrNewSettings<Address>(result);
			}
		}
	}
}
