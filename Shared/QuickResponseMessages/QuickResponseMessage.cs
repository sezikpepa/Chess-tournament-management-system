using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessTournamentMate.Shared
{
	/// <summary>
	/// Parent class more messages, which are shown in UI
	/// </summary>
	public class QuickResponseMessage
	{

		public string? Comment { get; set; }

		public QuickResponseMessageTypes MessageType { get; set; }

	}


	public enum QuickResponseMessageTypes
	{
		Success,
		Error,
		NotExistsInDatabase
	}
}
