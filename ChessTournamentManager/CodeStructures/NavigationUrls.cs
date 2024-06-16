using ChessTournamentManager.Components.Pages;
using DatabaseCommunicator.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace ChessTournamentManager.CodeStructures
{
    /// <summary>
    /// Methods which performs redirect to specific page of the application
    /// </summary>
    public class NavigationUrls
    {
        private readonly NavigationManager _navigation;

        public NavigationUrls(NavigationManager navigation) 
        {
            _navigation = navigation;
        }

        /// <summary>
        /// Page with player account.
        /// </summary>
        /// <param name="playerId"></param>
        public void ToPlayerProfile(string playerId)
        {
            _navigation.NavigateTo($"/profiles/{playerId}");
        }

        /// <summary>
        /// Page with all tournaments.
        /// </summary>
        public void ToTournamentsPage()
        {
            _navigation.NavigateTo($"/tournaments");
        }

        /// <summary>
        /// Page with performance of a player in the tournament.
        /// </summary>
        /// <param name="tournamentId"></param>
        /// <param name="playerId"></param>
        public void ToPlayerTournamentPerformance(string tournamentId, string playerId)
        {
            _navigation.NavigateTo($"/tournaments/{tournamentId}/performance/{playerId}");
        }

        /// <summary>
        /// Page where manager of the tournament can manage this tournament.
        /// </summary>
        /// <param name="tournamentId"></param>
        public void ToTournamentManagementPage(string tournamentId)
        {
            _navigation.NavigateTo($"/manageTournament/{tournamentId}");
        }

        /// <summary>
        /// Page with information about tournaments.
        /// </summary>
        /// <param name="tournamentId"></param>
        public void ToTournamentPageForPlayer(string tournamentId)
        {
            _navigation.NavigateTo($"/tournamentInformation/{tournamentId}");
        }

        /// <summary>
        /// Page where manager of the team can manage this team.
        /// </summary>
        /// <param name="teamId"></param>
        public void ToTeamManagementPage(string teamId)
        {
            _navigation.NavigateTo($"/teamsManagement/{teamId}");
        }

        /// <summary>
        /// Page where all managed tournaments of a logged user are shown.
        /// </summary>
        public void ToTeamsManagementPage()
        {
            _navigation.NavigateTo("/teamsManagement");
        }

        /// <summary>
        /// Page where it is possible to register to the tournament.
        /// </summary>
        /// <param name="tournamentId"></param>
        public void ToTournamentRegistrationPage(string tournamentId)
        {
            _navigation.NavigateTo($"/registrationForTournament/{tournamentId}");
        }

        /// <summary>
        /// Page where user can change personal information shown on his/her profile.
        /// </summary>
        /// <param name="accountId"></param>
        public void ToEditProfilePage(string accountId)
        {
			_navigation.NavigateTo($"/editProfile/{accountId}");
		}
        
        /// <summary>
        /// Page where profile of a logged user is shown.
        /// </summary>
        public void ToMyAccount()
        {
            _navigation.NavigateTo($"/myProfile");
        }

        /// <summary>
        /// Page where it is possible to unregister from the tournament. (Personal account)
        /// </summary>
        /// <param name="tournamentId"></param>
        public void ToUnregisterFromTournament(string tournamentId)
        {
            _navigation.NavigateTo($"/tournament/{tournamentId}/unregisterSingle");
        }

        /// <summary>
        /// Page where it is possible to unregister the team from the tournament.
        /// </summary>
        /// <param name="tournamentId"></param>
        /// <param name="teamId"></param>
		public void ToUnregisterFromTournamentTeam(string tournamentId, string teamId)
		{
			_navigation.NavigateTo($"/tournament/{tournamentId}/unregisterTeam/{teamId}");
		}

        /// <summary>
        /// Page with tournaments which are somehow connected to logged user.
        /// </summary>
		public void ToMyTournaments()
        {
            _navigation.NavigateTo("/myTournaments");
        }

        /// <summary>
        /// Page where managed tournaments are shown.
        /// </summary>
        public void ToManageTournamentsPage()
        {
            _navigation.NavigateTo("/manageTournaments");
        }

        /// <summary>
        /// Page where it is possible to create a new team.
        /// </summary>
        public void ToCreateNewTeam()
        {
            _navigation.NavigateTo("/createNewTeam");
        }
	}
}