namespace ChessTournamentManager.DataServices.Filters
{
	/// <summary>
	/// Settings which servers as a filter for methods which retreives players from the database.
	/// </summary>
	public class PlayerServiceFilter
	{
		/// <summary>
		/// First name of the player
		/// </summary>
		public string? FirstName { get; set; }
		
		/// <summary>
		/// Last name of the player
		/// </summary>
		public string? LastName { get; set; }

		/// <summary>
		/// Name of the chess where is the player registrated
		/// </summary>
		public string? ClubName { get; set; }

		public PlayerServiceFilter() { }

		public PlayerServiceFilter(string? firstName, string? lastName, string? clubName) 
		{ 
			FirstName = firstName;
			LastName = lastName;
			ClubName = clubName;		
		}

	}
}
