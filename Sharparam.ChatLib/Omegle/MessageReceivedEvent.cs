using System;

namespace Sharparam.ChatLib.Omegle
{
    /// <summary>
    /// EventArgs for the MessageReceived event.
    /// </summary>
    public class MessageReceivedEventArgs : EventArgs
    {
        /// <summary>
        /// The chat data.
        /// </summary>
        public readonly ChatData Data;

        internal MessageReceivedEventArgs(ChatData data)
        {
            Data = data;
        }
    }

    /// <summary>
    /// Event handler delegate for the MessageReceived event.
    /// </summary>
    /// <param name="sender">The object that called the event.</param>
    /// <param name="args">Event arguments.</param>
    public delegate void MessageReceivedEventHandler(object sender, MessageReceivedEventArgs args);
}
