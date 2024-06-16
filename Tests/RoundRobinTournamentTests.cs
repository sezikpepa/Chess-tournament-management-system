using System;
using TournamentLibrary.Participants;
using TournamentLibrary.TournamentHandlers;
using TournamentLibrary.TournamentParts;

namespace Tests
{

	public class RoundRobinTournamentTests
    {

        public static IEnumerable<object[]> PlayersForTestingInvalidNumbers = new[]
        {
            new object[] { MockMethods.GeneratePlayersForTournament(0) },
            new object[] { MockMethods.GeneratePlayersForTournament(1) },
            new object[] { MockMethods.GeneratePlayersForTournament(2) },
            new object[] { MockMethods.GeneratePlayersForTournament(3) }
        };


        public static IEnumerable<object[]> PlayersForTesting = new[]
        {
            //small even
            new object[] { MockMethods.GeneratePlayersForTournament(4) },

            //small odd
            new object[] { MockMethods.GeneratePlayersForTournament(5) },


            //large even
            new object[] { MockMethods.GeneratePlayersForTournament(100) },

            //large odd
            new object[] { MockMethods.GeneratePlayersForTournament(101) },
        };

        [Fact]
		public void RoundRobinTournamentCorrectlyCalculatesNumberOfRounds()
		{
            var playersEven = MockMethods.GeneratePlayersForTournament(100);
			var playersOdd = MockMethods.GeneratePlayersForTournament(51);

			RoundRobinTournament<TournamentPlayer> tournamentEven = new RoundRobinTournament<TournamentPlayer>(playersEven);
			RoundRobinTournament<TournamentPlayer> tournamentOdd = new RoundRobinTournament<TournamentPlayer>(playersOdd);


			int expectedNumberOfRoundsEven = tournamentEven.ExpectedTournamentRounds;
            int expectedNumberOfRoundOdd = tournamentOdd.ExpectedTournamentRounds;


            Assert.Equal(99, expectedNumberOfRoundsEven);
			Assert.Equal(51, expectedNumberOfRoundOdd);

		}




		[Theory]
        [MemberData(nameof(PlayersForTestingInvalidNumbers))]
        public void RoundRobinTournamentDoesNotStartWithLowNumberOfPlayers(List<TournamentPlayer> players)
        {
            RoundRobinTournament<TournamentPlayer> tournament = new RoundRobinTournament<TournamentPlayer>();

            foreach (var player in players)
            {
                tournament.AddParticipant(player);
            }
        }


        [Theory]
        [MemberData(nameof(PlayersForTesting))]
        public void CorrectNumberOfMatchesInRound(List<TournamentPlayer> players)
        {
            RoundRobinTournament<TournamentPlayer> tournament = new RoundRobinTournament<TournamentPlayer>();
          
            foreach(var player in players)
            {
                tournament.AddParticipant(player);
            }


            for (int i = 1; i < tournament.ExpectedTournamentRounds + 1; i++)
            {


                RoundDraw<TournamentPlayer> roundDraw = tournament.GenerateRoundDraw(i, null);


				Assert.Equal(roundDraw.GetRoundPairs().Count, players.Count / 2);
            }
        }


        [Theory]
        [MemberData(nameof(PlayersForTesting))]
        public void EveryPlayerHasExactlyOneMatchPerRound(List<TournamentPlayer> players)
        {
            RoundRobinTournament<TournamentPlayer> tournament = new RoundRobinTournament<TournamentPlayer>();

            foreach (var player in players)
            {
                tournament.AddParticipant(player);
            }

            for (int i = 1; i <= tournament.ExpectedTournamentRounds; i++)
            {
                

                RoundDraw<TournamentPlayer> roundDraw = tournament.GenerateRoundDraw(i, null);

				var playersFromRoundDraw = new List<TournamentPlayer>();

                foreach (var pair in roundDraw.GetRoundPairs())
                {
                    playersFromRoundDraw.Add(pair.White);
                    playersFromRoundDraw.Add(pair.Black);
                }

                if(players.Count % 2 == 0)
                    Assert.Equal(playersFromRoundDraw.DistinctBy(x => x.Id).Count(), players.Count());
                else
                    Assert.Equal(playersFromRoundDraw.DistinctBy(x => x.Id).Count(), players.Count() - 1);
            }        
        }


        [Theory]
        [MemberData(nameof(PlayersForTesting))]
        public void EveryPlayerPlaysAgainsAllOtherPlayers(List<TournamentPlayer> players)
        {
            RoundRobinTournament<TournamentPlayer> tournament = new RoundRobinTournament<TournamentPlayer>();

            foreach (var player in players)
            {
                tournament.AddParticipant(player);
            }

            var pairsFromRoundDraw = new List<RoundPair<TournamentPlayer>>();

            for (int i = 1; i <= tournament.ExpectedTournamentRounds; i++)
            {
                RoundDraw<TournamentPlayer> roundDraw = tournament.GenerateRoundDraw(i, null);
               
                pairsFromRoundDraw.AddRange(roundDraw.GetRoundPairs()); 
            }

            ////
            ///

            foreach (var player in players)
            {
                IEnumerable<TournamentPlayer> opponentsWhite = pairsFromRoundDraw.Where(x => x.Black.Id == player.Id).Select(x => x.White);
                IEnumerable<TournamentPlayer> opponentsBlack = pairsFromRoundDraw.Where(x => x.White.Id == player.Id).Select(x => x.Black);

                List<TournamentPlayer> opponents = new List<TournamentPlayer>();
                opponents.AddRange(opponentsWhite);
                opponents.AddRange(opponentsBlack);

                Assert.Equal(opponents.Count, players.Count - 1);
            }
        }

        [Theory]
        [MemberData(nameof(PlayersForTesting))]
        public void BalancedWhiteAndBlackPiecesAcrossPlayers(List<TournamentPlayer> players)
        {
            RoundRobinTournament<TournamentPlayer> tournament = new RoundRobinTournament<TournamentPlayer>();

            foreach (var player in players)
            {
                tournament.AddParticipant(player);
            }

            var pairsFromRoundDraw = new List<RoundPair<TournamentPlayer>>();

            for (int i = 1; i <= tournament.ExpectedTournamentRounds; i++)
            {
                RoundDraw<TournamentPlayer> roundDraw = tournament.GenerateRoundDraw(i, null);

				pairsFromRoundDraw.AddRange(roundDraw.GetRoundPairs());
            }

            ////
            ///

            foreach (var player in players)
            {
                IEnumerable<TournamentPlayer> opponentsWhite = pairsFromRoundDraw.Where(x => x.Black.Id == player.Id).Select(x => x.White);
                IEnumerable<TournamentPlayer> opponentsBlack = pairsFromRoundDraw.Where(x => x.White.Id == player.Id).Select(x => x.Black);

                Assert.True(Math.Abs(opponentsBlack.Count() - opponentsWhite.Count()) <= 1);
            }


        }





    }
}