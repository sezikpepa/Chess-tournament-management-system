
using ChessTournamentManager.Shared.AnnotationErrors;
using System.ComponentModel.DataAnnotations;
using TournamentLibrary.Participants;

namespace DatabaseCommunicator.Models;

/// <summary>
/// Every person in the database.
/// </summary>
public partial class Player : TournamentPlayer
{
	/// <summary>
	/// Id of the cccount linked to this user.
	/// </summary>
    public string? AccountId { get; set; }

	/// <summary>
	/// Date of birth
	/// </summary>
    public DateTime? DateOfBirth { get; set; }

	/// <summary>
	/// Chessclub where is the player registrated
	/// </summary>
	[StringLength(50, MinimumLength = 0, ErrorMessageResourceName = "chessClubLength", ErrorMessageResourceType = typeof(AnnotationErrors))]
	public string? ChessClub { get; set; }

	/// <summary>
	/// Id of the person in the FIDE database
	/// </summary>
	[StringLength(20, MinimumLength = 0, ErrorMessageResourceName = "FideIdLength", ErrorMessageResourceType = typeof(AnnotationErrors))]
	public string? FideId { get;set; }

	/// <summary>
	/// Performance coefficient
	/// </summary>
	[Range(1000, double.MaxValue, ErrorMessageResourceName = "playerElo", ErrorMessageResourceType = typeof(AnnotationErrors))]
	public int? Elo { get; set; }

	/// <summary>
	/// Name of the file witch to profile picture of this person.
	/// </summary>
	public string? ProfilePictureName { get; set; }

	/// <summary>
	/// Nationality of the player
	/// </summary>
	public string? Country { get; set; }

	public override bool Equals(object? obj)
	{
		if (obj is Player other)
		{
			return other.Id == Id;
		}
		return false;
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(Id);
	}
}