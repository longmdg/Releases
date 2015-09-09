namespace SparkTech
{
    using System;
    using LeagueSharp;
    using LeagueSharp.Common;

    public static class Comms
    {
        public static void Print(string message, bool forceconsole = false) // to be expanded.
        {
            if (forceconsole)
            {
                Console.WriteLine(message);
                return;
            }

            Game.PrintChat(message);
        }
    }
}