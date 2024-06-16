using ChessTournamentManager.Data;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;

namespace ChessTournamentManager.CodeStructures
{
	/// <summary>
	/// Provides information about logged user.
	/// </summary>
	public class UserInformation
	{
		private readonly AuthenticationStateProvider _authenticationStateProvider;
		private readonly UserManager<ApplicationUser> _userManager;

		public UserInformation(AuthenticationStateProvider authenticationStateProvider, UserManager<ApplicationUser> userManager)
		{
			_authenticationStateProvider = authenticationStateProvider;
			_userManager = userManager;
		}

		/// <summary>
		/// Returns Id of the logged user from account database
		/// </summary>
		/// <returns></returns>
		public async Task<string?> GetLoggedUserIdAsync()
		{
			var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();

			var userPrincipal = authState.User;

			var user = await _userManager.GetUserAsync(userPrincipal);


			return user?.Id;
		}
	}

}
