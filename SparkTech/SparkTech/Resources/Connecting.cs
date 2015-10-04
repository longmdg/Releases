using System;

using LeagueSharp;
using LeagueSharp.Common;

namespace SparkTech.Resources
{
    internal static class Connecting
    {
        static Connecting()
        {
            Game.OnStart += OnStart;
            Features();
        }

        internal static void Instance() { }

        private static void OnStart(EventArgs args)
        {
            Game.OnStart -= OnStart;
            MenuST.Instance();
        }

        private static void Features()
        {
            Utility.DelayAction.Add(250, () =>
                {
                    if (Game.Mode != GameMode.Connecting)
                    {
                        return;
                    }

                    Comms.Print("You are on the loading screen!", true);
                });
        }
    }
}