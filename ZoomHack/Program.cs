using LeagueSharp.Common;

namespace ZoomHack
{
    static class Program
    {
        private static void Main()
        {
            CustomEvents.Game.OnGameLoad += eventArgs =>
            {
                SparkTech.Hack.LoadZoomHackStandalone();
            };
        }
    }
}
