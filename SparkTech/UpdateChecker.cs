using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using LeagueSharp;
using LeagueSharp.Common;
using Version = System.Version;

//copy pasterino a bit
//https://github.com/Hellsing/LeagueSharp/blob/master/Avoid/UpdateChecker.cs

namespace SparkTech
{
    public class UpdateChecker
    {
        internal static void LibraryUpdatecheck()
        {
            using (var client = new WebClient())
            {
                Utility.DelayAction.Add(2500, () =>

                    new Thread(async () =>
                    {
                        try
                        {
                            // will this return Testing's name or SparkTech's?
                            var assemblyName = Assembly.GetExecutingAssembly().GetName();
                            Game.PrintChat(assemblyName.ToString());

                            var data =
                                await
                                    // ReSharper disable once AccessToDisposedClosure
                                    client.DownloadStringTaskAsync(
                                        string.Format(
                                            "https://raw.github.com/Wiciaki/Releases/master/SparkTech/Properties/AssemblyInfo.cs"));

                            var version =
                                Version.Parse(new Regex("AssemblyFileVersion\\((\"(.+?)\")\\)").Match(data).Groups[1]
                                    .Value.Replace(
                                        "\"", ""));

                            Game.PrintChat(version.ToString());

                            if (version > assemblyName.Version)
                            {
                                Game.PrintChat("Library update available: {1} => {2}!",
                                    assemblyName.Name,
                                    assemblyName.Version,
                                    version);
                            }
                        }
                        catch
                        {
                            // ignored
                        }
                    }
                        ).Start());
            }
        }
    }
}