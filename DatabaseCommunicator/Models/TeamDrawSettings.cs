

using ChessTournamentManager.Shared.AnnotationErrors;
using System.ComponentModel.DataAnnotations;

namespace DatabaseCommunicator.Models
{
    /// <summary>
    /// Specify how to performs draws of teams.
    /// </summary>
    public class TeamDrawSettings
    {
        /// <summary>
        /// Defines size of the team which can attend the tournament.
        /// </summary>
        [Required]
        [Range(2, 9, ErrorMessageResourceName = "teamDrawSize", ErrorMessageResourceType = typeof(AnnotationErrors))]
        public int TeamSizeForRound { get; set; } = -1;

        public TeamDrawSettings() 
        { 
        
        }
    }
}
