using DatabaseCommunicator.Models;
using DatabaseCommunicator.ModelsManipulators.SettingsManipulators;
using DatabaseCommunicator.ModelsManipulators;
using DatabaseCommunicator;
using ChessTournamentMate.Shared;
using TournamentLibrary.TournamentHandlers;
using TournamentLibrary.TournamentParts;
using ChessTournamentMate.Shared.AvailableValues;

namespace ChessTournamentManager.Controllers
{
    public class RoundDrawGenerator
	{
		private readonly TournamentManipulator _tournamentManipulator;
		private readonly TeamsWithPlayersManipulator _teamsWithPlayersManipulator;
		private readonly TournamentTeamsManipulator _tournamentTeamsManipulator;
		private readonly TournamentManager _tournamentManager;
		private readonly TournamentTeamDrawSettingsManipulator _tournamentTeamDrawSettingsManipulator;

		public RoundDrawGenerator(TournamentManipulator tournamentManipulator,
								  TeamsWithPlayersManipulator teamsWithPlayersManipulator,
								  TournamentTeamsManipulator tournamentTeamsManipulator,
								  TournamentManager tournamentManager,
								  TournamentTeamDrawSettingsManipulator tournamentTeamDrawSettingsManipulator)
		{
			_tournamentManipulator = tournamentManipulator;
			_teamsWithPlayersManipulator = teamsWithPlayersManipulator;
			_tournamentTeamsManipulator = tournamentTeamsManipulator;
			_tournamentManager = tournamentManager;
			_tournamentTeamDrawSettingsManipulator = tournamentTeamDrawSettingsManipulator;
		}

		/// <summary>
		/// Generates matches between teams in team tournament and also matches between inidividual players.
		/// </summary>
		/// <param name="tournamentId">Id of tournament</param>
		/// <param name="roundNumber">Round number</param>
		/// <returns></returns>
		public async Task GenerateRoundDrawTeamAsync(string tournamentId, int roundNumber)
		{
			Tournament tournamentInformation = await _tournamentManipulator.GetTournamentInformation(tournamentId);

			ITournament<Team> tournament;

			TournamentHandlerFactory<Team> handlerFactory = new();
			tournament = handlerFactory.CreateTournamentHandler(tournamentInformation.TournamentType);

			List<Team> teamsInTournament = await _tournamentTeamsManipulator.GetRegisteredTeamsAsync(tournamentId);

			tournament.AddParticipantRange(teamsInTournament);

			List<RoundDraw<Team>> roundDraws = [];

			for (int i = 1; i < roundNumber; i++)
			{
				roundDraws.Add(await _tournamentManager.GetRoundDrawTeamsAsync(tournamentId, i));
			}


			RoundDraw<Team> toSend = tournament.GenerateRoundDraw(roundNumber, roundDraws);
			var settings = await _tournamentTeamDrawSettingsManipulator.GetTournamentTeamDrawSettingsAsync(tournamentId);

			Dictionary<RoundPair<Team>, List<RoundPair<Player>>> subMatches = [];
			foreach (var teamPair in toSend.GetRoundPairs())
			{
				if(teamPair.White is null && teamPair.Black is null)
				{
                    teamPair.Result = new SingleMatchResult(0, 0);
                }
                else if(teamPair.White is null)
				{
					teamPair.Result = new SingleMatchResult(0, settings.TeamSizeForRound);
				}
				else if(teamPair.Black is null)
				{
					teamPair.Result = new SingleMatchResult(settings.TeamSizeForRound, 0);
				}

				subMatches[teamPair] = await GenerateSubMatches(teamPair, settings.TeamSizeForRound);
			}



			await _tournamentManager.SaveTournamentRoundDrawTeams(toSend, tournamentId, roundNumber);

			await _tournamentManager.SaveSubMatches(subMatches, tournamentId, roundNumber);
		}

		/// <summary>
		/// Generates matches between players in single tournament
		/// </summary>
		/// <param name="tournamentId">Id of tournament</param>
		/// <param name="roundNumber">Round number</param>
		/// <returns></returns>
		public async Task GenerateRoundDrawSingleAsync(string tournamentId, int roundNumber)
		{
			Tournament tournamentInformation = await _tournamentManipulator.GetTournamentInformation(tournamentId);

			ITournament<Player> tournament;

			TournamentHandlerFactory<Player> handlerFactory = new();
			tournament = handlerFactory.CreateTournamentHandler(tournamentInformation.TournamentType);

			List<Player> playersInTournament = await _tournamentManager.GetPlayersInTournament(tournamentId);

			tournament.AddParticipantRange(playersInTournament);

			List<RoundDraw<Player>> roundDraws = [];

			for (int i = 1; i < roundNumber; i++)
			{
				roundDraws.Add(await _tournamentManager.GetRoundDrawAsync(tournamentId, i));
			}

			RoundDraw<Player> toSend = tournament.GenerateRoundDraw(roundNumber, roundDraws);


			await _tournamentManager.SaveTournamentRoundDraw(toSend, tournamentId, roundNumber);
		}

		/// <summary>
		/// Eventhough in team tournaments matches are playerd between teams, every round contains matches between individual players from this teams.
		/// This method generated these matches between players.
		/// </summary>
		/// <param name="teamPair">Pair of team for matches generation of single players</param>
		/// <param name="size">How many players of the team should be in matches</param>
		/// <returns></returns>
		private async Task<List<RoundPair<Player>>> GenerateSubMatches(RoundPair<Team> teamPair, int size)
		{
			List<RoundPair<Player>> subMatches = [];


			List<Player> whitePlayers = [];
			List<Player> blackPlayers = [];


            if (teamPair.White is not null)
			{
                whitePlayers = await _teamsWithPlayersManipulator.GetPlayersInTeamAsync(teamPair.White.Id);
            }
            if (teamPair.Black is not null)
			{
                blackPlayers = await _teamsWithPlayersManipulator.GetPlayersInTeamAsync(teamPair.Black.Id);
            }


			for (int i = 0; i < size; i++)
			{
				Player whiteSub = null;
				Player blackSub = null;

				if(i < whitePlayers.Count)
				{
                    whiteSub = whitePlayers[i];
                }
                if (i < blackPlayers.Count)
                {
                    blackSub = blackPlayers[i];
                }

                var subMatch = new RoundPair<Player>(whiteSub, blackSub);
				if(whiteSub == null && blackSub == null)
				{
					subMatch.Result = new SingleMatchResult(0, 0);
				}
				else if(whiteSub == null)
				{
					subMatch.Result = new SingleMatchResult(0, 1);
				}
                else if (blackSub == null)
                {
                    subMatch.Result = new SingleMatchResult(1, 0);
                }
				else
				{
					subMatch.Result = new SingleMatchResult("Unknown");
				}
                subMatches.Add(subMatch);
			}

			return subMatches;
		}
	}
}
