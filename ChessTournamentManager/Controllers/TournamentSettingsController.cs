using DatabaseCommunicator.Models;
using DatabaseCommunicator.ModelsManipulators.SettingsManipulators;

namespace ChessTournamentManager.Controllers
{
	/// <summary>
	/// Contains methods to save and load different types of settings of the tournament.
	/// </summary>
    public class TournamentSettingsController
	{
		private readonly TournamentTimeControlManipulator _tournamentTimeControlManipulator;
		private readonly TournamentTeamDrawSettingsManipulator _tournamentTeamDrawSettingsManipulator;
		private readonly TournamentRegistrationSettingsManipulator _tournamentRegistrationSettingsManipulator;
        private readonly TournamentResultsSettingsManipulator _tournamentResultsSettingsManipulator;
		private readonly AddressSettingsManipulator _addressSettingsManipulator;
		private readonly SwissSettingsManipulator _swissSettingsManipulator;

		public TournamentSettingsController(TournamentTimeControlManipulator tournamentTimeControlManipulator,
											TournamentTeamDrawSettingsManipulator tournamentTeamDrawSettingsManipulator,
											TournamentRegistrationSettingsManipulator tournamentRegistrationSettingsManipulator,
                                            TournamentResultsSettingsManipulator tournamentResultsSettingsManipulator,
											AddressSettingsManipulator addressSettingsManipulator,
											SwissSettingsManipulator swissSettingsManipulator) 
		{
			_tournamentTimeControlManipulator = tournamentTimeControlManipulator;
			_tournamentTeamDrawSettingsManipulator = tournamentTeamDrawSettingsManipulator;
			_tournamentRegistrationSettingsManipulator = tournamentRegistrationSettingsManipulator;
            _tournamentResultsSettingsManipulator = tournamentResultsSettingsManipulator;
			_addressSettingsManipulator = addressSettingsManipulator;
			_swissSettingsManipulator = swissSettingsManipulator;
		}

		public async Task SaveTimeSettingsAsync(string tournamentId, TimeControlSettings settings)
		{
			await _tournamentTimeControlManipulator.DeleteTournamentTimeSettingsAsync(tournamentId);
			await _tournamentTimeControlManipulator.SaveTournamentTimeSettingsAsync(tournamentId, settings);
		}

		public async Task<TimeControlSettings> GetTournamentTimeControlAsync(string tournamentId)
		{
			return await _tournamentTimeControlManipulator.GetTournamentTimeControl(tournamentId);
		}

		public async Task SaveTeamDrawSettingsAsync(string tournamentId, TeamDrawSettings settings)
		{
			await _tournamentTeamDrawSettingsManipulator.DeleteTournamentTeamDrawSettingsAsync(tournamentId);
			await _tournamentTeamDrawSettingsManipulator.SaveTournamentTeamDrawSettingsAsync(tournamentId, settings);
		}

		public async Task<TeamDrawSettings> GetTournamentTeamDrawSettingsAsync(string tournamentId)
		{
			return await _tournamentTeamDrawSettingsManipulator.GetTournamentTeamDrawSettingsAsync(tournamentId);
		}

		public async Task SaveRegistrationSettingsAsync(string tournamentId, RegistrationSettings settings)
		{
			await _tournamentRegistrationSettingsManipulator.DeleteTournamentRegistrationSettingsAsync(tournamentId);
			await _tournamentRegistrationSettingsManipulator.SaveTournamentRegistrationSettingsAsync(tournamentId, settings);
		}

		public async Task<RegistrationSettings> GetRegistrationSettingsAsync(string tournamentId)
		{
			return await _tournamentRegistrationSettingsManipulator.GetTournamentRegistrationSettingsAsync(tournamentId);
		}

        public async Task SaveResultsSettingsAsync(string tournamentId, ResultsSettings settings)
		{
			await _tournamentResultsSettingsManipulator.DeleteTournamentResultsSettingsAsync(tournamentId);
			await _tournamentResultsSettingsManipulator.SaveTournamentResultsSettingsAsync(tournamentId, settings);
		}
		public async Task<ResultsSettings> GetResultsSettingsAsync(string tournamentId)
		{
			return await _tournamentResultsSettingsManipulator.GetTournamentResultSettingsAsync(tournamentId);
		}


		public async Task SaveTournamentAddress(string tournamentId, Address address)
		{
			await _addressSettingsManipulator.DeleteTournamentAddressAsync(tournamentId);
			await _addressSettingsManipulator.SaveTournamenAddressAsync(tournamentId, address);
		}

		public async Task<Address> GetTournamentAddress(string tournamentId)
		{
			return await _addressSettingsManipulator.GetTournamentAddressAsync(tournamentId);
		}


		public async Task<SwissTournamentSettings> GetSwissSettings(string tournamentId)
		{
			return await _swissSettingsManipulator.GetSwissSettingsAsync(tournamentId);
		}

		public async Task SaveSwissSettings(string tournamentId, SwissTournamentSettings settings)
		{
			await _swissSettingsManipulator.DeleteSwissSettingsAsync(tournamentId);
			await _swissSettingsManipulator.SaveSwissSettingsAsync(tournamentId, settings);
		}
	}
}
