using DatabaseCommunicator;
using DatabaseCommunicator.Models;
using DatabaseCommunicator.ModelsManipulators;
using Microsoft.AspNetCore.Mvc;
using TournamentLibrary.Participants;
using TournamentLibrary.TournamentParts;

namespace ChessTournamentManager.Controllers
{
	/// <summary>
	/// Retreives all basic information about tournaments
	/// </summary>
	public class TournamentController : ControllerBase
	{
		private readonly TournamentTeamsManipulator _tournamentTeamsManipulator;
		private readonly TournamentManager _tournamentManager;
		private readonly TournamentManipulator _tournamentManipulator;

		public TournamentController(TournamentTeamsManipulator tournamentTeamsManipulator,
									TournamentManager tournamentManager,
									TournamentManipulator tournamentManipulator)
		{
			_tournamentTeamsManipulator = tournamentTeamsManipulator;
			_tournamentManager = tournamentManager;
			_tournamentManipulator = tournamentManipulator;
		}

		/// <summary>
		/// Retreives all tournaments from database based on values in filter
		/// </summary>
		/// <param name="filter">Requirements which tournament have to meet</param>
		/// <returns>Tournaments based on requirements</returns>
		public async Task<IEnumerable<Tournament>> GetAllTournamentsAsync(TournamentsFilter filter, int limit = 100)
		{
			return await _tournamentManipulator.GetAllTournaments(filter, limit);
		}

		/// <summary>
		/// Retreives basic information about the tournament, date, name...
		/// </summary>
		/// <param name="tournamentId">Id of tournament</param>
		/// <returns></returns>
		public async Task<Tournament?> GetTournamentInformation(string tournamentId)
		{
			return await _tournamentManipulator.GetTournamentInformation(tournamentId);
		}

		/// <summary>
		/// Returns all players which will be attending the tournament. In case of a team tournament it also returns all players.
		/// </summary>
		/// <param name="tournamentId"></param>
		/// <returns></returns>
        public async Task<IEnumerable<Player>> GetAllPlayersInTournament(string tournamentId)
        {
            List<Player> players = await _tournamentManager.GetPlayersInTournament(tournamentId);
			return players;
        }

		/// <summary>
		/// Returns all teams which are registered to tournament. Every team will be included with players who play for the team.
		/// </summary>
		/// <param name="tournamentId">Id of tournament</param>
		/// <returns></returns>
		public async Task<IEnumerable<TeamWithPlayers>> GetRegisteredTeamsWithPlayers(string tournamentId)
		{
			return await _tournamentTeamsManipulator.GetRegisteredTeamsWithPlayersAsync(tournamentId);
		}

		/// <summary>
		/// Returns all matches which have already been generated in the tournament. (Single tournament)
		/// </summary>
		/// <param name="tournamentId">Id of tournament</param>
		/// <returns></returns>
		public async Task<List<RoundPair<Player>>> GetTournamentPairs(string tournamentId)
		{
			return await _tournamentManager.GetTournamentPairs(tournamentId);
		}

		/// <summary>
		/// Returns all pairs which have already been generated. (Team tournament)
		/// </summary>
		/// <param name="tournamentId">Id of tournament</param>
		/// <returns></returns>
		public async Task<List<RoundPair<Team>>> GetAllTournamentPairsTeamsAsync(string tournamentId)
		{
			return await _tournamentManager.GetAllTournamentPairsTeams(tournamentId);
		}

		/// <summary>
		/// Returns all matches between individual players in tournament. (Team tournament)
		/// </summary>
		/// <param name="tournamentId">Id of tournament</param>
		/// <returns></returns>
		public async Task<List<RoundPair<Player>>> GetAllPlayerPairsInTeamTournamentAsync(string tournamentId)
		{
			return await _tournamentManager.GetAllPlayerPairsInTeamTournamentAsync(tournamentId);
        }

		/// <summary>
		/// Returns how many players in currently registered to tournament.
		/// </summary>
		/// <param name="tournamentId">Id of tournament</param>
		/// <returns></returns>
		public async Task<int> GetNumberOfAllPlayersInTournament(string tournamentId)
		{
			return (await _tournamentManager.GetPlayersInTournament(tournamentId)).Count;
		}

		/// <summary>
		/// Returns number of registered teams into the tournament. (Team tournament)
		/// </summary>
		/// <param name="tournamentId">Id of tournament</param>
		/// <returns></returns>
		public async Task<int> GetNumberOfRegisteredTeamsWithPlayers(string tournamentId)
		{
			return (await _tournamentTeamsManipulator.GetRegisteredTeamsWithPlayersAsync(tournamentId)).Count;
		}
	}
}
