using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseCommunicator
{
    /// <summary>
    /// Settings based on which methods which retreives tournament from database can filter tournament which are not requested.
    /// </summary>
    public class TournamentsFilter 
    {
        /// <summary>
        /// Name of the tournament
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Registration period of the tournament starts from
        /// </summary>
        public DateOnly? StartDateFrom { get; set; }

        /// <summary>
        /// Registration period of the tournaments ends on
        /// </summary>
        public DateOnly? StartDateTo { get; set; }

        /// <summary>
        /// Location of the tournament - city
        /// </summary>
        public string? City { get; set; }

        /// <summary>
        /// Location of the tournament - country
        /// </summary>
        public string? Country { get; set; }

        public TournamentsFilter() { }

    }
}
