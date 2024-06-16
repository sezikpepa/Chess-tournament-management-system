using DatabaseCommunicator;
using DatabaseCommunicator.Models;
using DatabaseCommunicator.ModelsManipulators;
using TournamentLibrary;
using TournamentLibrary.TournamentParts;
using TournamentLibrary.Extensions;


namespace ChessTournamentManager.Controllers
{
    /// <summary>
    /// It handles actions performed during tournament round generating - generates new round, returns already generated and other...
    /// </summary>
    public class RoundDrawController
    {
        private readonly TournamentManipulator _tournamentManipulator;
        private readonly TournamentTeamsManipulator _tournamentTeamsManipulator;
		private readonly TournamentManager _tournamentManager;

		public RoundDrawController(TournamentManipulator tournamentManipulator,
                                   TournamentTeamsManipulator tournamentTeamsManipulator,
								   TournamentManager tournamentManager)
        {
            _tournamentManipulator = tournamentManipulator;
            _tournamentTeamsManipulator = tournamentTeamsManipulator;
			_tournamentManager = tournamentManager;
		}
        /// <summary>
        /// Retreives round draw of round based on its number in the tournament (Team tournament)
        /// </summary>
        /// <param name="tournamentId">Tournament id for which we want round draw</param>
        /// <param name="roundNumber">Round number from the beginning of the tournament</param>
        /// <returns>Matches between teams and corresponding matches between players or null if the round draw is not generated</returns>
        public async Task<(RoundDraw<Team>, List<Tuple<RoundPair<Team>, List<RoundPair<Player>>>>)?> GetRoundDrawTeam(string tournamentId, int roundNumber)
        {
            if (await _tournamentManager.RoundDrawExistsAsync(tournamentId, roundNumber))
            {
                RoundDraw<Team> roundDraw = await _tournamentManager.GetRoundDrawTeamsAsync(tournamentId, roundNumber);

                List<RoundDraw<Team>> previousRoundDraws = [];
                for (int i = 1; i < roundNumber; i++)
                {
                    previousRoundDraws.Add(await _tournamentManager.GetRoundDrawTeamsAsync(tournamentId, i));
                }

                List<Tuple<RoundPair<Team>, List<RoundPair<Player>>>> subPairs = await _tournamentManager.GetSubPairs(tournamentId, roundNumber, roundDraw);

                return (roundDraw, subPairs);
            }
            else
            {
                return null;
            }

        }

        /// <summary>
        /// Eventhought in team tournament teams play against each other, matches are still held between players. If we would want to see points of
        /// individual players during tournaments, this method will calculate score from matches in the tournament.
        /// </summary>
        /// <param name="tournamentId">Id of tournament for which we want results</param>
        /// <param name="untilRound">Until which round we want to calculate points. Rounds with numbers larger than this number are ignored</param>
        /// <returns></returns>
        public async Task<IEnumerable<ParticipantWithPoints<Player>>> GetPlayersWithPointsTeamTournament(string tournamentId, int untilRound)
        {
            IEnumerable<Player> registeredPlayers = await _tournamentManager.GetPlayersInTournament(tournamentId);

            List<RoundPair<Player>> subPairs = [];

            for (int i = 1; i < untilRound; i++)
            {
                RoundDraw<Team> roundDraw = await _tournamentManager.GetRoundDrawTeamsAsync(tournamentId, i);

                List<Tuple<RoundPair<Team>, List<RoundPair<Player>>>> tempSubPairs = await _tournamentManager.GetSubPairs(tournamentId, i, roundDraw);

                subPairs.AddRange(tempSubPairs.SelectMany(x => x.Item2));
            }


            return registeredPlayers.CalculatePlayerPoints(subPairs);
        }

        /// <summary>
        /// Retreives round draw of round based on its number in the tournament (Team tournament)
        /// </summary>
        /// <param name="tournamentId">Id of tournament</param>
        /// <param name="roundNumber">Number of the round from the beginning of the tournament</param>
        /// <returns>RoundDraw or null if the round draw is not generated</returns>
        public async Task<RoundDraw<Player>?> GetRoundDrawSingle(string tournamentId, int roundNumber)
        {
            if (await _tournamentManager.RoundDrawExistsAsync(tournamentId, roundNumber))
            {
                return await _tournamentManager.GetRoundDrawAsync(tournamentId, roundNumber);
            }

            else
            {
                return null;
            }
        }

