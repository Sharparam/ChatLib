using System.Collections.Generic;

namespace Sharparam.ChatLib
{
    /// <summary>
    /// Provides extension methods for various system types.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Returns the time (in milliseconds) it would take a human to
        /// write the specified character.
        /// </summary>
        /// <param name="c">The character to check.</param>
        /// <returns>Time (in milliseconds) it would take an average human
        /// to write the character.</returns>
        public static uint TimeToWrite(this char c)
        {
            return Utils.TimeToWrite(c);
        }

        /// <summary>
        /// Returns the time (in milliseconds) it would take a human to
        /// write the specified string.
        /// </summary>
        /// <param name="s">The string to check.</param>
        /// <returns>Time (in milliseconds) it would take an average human
        /// to write the string.</returns>
        public static uint TimeToWrite(this string s)
        {
            return Utils.TimeToWrite(s);
        }

        /// <summary>
        /// Encodes a dictionary to a URL-friendly format,
        /// {key}={value}[&{key}={value}...].
        /// </summary>
        /// <typeparam name="TKey">Type of the dictionary keys.</typeparam>
        /// <typeparam name="TValue">Type of the dictionary values.</typeparam>
        /// <param name="dict">Dictionary to encode.</param>
        /// <returns>The encoded key/value string.</returns>
        public static string ToUrlParams<TKey, TValue>(this IDictionary<TKey, TValue> dict)
        {
            return Utils.EncodeDictToUrlParams(dict);
        }

        /// <summary>
        /// Creates an MD5 hash from the specified value.
        /// </summary>
        /// <param name="value">Value to hash.</param>
        /// <typeparam name="T">Type of the value.</typeparam>
        /// <returns>The MD5 hash of the value.</returns>
        public static string ToMD5<T>(this T value)
        {
            return Utils.MD5(value);
        }

        /// <summary>
        /// Returns the string at the specified index in the array.
        /// Returns an empty string if the index is invalid.
        /// </summary>
        /// <param name="arr">Array to search.</param>
        /// <param name="index">Index in array to get string at.</param>
        /// <returns>The string at the specified index of the array,
        /// or an empty string if the index was invalid.</returns>
        public static string AtIndex(this string[] arr, int index)
        {
            return Utils.StringAtIndex(arr, index);
        }
    }
}
