using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Sharparam.ChatLib.Omegle
{
    /// <summary>
    /// Connects to a random stranger on Omegle
    /// and creates a chat session with them.
    /// </summary>
    public class OmegleClient
    {
        /// <summary>
        /// Successfully connected to Omegle servers.
        /// </summary>
        public event EventHandler Connected;

        /// <summary>
        /// Disconnected from Omegle servers.
        /// </summary>
        public event EventHandler Disconnected;

        /// <summary>
        /// Raised when client is waiting for a stranger.
        /// </summary>
        public event EventHandler Waiting;

        /// <summary>
        /// Stranger has connected.
        /// </summary>
        public event EventHandler StrangerConnected;

        /// <summary>
        /// Stranger has disconnected.
        /// </summary>
        public event EventHandler StrangerDisconnected;

        /// <summary>
        /// Stranger has started typing a message.
        /// </summary>
        public event EventHandler StrangerTyping;

        /// <summary>
        /// Stranger has stopped typing their message.
        /// </summary>
        public event EventHandler StrangerStoppedTyping;

        /// <summary>
        /// Raised when SendRawMessage has sent a message.
        /// </summary>
        public event MessageSentEventHandler RawMessageSent;

        /// <summary>
        /// We have sent a message.
        /// </summary>
        public event MessageSentEventHandler MessageSent;

        /// <summary>
        /// Stranger has sent a message.
        /// </summary>
        public event MessageReceivedEventHandler MessageReceived;

        /// <summary>
        /// Raised when the user count for the current server was updated.
        /// </summary>
        /// <remarks>Access user count value through GetUserCount.</remarks>
        public event EventHandler UserCountUpdated;

        /// <summary>
        /// Captcha needs solving to connect.
        /// </summary>
        public event CaptchaRequiredEventHandler CaptchaRequired;

        /// <summary>
        /// Captcha solution was invalid.
        /// </summary>
        public event EventHandler CaptchaRejected;

        /// <summary>
        /// An event from the Omegle servers went unhandled.
        /// </summary>
        public event UnhandledEventEventHandler UnhandledEvent;

        // .omegle.com
        private static readonly string[] ServerList = new[]
        {
            "", "bajor", "cardassia", "promenade", "odo-bucket"
        };

        // Holds user count for each server.
        private static readonly int[] UserCount = new int[ServerList.Length];

        // Incremented on each connect to "randomize" the servers
        // (never connects to the same server twice in a row)
        private static int _serverIndex = -1;

        private Thread _listenThread;

        private readonly Stopwatch _listenLatencyTimer;

        /// <summary>
        /// Gets whether or not the client is connected to Omegle servers.
        /// </summary>
        public bool IsConnected { get; private set; }

        /// <summary>
        /// Gets whether or not a stranger is currently connected.
        /// </summary>
        public bool IsStrangerConnected { get; private set; }

        /// <summary>
        /// The ID of the current chat session.
        /// </summary>
        public string SessionId { get; private set; }

        /// <summary>
        /// Creates a new OmegleClient.
        /// </summary>
        public OmegleClient()
        {
            _listenLatencyTimer = new Stopwatch();
        }

        private void OnConnected()
        {
            var func = Connected;
            if (func != null)
                func(this, null);
        }

        private void OnDisconnected()
        {
            var func = Disconnected;
            if (func != null)
                func(this, null);
        }

        private void OnWaiting()
        {
            var func = Waiting;
            if (func != null)
                func(this, null);
        }

        private void OnStrangerConnected()
        {
            var func = StrangerConnected;
            if (func != null)
                func(this, null);
        }

        private void OnStrangerDisconnected()
        {
            var func = StrangerDisconnected;
            if (func != null)
                func(this, null);
        }

        private void OnStrangerTyping()
        {
            var func = StrangerTyping;
            if (func != null)
                func(this, null);
        }

        private void OnStrangerStoppedTyping()
        {
            var func = StrangerStoppedTyping;
            if (func != null)
                func(this, null);
        }

        private void OnRawMessageSent(ChatData data)
        {
            var func = RawMessageSent;
            if (func != null)
                func(this, new MessageSentEventArgs(data));
        }

        private void OnMessageSent(ChatData data)
        {
            var func = MessageSent;
            if (func != null)
                func(this, new MessageSentEventArgs(data));
        }

        private void OnMessageReceived(ChatData data)
        {
            var func = MessageReceived;
            if (func != null)
                func(this, new MessageReceivedEventArgs(data));
        }

        private void OnUserCountUpdated()
        {
            var func = UserCountUpdated;
            if (func != null)
                func(this, null);
        }

        private void OnCaptchaRequired(string id)
        {
            var func = CaptchaRequired;
            if (func != null)
                func(this, new CaptchaRequiredEventArgs(id));
        }

        private void OnCaptchaRejected()
        {
            var func = CaptchaRejected;
            if (func != null)
                func(this, null);
        }

        private void OnUnhandledEvent(JToken e)
        {
            var func = UnhandledEvent;
            if (func != null)
                func(this, new UnhandledEventEventArgs(e));
        }

        /// <summary>
        /// Gets the number of users on the specified Omegle server.
        /// </summary>
        /// <param name="server">Server index to check.</param>
        /// <returns>The number of users on the specified server.</returns>
        /// <remarks>Data may not be up to date for servers other than the current one.</remarks>
        public static int GetUserCount(int server)
        {
            return UserCount[_serverIndex];
        }

        /// <summary>
        /// Gets the number of users on the current Omegle server.
        /// </summary>
        /// <returns>The number of users on the current server.</returns>
        public int GetUserCount()
        {
            return GetUserCount(_serverIndex);
        }

        private static int GetNewServerIndex()
        {
            var value = _serverIndex + 1;
            if (value >= ServerList.Length)
                value = 0;
            return value;
        }

        private static string GetOmegleUrl(string sub)
        {
            var top = ServerList[_serverIndex];
            return String.Format("http://{0}{1}omegle.com/{2}", top, String.IsNullOrEmpty(top) ? "" : ".", sub);
        }

        /// <summary>
        /// Connects to the specified Omegle server.
        /// </summary>
        /// <param name="server">Server index to connect to.</param>
        public void Connect(int server)
        {
            if (server >= ServerList.Length)
                throw new ArgumentOutOfRangeException("server", "Server index larger than server array length");
            
            _serverIndex = server;

            var url = GetOmegleUrl("start");

            var response = Utils.PostRequest(url, new Dictionary<string, string>
            {
                {"rcs", "1"}
            });

            SessionId = response.TrimStart('"').TrimEnd('"');

            IsConnected = true;

            OnConnected();

            _listenThread = new Thread(o => Listen()) {IsBackground = true};
            _listenThread.Start();
        }

        /// <summary>
        /// Connects to a new Omegle server.
        /// </summary>
        public void Connect()
        {
            Connect(GetNewServerIndex());
        }

        /// <summary>
        /// Disconnects from current Omegle server.
        /// </summary>
        public void Disconnect()
        {
            var url = GetOmegleUrl("disconnect");
            Utils.PostRequest(url, new Dictionary<string, string> {{"id", SessionId}});
            IsStrangerConnected = false;
            IsConnected = false;
            OnDisconnected();
        }

        /// <summary>
        /// Reconnects to the specified Omegle server.
        /// </summary>
        /// <param name="server">Server index to reconnect to.</param>
        public void Reconnect(int server)
        {
            Disconnect();
            Connect(server);
        }

        /// <summary>
        /// Reconnects to a new Omegle server.
        /// </summary>
        public void Reconnect()
        {
            Reconnect(GetNewServerIndex());
        }

        /// <summary>
        /// Notifies the Omegle server that typing has started.
        /// </summary>
        public void SendTyping()
        {
            var url = GetOmegleUrl("typing");
            Utils.PostRequest(url, new Dictionary<string, string> {{"id", SessionId}});
        }

        /// <summary>
        /// Notifies the Omegle server that typing has stopped.
        /// </summary>
        public void SendTypingStopped()
        {
            var url = GetOmegleUrl("stoppedtyping");
            Utils.PostRequest(url, new Dictionary<string, string> {{"id", SessionId}});
        }

        /// <summary>
        /// Sends a message to the currently connected
        /// stranger, without HTML-encoding it first.
        /// </summary>
        /// <param name="message">The raw message to send.</param>
        /// <param name="latency">Latency to add.</param>
        public void SendRawMessage(string message, int latency = 0)
        {
            if (!IsConnected || !IsStrangerConnected)
                return;

            var timer = new Stopwatch();
            timer.Start();

            var url = GetOmegleUrl("send");

            Utils.PostRequest(url, new Dictionary<string, string>
            {
                {"id", SessionId},
                {"msg", message}
            });

            timer.Stop();

            OnRawMessageSent(new ChatData(message, latency + (int) timer.ElapsedMilliseconds));
        }

        /// <summary>
        /// Sends a message to the currently connected stranger.
        /// </summary>
        /// <param name="message">The message to send.</param>
        /// <param name="latency">Latency to add.</param>
        public void SendMessage(string message, int latency = 0)
        {
            if (!IsConnected || !IsStrangerConnected)
                return;

            var timer = new Stopwatch();
            timer.Start();

            SendRawMessage(HttpUtility.UrlEncode(message));

            timer.Stop();

            OnMessageSent(new ChatData(message, latency + (int) timer.ElapsedMilliseconds));
        }

        /// <summary>
        /// Sends a reCAPTCHA response to the Omegle servers.
        /// </summary>
        /// <param name="challenge">reCAPTCHA challenge field.</param>
        /// <param name="response">reCAPTCHA response field.</param>
        public void SendCaptchaResponse(string challenge, string response)
        {
            var url = GetOmegleUrl("recaptcha");
            Utils.PostRequest(url, new Dictionary<string, string>
            {
                {"id", SessionId},
                {"challenge", challenge},
                {"response", response}
            });
        }

        private bool HandleEvents(string data, long elapsed = 0)
        {
            var events = JsonConvert.DeserializeObject<JArray>(data);

            if (events == null || events.Count == 0)
                return false;

            foreach (var e in events)
            {
                switch (e[0].ToString().TrimStart('"').TrimEnd('"'))
                {
                    case "connected":
                        IsStrangerConnected = true;
                        OnStrangerConnected();
                        break;
                    case "strangerDisconnected":
                        IsStrangerConnected = false;
                        OnStrangerDisconnected();
                        Disconnect();
                        break;
                    case "gotMessage":
                        OnMessageReceived(new ChatData(e[1].ToString().TrimStart('"').TrimEnd('"'), (int) elapsed));
                        break;
                    case "waiting":
                        IsStrangerConnected = false;
                        OnWaiting();
                        break;
                    case "typing":
                        OnStrangerTyping();
                        break;
                    case "stoppedTyping":
                        OnStrangerStoppedTyping();
                        break;
                    case "count":
                        int count;
                        var valid = int.TryParse(e[1].ToString(), out count);
                        if (valid)
                            UserCount[_serverIndex] = count;
                        OnUserCountUpdated();
                        break;
                    case "recaptchaRequired":
                        OnCaptchaRequired(e[1].ToString());
                        break;
                    case "recaptchaRejected":
                        OnCaptchaRejected();
                        break;
                    case "error":
                    case "suggestSpyee":
                    case "spyTyping":
                    case "spyStoppedTyping":
                    case "spyDisconnected":
                    case "question":
                    default:
                        OnUnhandledEvent(e);
                        break;
                }
            }

            return true;
        }

        private void Listen()
        {
            try
            {
                while (IsConnected)
                {
                    var url = GetOmegleUrl("events");

                    _listenLatencyTimer.Reset();
                    _listenLatencyTimer.Start();

                    var response = Utils.PostRequest(url, new Dictionary<string, string> { { "id", SessionId } });

                    _listenLatencyTimer.Stop();

                    if (response == null || response.Trim() == "" || response.ToLower() == "null" ||
                        !HandleEvents(response, _listenLatencyTimer.ElapsedMilliseconds))
                        Thread.Sleep(1000);
                }
            }
            catch (WebException)
            {
                IsStrangerConnected = false;
                IsConnected = false;
                OnDisconnected();
            }
            
        }
    }
}
