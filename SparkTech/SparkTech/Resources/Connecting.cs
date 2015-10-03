using System;

using LeagueSharp;
using LeagueSharp.Common;

namespace SparkTech.Resources
{
    internal static class Connecting
    {
        static Connecting()
        {
            Game.OnStart += OnGameStart;
            Features();
        }

        internal static void Instance() { }

        private static void OnGameStart(EventArgs args)
        {
            Boot.FireOnInit();
            Game.OnStart -= OnGameStart;
        }

        private static void Features()
        {
            Utility.DelayAction.Add(125, () =>
                {
                    if (Game.Mode != GameMode.Connecting)
                    {
                        return;
                    }

                    Console.WriteLine(@"loading screen xdd");
                });
        }
    }
}