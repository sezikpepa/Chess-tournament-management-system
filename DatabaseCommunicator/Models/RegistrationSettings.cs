

using ChessTournamentManager.Shared.AnnotationErrors;
using System.ComponentModel.DataAnnotations;

namespace DatabaseCommunicator.Models
{
    /// <summary>
    /// Defines period in which it is possible to register into the tournament and the capaty of the tournament
    /// </summary>
    public class RegistrationSettings
    {
		/// <summary>
		/// When the registration period starts.
		/// </summary>
		[Required(ErrorMessageResourceName = "registrationStart", ErrorMessageResourceType = typeof(AnnotationErrors))]
		public DateTime? RegistrationStart { get; set; }

		/// <summary>
		/// When the registration period ends.
		/// </summary>
		[Required(ErrorMessageResourceName = "registrationEnd", ErrorMessageResourceType = typeof(AnnotationErrors))]
		public DateTime? RegistrationEnd { get; set; }

        /// <summary>
        /// Defines how many participants can attend the tournament.
        /// </summary>
        [Required]
        [Range(1, 300, ErrorMessageResourceName = "registrationSettings", ErrorMessageResourceType = typeof(AnnotationErrors))]
        public int MaximumNumberOfParticipants { get; set; }
    }
}
