namespace SparkTech.Resources
{
    using System;

    public static class Comms
    {
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