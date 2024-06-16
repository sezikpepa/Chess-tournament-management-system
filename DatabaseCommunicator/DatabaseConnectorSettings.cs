using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseCommunicator
{
	/// <summary>
	/// Settings of connection to the database.
	/// </summary>
	public class DatabaseConnectorSettings
	{

		public string Uri { get; set; }

		public string UserName { get; set; }

		public string Password { get; set; }

	}
}
