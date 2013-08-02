using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Sharparam.ChatLib.Bots
{
    /// <summary>
    /// Pandora bot: http://pandorabots.com/
    /// </summary>
    public sealed class Pandora : Chatbot
    {
        private const string RequestUrl = "http://www.pandorabots.com/pandora/talk-xml";

        private readonly Dictionary<string, string> _requestParams; 

        /// <summary>
        /// Creates a new Pandora bot.
        /// </summary>
        /// <param name="id">Bot ID to use.</param>
        public Pandora(string id)
        {
            _requestParams = new Dictionary<string, string>
            {
                {"botid", id},
                {"custid", Guid.NewGuid().ToString()}
            };
        }

        public override ChatData Process(ChatData data)
        {
            if (IsProcessing)
                throw new Exception("Bot is already processing!");

            IsProcessing = true;
            OnProcessingStarted(data);

            var timer = new Stopwatch();
            timer.Start();

            _requestParams["input"] = data.Text;

            var response = Utils.PostRequest(RequestUrl, _requestParams);

            timer.Stop();

            var result = new ChatData(Utils.XPathSearch(response, "//result/that/text()"), (int) timer.ElapsedMilliseconds);

            IsProcessing = false;
            OnProcessingFinished(result);

            return result;
        }
    }
}
