// Taken (yet modified a lot) from https://github.com/Hellsing/LeagueSharp/blob/master/Avoid/UpdateChecker.cs
// Which was c+p'd too, in fact ¯\_(ツ)_/¯

namespace SparkTech.Resources
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Net;
    using System.Reflection;
    using System.Text.RegularExpressions;

    //TODO: Get rid of that!
    [SuppressMessage("ReSharper", "UseStringInterpolation")]

    public static class Updater
    {
        static Updater()
        {
            Comms.Print(Settings.UpdateCheck ? "[ST] - Updating is ON!" : "[ST] - Updating is OFF!", true);
        }

        public static async void Check(string gitName)
        {
            try
            {
                var assemblyName = Assembly.GetCallingAssembly().GetName();

                Comms.Print("Assembly name: " + assemblyName.Name, true);

                if (!Settings.UpdateCheck)
                {
                    return;
                }

                using (var client = new WebClient())
                {

                    var data = await client.DownloadStringTaskAsync(string.Format("https://raw.github.com/Wiciaki/Releases/master/SparkTech/{0}/Properties/AssemblyInfo.cs", gitName));
                    var version = Version.Parse(new Regex("AssemblyVersion\\((\"(.+?)\")\\)").Match(data).Groups[2].Value);
                    if (version == assemblyName.Version)
                    {
                        if (!Settings.SkipNoUpdate)
                        {
                            Comms.Print(gitName == "SparkTech" ? "Your spaghetti sauce is the hottest!" : string.Format("You are using the latest version of \"{0}\"", gitName));
                        }
                    }
                    else
                    {
                        Comms.Print(gitName == "SparkTech" ? string.Format("A new spaghetti sauce is available: {0} => {1}", assemblyName.Version, version) : string.Format("\"{2}\" - a new version is available: {0} => {1}", assemblyName.Version, version, gitName));
                    }
                }
            }
            catch (Exception ex)
            {
                Comms.Print("Checking for an update failed!\n" + ex, true);
            }
        }
    }
}