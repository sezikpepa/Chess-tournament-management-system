using ChessTournamentManager.LanguageAssets.TournamentRegistration;
using ChessTournamentMate.Shared;
using ChessTournamentMate.Shared.QuickResponseMessages;
using DatabaseCommunicator.Models;
using DatabaseCommunicator.ModelsManipulators;
using DatabaseCommunicator.ModelsManipulators.SettingsManipulators;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;



namespace ChessTournamentManager.Controllers
{
    /// <summary>
    /// Handles registration to the tournaments and their cancellation.
    /// </summary>
    [Route("api/tournamentRegistration")]
    [ApiController]
    public class TournamentRegistrationController : ControllerBase
    {
        private readonly PlayerTournamentRegisteringManipulator _playerTournamentRegisteringManipulator;
		private readonly TournamentManipulator _tournamentManipulator;
		private readonly TournamentRegistrationSettingsManipulator _tournamentRegistrationSettingsManipulator;
		private readonly ProfileManipulator _profileManipulator;
		private readonly TournamentTeamsManipulator _tournamentTeamsManipulator;
		private readonly TeamsWithPlayersManipulator _teamsWithPlayersManipulator;
        private readonly TournamentTeamDrawSettingsManipulator _tournamentTeamDrawSettingsManipulator;
        private readonly TournamentController _tournamentController;
		private readonly IStringLocalizer<TournamentRegistrationLabels> _localizer;

		public TournamentRegistrationController(PlayerTournamentRegisteringManipulator playerTournamentRegisteringManipulator,
                                                TournamentManipulator tournamentManipulator,
                                                TournamentRegistrationSettingsManipulator tournamentRegistrationSettingsManipulator,
                                                ProfileManipulator profileManipulator,
                                                TournamentTeamsManipulator tournamentTeamsManipulator,
												TeamsWithPlayersManipulator teamsWithPlayersManipulator,
												TournamentTeamDrawSettingsManipulator tournamentTeamDrawSettingsManipulator,
												TournamentController tournamentController,
												IStringLocalizer<TournamentRegistrationLabels> localizer) 
        {
            _playerTournamentRegisteringManipulator = playerTournamentRegisteringManipulator;
			_tournamentManipulator = tournamentManipulator;
			_tournamentRegistrationSettingsManipulator = tournamentRegistrationSettingsManipulator;
			_profileManipulator = profileManipulator;
			_tournamentTeamsManipulator = tournamentTeamsManipulator;
			_teamsWithPlayersManipulator = teamsWithPlayersManipulator;
            _tournamentTeamDrawSettingsManipulator = tournamentTeamDrawSettingsManipulator;
            _tournamentController = tournamentController;
			_localizer = localizer;
		}

		/// <summary>
		/// Permorms registration process to register player to the tournament
		/// </summary>
		/// <param name="tournamentId"></param>
		/// <param name="accountId"></param>
		/// <returns></returns>
        [HttpPost("{tournamentId}/register")]
        public async Task RegisterPlayerForTournament(string tournamentId, [FromBody] string accountId)
        {
            await _playerTournamentRegisteringManipulator.RegisterPlayerForTournament(accountId, tournamentId);
		}

