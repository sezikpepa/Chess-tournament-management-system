using DatabaseCommunicator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using TournamentLibrary.Extensions;
using TournamentLibrary.Participants;
using TournamentLibrary.TournamentHandlers;
using TournamentLibrary.TournamentParts;

namespace TournamentLibrary.Tournaments
{

	public class PlayoffTournament<T> : TournamentHandler<T>, ITournament<T> where T : class, IParticipant
    {
		public int ExpectedTournamentRounds => (int)(Math.Ceiling(Math.Log2(Participants.Count)));

		public bool IsDrawPossible => false;

		public bool AllMatchResultsBeforeNextRound => true;

        public bool CanChangePreviousRounds => false;

        public PlayoffTournament(List<T> participants) : base(participants) { }

		public PlayoffTournament() { }

		private int GetNumberOfPlayersForPairs(int particpantsCount)
		{
			int result = 1;

			while (result * 2 <= particpantsCount)
			{
				result *= 2;
			}

			if(result != particpantsCount)
			{
				return (particpantsCount - result) * 2;
			}

			return result;
		}

		public RoundDraw<T> GenerateRoundDraw(int roundNumber, List<RoundDraw<T>> roundDraws)
		{
			RoundDraw<T> generatedRoundDraw = new();


			List<T> participants = Participants;

			foreach(var roundDraw in  roundDraws)
			{
				foreach(RoundPair<T> pair in roundDraw.GetRoundPairs())
				{
					var loser = pair.GetLoser();

					participants.RemoveAll(x => x.Id == loser?.Id);
				}
			}

			int numberOfPlayersForPairs= GetNumberOfPlayersForPairs(participants.Count);


			for (int i = 0; i < numberOfPlayersForPairs; i += 2)
			{
                RoundPair<T> pair = new(participants[1], participants[0], new SingleMatchResult(0, 0, true));
				participants.RemoveAll(x => x.Id == participants[0].Id || x.Id == participants[1].Id);
				generatedRoundDraw.AddPair(pair);
			}

			foreach(var participant in participants)
			{
				generatedRoundDraw.AddPair(new RoundPair<T>(participant, null, new SingleMatchResult(1, 0)));
			}

			return generatedRoundDraw;

		}
	}
}
