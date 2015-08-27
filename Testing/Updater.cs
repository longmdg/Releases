using System.Reflection;
using LeagueSharp.Common;
using SparkTech;

namespace Testing
{
    class Updater
    {
        private static UpdateChecker _updateChecker;

        internal static void Init()
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
        }

        internal static void AssemblyUtil_onGetVersionCompleted(OnGetVersionCompletedArgs args)
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
