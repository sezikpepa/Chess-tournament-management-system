
using ChessTournamentManager.Shared.AnnotationErrors;
using System.ComponentModel.DataAnnotations;

namespace DatabaseCommunicator.Models;

/// <summary>
/// Address, used as a specifictation of location of the tournament
/// </summary>
public class Address
{
    [StringLength(100, MinimumLength = 1, ErrorMessageResourceName = "addressStreet", ErrorMessageResourceType = typeof(AnnotationErrors))]
    [Required]
    public string? Street { get; set; }

    [StringLength(100, MinimumLength = 1, ErrorMessageResourceName = "addressHouseNumber", ErrorMessageResourceType = typeof(AnnotationErrors))]
    [Required]
    public string? HouseNumber { get; set; }

    [StringLength(100, MinimumLength = 1, ErrorMessageResourceName = "addressCity", ErrorMessageResourceType = typeof(AnnotationErrors))]
    [Required]
    public string? City { get; set; }

    [Required]
    public string? Country { get; set; }

    [StringLength(10, MinimumLength = 1, ErrorMessageResourceName = "addressZip", ErrorMessageResourceType = typeof(AnnotationErrors))]
    [Required]
	public string? Zip { get; set; }

    public bool IsFilled => !(Street is null || HouseNumber is null || City is null || Country is null || Zip is null);

    public override string ToString()
    {
        if(!IsFilled)
        {
            return "";
        }
        return $"{Street} {HouseNumber}, {City} {Zip}, {Country}";
    }
}