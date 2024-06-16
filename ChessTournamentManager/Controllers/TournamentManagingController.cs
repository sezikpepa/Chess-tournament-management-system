using ChessTournamentManager.LanguageAssets.TournamentManagementLabels;
using ChessTournamentMate.Shared;
using ChessTournamentMate.Shared.AvailableValues;
using ChessTournamentMate.Shared.QuickResponseMessages;
using DatabaseCommunicator;
using DatabaseCommunicator.Models;
using DatabaseCommunicator.ModelsManipulators;
using DatabaseCommunicator.ModelsManipulators.SettingsManipulators;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using TournamentLibrary.Participants;
using TournamentLibrary.TournamentHandlers;



namespace ChessTournamentManager.Controllers
{
	/// <summary>
	/// Methods useful for manager of the tournament, possible to manage tournament from different points of view.
	/// </summary>
    [Route("api/tournamentManaging")]
    [ApiController]
    public class TournamentManagingController : ControllerBase
    {
        private readonly PlayerTournamentRegisteringManipulator _playerTournamentRegisteringManipulator;
		private readonly TournamentManipulator _tournamentManipulator;
		private readonly TournamentTeamsManipulator _tournamentTeamsManipulator;
        private readonly ManagedEntitiesManipulator _managedEntitiesManipulator;
		private readonly AddressSettingsManipulator _addressSettingsManipulator;
		private readonly TournamentRegistrationSettingsManipulator _tournamentRegistrationSettingsManipulator;
		private readonly TournamentResultsSettingsManipulator _tournamentResultsSettingsManipulator;
		private readonly TournamentTeamDrawSettingsManipulator _tournamentTeamDrawSettingsManipulator;
		private readonly TournamentTimeControlManipulator _tournamentTimeControlManipulator;
		private readonly TournamentManager _tournamentManager;
		private readonly IStringLocalizer<TournamentManagementLabels> _localizer;

		public TournamentManagingController(PlayerTournamentRegisteringManipulator playerTournamentRegisteringManipulator,
                                            TournamentManipulator tournamentManipulator,
											TournamentTeamsManipulator tournamentTeamsManipulator,
                                            ManagedEntitiesManipulator managedEntitiesManipulator,
											AddressSettingsManipulator addressSettingsManipulator,
											TournamentRegistrationSettingsManipulator tournamentRegistrationSettingsManipulator,
											TournamentResultsSettingsManipulator tournamentResultsSettingsManipulator,
											TournamentTeamDrawSettingsManipulator tournamentTeamDrawSettingsManipulator,
											TournamentTimeControlManipulator tournamentTimeControlManipulator,
											TournamentManager tournamentManager,
											IStringLocalizer<TournamentManagementLabels> localizer)
        {
            _playerTournamentRegisteringManipulator = playerTournamentRegisteringManipulator;
			_tournamentManipulator = tournamentManipulator;
			_tournamentTeamsManipulator = tournamentTeamsManipulator;
            _managedEntitiesManipulator = managedEntitiesManipulator;
			_addressSettingsManipulator = addressSettingsManipulator;
			_tournamentRegistrationSettingsManipulator = tournamentRegistrationSettingsManipulator;
			_tournamentResultsSettingsManipulator = tournamentResultsSettingsManipulator;
			_tournamentTeamDrawSettingsManipulator = tournamentTeamDrawSettingsManipulator;
			_tournamentTimeControlManipulator = tournamentTimeControlManipulator;
			_tournamentManager = tournamentManager;
			_localizer = localizer;
		}

		/// <summary>
		/// If an unregistered player to the tournament. Manager of the tournament can register him as an unknown player into the tournament. This method performs the registration.
		/// </summary>
		/// <param name="tournamentId">Id of tournament</param>
		/// <param name="newPlayer">Information about the player</param>
		/// <returns></returns>
        [HttpPost("{tournamentId}/registerUnknownPlayer")]
        public async Task RegisterUnknownPlayerToTournament(string tournamentId, [FromBody] Player newPlayer)
        {
            await _playerTournamentRegisteringManipulator.RegisterUnknownPlayerInTournament(newPlayer, tournamentId);
        }

		/// <summary>
		/// Creates new tournament and assign a user as the manager
		/// </summary>
		/// <param name="newTournament">Information about the tournament</param>
		/// <param name="accountId">AccountId of the manager</param>
		/// <returns></returns>
        [HttpPost("createNewTournament/{accountId}")]
        public async Task<string> CreateNewTournament(Tournament newTournament, string accountId)
        {
			return await _tournamentManipulator.CreateNewTournament(newTournament, accountId);
		}

