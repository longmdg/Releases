#region Credits

/*

                                          Taken from https://github.com/Hellsing/LeagueSharp/blob/master/Avoid/UpdateChecker.cs
                                                                 Which was c+p'd too, in fact ¯\_(ツ)_/¯

*/

#endregion

namespace SparkTech
{
    using System;
    using System.Net;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using System.Threading;

    // TODO: Remove when .NET 4.6 / new loader arrives
    using System.Diagnostics.CodeAnalysis;

    [SuppressMessage("ReSharper", "UseStringInterpolation")]

    public static class Updater
    {
        public static void Check(string gitName)
        {
            if (Settings.UpdateCheck)
            {
                try
                {
                    using (WebClient client = new WebClient())
                    {
                        new Thread(
                            async () =>
                                {
                                    AssemblyName assemblyName = Assembly.GetExecutingAssembly().GetName();
                                    Comms.Print("Assembly Name: " + Environment.NewLine + assemblyName, true);
                                    // ReSharper disable once AccessToDisposedClosure
                                    string data =
                                        await
                                        client.DownloadStringTaskAsync(
                                            string.Format(
                                                "https://raw.github.com/Wiciaki/Releases/master/{0}/Properties/AssemblyInfo.cs",
                                                gitName));
                                    Version version =
                                        Version.Parse(
                                            new Regex("AssemblyFileVersion\\((\"(.+?)\")\\)").Match(data).Groups[1]
                                                .Value.Replace("\"", ""));
                                    if (version == assemblyName.Version)
                                    {
                                        if (Settings.SkipNoUpdate)
                                        {
                                            return;
                                        }
                                        Comms.Print(
                                            gitName == "SparkTech"
                                                ? "You are using the latest version of the L# Spaghetti."
                                                : string.Format("You are using the latest version of {0}", gitName));
                                    }
                                    else if (version != assemblyName.Version)
                                    {
                                        Comms.Print(
                                            gitName == "SparkTech"
                                                ? string.Format(
                                                    "A new spaghetti sauce is available: {0} => {1}",
                                                    assemblyName.Version,
                                                    version)
                                                : string.Format(
                                                    "{2} - a new version is available: {0} => {1}",
                                                    assemblyName.Version,
                                                    version,
                                                    gitName));
                                    }
                                    else
                                    {
                                        Comms.Print(string.Format("{0} - Checking for an update FAILED!", gitName));
                                    }
                                }).Start();
                    }
                }
                catch (Exception e)
                {
                    Comms.Print(
                        string.Format(
                            "{0} - Checking for an update FAILED! Exception: " + Environment.NewLine + e,
                            gitName),
                        true);
                }
            }
            else
            {
                Comms.Print(string.Format("{0} - Updating is off!", gitName), true);
            }
        }
    }
}