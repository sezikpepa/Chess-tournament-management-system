using DatabaseCommunicator.Models;
using TournamentLibrary;
using TournamentLibrary.Extensions;
using TournamentLibrary.Participants;
using TournamentLibrary.TournamentParts;
using TournamentLibrary.Tournaments;

namespace Tests
{
    public class TimeControlSettingsPieceTests
    {
        [Fact]
        public void TestToString()
        {
			TimeControlSettingsPiece timeSettingsPiece = new TimeControlSettingsPiece()
			{
				Increment = 5,
				AvailableTimeHours = 10,
				AvailableTimeMinutes = 15,
				AvailableTimeSeconds = 10,
				UntilMove = 40
			};

			string stringRepresentation = timeSettingsPiece.ToString();


			Assert.Equal("Until move 40: 10h 15m 10s + 5s per move", stringRepresentation);
		}
	}
}
