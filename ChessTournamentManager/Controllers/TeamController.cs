
using DatabaseCommunicator.Models;
using DatabaseCommunicator.ModelsManipulators;
using Microsoft.AspNetCore.Mvc;


namespace ChessTournamentManager.Controllers
{
    /// <summary>
    /// Retreives information about teams from database
    /// </summary>
    [Route("api/teams")]
    [ApiController]
    public class TeamController : ControllerBase
    {
		private readonly TournamentManipulator _tournamentManipulator;
		private readonly TeamsWithPlayersManipulator _teamsWithPlayersManipulator;

		public TeamController(TournamentManipulator tournamentManipulator,
							  TeamsWithPlayersManipulator teamsWithPlayersManipulator)
		{
			_tournamentManipulator = tournamentManipulator;
			_teamsWithPlayersManipulator = teamsWithPlayersManipulator;
		}

        /// <summary>
        /// Returns team based on its id from database with its players
        /// </summary>
        /// <param name="teamId">Id of team</param>
        /// <returns>Team with players</returns>
		[HttpGet("{teamId}")]
        public async Task<TeamWithPlayers> GetTeamWithPlayers(string teamId)
        {
            return await _teamsWithPlayersManipulator.GetTeamWithPlayers(teamId);
        }

        /// <summary>
        /// Returns tournament where team is/was registered
        /// </summary>
        /// <param name="teamId"></param>
        /// <returns>Tournaments where team is/was registered</returns>
        public async Task<IEnumerable<Tournament>> GetTournamentsOfTeam(string teamId)
        {
            return await _tournamentManipulator.GetTournamentsOfTeam(teamId);
        }
	}
}
