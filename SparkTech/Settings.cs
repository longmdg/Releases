namespace SparkTech
{
    using System;
    using LeagueSharp;
    using LeagueSharp.Common;
    using System.Diagnostics.CodeAnalysis;

    [SuppressMessage("ReSharper", "ConvertPropertyToExpressionBody")] // TODO: Reenable when L# supports .NET 4.6

    public static class Settings
    {
        internal static bool Debug = true;

        public static bool SkipNoUpdate = false;
        public static bool UpdateCheck = true;
        public static int UpdateCheckDelay = 250;

        private static float spaghettiLimiter;

        internal static bool LoadZoomHack
        {
            get { return STMenu.LibraryMenu.Item("hack").GetValue<bool>(); }
        }

        internal static void OnSettingsChange(EventArgs args)
        {
            if (Environment.TickCount - spaghettiLimiter < STMenu.LibraryMenu.Item("onupdatedelay").GetValue<Slider>().Value)
            {
                return;
            }
            spaghettiLimiter = Environment.TickCount;
        }


        /*

F5Settings -> Default Settings?
F5Settings -> Subsciption Manager
F5Settings -> Target Selector
F5Settings -> Orbwalker
F5Settings -> Prediction

Messaging -> Message output (PrintChat, Notifs, Console)
Messaging -> Delay / Time

Updater -> Check for updates
Updater -> Display the message only if update found
Updater -> Update delay

more 2 come

*/

        /*

        Code

        */
    }
}