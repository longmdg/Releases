namespace SparkTech.Web
{
    using System;

    /// <summary>
    /// Provides additional utility related to the web client
    /// </summary>
    public static class Utility
    {
        /// <summary>
        /// Determines whether the input link is a valid link as well as creates an <see cref="Uri"/> out of it
        /// </summary>
        /// <param name="link">The link to be inspected</param>
        /// <param name="u">The output <see cref="Uri"/></param>
        /// <param name="assinfoValidation">Allows you to validate if the link is a valid raw assembly info link, too</param>
        /// <returns></returns>
        public static bool IsLinkValid(string link, out Uri u, bool assinfoValidation = true)
        {
            if (string.IsNullOrWhiteSpace(link))
            {
                u = null;
                return false;
            }

            if (!Uri.TryCreate(link, UriKind.Absolute, out u) || u.Scheme != Uri.UriSchemeHttp && u.Scheme != Uri.UriSchemeHttps)
            {
                return false;
            }

            if (!assinfoValidation)
            {
                return true;
            }

            var low = link.ToLower();

            return low.Contains("assemblyinfo.cs") && low.Contains("raw.githubusercontent.com");
        }

        /// <summary>
        /// Determines whether the input link is a valid link
        /// </summary>
        /// <param name="link">The link to be inspected</param>
        /// <param name="assinfoValidation">Allows you to validate if the link is a valid assembly info link, too</param>
        /// <returns></returns>
        public static bool IsLinkValid(string link, bool assinfoValidation = true)
        {
            Uri dump;
            return IsLinkValid(link, out dump, assinfoValidation);
        }
    }
}