		/// <summary>
		/// Returns if the user can register to the tournament. (registration period, capacity...)
		/// </summary>
		/// <param name="tournamentId"></param>
		/// <param name="accountId"></param>
		/// <returns></returns>
		[HttpGet("{tournamentId}/possible/{accountId}")]
		public async Task<QuickResponseMessage> CanRegisterAccountToTournament(string tournamentId, string accountId)
		{
			string? playerId = (await _profileManipulator.GetIdOfAccount(accountId));

			Tournament tournament = await _tournamentManipulator.GetTournamentInformation(tournamentId);
            RegistrationSettings registrationSettings = await _tournamentRegistrationSettingsManipulator.GetTournamentRegistrationSettingsAsync(tournamentId);
            bool isRegistered = (await _profileManipulator.GetPlayersTournament(playerId)).Contains(tournament);
			int currentNumberOfPlayers = await _tournamentController.GetNumberOfAllPlayersInTournament(tournamentId);

            if (tournament.HasStarted)
            {
                return new UnsuccessfulMessage(_localizer["singleWarningHasStarted"]);
            }
            if (currentNumberOfPlayers >= registrationSettings.MaximumNumberOfParticipants)
            {
                return new UnsuccessfulMessage(_localizer["singleWarningMaximumPlayers"]);
            }
            if (registrationSettings.RegistrationStart?.Date > DateTime.Now.Date)
            {
                return new UnsuccessfulMessage($"{_localizer["singleWarningRegistrationNotOpened"]} {registrationSettings.RegistrationStart:dd.MM.yyyy}");
            }
			if (registrationSettings.RegistrationEnd?.Date < DateTime.Now.Date)
			{
				return new UnsuccessfulMessage(_localizer["singleWarningRegistrationClosed"]);
			}
            if (isRegistered)
            {
                return new UnsuccessfulMessage(_localizer["singleWarningIsRegistered"]);
            }

            return new SuccessfulMessage(_localizer["singleCanRegister"]);
		}

		/// <summary>
		/// Returns if the team can register to the tournament. (Registration period, capacity...)
		/// </summary>
		/// <param name="tournamentId"></param>
		/// <param name="teamId"></param>
		/// <returns></returns>
        public async Task<QuickResponseMessage> CanTeamRegisterToTournament(string tournamentId, string teamId)
        {
			Tournament tournament = await _tournamentManipulator.GetTournamentInformation(tournamentId);
			RegistrationSettings registrationSettings = await _tournamentRegistrationSettingsManipulator.GetTournamentRegistrationSettingsAsync(tournamentId);
			List<TeamWithPlayers> registeredTeams = await _tournamentTeamsManipulator.GetRegisteredTeamsWithPlayersAsync(tournamentId);
            int currentNumberOfTeams = await _tournamentController.GetNumberOfRegisteredTeamsWithPlayers(tournamentId);

            TeamDrawSettings drawSettings = await _tournamentTeamDrawSettingsManipulator.GetTournamentTeamDrawSettingsAsync(tournamentId);

			List<Player> playersInTeam = await _teamsWithPlayersManipulator.GetPlayersInTeamAsync(teamId);

			if (tournament.HasStarted)
			{
				return new UnsuccessfulMessage(_localizer["teamWarningHasStarted"]);
			}
			if (currentNumberOfTeams >= registrationSettings.MaximumNumberOfParticipants)
			{
				return new UnsuccessfulMessage(_localizer["teamWarningMaximumTeams"]);
			}
			if (registrationSettings.RegistrationStart?.Date > DateTime.Now.Date)
			{
				return new UnsuccessfulMessage($"{_localizer["teamWarningRegistrationNotOpened"]} {registrationSettings.RegistrationStart:dd.MM.yyyy}");
			}
			if (registrationSettings.RegistrationEnd?.Date < DateTime.Now.Date)
			{
				return new UnsuccessfulMessage(_localizer["teamWarningRegistrationClosed"]);
			}
			if(registeredTeams.Select(x => x.Team).Select(x => x.Id).Contains(teamId))
			{
				return new UnsuccessfulMessage(_localizer["teamWarningAlreadyRegistered"]);				
			}

			foreach(var registeredTeam in registeredTeams)
			{
				if(registeredTeam.Players.Select(x => x.Id).Intersect(playersInTeam.Select(x => x.Id)).Any())
				{
					return new UnsuccessfulMessage(_localizer["teamWarningPlayerInTeamIsInTournament"]);
				}
			}

			if(drawSettings.TeamSizeForRound != playersInTeam.Count())
			{
				return new UnsuccessfulMessage($"{_localizer["teamWarningWrongSizePart1"]} {drawSettings.TeamSizeForRound} {_localizer["teamWarningWrongSizePart2"]} {playersInTeam.Count()} {_localizer["teamWarningWrongSizePart3"]}");
			}

			return new SuccessfulMessage(_localizer["teamCanRegister"]);
        }

