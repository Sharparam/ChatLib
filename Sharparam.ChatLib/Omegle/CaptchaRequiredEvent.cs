using System;

namespace Sharparam.ChatLib.Omegle
{
    /// <summary>
    /// EventArgs for the CaptchaRequired event.
    /// </summary>
    public class CaptchaRequiredEventArgs : EventArgs
    {
        /// <summary>
        /// ID of the reCAPTCHA.
        /// </summary>
        /// <remarks>
        /// Construct URL per:
        /// http://www.google.com/recaptcha/api/challenge?k={ID}&ajax=1&cachestop=0.7569315146943529
        /// </remarks>
        public readonly string Id;

        internal CaptchaRequiredEventArgs(string id)
        {
            Id = id;
        }
    }

    /// <summary>
    /// Event handler delegate for the CaptchaRequired event.
    /// </summary>
    /// <param name="sender">The object that raised the event.</param>
    /// <param name="args">Event arguments.</param>
    public delegate void CaptchaRequiredEventHandler(object sender, CaptchaRequiredEventArgs args);
}
