namespace STAutoPlay
{
    using System;

    using LeagueSharp;
    using LeagueSharp.Common;

    internal class Program : Core
    {
        private static void Main(string[] args)
        {
            if (args == null)
            {
                Console.WriteLine(new ArgumentNullException(nameof(args)));
            }

            CustomEvents.Game.OnGameLoad += OnLoadNative;
            Game.OnStart += OnStartNative;
        }
    }
}