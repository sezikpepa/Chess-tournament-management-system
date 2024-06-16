using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TournamentLibrary.TournamentParts
{
	/// <summary>
	/// Interface for classes which represents score of matches - single or team
	/// </summary>
	public interface IMatchResult
	{
		/// <summary>
		/// Points for white player, or team with white player on board one
		/// </summary>
		public decimal WhitePoints { get; set; }

		/// <summary>
		/// Points for black player, or team with black player on board one
		/// </summary>
		public decimal BlackPoints { get; set; }


	}
}
