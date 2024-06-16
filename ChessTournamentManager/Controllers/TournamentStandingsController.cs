using DatabaseCommunicator.Models;
using DatabaseCommunicator.ModelsManipulators;
using DatabaseCommunicator;
using ChessTournamentMate.Shared;
using TournamentLibrary.TournamentHandlers;
using TournamentLibrary.TournamentParts;
using TournamentLibrary;
using DatabaseCommunicator.ModelsManipulators.SettingsManipulators;

namespace ChessTournamentManager.Controllers
{
	public class TournamentStandingsController
	{
		private readonly TournamentManager _tournamentManager;
		private readonly TournamentManipulator _tournamentManipulator;
		private readonly TournamentTeamsManipulator _tournamentTeamsManipulator;
		private readonly TournamentResultsSettingsManipulator _tournamentResultsSettingsManipulator;

		public TournamentStandingsController(TournamentManager tournamentManager,
											 TournamentManipulator tournamentManipulator,
											 TournamentTeamsManipulator tournamentTeamsManipulator,
											 TournamentResultsSettingsManipulator tournamentResultsSettingsManipulator)
		{
			_tournamentManager = tournamentManager;
		    _tournamentManipulator = tournamentManipulator;
			_tournamentTeamsManipulator = tournamentTeamsManipulator;
			_tournamentResultsSettingsManipulator = tournamentResultsSettingsManipulator;
		}

		/// <summary>
		/// Returns current standings in the tournament. (Single tournament)
		/// </summary>
		/// <param name="tournamentId"></param>
		/// <param name="untilRoundNumber"></param>
		/// <returns></returns>
		public async Task<List<ParticipantWithPoints<Player>>> GetCurrentStandings(string tournamentId, int untilRoundNumber)
		{
			List<RoundDraw<Player>> roundDraws = [];

			ResultsSettings resultsSettings = await _tournamentResultsSettingsManipulator.GetTournamentResultSettingsAsync(tournamentId);

			for (int i = 1; i < untilRoundNumber + 1; i++)
			{
				RoundDraw<Player> round = await _tournamentManager.GetRoundDrawAsync(tournamentId, i);
				roundDraws.Add(round);
			}

			Tournament tournament = await _tournamentManipulator.GetTournamentInformation(tournamentId);

			ITournament<Player> tournamentHandler = new TournamentHandlerFactory<Player>().CreateTournamentHandler(tournament.TournamentType);

			var resultParticipants = await _tournamentManager.GetPlayersInTournament(tournamentId);

			tournamentHandler.AddParticipantRange(resultParticipants);

			List<ParticipantWithPoints<Player>> result = tournamentHandler.GetCurrentStandings(roundDraws, resultsSettings);

			return result;
		}

		/// <summary>
		/// Returns current standings in the tournament. (Team tournament)
		/// </summary>
		/// <param name="tournamentId"></param>
		/// <param name="untilRoundNumber"></param>
		/// <returns></returns>
		public async Task<List<ParticipantWithPoints<Team>>> GetCurrentStandingsTeam(string tournamentId, int untilRoundNumber)
		{
			List<RoundDraw<Team>> roundDraws = [];

			ResultsSettings resultsSettings = await _tournamentResultsSettingsManipulator.GetTournamentResultSettingsAsync(tournamentId);


			for (int i = 1; i < untilRoundNumber + 1; i++)
			{
				RoundDraw<Team> round = await _tournamentManager.GetRoundDrawTeamsAsync(tournamentId, i);
				roundDraws.Add(round);
			}

			Tournament tournament = await _tournamentManipulator.GetTournamentInformation(tournamentId);

			ITournament<Team> tournamentHandler = new TournamentHandlerFactory<Team>().CreateTournamentHandler(tournament.TournamentType);

			List<Team> resultParticipants = await _tournamentTeamsManipulator.GetRegisteredTeamsAsync(tournamentId);

			tournamentHandler.AddParticipantRange(resultParticipants);

			List<ParticipantWithPoints<Team>> result = tournamentHandler.GetCurrentStandings(roundDraws, resultsSettings);

			return result;
		}


	}
}
