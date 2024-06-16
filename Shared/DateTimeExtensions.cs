using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessTournamentMate.Shared
{
	public static class DateTimeExtensions
	{

		//code with little changes from https://stackoverflow.com/questions/38039/how-can-i-get-the-datetime-for-the-start-of-the-week
		public static DateTime StartOfWeek(this DateTime date, DayOfWeek startOfWeekDay = DayOfWeek.Monday)
		{
			int offset = (7 + (date.DayOfWeek - startOfWeekDay)) % 7;
			return date.AddDays(-offset);
		}
	}
}
