using System;
using LeagueSharp.Common;

namespace SparkTech
{
    public class Load
    {
        internal static bool Summoned;

        public static void Library()
        {
            CustomEvents.Game.OnGameLoad += Loader;
        }

        private static void Loader(EventArgs args)
        {
            Lib.Load();
        }
    }
}

internal class Lib
{
    static Lib()
    {
        SparkTech.Load.Summoned = false;
    }

    internal static void Load()
    {
        if (!SparkTech.Load.Summoned)
        {
            SparkTech.Settings.LoadStuff();
            SparkTech.Load.Summoned = true;
        }

        else
        {
            Notifications.AddNotification("Error: Already Loaded!");
            //SparkTech.Comms.Print("Error: Already Loaded!");
        }
    }
}