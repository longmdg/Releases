using System.Reflection;
using LeagueSharp.Common;
using SparkTech;

namespace ZoomHack
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
                    _updateChecker = new UpdateChecker(Assembly.GetExecutingAssembly().GetName().Name);
                    _updateChecker.onGetVersionCompleted += AssemblyUtil_onGetVersionCompleted;
                    _updateChecker.GetLastVersionAsync();
                });
            }
        }

        private static void AssemblyUtil_onGetVersionCompleted(OnGetVersionCompletedArgs args)
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
