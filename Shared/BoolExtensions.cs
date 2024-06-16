using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessTournamentMate.Shared
{
	public static class BoolExtensions
	{


		public static string ToYesOrNo(this bool value)
		{
			return value ? "Yes" : "No";
		}

	}
}
