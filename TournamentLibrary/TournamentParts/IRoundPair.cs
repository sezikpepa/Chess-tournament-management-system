using System.Numerics;
using TournamentLibrary.Participants;

namespace TournamentLibrary.TournamentParts
{
    /// <summary>
    /// Interface for classes, which server as members of RoundDraw. It represent a match between
    /// two opponents
    /// </summary>
    public interface IRoundPair<T> where T : IParticipant
    {
        /// <summary>
        /// Opponent who is supposed to play with white pieces. If the program is used for other sports
        /// other then chess, it can be seen as the player 1.
        /// </summary>
		public T White { get; set; }

		/// <summary>
		/// Opponent who is supposed to play with black pieces. If the program is used for other sports
		/// other then chess, it can be seen as the player 2.
		/// </summary>
		public T Black { get; set; }

        /// <summary>
        /// If players are inserted in the wrong order, this method will solve this problem. It will
        /// change values in White and Black
        /// </summary>
		void SwapParticipants();

		public bool IsDraw { get; }

		public T GetLoser();

        public T GetWinner();

        public decimal GetScoreOfParticipant(T participant);

		public bool? TieBreakWinnerWhite { get; set; }

    }
}
