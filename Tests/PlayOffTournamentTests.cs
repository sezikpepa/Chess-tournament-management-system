using DatabaseCommunicator.Models;
using TournamentLibrary;
using TournamentLibrary.Extensions;
using TournamentLibrary.Participants;
using TournamentLibrary.TournamentParts;
using TournamentLibrary.Tournaments;

namespace Tests
{
    public class PlayOffTournamentTests
    {
        [Fact]
        public void TestExpectedNumberOfRounds()
        {
			List<TournamentPlayer> players10 = MockMethods.GetFirstPlayersForTournament(10);
			List<TournamentPlayer> players32 = MockMethods.GetFirstPlayersForTournament(32);

			PlayoffTournament<TournamentPlayer> handler10 = new(players10);
			PlayoffTournament<TournamentPlayer> handler32 = new(players32);

			int expectedNumberOfRounds10 = handler10.ExpectedTournamentRounds;
			int expectedNumberOfRounds32 = handler32.ExpectedTournamentRounds;


			Assert.Equal(4, expectedNumberOfRounds10);
			Assert.Equal(5, expectedNumberOfRounds32);
		}


		[Fact]
		public void TestOnlyWinnersAreSelectedForNextRound()
		{
			Player player1 = new Player() { Id = "1" };
            Player player2 = new Player() { Id = "2" };
            Player player3 = new Player() { Id = "3" };

			RoundPair<Player> roundPair1 = new RoundPair<Player>(player1, player2, new SingleMatchResult(0, 1));
			RoundPair<Player> roundPair2 = new RoundPair<Player>() { White = player3, Result = new SingleMatchResult(1, 0) };

			RoundDraw<Player> roundDraw = new RoundDraw<Player>([roundPair1, roundPair2]);

			PlayoffTournament<Player> tournament = new PlayoffTournament<Player>([player1, player2, player3]);

            RoundDraw<Player> secondRound = tournament.GenerateRoundDraw(2, [roundDraw]);

            List<RoundPair<Player>> pairs = secondRound.GetRoundPairs();
            List<Player> playersInPairs = pairs.GetAllParticipants();

			Assert.Single(pairs);
			Assert.Equal(2, playersInPairs.Count);
			Assert.Contains(player2, playersInPairs);
            Assert.Contains(player3, playersInPairs);
        }
    }
}