		/// <summary>
		/// Performs procedure the start the tournament.
		/// </summary>
		/// <param name="tournamentId">Id of the tournament</param>
		/// <returns></returns>
		[HttpPost("{tournamentId}/start")]
        public async Task<QuickResponseMessage> StartTournamentAsync(string tournamentId)
        {
			Tournament tournament = await _tournamentManipulator.GetTournamentInformation(tournamentId);

			int expectedNumberOfRounds;

			if (tournament.IsTeam)
			{
				List<Team> teams = await _tournamentTeamsManipulator.GetRegisteredTeamsAsync(tournamentId);
                if(teams.Count < 2)
                {
                    return new UnsuccessfulMessage(_localizer["startTournamentTeamNotEnough"]);
                }
				expectedNumberOfRounds = CalculateExpectedNumberOfRounds(teams, tournament.TournamentType);
			}
			else
			{
				List<Player> players = await _tournamentManager.GetPlayersInTournament(tournamentId);
				if (players.Count < 2)
				{
					return new UnsuccessfulMessage(_localizer["startTournamentSingleNotEnough"]);
				}
				expectedNumberOfRounds = CalculateExpectedNumberOfRounds(players, tournament.TournamentType);
			}

			await _tournamentManipulator.StartTournament(tournamentId, expectedNumberOfRounds);
            return new SuccessfulMessage(_localizer["startTournamentStarted"]);
		}

		/// <summary>
		/// Every tournament is set to the specific number of rounds based on the number of participants. This method calculates this number.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="participants">Participants in the tournament</param>
		/// <param name="tournamentType">Draw type of the tournament (Swiss, RoundRobin...)</param>
		/// <returns></returns>
		private int CalculateExpectedNumberOfRounds<T>(IEnumerable<T> participants, TournamentTypes tournamentType) where T : class, IParticipant, new()
        {
			ITournament<T> tournamentManagerHandler;

			TournamentHandlerFactory<T> handlerFactory = new();
			tournamentManagerHandler = handlerFactory.CreateTournamentHandler(tournamentType);


			tournamentManagerHandler.AddParticipantRange(participants);


			return tournamentManagerHandler.ExpectedTournamentRounds;
		}

		/// <summary>
		/// Returns all tournaments which are managed by the specific user.
		/// </summary>
		/// <param name="accountId">AccountId of the user</param>
		/// <returns></returns>
        public async Task<IEnumerable<Tournament>> GetManagedTournaments(string accountId)
        {
            return await _managedEntitiesManipulator.GetManagedTournaments(accountId);
        }

		/// <summary>
		/// Deletes selected players from the tournament. Terminates their registrations.
		/// </summary>
		/// <param name="tournamentId">Id of tournament</param>
		/// <param name="participantIds">Players to delete from the tournament</param>
		/// <returns></returns>
        public async Task DeleteParticipantsFromTournament(string tournamentId, IEnumerable<string> participantIds)
        {
            foreach(string id in participantIds)
            {
				await _playerTournamentRegisteringManipulator.DeletePlayerFromTournament(tournamentId, id);
			}
		}

		/// <summary>
		/// Deletes selected teams from the tournament. Terminates their registrations.
		/// </summary>
		/// <param name="tournamentId">Id of the tournament</param>
		/// <param name="teamIds">Team the delete from the tournament</param>
		/// <returns></returns>
        public async Task DeleteTeamFromTournament(string tournamentId, IEnumerable<string> teamIds)
        {
            foreach(string id in teamIds)
            {
				await _tournamentManipulator.DeleteTeamFromTournamentAsync(tournamentId, id);
			}
		}

		/// <summary>
		/// Deletes selected team from the tournament.
		/// </summary>
		/// <param name="tournamentId">Id of the tournament</param>
		/// <param name="teamId">Id of the team to delete</param>
		/// <returns></returns>
		public async Task DeleteTeamFromTournament(string tournamentId, string teamId)
		{
			await _tournamentManipulator.DeleteTeamFromTournamentAsync(tournamentId, teamId);			
		}

		/// <summary>
		/// Moves tournament to the next round so it is possible to generate it.
		/// </summary>
		/// <param name="tournamentId">Id of the tournament</param>
		/// <returns></returns>
		public async Task StartNewRoundAsync(string tournamentId)
        {
            await _tournamentManipulator.TournamentMovedToNextRound(tournamentId);
        }

		/// <summary>
		/// Checks if the user is manager of the tournament -> if the user can modify the tournament or not
		/// </summary>
		/// <param name="tournamentId">Id of the tournament</param>
		/// <param name="accountId">AccountId of the user, who would want to modify the tournament</param>
		/// <returns></returns>
		public async Task<bool> IsManagerOfTournament(string tournamentId, string accountId)
		{
			IEnumerable<Player> managers = await _tournamentManipulator.GetManagersOfTournaments(tournamentId);

			return managers.Select(x => x.AccountId).ToList().Contains(accountId);
		}

		/// <summary>
		/// Deletes tournament from the database with all settings and other data.
		/// </summary>
		/// <param name="tournamentId">Id of the tournament</param>
		/// <returns></returns>
		public async Task DeleteTournament(string tournamentId)
		{
			await _tournamentManipulator.DeleteTournamentNodeAsync(tournamentId);
			await _addressSettingsManipulator.DeleteTournamentAddressAsync(tournamentId);
			await _tournamentRegistrationSettingsManipulator.DeleteTournamentRegistrationSettingsAsync(tournamentId);
			await _tournamentResultsSettingsManipulator.DeleteTournamentResultsSettingsAsync(tournamentId);
			await _tournamentTeamDrawSettingsManipulator.DeleteTournamentTeamDrawSettingsAsync(tournamentId);
			await _tournamentTimeControlManipulator.DeleteTournamentTimeSettingsAsync(tournamentId);
		}

	}
}
