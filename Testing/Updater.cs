using System.Reflection;
using LeagueSharp.Common;
using SparkTech;

namespace Testing
{
    internal class Updater
    {
        private static UpdateChecker updateChecker;

        internal static void Init()
        {
            if (Settings.UpdateCheck)
            {
                Utility.DelayAction.Add(Settings.UpdateCheckDelay, () =>
                {
                    updateChecker = new UpdateChecker(Assembly.GetExecutingAssembly().GetName().Name);
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