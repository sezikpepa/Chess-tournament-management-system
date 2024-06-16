using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessTournamentMate.Shared.AvailableValues
{
    /// <summary>
    /// Tournament types based on the length of the time control
    /// </summary>
    public enum TournamentTimeTypes
    {
        /// <summary>
        /// Time control was not selected
        /// </summary>
        None,
        Blitz,
        Rapid,
        Classic
    }
}
