namespace SparkTech.Resources
{
    using System;
    using LeagueSharp;

    internal static class Comms
    {
        // TODO expand, add menu items, checks, notifications and colors.
        internal static void Print(string msg, bool debug = false)
        {
            if (debug)
            {
                Console.WriteLine(msg);
            }
            else
            {
                Game.PrintChat(msg);
            }
        }

        internal static void Log(string error, string cName, Exception ex)
        {
            Print(Environment.NewLine + cName + " - " + error + Environment.NewLine + ex, true);
        }
    }
}
