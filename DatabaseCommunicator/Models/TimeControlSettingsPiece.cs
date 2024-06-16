using ChessTournamentManager.Shared.AnnotationErrors;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DatabaseCommunicator.Models
{
    /// <summary>
    /// Single section of the time control.
    /// </summary>
	public class TimeControlSettingsPiece
	{
        /// <summary>
        /// How many seconds per move each player receives. 
        /// </summary>
        [Range(0, 9999, ErrorMessageResourceName = "timeControlSettingsIncrement", ErrorMessageResourceType = typeof(AnnotationErrors))]
        public int Increment { get; set; }

        /// <summary>
        /// From which move of the game this section is valid
        /// </summary>
        [Range(1, 300, ErrorMessageResourceName = "timeControlSettingsToMove", ErrorMessageResourceType = typeof(AnnotationErrors))]
        public int UntilMove { get; set; } = 1;

        /// <summary>
        /// How many hours each player gets after number of moves corresponding with UntilMove
        /// </summary>
        [Range(0, 50, ErrorMessageResourceName = "timeControlSettingsHours", ErrorMessageResourceType = typeof(AnnotationErrors))]
        public int AvailableTimeHours { get; set; }

        /// <summary>
        /// How many minutes each player gets after number of moves corresponding with UntilMove
        /// </summary>
        [Range(0, 59, ErrorMessageResourceName = "timeControlSettingMinutes", ErrorMessageResourceType = typeof(AnnotationErrors))]
        public int AvailableTimeMinutes { get; set; }

        /// <summary>
        /// How many seconds each player gets after number of moves corresponding with UntilMove
        /// </summary>
        [Range(0, 59, ErrorMessageResourceName = "timeControlSettingsSeconds", ErrorMessageResourceType = typeof(AnnotationErrors))]
        public int AvailableTimeSeconds { get; set; }


        public TimeControlSettingsPiece()
		{

		}


        public string ToStringWithCustomTexts(string fromMove, string perMove)
        {
            StringBuilder builder = new StringBuilder();

            builder.Append($"{fromMove} {UntilMove}: ");
            builder.Append($"{AvailableTimeHours}h ");
            builder.Append($"{AvailableTimeMinutes}m ");
            builder.Append($"{AvailableTimeSeconds}s ");
            builder.Append($"+ {Increment}s {perMove}");

            return builder.ToString();
        }


        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            builder.Append($"From move {UntilMove}: ");
            builder.Append($"{AvailableTimeHours}h ");
            builder.Append($"{AvailableTimeMinutes}m ");
            builder.Append($"{AvailableTimeSeconds}s ");
            builder.Append($"+ {Increment}s per move");
            
            return builder.ToString();
        }
    }
}
