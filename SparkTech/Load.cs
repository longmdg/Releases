using System;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;

using LeagueSharp;
using LeagueSharp.Common;

namespace SparkTech
{
    public class Load
    {
        private static bool _summoned;

        static Load()
        {
            _summoned = false;
        }

        public static void Library()
        {
            CustomEvents.Game.OnGameLoad += Loader;
        }

        private static void Loader(EventArgs args)
        {
            if (!_summoned)
            {
                _summoned = true;
                Utility.DelayAction.Add(250, LibraryUpdateCheck);
                Settings.LoadStuff();
            }

            else
            {
                Comms.Print("Error: Library already loaded!");
            }
        }

        //partial copy pasterino from https://github.com/Hellsing/LeagueSharp/blob/master/Avoid/UpdateChecker.cs

        private static void LibraryUpdateCheck()
        {
            using (var client = new WebClient())
            {
                new Thread(async () =>
                {
                    try
                    {
                        var assemblyName = Assembly.GetExecutingAssembly().GetName();

                        if (Settings.Debug)
                        {
                            Game.PrintChat("Assembly Name: " + assemblyName);
                        }

                        var data =
                            await
                                // ReSharper disable once AccessToDisposedClosure
                                client.DownloadStringTaskAsync(
                                    "https://raw.github.com/Wiciaki/Releases/master/SparkTech/Properties/AssemblyInfo.cs");

                        var version =
                            System.Version.Parse(
                                new Regex("AssemblyFileVersion\\((\"(.+?)\")\\)").Match(data).Groups[1].Value
                                    .Replace("\"", ""));

                        if (version == assemblyName.Version)
                        {
                            Comms.Print("You are running the latest version of the library");
                        }
                        else if (version > assemblyName.Version)
                        {
                            Game.PrintChat("Updated version of the library is available: {1} => {2}",
                                assemblyName.Name,
                                assemblyName.Version,
                                version);
                        }
                        else
                        {
                            Comms.Print("Checking for an update FAILED! (else)");
                        }
                    }
                    catch (Exception ex)
                    {
                        Comms.Print("Checking for an update FAILED! (ex) " + ex);
                    }
                }
                    ).Start();
            }
        }
    }
}