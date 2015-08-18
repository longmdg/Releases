using System;
using System.Diagnostics.CodeAnalysis;
using LeagueSharp.Common;

namespace SparkTech
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class Load
    {
        private static bool _summoned;
        internal static bool MLGloaded;

        static Load()
        {
            _summoned = false;
        } 

        public static void Library(bool MLGload = false)
        {
            if (MLGload)
            {
                MLGloaded = true;
            }

            CustomEvents.Game.OnGameLoad += Loader;
        }

        private static void Loader(EventArgs args)
        {
            if (!_summoned)
            {
                _summoned = true;
                Settings.LoadStuff();
            }

            else
            {
                Notifications.AddNotification("Error: Already Loaded!", 1000);
                //SparkTech.Comms.Print("Error: Already Loaded!");
            }
        }
    }
}