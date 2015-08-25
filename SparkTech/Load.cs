using System;
using LeagueSharp.Common;

namespace SparkTech
{
    public class Load
    {
        private static bool _summoned;

        static Load()
        {
            _summoned = false;
        } 

        public static void Library()
        {
            CustomEvents.Game.OnGameLoad += Loader;
        }

        private static void Loader(EventArgs args)
        {
            if (!_summoned)
            {
                _summoned = true;
                Settings.LoadStuff();
                Utility.DelayAction.Add(250, UpdateChecker.LibraryUpdatecheck);
            }

            else
            {
                Notifications.AddNotification("Error: Already Loaded!", 1000);
            }
        }
    }
}