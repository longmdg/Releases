using System;
using LeagueSharp;
using LeagueSharp.Common;

namespace SparkTech
{
    class SimpleBoot
    {
        internal static bool Summoned;

        public static void LoadLibrary()
        {
            CustomEvents.Game.OnGameLoad += Loader;
        }

        private static void Loader(EventArgs args)
        {
            Lib.Load();
        }
    }
}

class Lib
{
    static Lib()
    {
        SparkTech.SimpleBoot.Summoned = false;
    }

    public static void Load()
    {
        if (!SparkTech.SimpleBoot.Summoned)
        {
            SparkTech.Settings.LoadStuff();
        }

        else
        {
            SparkTech.Comms.Print("Error: Already Loaded!");
        }
    }
}