using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Xml.XPath;

namespace Sharparam.ChatLib
{
    /// <summary>
    /// Various helpful utilities.
    /// </summary>
    public static class Utils
    {
        /// <summary>
        /// Human reaction time in milliseconds.
        /// </summary>
        public const uint HumanReactionTime = 100;

        private static readonly Dictionary<char, uint> CharOverrides = new Dictionary<char, uint>
        {
            // Number row      SHIFT       ALT GR
            {'§', 90},         {'½', 130},
            {'1', 90},         {'!', 130}, {'@',  130},
            {'2', 90},         {'"', 130}, {'£',  130},
            {'3', 90},         {'#', 130}, {'$',  130},
            {'4', 90},         {'¤', 130},
            {'5', 90},         {'%', 130},
            {'6', 90},         {'&', 130},
            {'7', 90},         {'/', 130}, {'{',  130},
            {'8', 90},         {'(', 130}, {'[',  130},
            {'9', 90},         {')', 130}, {']',  130},
            {'0', 90},         {'=', 130}, {'}',  130},
            {'+', 90},         {'?', 130}, {'\\', 130},
            // Above home row  SHIFT       ALT GR
            {'q',  80},        {'Q', 120},
            {'w',  80},        {'W', 120},
            {'e',  80},        {'E', 120},
            {'r',  80},        {'R', 120},
            {'t',  80},        {'T', 120},
            {'y',  80},        {'Y', 120},
            {'u',  80},        {'U', 120},
            {'i',  80},        {'I', 120},
            {'o',  80},        {'O', 120},
            {'p',  80},        {'P', 120},
            {'å',  80},        {'Å', 120},
            {'¨', 110},        {'^', 150}, {'~', 150},
            // Home row        SHIFT
            {'a',  60},        {'A', 90},
            {'s',  60},        {'S', 90},
            {'d',  60},        {'D', 90},
            {'f',  60},        {'F', 90},
            {'g',  60},        {'G', 90},
            {'h',  60},        {'H', 90},
            {'j',  60},        {'J', 90},
            {'k',  60},        {'K', 90},
            {'l',  60},        {'L', 90},
            {'ö',  60},        {'Ö', 90},
            {'ä',  60},        {'Ä', 90},
            {'\'', 60},        {'*', 90},
            // Below home row  SHIFT       ALT GR
            {'<', 80},         {'>', 120}, {'|', 120},
            {'z', 80},         {'Z', 120},
            {'x', 80},         {'X', 120},
            {'c', 80},         {'C', 120},
            {'v', 80},         {'V', 120},
            {'b', 80},         {'B', 120},
            {'n', 80},         {'N', 120},
            {'m', 80},         {'M', 120},
            {',', 80},         {';', 120},
            {'.', 80},         {':', 120},
            {'-', 80},         {'_', 120},
            // Space
            {' ', 30}
        };

        /// <summary>
        /// Returns the time (in milliseconds) it would take a human to
        /// write the specified character.
        /// </summary>
        /// <param name="c">The character to check.</param>
        /// <returns>Time (in milliseconds) it would take an average human
        /// to write the character.</returns>
        public static uint TimeToWrite(char c)
        {
            return CharOverrides.ContainsKey(c) ? CharOverrides[c] : HumanReactionTime;
        }

        /// <summary>
        /// Returns the time (in milliseconds) it would take a human to
        /// write the specified string.
        /// </summary>
        /// <param name="s">The string to check.</param>
        /// <returns>Time (in milliseconds) it would take an average human
        /// to write the string.</returns>
        public static uint TimeToWrite(string s)
        {
            return s.Aggregate<char, uint>(0, (current, c) => current + TimeToWrite(c));
        }

        /// <summary>
        /// Encodes a dictionary to a URL-friendly format,
        /// {key}={value}[&{key}={value}...].
        /// </summary>
        /// <typeparam name="TKey">Type of the dictionary keys.</typeparam>
        /// <typeparam name="TValue">Type of the dictionary values.</typeparam>
        /// <param name="dict">Dictionary to encode.</param>
        /// <returns>The encoded key/value string.</returns>
        public static string EncodeDictToUrlParams<TKey, TValue>(IDictionary<TKey, TValue> dict)
        {
            var result = new StringBuilder();
            foreach (var key in dict.Keys)
            {
                var value = dict[key].ToString();
                if (result.Length > 0)
                    result.Append('&');
                result.Append(HttpUtility.UrlEncode(key.ToString()));
                result.Append('=');
                result.Append(HttpUtility.UrlEncode(value));
            }
            return result.ToString();
        }

        /// <summary>
        /// Creates an MD5 hash from the specified value.
        /// </summary>
        /// <param name="value">Value to hash.</param>
        /// <typeparam name="T">Type of the value.</typeparam>
        /// <returns>The MD5 hash of the value.</returns>
        public static string MD5<T>(T value)
        {
            return FormsAuthentication.HashPasswordForStoringInConfigFile(value.ToString(), "MD5");
        }

        /// <summary>
        /// Makes a post request to the specified URL with
        /// the provided parameters.
        /// </summary>
        /// <typeparam name="TKey">Key type of parameter dictionary.</typeparam>
        /// <typeparam name="TValue">Value type of parameter dictionary.</typeparam>
        /// <param name="url">URL to make the request to.</param>
        /// <param name="parameters">Parameters to send.</param>
        /// <exception cref="Exception">Throws exception if the response stream
        /// is null after making the request.</exception>
        /// <returns>The result of the request.</returns>
        public static string PostRequest<TKey, TValue>(string url, IDictionary<TKey, TValue> parameters)
        {
            var postParams = parameters.ToUrlParams();
            var data = Encoding.ASCII.GetBytes(postParams);

            var request = WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = data.Length;

            var requestStream = request.GetRequestStream();
            requestStream.Write(data, 0, data.Length);
            requestStream.Close();
            requestStream.Dispose();

            var response = request.GetResponse();
            var responseStream = response.GetResponseStream();
            if (responseStream == null)
                throw new NullReferenceException("Response stream from post request was null!");
            var responseReader = new StreamReader(responseStream);
            var result = responseReader.ReadToEnd().Trim();
            responseReader.Close();
            responseReader.Dispose();
            return result;
        }

        /// <summary>
        /// Returns the string at the specified index in the array.
        /// Returns an empty string if the index is invalid.
        /// </summary>
        /// <param name="arr">Array to search.</param>
        /// <param name="index">Index in array to get string at.</param>
        /// <returns>The string at the specified index of the array,
        /// or an empty string if the index was invalid.</returns>
        public static string StringAtIndex(string[] arr, int index)
        {
            return index >= arr.Length ? string.Empty : arr[index];
        }

        /// <summary>
        /// Searches the input using the specified expression.
        /// </summary>
        /// <param name="input">Input value to search.</param>
        /// <param name="expression">Expression to use.</param>
        /// <returns>The result of the search.</returns>
        /// <exception cref="Exception">Throws an exception if the node could not be found.</exception>
        public static string XPathSearch(string input, string expression)
        {
            var document = new XPathDocument(new MemoryStream(Encoding.ASCII.GetBytes(input)));
            var navigator = document.CreateNavigator();
            var node = navigator.SelectSingleNode(expression);
            if (node == null)
                throw new Exception("Node is null!");
            return node.Value;
        }
    }
}
