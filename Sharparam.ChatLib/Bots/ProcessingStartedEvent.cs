using System;

namespace Sharparam.ChatLib.Bots
{
    /// <summary>
    /// EventArgs class for the ProcessingStarted event.
    /// </summary>
    public class ProcessingStartedEventArgs : EventArgs
    {
        /// <summary>
        /// The chat data.
        /// </summary>
        public readonly ChatData Data;

        internal ProcessingStartedEventArgs(ChatData data)
        {
            Data = data;
        }
    }

    /// <summary>
    /// Event handler for the ProcessingStarted event.
    /// </summary>
    /// <param name="sender">The object that called the event.</param>
    /// <param name="args">Event arguments.</param>
    public delegate void ProcessingStartedEventHandler(object sender, ProcessingStartedEventArgs args);
}
