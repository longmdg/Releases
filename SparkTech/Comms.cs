using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;

namespace SparkTech
{
    public static class Comms
    {
        public static void Print1(string message) // to be expanded
        {
            Game.PrintChat(message);
        }

        public static void Print(string message, string color = Color.DodgerBlue)
        {
            Game.PrintChat("<font color='{0}'>{1}</font>", color, message);
        }
    }
}
