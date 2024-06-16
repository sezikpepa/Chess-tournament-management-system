using TournamentLibrary;
using TournamentLibrary.Extensions;
using TournamentLibrary.Participants;
using TournamentLibrary.TournamentParts;
using TournamentLibrary.Tournaments;

namespace Tests
{
    public class AuxiliaryPointsExtensionsTests
    {

        public static IEnumerable<object[]> PlayersForTesting = new[]
        {
            new object[] { MockMethods.GetFirstPlayersForTournament(4) },
        };


        private static IEnumerable<RoundDraw<TournamentPlayer>> GetRoundDraws(List<TournamentPlayer> players)
        {
			RoundDraw<TournamentPlayer> roundDraw1 = new([new RoundPair<TournamentPlayer>(players[0], players[1], new SingleMatchResult(1, 0)),
				                                          new RoundPair<TournamentPlayer>(players[2], players[3], new SingleMatchResult(0.5m, 0.5m))]);

			RoundDraw<TournamentPlayer> roundDraw2 = new([new RoundPair<TournamentPlayer>(players[0], players[2], new SingleMatchResult(1, 0)),
				                                          new RoundPair<TournamentPlayer>(players[1], players[3], new SingleMatchResult(1, 0))]);

			return [roundDraw1, roundDraw2];
		}

        [Theory]
        [MemberData(nameof(PlayersForTesting))]
        public void CalculateBlackPiecesPlayedTest(List<TournamentPlayer> players)
        {
			List<ParticipantWithPoints<TournamentPlayer>> playersWithPoints = players.ToPlayersWithPoints().ToList();

            playersWithPoints.CalculateBlackPiecesPlayed(GetRoundDraws(players));

            Assert.Equal(playersWithPoints[0].BlackPiecesPlayed, 0);
            Assert.Equal(playersWithPoints[1].BlackPiecesPlayed, 1);
            Assert.Equal(playersWithPoints[2].BlackPiecesPlayed, 1);
            Assert.Equal(playersWithPoints[3].BlackPiecesPlayed, 2);
        }

		[Theory]
		[MemberData(nameof(PlayersForTesting))]
		public void CalculateBuchholzTest(List<TournamentPlayer> players)
		{
			var roundDraws = GetRoundDraws(players);

			List<ParticipantWithPoints<TournamentPlayer>> playersWithPoints = players.CalculatePlayerPoints(roundDraws).ToList();

			playersWithPoints.CalculateBuchholz(roundDraws);

			Assert.Equal(playersWithPoints[0].Buchholz, 1.5m);
			Assert.Equal(playersWithPoints[1].Buchholz, 2.5m);
			Assert.Equal(playersWithPoints[2].Buchholz, 2.5m);
			Assert.Equal(playersWithPoints[3].Buchholz, 1.5m);
		}

		[Theory]
		[MemberData(nameof(PlayersForTesting))]
		public void CalculateSonnenBornBergerTest(List<TournamentPlayer> players)
		{
			var roundDraws = GetRoundDraws(players);

			List<ParticipantWithPoints<TournamentPlayer>> playersWithPoints = players.CalculatePlayerPoints(roundDraws).ToList();

			playersWithPoints.CalculateSonnenBornBerger(roundDraws);

			Assert.Equal(playersWithPoints[0].SonnenbornBerger, 1.5m);
			Assert.Equal(playersWithPoints[1].SonnenbornBerger, 0.5m);
			Assert.Equal(playersWithPoints[2].SonnenbornBerger, 0.25m);
			Assert.Equal(playersWithPoints[3].SonnenbornBerger, 0.25m);
		}
	}
}
