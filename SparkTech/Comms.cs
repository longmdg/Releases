namespace SparkTech
{
    using System;
    using LeagueSharp;
    using LeagueSharp.Common;

    public static class Comms
    {
        // TODO expand, add menu items, checks, notifications and colors.
        public static void Print(string message, bool forceConsole = false) 
        {
            if (forceConsole)
            {
                Console.WriteLine(message);
            }
            else
            {
                Game.PrintChat(message);
            }
            
        }
    }
}