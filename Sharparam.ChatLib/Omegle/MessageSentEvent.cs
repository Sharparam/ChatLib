using System;

namespace Sharparam.ChatLib.Omegle
{
    /// <summary>
    /// EventArgs for the MessageSent event.
    /// </summary>
    public class MessageSentEventArgs : EventArgs
    {
        /// <summary>
        /// The chat data.
        /// </summary>
        public ChatData Data;

        internal MessageSentEventArgs(ChatData data)
        {
            Data = data;
        }
    }

    /// <summary>
    /// Event handler delegate for the MessageSent event.
    /// </summary>
    /// <param name="sender">The object that called the event.</param>
    /// <param name="args">Event arguments.</param>
    public delegate void MessageSentEventHandler(object sender, MessageSentEventArgs args);
}
