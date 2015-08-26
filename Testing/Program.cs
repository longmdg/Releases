using LeagueSharp;
using LeagueSharp.Common;

using System;
using System.Linq;
using System.Reflection;

using SparkTech;

namespace Testing
{
    class Program
    {
        static AssemblyUtil _assemblyUtil;

        static void Main()
        {
            Load.Library();

            try
            {
                _assemblyUtil = new AssemblyUtil(Assembly.GetExecutingAssembly().GetName().Name);
                _assemblyUtil.onGetVersionCompleted += AssemblyUtil_onGetVersionCompleted;
                _assemblyUtil.GetLastVersionAsync();
            }
            catch
            {
                Comms.Print("Error 1");
            }
        }

        static void AssemblyUtil_onGetVersionCompleted(OnGetVersionCompletedArgs args)
        {
            Game.PrintChat(args.LastAssemblyVersion == Assembly.GetExecutingAssembly().GetName().Version.ToString()
                ? string.Format("<font color='#fb762d'>Testing :: You have the latest version.</font>")
                : string.Format(
                    "<font color='#fb762d'>Testing :: NEW VERSION available! Tap F8 for Update! {0}</font>",
                    args.LastAssemblyVersion));
        }
    }
}
