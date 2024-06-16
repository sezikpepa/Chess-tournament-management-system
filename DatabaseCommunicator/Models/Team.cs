
using ChessTournamentManager.Shared.AnnotationErrors;
using System.ComponentModel.DataAnnotations;
using TournamentLibrary.Participants;

namespace DatabaseCommunicator.Models;

public partial class Team : IParticipant
{
    public string Id { get; set; }

    /// <summary>
    /// Name of the team - can be proffesional or fun - does not have to be official
    /// </summary>
    [Required(ErrorMessageResourceName = "teamNameRequired", ErrorMessageResourceType = typeof(AnnotationErrors))]
    [StringLength(50, MinimumLength = 1, ErrorMessageResourceName = "teamName", ErrorMessageResourceType = typeof(AnnotationErrors))]
    public string Name { get; set; }

    public string DisplayName => Name;
}