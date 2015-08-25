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
                Utility.DelayAction.Add(250, UpdateChecker.LibraryUpdateCheck);
                Settings.LoadStuff();
            }

            else
            {
                Comms.Print("Error: Already Loaded!");
            }
        }
    }
}