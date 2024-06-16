using ChessTournamentManager.Shared.AnnotationErrors;
using ChessTournamentMate.Shared.AvailableValues;
using DatabaseCommunicator.Annotations;
using System.Text;

namespace DatabaseCommunicator.Models
{
	/// <summary>
	/// Specify time control of the tournament.
	/// </summary>
	public class TimeControlSettings
	{
        /// <summary>
        /// Collection of single sections of the time control.
        /// </summary>
        [TimeControlSettingsListMovesOrderedAttribute(ErrorMessageResourceName = "timeControlNotValid", ErrorMessageResourceType = typeof(AnnotationErrors))]
        public List<TimeControlSettingsPiece> Elements { get; set; } = [];


		public TimeControlSettings() 
		{ 
		
		}

		public TimeControlSettings(IEnumerable<TimeControlSettingsPiece> settings)
		{
			Elements = settings.OrderBy(x => x.UntilMove).ToList();
		}

		/// <summary>
		/// Based on single sections defines the time type of the tournament, based on FIDE. Rapid, Classical, Blitz.
		/// </summary>
		/// <returns></returns>
		public TournamentTimeTypes GetTournamentTimeType()
		{
			if(Elements.Count == 0) 
			{
				return TournamentTimeTypes.None;
			}

			TimeSpan totalTime = new(0, 0, 0);

			foreach(var element in Elements)
			{
                totalTime = totalTime.Add(TimeSpan.FromHours(element.AvailableTimeHours));
                totalTime = totalTime.Add(TimeSpan.FromMinutes(element.AvailableTimeMinutes));
                totalTime = totalTime.Add(TimeSpan.FromSeconds(element.AvailableTimeSeconds));
            }

			int previousUntilMove = 0;
			for(int i = 0; i < Elements.Count; i++)
			{
				int howManyTimes = Elements[i].UntilMove - previousUntilMove;
				if (Elements[i].UntilMove > 60)
				{
                    howManyTimes = 60 - previousUntilMove;
                }
                totalTime = totalTime.Add(TimeSpan.FromSeconds(howManyTimes * Elements[i].Increment));
				previousUntilMove = Elements[i].UntilMove;
                if (Elements[i].UntilMove > 60)
                {
                    break;
                }
            }

			double totalMinutes = totalTime.TotalMinutes;

			if(totalMinutes <= 15)
			{
				return TournamentTimeTypes.Blitz;
			}

			if(totalMinutes <= 60)
			{
				return TournamentTimeTypes.Rapid;
			}

			return TournamentTimeTypes.Classic;
		}

		public string ToStringWithCustomTexts(string fromMove, string perMove)
		{
            StringBuilder builder = new();
            foreach (var element in Elements)
            {
                builder.Append(element.ToStringWithCustomTexts(fromMove, perMove));
                builder.Append('\n');
            }
            return builder.ToString();
        }

		/// <summary>
		/// Returns string representation of time control.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			StringBuilder builder = new();
			foreach (var element in Elements)
			{
				builder.Append(element.ToString());
				builder.Append('\n');
			}
			return builder.ToString();
		}
	}
}
