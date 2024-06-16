
using ChessTournamentManager.Shared.AnnotationErrors;
using ChessTournamentMate.Shared.AvailableValues;
using System.ComponentModel.DataAnnotations;

namespace DatabaseCommunicator.Models;

public partial class Tournament
{
    public string Id { get; set; }

    /// <summary>
    /// Name of the tournament - selected by manager of the tournament.
    /// </summary>
    [Required(ErrorMessageResourceName = "tournamentName", ErrorMessageResourceType = typeof(AnnotationErrors))]
    [StringLength(50, MinimumLength = 1, ErrorMessageResourceName = "tournamentNameLength", ErrorMessageResourceType = typeof(AnnotationErrors))]
    public string TournamentName { get; set; }

    /// <summary>
    /// Short description of the tournament - for example short history of a tournament series.
    /// </summary>
    [Required(ErrorMessageResourceName = "tournamentDescription", ErrorMessageResourceType = typeof(AnnotationErrors))]
    [StringLength(50, MinimumLength = 1, ErrorMessageResourceName = "tournamentDescriptionLength", ErrorMessageResourceType = typeof(AnnotationErrors))]
	public string ShortDescription { get; set; }

    /// <summary>
    /// When the tournament starts.
    /// </summary>
    public DateTime StartDate { get; set; }

    /// <summary>
    /// When the tournament ends. (Can be the same date)
    /// </summary>
    public DateTime EndDate { get; set; }

    /// <summary>
    /// Current round of the tournament.
    /// </summary>
    public int CurrentRound { get; set; }

    /// <summary>
    /// How many rounds will be held in the tournament.
    /// </summary>
    public int? ExpectedNumberOfRounds { get; set; }

    /// <summary>
    /// Type of pairing during the tournament
    /// </summary>
    public TournamentTypes TournamentType { get; set; }

    /// <summary>
    /// Tournament has already been started by the manager.
    /// </summary>
    public bool HasStarted { get; set; }

    /// <summary>
    /// Tournament is only for teams, not for single players. True - only for teams, False - only for single players.
    /// </summary>
    public bool IsTeam { get; set; }

    /// <summary>
    /// FileName of the picture of the tournament.
    /// </summary>
    public string? ProfilePictureName { get; set; }

	public override bool Equals(object? obj)
	{
		if(obj is Tournament tournament)
        {
            return Id == tournament.Id;
        }

        return false;
	}

	public override int GetHashCode()
	{
        return base.GetHashCode();
	}
}