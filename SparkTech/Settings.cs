namespace SparkTech
{
    using System;
    using LeagueSharp;
    using LeagueSharp.Common;

    public static class Settings
    {
        private static float spaghettiLimiter;

        // F5Settings
        internal static bool LoadZoomHack;

        // Update
        internal static bool SkipNoUpdate = false;
        internal static bool UpdateCheck = true;
        internal static int UpdateCheckDelay = 250;

        // Cheats
        internal static bool ChValue1;
        internal static bool ChValue2;


        internal static void OnSettingsChange(EventArgs args)
        {
            if (Environment.TickCount - spaghettiLimiter < STMenu.LibraryMenu.Item("onupdatedelay").GetValue<Slider>().Value)
            {
                return;
            }
            spaghettiLimiter = Environment.TickCount;
        }
    }
}