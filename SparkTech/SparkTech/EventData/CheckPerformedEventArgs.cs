namespace SparkTech.EventData
{
    using System;
    using System.Reflection;

    using SparkTech.Enumerations;
    using SparkTech.Helpers;

    /// <summary>
    /// The instance containing event data regarding any of the updaters
    /// </summary>
    public class CheckPerformedEventArgs : EventArgs
    {
        /// <summary>
        /// This method prints the default check with respect to your assembly name, MessageLanguage and individual user settings
        /// </summary>
        public void Notify()
        {
            if (notified)
                return;

            notified = true;

            // ex. "You are using the latest version of [NAME]"
            Comms.Print(
                LanguageData.GetTranslation(
                    ("updater_" + (Success ? GitVersion == LocalVersion ? "updated" : "available" : "error")).Replace(
                        "[NAME]",
                        aName),
                    MessageLanguage));
        }

        /// <summary>
        /// The cloud <see cref="Version"/> of the assembly
        /// </summary>
        public readonly Version GitVersion;

        /// <summary>
        /// The local <see cref="Version"/> of the assembly
        /// </summary>
        public readonly Version LocalVersion;

        /// <summary>
        /// Determines whether the check was successful
        /// </summary>
        public readonly bool Success;

        /// <summary>
        /// The message <see cref="Language"/>
        /// </summary>
        public Language MessageLanguage = Core.ActiveLanguage;

        /// <summary>
        /// The locally saved assembly name
        /// </summary>
        private readonly string aName;

        /// <summary>
        /// Inidicates whether the check has already been printed
        /// </summary>
        private bool notified;

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckPerformedEventArgs"/> class by a cloud <see cref="Version"/> and an <see cref="Assembly"/>
        /// </summary>
        /// <param name="gitVersion">The version of the assembly on the cloud</param>
        /// <param name="callingAssembly">The calling assembly</param>
        internal CheckPerformedEventArgs(Version gitVersion, Assembly callingAssembly)
        {
            var name = callingAssembly.GetName();

            LocalVersion = name.Version;

            aName = name.Name;

            GitVersion = gitVersion;

            Success = gitVersion != null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckPerformedEventArgs"/> class by a cloud <see cref="Version"/>, a local <see cref="Version"/> and a assembly name
        /// </summary>
        /// <param name="gitVersion">The <see cref="Version"/> of the assembly on the cloud</param>
        /// <param name="localVersion">The local <see cref="Version"/> of the assembly</param>
        /// <param name="assemblyName">The assembly name</param>
        internal CheckPerformedEventArgs(Version gitVersion, Version localVersion, string assemblyName)
        {
            LocalVersion = localVersion;

            aName = assemblyName;

            GitVersion = gitVersion;

            Success = gitVersion != null;
        }
    }
}