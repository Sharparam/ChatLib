using System;
using Sharparam.ChatLib;
using Sharparam.ChatLib.Bots;
using Sharparam.ChatLib.Omegle;

namespace BotTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Idle...";

            var omegle = new OmegleClient();
            IChatbot bot = null;

            omegle.Connected += (o, e) => Console.WriteLine("Connected to Omegle! SessionID: {0}", omegle.SessionId);
            omegle.Disconnected += (o, e) => Console.WriteLine("Disconnected from Omegle!");
            omegle.Waiting += (o, e) => Console.WriteLine("Waiting...");
            omegle.StrangerConnected += (o, e) => Console.WriteLine("Stranger connected!");
            omegle.StrangerDisconnected += (o, e) => Console.WriteLine("Stranger disconnected!");
            omegle.StrangerTyping += (o, e) => Console.Title = "Stranger is typing...";
            omegle.StrangerStoppedTyping += (o, e) => Console.Title = String.Format("Stranger stopped typing after {0}ms.", e.Elapsed);
            omegle.CaptchaRequired += (o, e) => Console.WriteLine("Omegle is requesting a captcha response (NYI)!");
            omegle.CaptchaRejected += (o, e) => Console.WriteLine("Omegle rejected the captcha response!");
            omegle.UserCountUpdated += (o, e) => Console.WriteLine("User count updated: {0}", omegle.GetUserCount());
            //omegle.UnhandledEvent += (o, e) => Console.WriteLine("Unhandled: {0}", e.Event);
            omegle.MessageReceived += (o, e) =>
            {
                Console.Title = "Message received!";
                Console.WriteLine("Stranger: {0} (Latency: {1}ms)", e.Data.Text, e.Data.Latency);
                
                if (bot == null)
                    return;

                if (bot.IsProcessing)
                    return;

                try
                {
                    bot.ProcessAsync(e.Data);
                }
                catch (BotAlreadyProcessingException ex)
                {
                    Console.WriteLine("Tried to call bot.ProcessAsync when it was already processing!");
                }
            };
            omegle.MessageSent += (o, e) =>
            {
                Console.Title = "Bot has sent a message!";
                Console.WriteLine("     Bot: {0} (Latency: {1}ms)", e.Data.Text, e.Data.Latency);
            };

            string input = null;

            while (input != "e")
            {
                bot = new Cleverbot();

                bot.ProcessingStarted += (o, e) =>
                {
                    Console.Title = "Bot is thinking...";
                    omegle.SendTyping();
                };
                bot.ProcessingFinished += (o, e) =>
                {
                    if (!omegle.IsConnected || !omegle.IsStrangerConnected)
                        return;
                    Console.Title = "Bot is sending a message...";
                    omegle.SendTypingStopped();
                    omegle.SendMessage(e.Data.Text.Replace("Cleverbot", "Alice"), e.Data.Latency);
                };

                Console.WriteLine("Calling connect...");

                if (omegle.IsConnected)
                    omegle.Disconnect();
                omegle.Connect();

                while (omegle.IsConnected)
                    System.Threading.Thread.Sleep(1000);

                Console.WriteLine("Omegle connection closed!");
                Console.Title = "Idle...";
                input = Console.ReadLine();
            }
            
        }
    }
}
