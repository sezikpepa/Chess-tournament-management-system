using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TournamentLibrary.Participants;

namespace TournamentLibrary
{
    /// <summary>
    /// Participant of the tournament with obtained points.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ParticipantWithPoints<T> where T : IParticipant
    {
        /// <summary>
        /// Participant of the tournament
        /// </summary>
        public T Participant { get; set; }

        /// <summary>
        /// How many points player obtained during tournament.
        /// </summary>
        public decimal Score { get; set; }

        /// <summary>
        /// Auxiliary point - Buchholz
        /// </summary>
        public decimal? Buchholz { get; set; }

		/// <summary>
		/// Auxiliary point - Buchholz without the matches with the strongest and the weakest opponents of all opponents participant have player.
		/// </summary>
		public decimal? BuchholzShortened { get; set; }

		/// <summary>
		/// Auxiliary point - Sonnenborn-Berger
		/// </summary>
		public decimal? SonnenbornBerger { get; set; }

        //stored as decimal? to remain consistent, it is used by reflection and it would create a lot of more code to handle different types
        /// <summary>
        /// How many times a player played as black during tournament.
        /// </summary>
        public decimal? BlackPiecesPlayed { get; set; }


        public ParticipantWithPoints()
        {

        }


        public ParticipantWithPoints(T participant)
        {
            Participant = participant;
            Score = 0;
        }
        

        public ParticipantWithPoints(T participant, decimal score) 
        { 
            Participant = participant;
            Score = score;       
        }

        public void AddScore(decimal points)
        {
            Score += points;
        }

    }
}
