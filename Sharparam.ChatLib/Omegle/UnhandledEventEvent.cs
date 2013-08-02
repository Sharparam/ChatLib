using System;
using Newtonsoft.Json.Linq;

namespace Sharparam.ChatLib.Omegle
{
    /// <summary>
    /// EventArgs for the UnhandledEvent event.
    /// </summary>
    public class UnhandledEventEventArgs : EventArgs
    {
        /// <summary>
        /// The name/type of event that was unhandled.
        /// </summary>
        public readonly string Event;

        /// <summary>
        /// Raw event data.
        /// </summary>
        public readonly string Data;
    
        internal UnhandledEventEventArgs(JToken e)
        {
            Event = e[0].ToString();
            Data = e.ToString();
        }
    }

    /// <summary>
    /// Event handler delegate for the UnhandledEvent event.
    /// </summary>
    /// <param name="sender">The object that raised the event.</param>
    /// <param name="args">Event arguments.</param>
    public delegate void UnhandledEventEventHandler(object sender, UnhandledEventEventArgs args);
}
