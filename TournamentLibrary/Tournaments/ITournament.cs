using DatabaseCommunicator.Models;
using TournamentLibrary.Participants;
using TournamentLibrary.TournamentParts;

namespace TournamentLibrary.TournamentHandlers
{
    /// <summary>
    /// Every tournament in library has to implement this interface, basic facts about tournaments
    /// are mentiond in properties
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ITournament<T> where T : class, IParticipant
    {
        /// <summary>
        /// Dependends on the tournament. Values can be automaticly compute by a program, for example
        /// Round Robin tournament have predetermined number of round. Swiss Tournament for example
        /// could be set by a user
        /// </summary>
		public int ExpectedTournamentRounds { get; }

        /// <summary>
        /// List of particicpants which are competing in the tournament.
        /// </summary>

        public List<T> Participants { get; set; }

        public bool IsDrawPossible { get; }

        public bool AllMatchResultsBeforeNextRound { get; }

        public bool CanChangePreviousRounds { get; }

        public List<ParticipantWithPoints<T>> GetCurrentStandings(List<RoundDraw<T>> roundDraws, ResultsSettings resultsSettings, bool calculateAuxiliaryPoints = true);

		public RoundDraw<T> GenerateRoundDraw(int roundNumber, List<RoundDraw<T>> roundDraws);
		public void AddParticipant(T tournamentPlayer);

        public void AddParticipantRange(IEnumerable<T> tournamentPlayers);
	}
}
