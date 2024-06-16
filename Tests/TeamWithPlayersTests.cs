using DatabaseCommunicator.Models;
using TournamentLibrary;
using TournamentLibrary.Extensions;
using TournamentLibrary.Participants;
using TournamentLibrary.TournamentParts;
using TournamentLibrary.Tournaments;

namespace Tests
{
    public class TeamWithPlayersTests
    {
        [Fact]
        public void GetFirstNumberOfPlayersAsString()
        {

            Player player1 = new Player()
            {
                FirstName = "player1",
                LastName = "player12"
            };

            Player player2 = new Player()
            {
                FirstName = "player2",
                LastName = "player22"
            };

            Player player3 = new Player()
            {
                FirstName = "player3",
                LastName = "player32"
            };

            Player player4 = new Player()
            {
                FirstName = "player4",
                LastName = "player42"
            };

            TeamWithPlayers team = new TeamWithPlayers()
            {
                Players = [player1, player2, player3, player4]
            };

            string stringShowZero = team.GetFirstNumberOfPlayersAsString(0);
            string stringShowTwo = team.GetFirstNumberOfPlayersAsString(2);
            string stringShowFive = team.GetFirstNumberOfPlayersAsString(5);


            Assert.Equal("...", stringShowZero);
            Assert.Equal("player12, player22...", stringShowTwo);
            Assert.Equal("player12, player22, player32, player42", stringShowFive);
        }
	}
}
