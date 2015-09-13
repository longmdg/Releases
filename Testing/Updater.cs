﻿namespace Testing
{
    using System.Reflection;
    using LeagueSharp.Common;
    using SparkTech;

    internal class Updater
    {
        private static STUpdater updateChecker;

        internal static void Init()
        {
            if (Settings.UpdateCheck)
            {
                Utility.DelayAction.Add(Settings.UpdateCheckDelay, () =>
                {
                    updateChecker = new STUpdater(Assembly.GetExecutingAssembly().GetName().Name);
                    updateChecker.OnGetVersionCompleted += AssemblyUtilOnGetVersionCompleted;
                    updateChecker.GetLastVersionAsync();
                });
            }
        }

        private static void AssemblyUtilOnGetVersionCompleted(OnGetVersionCompletedArgs args)
        {
            if (args.LastAssemblyVersion == Assembly.GetExecutingAssembly().GetName().Version.ToString())
            {
                if (Settings.SkipNoUpdate)
                {
                    return;
                }
                Comms.Print("You have the latest version of \"" + Program.AssemblyName + "\"");
                return;
            }
            Comms.Print("A new version of \"" + Program.AssemblyName + "\" is available!");
        }
    }
}