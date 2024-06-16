using ChessTournamentManager.DataServices.Filters;
using DatabaseCommunicator.Models;
using DatabaseCommunicator.ModelsManipulators;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ChessTournamentManager.Controllers
{
	[Route("api/player")]
	[ApiController]
	public class PlayerController : ControllerBase
	{
		private readonly ProfileManipulator _profileManipulator;

		public PlayerController(ProfileManipulator profileManipulator)
		{
			_profileManipulator = profileManipulator;
		}

		/// <summary>
		/// Returns information about the player from the database - personal information
		/// </summary>
		/// <param name="playerId">Id of the player</param>
		/// <returns></returns>
		[HttpGet("{playerId}")]
		public async Task<Player?> GetProfileInformation(string playerId)
		{
			return await _profileManipulator.GetPlayerInformation(playerId);
		}

		/// <summary>
		/// Returns all tournaments where the player participated/will participate
		/// </summary>
		/// <param name="playerId"></param>
		/// <returns></returns>
		[HttpGet("{playerId}/tournaments")]
		public async Task<IEnumerable<Tournament>> GetPlayersTournaments(string playerId)
		{
			return await _profileManipulator.GetPlayersTournament(playerId);
		}

		/// <summary>
		/// Returns all players from the database, which satisfy the criteria
		/// </summary>
		/// <param name="filter">Criteria for player search</param>
		/// <param name="skipCount">How many players from the begginning should not be returned</param>
		/// <param name="takeCount">How many players should be returned</param>
		/// <returns></returns>
		[HttpPost("all")]
		public async Task<IEnumerable<Player>> GetAllPlayersAsync(PlayerServiceFilter filter, long skipCount = 0, int takeCount = 20)
		{
			return await _profileManipulator.GetAllPlayersAsync(filter, skipCount, takeCount);
		}

		/// <summary>
		/// Returns information, how many players are in the database satisfying the criteria
		/// </summary>
		/// <param name="filter">Criteria for player search</param>
		/// <returns></returns>
		public async Task<long> GetTotalNumberOfPlayers(PlayerServiceFilter filter)
		{
			return await _profileManipulator.GetTotalNumberOfPlayers(filter);
		}

		/// <summary>
		/// Returns if the player on the account marked the tournament as favourite
		/// </summary>
		/// <param name="accountId">Id of the account</param>
		/// <param name="tournamentId">Id of the tournament</param>
		/// <returns></returns>
		public async Task<bool> GetTournamentFavouritnessForAccount(string accountId, string tournamentId)
		{
			string? playerId = (await _profileManipulator.GetIdOfAccount(accountId));

			return await _profileManipulator.GetTournamentFavouritness(playerId, tournamentId);

        }

		/// <summary>
		/// Marks tournament as favourite for specific user
		/// </summary>
		/// <param name="accountId">Id of the user</param>
		/// <param name="tournamentId">Id of the tournament</param>
		/// <returns></returns>
		public async Task MarkTournamentAsFavourite(string accountId, string tournamentId)
		{
			string? playerId = (await _profileManipulator.GetIdOfAccount(accountId));

			await _profileManipulator.MarkTournamentAsFavourite(playerId, tournamentId);
        }

		/// <summary>
		/// Unmarks tournament as favourite for specific user
		/// </summary>
		/// <param name="accountId">Id of the user</param>
		/// <param name="tournamentId">Id of the tournament</param>
		/// <returns></returns>
		public async Task UnMarkTournamentAsFavourite(string accountId, string tournamentId)
        {
			string? playerId = (await _profileManipulator.GetIdOfAccount(accountId));

            await _profileManipulator.UnMarkTournamentAsFavourite(playerId, tournamentId);
        }

		/// <summary>
		/// Returns all tournaments, which were marked as favourite by the user
		/// </summary>
		/// <param name="accountId">Id of the user</param>
		/// <returns></returns>
		public async Task<IEnumerable<Tournament>> GetFavouriteTournaments(string accountId)
		{
			string? playerId = (await _profileManipulator.GetIdOfAccount(accountId));

			return await _profileManipulator.GetFavouriteTournamets(playerId);
        }

    }
}
