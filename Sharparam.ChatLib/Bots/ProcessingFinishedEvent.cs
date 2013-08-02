using System;

namespace Sharparam.ChatLib.Bots
{
    /// <summary>
    /// EventArgs for the ProcessingFinished event.
    /// </summary>
    public class ProcessingFinishedEventArgs : EventArgs
    {
        /// <summary>
        /// The chat data.
        /// </summary>
        public readonly ChatData Data;

        internal ProcessingFinishedEventArgs(ChatData data)
        {
            Data = data;
        }
    }

    /// <summary>
    /// Event handler for the ProcessingFinished event.
    /// </summary>
    /// <param name="sender">The object that called the event.</param>
    /// <param name="args">Event arguments.</param>
    public delegate void ProcessingFinishedEventHandler(object sender, ProcessingFinishedEventArgs args);
}
