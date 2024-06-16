using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessTournamentMate.Shared
{
	public class AccountEmailSenderSettings
	{
		/// <summary>
		/// Email address from which are emails sended
		/// </summary>
		public string From { get; set; }

		/// <summary>
		/// Password to the email
		/// </summary>
		public string Password { get; set; }

		/// <summary>
		/// Web server of the email
		/// </summary>
		public string Smtp { get; set; }

		/// <summary>
		/// Port on which the sender should connect on email sender
		/// </summary>
		public int Port { get; set; }
	}

}
