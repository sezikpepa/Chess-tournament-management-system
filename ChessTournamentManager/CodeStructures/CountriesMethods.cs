using Blazor.Flags;

namespace ChessTournamentManager.CodeStructures
{
	/// <summary>
	/// Methods which use Blazor.Flags library to provide informations about countries
	/// </summary>
	public static class CountriesMethods
	{

		//https://github.com/kevinvenclovas/Blazor.Flags/blob/master/Blazor.Flags.Server.Example/Pages/Index.razor
		/// <summary>
		/// Returns all countries of the world + some territories
		/// </summary>
		public static IEnumerable<Country> AvailableCountries => Enum.GetValues(typeof(Country)).Cast<Country>().ToList();
	

	}
}
