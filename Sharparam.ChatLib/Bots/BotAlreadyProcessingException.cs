using System;

namespace Sharparam.ChatLib.Bots
{
    /// <summary>
    /// Thrown when a bot is told to process a message
    /// when it's already busy processing an earlier one.
    /// </summary>
    public class BotAlreadyProcessingException : Exception
    {
        internal BotAlreadyProcessingException()
        {
            
        }

        internal BotAlreadyProcessingException(string message) : base(message)
        {
            
        }

        internal BotAlreadyProcessingException(string message, Exception inner) : base(message, inner)
        {
            
        }
    }
}
