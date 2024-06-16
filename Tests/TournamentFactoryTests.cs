using ChessTournamentMate.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TournamentLibrary.Participants;
using TournamentLibrary.TournamentHandlers;
using ChessTournamentMate.Shared.AvailableValues;
using TournamentLibrary.Tournaments;

namespace Tests
{
	public class TournamentFactoryTests
	{

		[Fact]
		public void ReturnsCorrectTournamentHandler()
		{
			var factory = new TournamentHandlerFactory<TournamentPlayer>();

			Assert.Equal(new RoundRobinTournament<TournamentPlayer>().GetType(), factory.CreateTournamentHandler(TournamentTypes.RoundRobin).GetType());
			Assert.Equal(new PlayoffTournament<TournamentPlayer>().GetType(), factory.CreateTournamentHandler(TournamentTypes.Playoff).GetType());
			Assert.Equal(new SwissTournament<TournamentPlayer>().GetType(), factory.CreateTournamentHandler(TournamentTypes.Swiss).GetType());
		}




	}
}
