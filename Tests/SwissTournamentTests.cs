using TournamentLibrary.Participants;
using TournamentLibrary.TournamentParts;
using TournamentLibrary.Tournaments;

namespace Tests
{
    public class SwissTournamentTests
    {

        public static IEnumerable<object[]> PlayersForTesting =
		[
            //small even
            [MockMethods.GetFirstPlayersForTournament(4)],
        ];

        [Theory]
        [MemberData(nameof(PlayersForTesting))]
        public void SwissTournamentCorrectFirstRoundEven(List<TournamentPlayer> players)
        {
            List<RoundPair<TournamentPlayer>> referencePairs = [new RoundPair<TournamentPlayer>(players[1], players[3]),
                                                                new RoundPair<TournamentPlayer>(players[0], players[2])];

            SwissTournament<TournamentPlayer> tournament = new(players);

            RoundDraw<TournamentPlayer> result = tournament.GenerateRoundDraw(1, []);

            List<RoundPair<TournamentPlayer>> resultPairs = result.GetRoundPairs();

            ///////////////////////////////////////////////////////////////////////////
            ///

            Assert.Equal(referencePairs.Count, resultPairs.Count);

            int i = 0;
            foreach(var referencePair in referencePairs)
            {
                Assert.Equal(referencePair.White.Id, resultPairs[i].White.Id);
                Assert.Equal(referencePair.Black.Id, resultPairs[i].Black.Id);

                i++;
            }
        }


        [Theory]
        [MemberData(nameof(PlayersForTesting))]
        public void SwissTournamentCorrectSecondRoundEven(List<TournamentPlayer> players)
        {
            List<RoundPair<TournamentPlayer>> referencePairs = [new RoundPair<TournamentPlayer>(players[2], players[3]),
                new RoundPair<TournamentPlayer>(players[0], players[1])];

            RoundDraw<TournamentPlayer> firstRoundDraw = new();


            var firstPair = new RoundPair<TournamentPlayer>(players[0], players[2]);
            firstPair.Result = PossibleSingleMatchResults.WhiteWins;
            var secondPair = new RoundPair<TournamentPlayer>(players[1], players[3]);
            secondPair.Result = PossibleSingleMatchResults.WhiteWins;


            firstRoundDraw.AddPair(firstPair);
            firstRoundDraw.AddPair(secondPair);

            SwissTournament<TournamentPlayer> tournament = new(players);

            RoundDraw<TournamentPlayer> result = tournament.GenerateRoundDraw(2, [firstRoundDraw]);

            List<RoundPair<TournamentPlayer>> resultPairs = result.GetRoundPairs();

            ///////////////////////////////////////////////////////////////////////////
            ///

            Assert.Equal(referencePairs.Count, resultPairs.Count);

            int i = 0;
            foreach (var referencePair in referencePairs)
            {
                Assert.Equal(referencePair.White.Id, resultPairs[i].White.Id);
                Assert.Equal(referencePair.Black.Id, resultPairs[i].Black.Id);

                i++;
            }
        }





    }
}
