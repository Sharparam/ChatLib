using System;
using System.Threading;

namespace Sharparam.ChatLib.Bots
{
    /// <summary>
    /// Template Chatbot class, takes care of most common operations.
    /// </summary>
    public abstract class Chatbot : IChatbot
    {
        /// <summary>
        /// Called when processing of a request is started.
        /// ChatData object represents the data that is being processed.
        /// </summary>
        public event ProcessingStartedEventHandler ProcessingStarted;

        /// <summary>
        /// Called when processing of a request has finished.
        /// ChatData object represents that data that was returned by the bot.
        /// </summary>
        public event ProcessingFinishedEventHandler ProcessingFinished;

        protected Thread ProcessThread;

        public bool IsProcessing { get; protected set; }

        protected virtual void OnProcessingStarted(ChatData data)
        {
            var func = ProcessingStarted;
            if (func != null)
                func(this, new ProcessingStartedEventArgs(data));
        }

        protected virtual void OnProcessingFinished(ChatData data)
        {
            var func = ProcessingFinished;
            if (func != null)
                func(this, new ProcessingFinishedEventArgs(data));
        }

        public abstract ChatData Process(ChatData data);

        public virtual string Process(string message)
        {
            return Process(new ChatData(message)).Text;
        }

        public virtual void ProcessAsync(ChatData data)
        {
            if (IsProcessing || (ProcessThread != null && ProcessThread.IsAlive))
                throw new BotAlreadyProcessingException();

            ProcessThread = new Thread(o => Process((ChatData) o)) {IsBackground = true};
            ProcessThread.Start(data);
        }

        public virtual void ProcessAsync(string message)
        {
            ProcessAsync(new ChatData(message));
        }
    }
}
