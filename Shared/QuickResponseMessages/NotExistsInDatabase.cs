using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessTournamentMate.Shared.QuickResponseMessages
{
    /// <summary>
    /// Message which represents information, that required object is not in database.
    /// </summary>
    public class NotExistsInDatabase : QuickResponseMessage
    {

        public NotExistsInDatabase()
        {
            MessageType = QuickResponseMessageTypes.NotExistsInDatabase;
        }

        public NotExistsInDatabase(string message)
        {
            MessageType = QuickResponseMessageTypes.NotExistsInDatabase;
            Comment = message;
        }
    }
}
