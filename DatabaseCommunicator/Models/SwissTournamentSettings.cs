using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TournamentLibrary.Participants;
using TournamentLibrary;

namespace DatabaseCommunicator.Models
{
    /// <summary>
    /// Defines priorities of auxiliary points in swiss tournament.
    /// </summary>
    public class SwissTournamentSettings
    {
        /// <summary>
        /// Priorities of auxiliary points.
        /// </summary>
        public List<string> AuxiliaryPoints { get; set; } = [nameof(ParticipantWithPoints<TournamentPlayer>.Buchholz),
            nameof(ParticipantWithPoints<TournamentPlayer>.BuchholzShortened),
            nameof(ParticipantWithPoints<TournamentPlayer>.SonnenbornBerger),
            nameof(ParticipantWithPoints<TournamentPlayer>.BlackPiecesPlayed)];

    }
}
