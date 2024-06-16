using DatabaseCommunicator.Models;
using TournamentLibrary;
using TournamentLibrary.Extensions;
using TournamentLibrary.Participants;
using TournamentLibrary.TournamentParts;
using TournamentLibrary.Tournaments;

namespace Tests
{
    public class AddressTests
    {
        [Fact]
        public void TestToStringFilled()
        {
			Address address = new()
			{
				Street = "Národní",
				HouseNumber = "15/6",
				City = "Prague",
				Country = "Czechia",
				Zip = "89652"
			};

			string stringRepresentation = address.ToString();


			Assert.Equal("Národní 15/6, Prague 89652, Czechia", stringRepresentation);
		}

		[Fact]
		public void TestToStringNotFilled()
		{
			Address address = new()
			{
				Street = "Národní",
				HouseNumber = "15/6",
			};

			string stringRepresentation = address.ToString();


			Assert.Equal("", stringRepresentation);
		}

		[Fact]
		public void TestIsFilledFilled()
		{
			Address address = new()
			{
				Street = "Národní",
				HouseNumber = "15/6",
				City = "Prague",
				Country = "Czechia",
				Zip = "89652"
			};

			bool isFilled = address.IsFilled;


			Assert.True(isFilled);
		}

		[Fact]
		public void TestIsFilledNotFilled()
		{
			Address address = new()
			{
				Street = "Národní",
				HouseNumber = "15/6",
				City = "Prague",
				Zip = "89652"
			};

			bool isFilled = address.IsFilled;


			Assert.False(isFilled);
		}
	}
}
