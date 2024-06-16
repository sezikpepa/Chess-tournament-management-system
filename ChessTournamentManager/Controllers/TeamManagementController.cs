using DatabaseCommunicator.Models;
using DatabaseCommunicator;
using ChessTournamentMate.Shared;
using Microsoft.AspNetCore.Mvc;
using DatabaseCommunicator.ModelsManipulators;
using ChessTournamentMate.Shared.QuickResponseMessages;
using Microsoft.Extensions.Localization;
using ChessTournamentManager.LanguageAssets.TeamManagement;


namespace ChessTournamentManager.Controllers
{
	/// <summary>
	/// Methods for management of the team
	/// </summary>
	[Route("api/teamManagement")]
	[ApiController]
	public class TeamManagementController : ControllerBase
	{
		private readonly TournamentTeamsManipulator _tournamentTeamsManipulator;
		private readonly ManagedEntitiesManipulator _managedEntitiesManipulator;
		private readonly TournamentManager _tournamentManager;
		private readonly TeamsWithPlayersManipulator _teamsWithPlayersManipulator;
		private readonly IStringLocalizer<TeamManagementLabels> _localizer;

		public TeamManagementController(TournamentTeamsManipulator tournamentTeamsManipulator,
										ManagedEntitiesManipulator managedEntitiesManipulator,
										TournamentManager tournamentManager,
										TeamsWithPlayersManipulator teamsWithPlayersManipulator,
										IStringLocalizer<TeamManagementLabels> localizer)
		{
			_tournamentTeamsManipulator = tournamentTeamsManipulator;
			_managedEntitiesManipulator = managedEntitiesManipulator;
			_tournamentManager = tournamentManager;
			_teamsWithPlayersManipulator = teamsWithPlayersManipulator;
			_localizer = localizer;
		}

		/// <summary>
		/// Returns all teams which are managed by a user with specific accountId
		/// </summary>
		/// <param name="accountId">Account id of logged user</param>
		/// <returns>Managed teams of the user</returns>
		[HttpGet("getManagedTeams/{accountId}")]
        public async Task<IEnumerable<TeamWithPlayers>> GetManagedTeams(string accountId)
		{
			IEnumerable<Team> managedTeams = await _managedEntitiesManipulator.GetManagedTeams(accountId);

			List<TeamWithPlayers> managedTeamsWithPlayers = [];

			foreach (var team in managedTeams)
			{
				managedTeamsWithPlayers.Add(new TeamWithPlayers(team, await _teamsWithPlayersManipulator.GetPlayersInTeamAsync(team.Id)));
			}

			return managedTeamsWithPlayers;
        }

		/// <summary>
		/// Creates new team and assign user as a manager of this team
		/// </summary>
		/// <param name="accountId">Account id of the user</param>
		/// <param name="newTeam">New team informations</param>
		/// <returns></returns>
        [HttpPost("createNewTeam/{accountId}")]
		public async Task CreateNewTeam(string accountId, [FromBody] Team newTeam)
		{
			await _tournamentManager.CreateNewTeam(newTeam, accountId);
		}

		/// <summary>
		/// Saves changes in the roster of the team
		/// </summary>
		/// <param name="teamId">Team id</param>
		/// <param name="players">New roster</param>
		/// <returns></returns>
        [HttpPost("saveTeamPlayers/{teamId}")]
        public async Task SavePlayersOfTeam(string teamId, [FromBody] List<Player> players)
        {
            await _teamsWithPlayersManipulator.SaveTeamPlayers(teamId, players);
        }

		/// <summary>
		/// Checks if the user can modify the team, it is not possible to modify the team if it is registered to tournament.
		/// </summary>
		/// <param name="teamId">Id of the team</param>
		/// <returns></returns>
		public async Task<QuickResponseMessage> CanModifyTeam(string teamId)
		{
			IEnumerable<Tournament> tournaments = await _tournamentTeamsManipulator.GetTournamentsOfTeam(teamId);
			
			if(tournaments.Any())
			{
				return new UnsuccessfulMessage(_localizer["cannotModifyRegisteredToTournament"]);
			}
			return new SuccessfulMessage(_localizer["canModifyTeam"]);
		}

        /// <summary>
        /// Checks if the user is manager of the team. Based on this result, user can modify the team or not.
        /// </summary>
        /// <param name="teamId">Id of the team</param>
		/// /// <param name="challengerAccountId">Id of the user</param>
        /// <returns></returns>
        public async Task<bool> IsUserManagerOfTeam(string teamId, string challengerAccountId)
		{
			return challengerAccountId == (await _tournamentTeamsManipulator.GetManagerOfTeam(teamId)).AccountId;
		}

		/// <summary>
		/// Deletes team from the database
		/// </summary>
		/// <param name="teamId">Id of the team</param>
		/// <returns></returns>
		public async Task DeleteTeam(string teamId)
		{
			await _tournamentTeamsManipulator.DeleteTeam(teamId);
        }
	}
}
