using ChessTournamentMate.Shared.AvailableValues;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TournamentLibrary.Participants;
using TournamentLibrary.TournamentHandlers;
using TournamentLibrary.Tournaments;

namespace ChessTournamentMate.Shared
{
	/// <summary>
	/// Creates tournament handlers based on type of the tournament
	/// </summary>
	/// <typeparam name="T">Type of participants in the tournament</typeparam>
	public class TournamentHandlerFactory<T> where T : class, IParticipant, new()
    {

		public TournamentHandlerFactory() { }

		/// <summary>
		/// Returns tournament handler based on provided type of the tournament
		/// </summary>
		/// <param name="tournamentMatchingType">Type of the tournament</param>
		/// <returns></returns>
		/// <exception cref="ArgumentException">If the tournament type does not have a handler, exception is thrown</exception>
		public ITournament<T> CreateTournamentHandler(TournamentTypes tournamentMatchingType)
		{
			return tournamentMatchingType switch
			{
				TournamentTypes.RoundRobin => new RoundRobinTournament<T>(),
				TournamentTypes.Playoff => new PlayoffTournament<T>(),
				TournamentTypes.Swiss => new SwissTournament<T>(),
				_ => throw new ArgumentException("This tournament does not exists, but it can be implemented in the future"),
			};
		}
	}
}
