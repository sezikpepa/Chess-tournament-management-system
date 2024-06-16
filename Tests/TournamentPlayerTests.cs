using TournamentLibrary;
using TournamentLibrary.Extensions;
using TournamentLibrary.Participants;
using TournamentLibrary.TournamentParts;
using TournamentLibrary.Tournaments;

namespace Tests
{
    public class TournamentPlayerTests
    {
        [Fact]
        public void TestDisplayNameFull()
        {
			TournamentPlayer player = new TournamentPlayer
			{
				FirstName = "Name",
				MiddleName = "MiddleName",
				LastName = "LastName"
			};


			string displayName = player.DisplayName;


			Assert.Equal("Name MiddleName LastName", displayName);
		}

		[Fact]
		public void TestDisplayNameWithoutMiddleName()
		{
			TournamentPlayer player = new TournamentPlayer
			{
				FirstName = "Name",
				LastName = "LastName"
			};


			string displayName = player.DisplayName;


			Assert.Equal("Name LastName", displayName);
		}
	}
}
