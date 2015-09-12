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
            Comms.Print("Static constructor of the SparkTech Load.cs called", true);
            summoned = false;
        }

        public static void Library()
        {
            if (Game.Mode == GameMode.Running)
            {
                Comms.Print("SparkTech - Load.cs - Fired the first library init option", true);
                try
                {
                    Summoner(new EventArgs());
                }
                catch (Exception ex)
                {
                    Comms.Print("SparkTech - Load.cs - Error: Couldn't sign an event!", true);
                    Comms.Print("Error: "+ ex);
                }
            }
            else if (Game.Mode != GameMode.Running)
            {
                Comms.Print("SparkTech - Load.cs - Fired the second library init option", true);
                try
                {
                    CustomEvents.Game.OnGameLoad += Summoner;
                }
                catch (Exception ex)
                {
                    Comms.Print("SparkTech - Load.cs - Error: Couldn't sign an event!", true);
                    Comms.Print("Error: " + ex);
                }
            }
            else
            {
                Comms.Print("SparkTech - Load.cs - Error: No boot options matching!", true);
            }
        }

        private static void Summoner(EventArgs args)
        {
            if (!summoned)
            {
                summoned = true;
                Utility.DelayAction.Add(2000, LibraryUpdateCheck);
                Settings.Fire();
                return;
            }
            Comms.Print("Error: Library already loaded!", true);
        }

        #region LibraryUpdateCheck

        // Taken from https://github.com/Hellsing/LeagueSharp/blob/master/Avoid/UpdateChecker.cs
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
                        Comms.Print("Assembly Name: " + assemblyName, true);
                        // ReSharper disable once AccessToDisposedClosure
                        string data = await client.DownloadStringTaskAsync("https://raw.github.com/Wiciaki/Releases/master/SparkTech/Properties/AssemblyInfo.cs");
                        System.Version version = System.Version.Parse(new Regex("AssemblyFileVersion\\((\"(.+?)\")\\)").Match(data).Groups[1].Value.Replace("\"", ""));
                        if (version == assemblyName.Version)
                        {
                            if (!Settings.SkipNoUpdate)
                            {
                                Comms.Print("You are using the latest version of [ST] library.");
                            }
                        }
                        else if (version != assemblyName.Version)
                        {
                            Game.PrintChat(
                                "A new spaghetti sauce is available: {1} => {2}",
                                assemblyName.Name,
                                assemblyName.Version,
                                version);
                        }
                        else
                        {
                            Comms.Print("Checking for an update FAILED! (else)", true);
                        }
                    }
                    catch (Exception ex)
                    {
                        Comms.Print("Checking for an update FAILED! Exception: " + ex, true);
                    }
                }
                    ).Start();
            }
        }

        #endregion LibraryUpdateCheck

    }
}