        /// <summary>
        /// Returns all players with points they obtained in the tournament in matches (Single tournament)
        /// </summary>
        /// <param name="tournamentId">Id of tournament</param>
        /// <param name="untilRound">Until which round points should be count</param>
        /// <returns>Players with points</returns>
        public async Task<List<ParticipantWithPoints<Player>>> GetPlayersWithPoints(string tournamentId, int untilRound)
        {
            IEnumerable<Player> registeredPlayers = await _tournamentManager.GetTournamentParticipants(tournamentId);
            var registeredTournamentPlayers = registeredPlayers;

            var previousRoundDraws = new List<RoundDraw<Player>>();
            for (int i = 1; i < untilRound; i++)
            {
                previousRoundDraws.Add(await _tournamentManager.GetRoundDrawAsync(tournamentId, i));
            }


            return registeredTournamentPlayers.CalculatePlayerPoints(previousRoundDraws).ToList();
        }

        /// <summary>
        /// If the tournament manager saves results for matches, new results are supposed to be saved in the database. It is saved by this method. (Single tournaments)
        /// </summary>
        /// <param name="tournamentId">Id of tournament</param>
        /// <param name="roundNumber">Number of round</param>
        /// <param name="results">New results</param>
        /// <returns></returns>
		public async Task SendMatchResults(string tournamentId, int roundNumber, List<RoundPair<Player>> results)
		{
			await _tournamentManager.SaveMatchResults(tournamentId, roundNumber, results);
		}

        /// <summary>
        /// If the tournament manager saves results for matches, new results are supposed to be saved in the database. It is saved by this method.
        /// This method is for team tournaments, it saves current results between teams and also saves results of individual player matches.
        /// </summary>
        /// <param name="tournamentId">Id of tournament</param>
        /// <param name="roundNumber">Number of round</param>
        /// <param name="results">New results</param>
        /// <returns></returns>
		public async Task SavePairSubMatches(string tournamentId, int roundNumber, RoundPair<Team> teamPair, List<RoundPair<Player>> subPairs)
		{
			await _tournamentManager.SaveTeamResult(tournamentId, roundNumber, teamPair);
			await _tournamentManager.SaveSubMatchResultsAsync(tournamentId, roundNumber, teamPair, subPairs);
		}

        /// <summary>
        /// In some tournaments it is possible to generate new round only if all matches have been finished and results have been saved.
        /// This method checks if all matches have already been saved with result. (Single tournament)
        /// </summary>
        /// <param name="tournamentId">Id of tournament</param>
        /// <param name="roundNumber">Round number</param>
        /// <returns>true if all matches have result, otherwise false</returns>
        public async Task<bool> DoAllMatchesHaveResult(string tournamentId, int roundNumber)
        {
            RoundDraw<Player> roundDraw = await _tournamentManager.GetRoundDrawAsync(tournamentId, roundNumber);

            var pairs = roundDraw.GetRoundPairs();


            return pairs.AreAllMatchesSet();
        }

        /// <summary>
        /// In some tournaments it is possible to generate new round only if all matches have been finished and results have been saved.
        /// This method checks if all matches have already been saved with result. (Team tournament)
        /// </summary>
        /// <param name="tournamentId">Id of tournament</param>
        /// <param name="roundNumber">Round number</param>
        /// <returns>true if all matches have result, otherwise false</returns>
        public async Task<bool> DoAllMatchesHaveResultTeam(string tournamentId, int roundNumber, bool drawAcceptable)
        {
            RoundDraw<Team> roundDraw = await _tournamentManager.GetRoundDrawTeamsAsync(tournamentId, roundNumber);

            List<Tuple<RoundPair<Team>, List<RoundPair<Player>>>> subPairs = await _tournamentManager.GetSubPairs(tournamentId, roundNumber, roundDraw);

            IEnumerable<RoundPair<Player>> allSubPairsInList = subPairs.SelectMany(x => x.Item2);

            foreach(RoundPair<Team> pair in roundDraw.GetRoundPairs())
            {
                if (!drawAcceptable)
                {
					if (pair.IsDraw && pair.TieBreakWinnerWhite is null)
					{
						return false;
					}
				}
            }

            return allSubPairsInList.AreAllMatchesSet();
        }


    }
}
