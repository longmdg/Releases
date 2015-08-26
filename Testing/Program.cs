using System;
using System.Linq;
using System.Reflection;

using LeagueSharp;
using LeagueSharp.Common;
using SparkTech;

namespace Testing
{
    internal class Program
    {
        private static AssemblyUtil _assemblyUtil;
        private const string AssemblyName = "Testing";

        private static void Main()
        {
            if (Settings.UpdateCheck)
            {
                Utility.DelayAction.Add(Settings.UpdateCheckDelay, () =>
                {
                    try
                    {
                        _assemblyUtil = new AssemblyUtil(Assembly.GetExecutingAssembly().GetName().Name);
                        _assemblyUtil.onGetVersionCompleted += AssemblyUtil_onGetVersionCompleted;
                        _assemblyUtil.GetLastVersionAsync();
                    }
                        // ReSharper disable once EmptyGeneralCatchClause
                    catch { }
                });
            }

            Load.Library();
        }

        private static void AssemblyUtil_onGetVersionCompleted(OnGetVersionCompletedArgs args)
        {
            // Comms.Print(args.LastAssemblyVersion == Assembly.GetExecutingAssembly().GetName().Version.ToString()
            //    ? string.Format("You have the latest version of \"" + AssemblyName + "\"")
            //    : string.Format("A new version of \"" + AssemblyName + "\" is available!"));
            if (args.LastAssemblyVersion == Assembly.GetExecutingAssembly().GetName().Version.ToString())
                Game.PrintChat(string.Format("<font color='#fb762d'>DevAnnie :: You have the latest version.</font>"));
            else
                Game.PrintChat(string.Format("<font color='#fb762d'>DevAnnie :: NEW VERSION available! Tap F8 for Update! {0}</font>", args.LastAssemblyVersion));
        }
    }
}
