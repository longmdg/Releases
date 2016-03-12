namespace SparkTech.Web
{
    using System;
    using System.Text.RegularExpressions;

    using SparkTech.EventData;

    /// <summary>
    /// The base class for the future updater engines
    /// </summary>
    public abstract class Updater
    {
        /// <summary>
        /// The regular expression used to match the downloaded data
        /// </summary>
        protected static readonly Regex AssemblyVersionRegex = new Regex(@"\[assembly\: AssemblyVersion\(""(\d+\.\d+\.\d+\.\d+)""\)\]", RegexOptions.Compiled | RegexOptions.CultureInvariant);

        /// <summary>
        /// The <see cref="Uri"/> representation of the user provided link
        /// </summary>
        protected readonly Uri Link;

        /// <summary>
        /// Determines whether the provided link was valid
        /// </summary>
        protected readonly bool IsLinkValid;

        /// <summary>
        /// The <see cref="E:CheckPerformed"/> invokator
        /// </summary>
        /// <param name="args">The provided data to be processed</param>
        protected void RaiseEvent(CheckPerformedEventArgs args) => CheckPerformed?.Invoke(args);

        /// <summary>
        /// Fired when the check has finished. Doesn't fire if there was no correct data provided.
        /// </summary>
        public event EventDataHandler<CheckPerformedEventArgs> CheckPerformed;

        /// <summary>
        /// Initializes a new instance of the <see cref="Updater"/> base class
        /// </summary>
        /// <param name="link">The <see cref="string"/> representation of an user-provided link</param>
        protected Updater(string link)
        {
            if (!(IsLinkValid = Utility.IsLinkValid(link, out Link)))
            {
                new ArgumentException("[ST] Updater - The provided link was invalid!").Catch();
            }
        }
    }
}