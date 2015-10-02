// Taken from https://github.com/Hellsing/LeagueSharp/blob/master/Avoid/UpdateChecker.cs
// Which was c+p'd too, in fact ¯\_(ツ)_/¯

namespace SparkTech.Resources
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Net;
    using System.Reflection;
    using System.Text.RegularExpressions;

    [SuppressMessage("ReSharper", "UseStringInterpolation")]
    public static class Updater
    {
        public static async void Check(string gitName)
        {
            if (Settings.UpdateCheck)
                try
                {
                    using (WebClient client = new WebClient())
                    {
                        AssemblyName assemblyName = Assembly.GetExecutingAssembly().GetName();
                        Comms.Print(string.Format("Assembly Name:\n{0}", assemblyName), true);
                        string data = await client.DownloadStringTaskAsync(string.Format("https://raw.github.com/Wiciaki/Releases/master/{0}/Properties/AssemblyInfo.cs", gitName));
                        Version version = Version.Parse(new Regex("AssemblyFileVersion\\((\"(.+?)\")\\)").Match(data).Groups[2].Value);
                        if (version == assemblyName.Version && !Settings.SkipNoUpdate)
                            Comms.Print(gitName == "SparkTech" ? "Your spaghetti sauce is the hottest! (There's no update available)" : string.Format("You are using the latest version of {0}", gitName));
                        else if (version != assemblyName.Version)
                            Comms.Print(gitName == "SparkTech" ? string.Format("A new spaghetti sauce is available: {0} => {1}", assemblyName.Version, version) : string.Format("{2} - a new version is available: {0} => {1}", assemblyName.Version, version, gitName));
                    }
                }
                catch (Exception ex)
                {
                    Comms.Log("Checking for an update failed", "Updater.cs", ex);
                }
            else
                Comms.Print(string.Format("{0} - Updating is off!", gitName), true);
        }
    }
}