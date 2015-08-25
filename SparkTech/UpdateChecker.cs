using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using LeagueSharp;

//copy pasterino from https://github.com/Hellsing/LeagueSharp/blob/master/Avoid/UpdateChecker.cs

namespace SparkTech
{
    public class UpdateChecker
    {
        internal static void LibraryUpdateCheck()
        {
            using (var client = new WebClient())
            {
                new Thread(async () =>
                {
                    try
                    {
                        var assemblyName = Assembly.GetExecutingAssembly().GetName();

                        var data =
                            await
                                // ReSharper disable once AccessToDisposedClosure
                                client.DownloadStringTaskAsync(
                                    string.Format(
                                        "https://raw.github.com/Wiciaki/Releases/master/SparkTech/Properties/AssemblyInfo.cs"));

                        //dumb code warning
                        var version = System.Version.Parse(new Regex("AssemblyFileVersion\\((\"(.+?)\")\\)").Match(data).Groups[1].Value.Replace("\"", ""));
                        //end dumb code warning

                        Game.PrintChat(version.ToString());

                        if (version > assemblyName.Version)
                        {
                            Game.PrintChat("Updated version of the library is available: {1} => {2}",
                                assemblyName.Name,
                                assemblyName.Version,
                                version);
                        }
                        else
                        {
                            Game.PrintChat("Nice meme!");
                        }
                    }
                    catch
                    {
                        // ignored
                    }
                }
                    ).Start();
            }
        }
    }
}