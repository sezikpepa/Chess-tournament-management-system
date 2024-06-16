using ChessTournamentMate.Shared;
using DatabaseCommunicator.Models;
using DatabaseCommunicator.ModelsManipulators;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using TournamentLibrary.Participants;

namespace ChessTournamentManager.Controllers
{

	/// <summary>
	/// Provides methods to manipulate with user profile.
	/// </summary>

	[Route("api/ownProfile")]
	[ApiController]
	public class OwnProfileController : ControllerBase
	{
		private readonly ProfileManipulator _profileManipulator;

		public OwnProfileController(ProfileManipulator profileManipulator)
		{
			_profileManipulator = profileManipulator;
		}

		/// <summary>
		/// Gets information about the account from database. Returns information like name, surname, chess club.
		/// Used on player profile page.
		/// </summary>
		/// <param name="accountId"></param>
		/// <returns></returns>
		[HttpGet("{accountId}")]
		public async Task<Player?> GetAcountInformation(string accountId)
		{
			return await _profileManipulator.GetAcountInformation(accountId);
		}

		/// <summary>
		/// When user updates personal information via form, this method is called to perform update in database.
		/// </summary>
		/// <param name="player">All personal information, unchanged and changed informations are send as one object with correct data </param>
		/// <returns></returns>
		public async Task UpdatePlayerProfile(Player player)
		{
			await _profileManipulator.UpdatePlayerProfile(player);
		}

		/// <summary>
		/// Every player has its unique id. If we need to select tournaments for specific account we would like to reuse
		/// exising methods which accept this id. This method provides this id based on accountId of logged user.
		/// </summary>
		/// <param name="accountId">AccountId of logged user</param>
		/// <returns>Id of logged user</returns>

		[HttpGet("id/{accountId}")]
		public async Task<string?> GetId(string accountId)
		{
			return await _profileManipulator.GetIdOfAccount(accountId);
		}

		/// <summary>
		/// If the user is logged in, he/she has access to every page in the application. Some actions require personal
		/// informations to be filled in. This method checks if user already filled own account. Otherwise component should
		/// redirect him/her to page where it is possible to fill these informations and then continue in work.
		/// </summary>
		/// <param name="accountId">Account id of logged user</param>
		/// <returns>true if the account has all personal information filled. Otherwise false</returns>

        public async Task<bool> AccountHasFilledProfile(string accountId)
        {
			return (await _profileManipulator.GetIdOfAccount(accountId)) is not null;
        }
    }
}
