using ChessTournamentMate.Shared;
using DatabaseCommunicator.Models;
using Neo4j.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseCommunicator.ModelsManipulators
{
	public class ProfilePictureManipulator : DatabaseModelManipulator
	{

		public ProfilePictureManipulator(DatabaseConnectorSettings databaseConnectorSettings) : base(databaseConnectorSettings)
		{

		}

		/// <summary>
		/// Saves the information about new profile picture of the entity
		/// </summary>
		/// <param name="entityId">Id of the entity whose the picture belongs</param>
		/// <param name="imageName">Name of the file, where the picture is saved</param>
		/// <returns></returns>
		public async Task UpdateProfilePicture(string entityId, string imageName)
		{
			using (var session = _driver.AsyncSession())
			{

				var properties = new
				{
					Id = entityId,
					ImageName = imageName
				};

				var query = "MATCH (p {Id: $Id}) SET p.ProfilePictureName = $ImageName";

				await session.RunAsync(query, properties);
			}
		}

	}
}
