using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TournamentLibrary.AnnotationsTranslations;

namespace DatabaseCommunicator.Models
{
	public class ResultsSettings
	{
		/// <summary>
		/// Points in the tournament should not be count based on points from the matches, but based on points per win and per draw
		/// </summary>
		public bool CalculateByPointsPerWinDraw { get; set; }

		/// <summary>
		/// Defines how many points should the winner of the match get
		/// </summary>
		[Range(0, 100, ErrorMessageResourceName = "pointsPerWin", ErrorMessageResourceType = typeof(AnnotationTranslations))]
		public decimal? PointsForWin { get; set; } = 1;

		/// <summary>
		/// Defines how many points each player from the match should get, if the match was a draw.
		/// </summary>
		[Range(0, 100, ErrorMessageResourceName = "pointsPerDraw", ErrorMessageResourceType = typeof(AnnotationTranslations))]
		public decimal? PointsForDraw { get; set; } = 0;

		public ResultsSettings() { }

	}
}
