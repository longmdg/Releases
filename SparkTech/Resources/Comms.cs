namespace SparkTech.Resources
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    [SuppressMessage("ReSharper", "UseStringInterpolation")]
    public static class Comms
    {
        public static void Log(string error, string cName, Exception ex)
        {
            Print(string.Format("{0} - {1}{2}{3}", cName, error, Environment.NewLine, ex), true);
        }

        // TODO expand, add menu items, checks, notifications and colors.
        public static void Print(string msg, bool debug = false)
        {
            if (debug)
            {
                Console.WriteLine(msg);
            }
            else
            {
                LeagueSharp.Game.PrintChat(msg);
            }
        }
    }
}
