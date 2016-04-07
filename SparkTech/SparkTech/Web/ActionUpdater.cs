namespace SparkTech.Web
{
    using System;
    using System.Net;
    using System.Reflection;

    using SparkTech.EventData;

    /// <summary>
    /// <para>A child of the <see cref="Updater"/> class, uses a method which makes it change a thread, unfortunately (leading to possible bugsplats on drawings drawn in event method etc.)</para>
    /// All of the invokable methods exposed by me are knows to be thread-safe and here I mean mostly "args.Notify()"
    /// </summary>
    public class ActionUpdater : Updater
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ActionUpdater"/> class with the specified link and a calling <see cref="Assembly"/>
        /// </summary>
        /// <param name="fullRawAssemblyInfoLink">he link to the assemblyinfo.cs file that can be found in your github. This has to be a raw link</param>
        /// <param name="callingAssembly">The <see cref="Assembly"/> you are retrieving data for</param>
        public ActionUpdater(string fullRawAssemblyInfoLink, Assembly callingAssembly) : this(fullRawAssemblyInfoLink, callingAssembly.GetName().Version, callingAssembly.GetName().Name)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ActionUpdater"/> class
        /// </summary>
        /// <param name="fullRawAssemblyInfoLink">The link to the assemblyinfo.cs file that can be found in your github. This has to be a raw link</param>
        /// <param name="localVersion">The local version of the assembly to compare</param>
        /// <param name="assemblyName">The name of the <see cref="Assembly"/></param>
        public ActionUpdater(string fullRawAssemblyInfoLink, Version localVersion, string assemblyName) : base(fullRawAssemblyInfoLink)
        {
            // Don't try to download or invoke anything if the developer couldn't even provide a proper link...
            if (!IsLinkValid)
            {
                return;
            }


            new Action(
                async delegate
                {
                    using (var client = new WebClient())
                    {
                        var match = AssemblyVersionRegex.Match(await client.DownloadStringTaskAsync(Link));

                        RaiseEvent(
                            new CheckPerformedEventArgs(
                                match.Success ? new Version(match.Groups[1].Value) : null,
                                localVersion,
                                assemblyName));
                    }
                }).Invoke();
        }
    }
}