		/// <summary>
		/// Permorms registration process of the provided team to the tournament.
		/// </summary>
		/// <param name="tournamentId"></param>
		/// <param name="teamId"></param>
		/// <returns></returns>
		public async Task RegisterTeamInTournament(string tournamentId, string teamId)
		{
			await _tournamentTeamsManipulator.RegisterTeamInTournament(tournamentId, teamId);
		}


		/// <summary>
		/// Checks if the user can cancel registration in the tournament.
		/// </summary>
		/// <param name="tournamentId"></param>
		/// <param name="accountId"></param>
		/// <returns></returns>
		public async Task<QuickResponseMessage> CanUnregisterSingle(string tournamentId, string accountId)
		{
			string? playerId = (await _profileManipulator.GetIdOfAccount(accountId));

			Tournament tournament = await _tournamentManipulator.GetTournamentInformation(tournamentId);
			RegistrationSettings registrationSettings = await _tournamentRegistrationSettingsManipulator.GetTournamentRegistrationSettingsAsync(tournamentId);
			bool isRegistered = (await _profileManipulator.GetPlayersTournament(playerId)).Contains(tournament);

			if (tournament.HasStarted)
			{
				return new UnsuccessfulMessage(_localizer["singleUnregisterWarningHasStarted"]);
			}
			if (registrationSettings.RegistrationEnd?.Date < DateTime.Now.Date)
			{
				return new UnsuccessfulMessage(_localizer["singleUnregisterWarningRegistrationEnded"]);
			}
			if (!isRegistered)
			{
				return new UnsuccessfulMessage(_localizer["singleUnregisterWarningNotRegistered"]);
			}

			return new SuccessfulMessage(_localizer["singleUnregisterCanUnregister"]);
		}

		/// <summary>
		/// Performs registration cancellation of the user in the tournament.
		/// </summary>
		/// <param name="tournamentId"></param>
		/// <param name="accountId"></param>
		/// <returns></returns>
		public async Task UnregisterFromTournamentSingle(string tournamentId, string accountId)
		{
			string? playerId = (await _profileManipulator.GetIdOfAccount(accountId));

			await _playerTournamentRegisteringManipulator.DeletePlayerFromTournament(tournamentId, playerId);
		}

		/// <summary>
		/// Checks if the team can unregister team from the tournament.
		/// </summary>
		/// <param name="tournamentId"></param>
		/// <param name="teamId"></param>
		/// <returns></returns>
		public async Task<QuickResponseMessage> CanUnregisterTeam(string tournamentId, string teamId)
		{
			Tournament tournament = await _tournamentManipulator.GetTournamentInformation(tournamentId);
			RegistrationSettings registrationSettings = await _tournamentRegistrationSettingsManipulator.GetTournamentRegistrationSettingsAsync(tournamentId);
			bool isRegistered = (await _tournamentTeamsManipulator.GetRegisteredTeamsAsync(tournamentId)).Select(x => x.Id).ToList().Contains(teamId);

			if (tournament.HasStarted)
			{
				return new UnsuccessfulMessage(_localizer["teamUnregisterWarningHasStarted"]);
			}
			if (registrationSettings.RegistrationEnd?.Date < DateTime.Now.Date)
			{
				return new UnsuccessfulMessage(_localizer["teamUnregisterWarningRegistrationEnded"]);
			}
			if (!isRegistered)
			{
				return new UnsuccessfulMessage(_localizer["teamUnregisterWarningNotRegistered"]);
			}

			return new SuccessfulMessage(_localizer["teamUnregisterCanUnregister"]);
		}

	}
}
