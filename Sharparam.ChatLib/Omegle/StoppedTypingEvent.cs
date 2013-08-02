using System;

namespace Sharparam.ChatLib.Omegle
{
    /// <summary>
    /// EventArgs for the StoppedTyping event.
    /// </summary>
    public class StoppedTypingEventArgs : EventArgs
    {
        /// <summary>
        /// Amount of time that elapsed (in milliseconds)
        /// from when the user started typing and then stopped.
        /// </summary>
        public readonly int Elapsed;

        internal StoppedTypingEventArgs(int elapsed)
        {
            Elapsed = elapsed;
        }
    }

    /// <summary>
    /// Event handler delegate for the StoppedTyping event.
    /// </summary>
    /// <param name="sender">The object that raised the event.</param>
    /// <param name="args">Event arguments.</param>
    public delegate void StoppedTypingEventHandler(object sender, StoppedTypingEventArgs args);
}
