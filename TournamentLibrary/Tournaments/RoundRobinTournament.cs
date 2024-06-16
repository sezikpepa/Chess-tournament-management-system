using TournamentLibrary.Participants;
using TournamentLibrary.RoundHandlers;
using TournamentLibrary.TournamentParts;
using TournamentLibrary.Tournaments;

namespace TournamentLibrary.TournamentHandlers
{
	public class RoundRobinTournament<T> : TournamentHandler<T>, ITournament<T> where T : class, IParticipant
    {
        public int ExpectedTournamentRounds => (((Participants.Count + 1) / 2) * 2) - 1;

        public bool IsDrawPossible => true;

        public bool AllMatchResultsBeforeNextRound => false;

        public bool CanChangePreviousRounds => true;

        public RoundRobinTournament(List<T> participants) : base(participants) { }

		public RoundRobinTournament() { }


		public RoundDraw<T> GenerateRoundDraw(int roundNumber, List<RoundDraw<T>> roundDraws)
		{
            RoundRobinRoundHandler<T> roundHandler = new(Participants);

            for(int i = 1; i < roundNumber; i++)
            {
                roundHandler.MoveToNextRound();
            }

            roundHandler.GenerateCurrentRound();
            return roundHandler.CurrentRoundDraw;
		}

	}
}
