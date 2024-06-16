using ChessTournamentManager.FoldersServices;
using ChessTournamentMate.Shared;
using DatabaseCommunicator.ModelsManipulators;
using Microsoft.AspNetCore.Mvc;
using System.Net.NetworkInformation;


namespace ChessTournamentManager.Controllers
{

	/// <summary>
	/// Users can set own profile picture on their profile page and also set profile picture of tournament they manage.
	/// These picture need to be handled and saved in the server. This class accepts the picture, checks file type, generates new and saves the picture.
	/// </summary>
	public class ProfilePictureUploadController : ControllerBase
	{
		/// <summary>
		/// Only files for images can be uploaded. This property contains all valid image types.
		/// </summary>
		public static readonly string[] SupportedFileExtensions = [".jpg", ".jpeg", ".png", ".bmg", ".heif", ".png"];

		private readonly ProfilePictureModel _profilePictureModel;
		private readonly ProfilePictureManipulator _profilePictureManipulator;

		public ProfilePictureUploadController(ProfilePictureModel profilePictureModel,
											  ProfilePictureManipulator profilePictureManipulator)
		{
			_profilePictureModel = profilePictureModel;
			_profilePictureManipulator = profilePictureManipulator;
		}

		/// <summary>
		/// After the picture is uploaded by user, it is handled via this method, which checks if the file type is supported.
		/// Then it assign it a name based on entity id for which the picture is uploaded (Player.Id or Tournament.Id). Then it is saved in the server storage.
		/// </summary>
		/// <param name="value">Upload picture to be saved for an entity</param>
		/// <returns></returns>
		/// <exception cref="ArgumentException">File type is check in the UI, if the user would try to bypass this check, it is checked once again here, but it throws an error.</exception>
		public async Task SaveProfilePicture(UploadProfilePicturePayLoad value)
		{
			if (!SupportedFileExtensions.Contains(value.FileExtension.ToLower()))
			{
				throw new ArgumentException($"Unsuported extension: use only {string.Join(", ", SupportedFileExtensions)}");
			}

			_profilePictureModel.DeletePicture(value.ForElement);
			await _profilePictureModel.SavePicture(value);

			await _profilePictureManipulator.UpdateProfilePicture(value.ForElement, $"{value.ForElement}-picture{value.FileExtension}");
		}
	}
}
