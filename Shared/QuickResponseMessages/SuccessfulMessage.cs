using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessTournamentMate.Shared.QuickResponseMessages
{
    /// <summary>
    /// Message which represents information, that the action was successful
    /// </summary>
    public class SuccessfulMessage : QuickResponseMessage
    {

        public SuccessfulMessage()
        {
            MessageType = QuickResponseMessageTypes.Success;
        }

        public SuccessfulMessage(string message)
        {
            MessageType = QuickResponseMessageTypes.Success;
            Comment = message;
        }
    }
}
