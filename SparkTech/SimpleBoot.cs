using System;
using LeagueSharp;
using LeagueSharp.Common;

namespace SparkTech
{
    class SimpleBoot
    {
        public static bool MenuLoaded;

        public static void TempName1()
        {
            CustomEvents.Game.OnGameLoad += Tracker;
        }

        private static void Tracker(EventArgs args)
        {
            MenuLoaded = false;

            if (Settings.MustDebug)
            {
                Game.PrintChat("Set MenuLoaded to false");
            }
        }

        public static void LoadLibrary()
        {
            if (MenuLoaded)
            {
                if (Settings.MustDebug)
                {
                   Notifications.AddNotification("Multiload!", 2000);
                }
            }
            else
            {
                MenuLoaded = true;
                Settings.LoadStuff();
            }
        }
    }
}
