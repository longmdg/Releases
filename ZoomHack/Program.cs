using LeagueSharp.Common;

namespace ZoomHack
{
    internal static class Program
    {
        internal const string AssemblyName = "ZoomHack";

        private static void Main()
        {
            Updater.Init();
            SparkTech.Hack.LoadZoomHackStandalone();
        }
    }
}
