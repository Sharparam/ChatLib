using System;

namespace Sharparam.ChatLib
{
    /// <summary>
    /// Returned by or provided to chat methods,
    /// represents the data that should be sent or that was received.
    /// </summary>
    public class ChatData : EventArgs
    {
        /// <summary>
        /// Text representation of the data.
        /// </summary>
        public readonly string Text;

        /// <summary>
        /// Time (in milliseconds) it took for
        /// the source to produce the data.
        /// </summary>
        public readonly int Latency;

        /// <summary>
        /// Creates a new ChatData instance.
        /// </summary>
        /// <param name="text">Text representation of the data.</param>
        /// <param name="latency">Time (in milliseconds) it took
        /// to produce the data.</param>
        public ChatData(string text, int latency = 0)
        {
            Text = text;
            Latency = latency;
        }
    }
}
