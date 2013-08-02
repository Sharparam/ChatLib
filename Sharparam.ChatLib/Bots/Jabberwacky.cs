using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Sharparam.ChatLib.Bots
{
    /// <summary>
    /// Jabberwacky: http://jabberwacky.com/
    /// </summary>
    public sealed class Jabberwacky : Chatbot
    {
        private const string RequestURL = "http://jabberwacky.com/webservicemin";

        private readonly Dictionary<string, string> _requestParams;

        /// <summary>
        /// Creates a new CleverBot instance.
        /// </summary>
        public Jabberwacky()
        {
            _requestParams = new Dictionary<string, string>
            {
                {"start", "y"},
                {"icognoid", "wsf"},
                {"fno", "0"},
                {"sub", "Say"},
                {"islearning", "1"},
                {"cleanslate", "false"},
            };
        }

        public override ChatData Process(ChatData data)
        {
            if (IsProcessing)
                throw new Exception("Bot is already processing data.");

            IsProcessing = true;
            OnProcessingStarted(data);

            var timer = new Stopwatch();
            timer.Start();

            _requestParams["stimulus"] = data.Text;

            var encoded = _requestParams.ToUrlParams();
            var encodedDigest = encoded.Substring(9, 20).ToMD5();

            _requestParams["icognocheck"] = encodedDigest;

            var response = Utils.PostRequest(RequestURL, _requestParams);

            var responseVals = response.Split('\r');

            _requestParams["sessionid"]         = responseVals.AtIndex(1);
            _requestParams["logurl"]            = responseVals.AtIndex(2);
            _requestParams["vText8"]            = responseVals.AtIndex(3);
            _requestParams["vText7"]            = responseVals.AtIndex(4);
            _requestParams["vText6"]            = responseVals.AtIndex(5);
            _requestParams["vText5"]            = responseVals.AtIndex(6);
            _requestParams["vText4"]            = responseVals.AtIndex(7);
            _requestParams["vText3"]            = responseVals.AtIndex(8);
            _requestParams["vText2"]            = responseVals.AtIndex(9);
            _requestParams["prevref"]           = responseVals.AtIndex(10);
            _requestParams["emotionalhistory"]  = responseVals.AtIndex(12);
            _requestParams["ttsLocMP3"]         = responseVals.AtIndex(13);
            _requestParams["ttsLocTXT"]         = responseVals.AtIndex(14);
            _requestParams["ttsLocTXT3"]        = responseVals.AtIndex(15);
            _requestParams["ttsText"]           = responseVals.AtIndex(16);
            _requestParams["lineRef"]           = responseVals.AtIndex(17);
            _requestParams["lineURL"]           = responseVals.AtIndex(18);
            _requestParams["linePOST"]          = responseVals.AtIndex(19);
            _requestParams["lineChoices"]       = responseVals.AtIndex(20);
            _requestParams["lineChoicesAbbrev"] = responseVals.AtIndex(21);
            _requestParams["typing"]            = responseVals.AtIndex(22);
            _requestParams["divert"]            = responseVals.AtIndex(23);

            timer.Stop();

            var result = new ChatData(responseVals.AtIndex(16), (int) timer.ElapsedMilliseconds);

            IsProcessing = false;
            OnProcessingFinished(result);

            return result;
        }
    }
}
