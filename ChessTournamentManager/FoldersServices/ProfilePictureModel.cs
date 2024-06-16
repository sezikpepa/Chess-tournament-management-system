using ChessTournamentMate.Shared;
using Microsoft.Extensions.Hosting.Internal;

namespace ChessTournamentManager.FoldersServices
{
	/// <summary>
	/// Handles profile pictures - loads them from the user and saves them on the server.
	/// </summary>
	public class ProfilePictureModel
	{
		/// <summary>
		/// Defines where are pictures located
		/// </summary>
		private readonly string _picturesPath;

		public ProfilePictureModel(IWebHostEnvironment webHostEnvironment)
		{
#if DEBUG
			_picturesPath = webHostEnvironment.WebRootPath + "//profilePictures";

#else
            _picturesPath = "/app/wwwroot/ProfilePictures";
#endif

        }

		/// <summary>
		/// Accepts uploaded pictures and saves them with the name corresponding the entity id.
		/// </summary>
		/// <param name="payload"></param>
		/// <returns></returns>
		public async Task SavePicture(UploadProfilePicturePayLoad payload)
		{
			await File.WriteAllBytesAsync($"{_picturesPath}/{payload.ForElement}-picture{payload.FileExtension}", payload.File);
		}

		/// <summary>
		/// Deletes picture from the server of the specific entity.
		/// </summary>
		/// <param name="forEntity"></param>
		/// <returns></returns>
		public void DeletePicture(string forEntity)
		{
			string[] files = Directory.GetFiles(_picturesPath);

			foreach (string filePath in files)
			{
				if (Path.GetFileNameWithoutExtension(filePath).Equals($"{forEntity}-picture", StringComparison.OrdinalIgnoreCase))
				{
					File.Delete(filePath);
				}
			}	
		}
	}
}
