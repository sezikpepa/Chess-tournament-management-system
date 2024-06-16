using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessTournamentMate.Shared.QuickResponseMessages
{
    /// <summary>
    /// Message which represents information, that the action was not successful
    /// </summary>
    public class UnsuccessfulMessage : QuickResponseMessage
    {
        public UnsuccessfulMessage()
        {
            MessageType = QuickResponseMessageTypes.Error;
        }

        public UnsuccessfulMessage(string message)
        {
            MessageType = QuickResponseMessageTypes.Error;
            Comment = message;
        }

    }
}
