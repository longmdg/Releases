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
        private static UpdateChecker _updateChecker;
        private const string AssemblyName = "Testing";

        private static void Main()
        {
            if (Settings.UpdateCheck)
            {
                Utility.DelayAction.Add(Settings.UpdateCheckDelay, () =>
                {
                    try
                    {
                        _updateChecker = new UpdateChecker(Assembly.GetExecutingAssembly().GetName().Name);
                        _updateChecker.onGetVersionCompleted += AssemblyUtil_onGetVersionCompleted;
                        _updateChecker.GetLastVersionAsync();
                    }
                        // ReSharper disable once EmptyGeneralCatchClause
                    catch { }
                });
            }
            Load.Library();
        }

        private static void AssemblyUtil_onGetVersionCompleted(OnGetVersionCompletedArgs args)
        {
            if (args.LastAssemblyVersion == Assembly.GetExecutingAssembly().GetName().Version.ToString() && !Settings.SkipNoUpdate)
            {
                if (!Settings.SkipNoUpdate)
                {
                    Comms.Print("You have the latest version of \"" + AssemblyName + "\"");
                }
            }
            else
            {
                Comms.Print("A new version of \"" + AssemblyName + "\" is available!");
            }
        }
    }
}
