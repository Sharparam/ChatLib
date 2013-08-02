using System;

namespace Sharparam.ChatLib.Bots
{
    /// <summary>
    /// Bot capable of processing input and returning a relevant response.
    /// </summary>
    public interface IChatbot
    {
        /// <summary>
        /// Called when processing of a request is started.
        /// ChatData object represents the data that is being processed.
        /// </summary>
        event ProcessingStartedEventHandler ProcessingStarted;

        /// <summary>
        /// Called when processing of a request has finished.
        /// ChatData object represents that data that was returned by the bot.
        /// </summary>
        event ProcessingFinishedEventHandler ProcessingFinished;

        /// <summary>
        /// Gets whether the bot is currently processing data.
        /// </summary>
        bool IsProcessing { get; }

        /// <summary>
        /// Processes data,
        /// blocking the calling thread until the operation completes.
        /// </summary>
        /// <param name="data">The data to process.</param>
        /// <returns>The data returned by the bot.</returns>
        ChatData Process(ChatData data);

        /// <summary>
        /// Processes a message,
        /// blocking the calling thread until the operation completes.
        /// </summary>
        /// <param name="message">The message to process.</param>
        /// <returns>The message returned by the bot.</returns>
        string Process(string message);

        /// <summary>
        /// Begins processing data,
        /// returning control to the calling thread after the request has been sent.
        /// The Processed event will be called when operation completes.
        /// </summary>
        /// <param name="data">The data to process.</param>
        void ProcessAsync(ChatData data);

        /// <summary>
        /// Begins processing a message,
        /// returning control to the calling thread after the request has been sent.
        /// The Processed event will be called when operation completes.
        /// </summary>
        /// <param name="message">The message to process.</param>
        void ProcessAsync(string message);
    }
}
