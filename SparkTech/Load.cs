namespace SparkTech
{
    using LeagueSharp;
    using LeagueSharp.Common;
    using System;
    using System.Net;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using System.Threading;

    public static class Load
    {
        private static bool summoned;

        static Load()
        {
            summoned = false;
        }

        public static void Library()
        {
            if (Game.Mode == GameMode.Running)
            {
                OnLoad();
            }
            else if (Game.Mode != GameMode.Running)
            {
                CustomEvents.Game.OnGameLoad += eventArgs =>
                {
                    OnLoad();
                };
            }
            else
            {
                Comms.Print(@"Error loading!", true);
            }
        }

        private static void OnLoad()
        {
            if (!summoned)
            {
                summoned = true;

                if (Settings.UpdateCheck)
                {
                    Utility.DelayAction.Add(Settings.UpdateCheckDelay, LibraryUpdateCheck);
                }

                Settings.Launch();
            }
            else
            {
                if (!Settings.SkipNoUpdate)
                {
                    Comms.Print("Error: Library already loaded!", true);
                }
            }
        }

        #region LibraryUpdateCheck

        // Credits to https://github.com/Hellsing/LeagueSharp/blob/master/Avoid/UpdateChecker.cs
        // Which was c+p'd too, in fact ¯\_(ツ)_/¯

        private static void LibraryUpdateCheck()
        {
            using (WebClient client = new WebClient())
            {
                new Thread(async () =>
                {
                    try
                    {
                        AssemblyName assemblyName = Assembly.GetExecutingAssembly().GetName();

                        if (Settings.Debug)
                        {
                            Game.PrintChat("Assembly Name: " + assemblyName);
                        }

                        string data =
                            await
                                // ReSharper disable once AccessToDisposedClosure
                                client.DownloadStringTaskAsync(
                                    "https://raw.github.com/Wiciaki/Releases/master/SparkTech/Properties/AssemblyInfo.cs");

                        System.Version version =
                            System.Version.Parse(
                                new Regex("AssemblyFileVersion\\((\"(.+?)\")\\)").Match(data).Groups[1].Value
                                    .Replace("\"", ""));

                        if (version == assemblyName.Version)
                        {
                            if (!Settings.SkipNoUpdate)
                            {
                                Comms.Print("You are using the latest version of [ST] library.");
                            }
                        }
                        else if (version != assemblyName.Version)
                        {
                            Game.PrintChat("Updated version of the [ST] lib is available: {1} => {2}",
                                assemblyName.Name,
                                assemblyName.Version,
                                version);
                        }
                        else
                        {
                            if (Settings.Debug)
                            {
                                Comms.Print("Checking for an update FAILED! (else)");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        if (Settings.Debug)
                        {
                            Comms.Print("Checking for an update FAILED! (ex)" + ex);
                        }
                    }
                }
                    ).Start();
            }
        }

        #endregion LibraryUpdateCheck

    }
}