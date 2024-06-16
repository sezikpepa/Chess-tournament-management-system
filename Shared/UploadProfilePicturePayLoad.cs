using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessTournamentMate.Shared
{
	/// <summary>
	/// When profile picture is uploaded, it is 
	/// </summary>
	public class UploadProfilePicturePayLoad
	{
		/// <summary>
		/// Type of the file (png, svg...)
		/// </summary>
		public string FileExtension { get; set; }

		/// <summary>
		/// Representation of the picture in bytes.
		/// </summary>
		public byte[] File { get; set; }

		/// <summary>
		/// For which element in the database is the picture
		/// </summary>
		public string ForElement { get; set; }

		public UploadProfilePicturePayLoad() { }

		public UploadProfilePicturePayLoad(string fileExtension, byte[] file, string forElement)
		{
			FileExtension = fileExtension;
			File = file;
			ForElement = forElement;
		}
	}
}
