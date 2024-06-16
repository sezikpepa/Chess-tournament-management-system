using Neo4j.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseCommunicator.ModelsManipulators
{
	/// <summary>
	/// Provides basic methods for communication with database.
	/// </summary>
	public class DatabaseModelManipulator
	{
		/// <summary>
		/// Connection string.
		/// </summary>
		private readonly DatabaseConnectorSettings _databaseConnectorSettings;

		/// <summary>
		/// Provides direct access to the database
		/// </summary>
		protected readonly IDriver _driver;

		public DatabaseModelManipulator(DatabaseConnectorSettings databaseConnectorSettings) 
		{
			_databaseConnectorSettings = databaseConnectorSettings;
			_driver = GetDriver();
		}

		/// <summary>
		/// Returns unique identifiers for verticies of the database to be stored.
		/// </summary>
		/// <returns></returns>
		protected static string GetRandomId()
		{
            return Guid.NewGuid().ToString();
		}

		/// <summary>
		/// Returns instance of the class which provides direct communication to the database.
		/// </summary>
		/// <returns></returns>
		protected IDriver GetDriver()
		{
			var uri = new Uri(_databaseConnectorSettings.Uri);
			var username = _databaseConnectorSettings.UserName;
			var password = _databaseConnectorSettings.Password;

			return GraphDatabase.Driver(uri, AuthTokens.Basic(username, password));
		}
	}
}
