using System.Text;

namespace DatabaseCommunicator.Models
{
	/// <summary>
	/// Represents team and its players
	/// </summary>
	public class TeamWithPlayers
	{
		/// <summary>
		/// Information about the team
		/// </summary>
		public Team Team { get; set; }

		/// <summary>
		/// Players which represents the team.
		/// </summary>
		public List<Player> Players { get; set; }


		public TeamWithPlayers(Team team, List<Player> players) 
		{ 
			Team = team;
			Players = players;
		}

		public TeamWithPlayers()
		{

		}

		public string GetFirstNumberOfPlayersAsString(int maximumToShow)
		{
			StringBuilder result = new();
			for(int i = 0; i < Players.Count && i < maximumToShow; i++)
			{
				result.Append(Players[i].LastName);
				result.Append(", ");
			}

			if(result.Length > 1)
			{
				result.Remove(result.Length - 2, 2);
			}

			if(Players.Count > maximumToShow)
			{
				result.Append("...");
			}

			return result.ToString();
		}
	}
}
