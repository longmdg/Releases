using LeagueSharp.Common;

namespace SparkTech
{
    internal class Test
    {
        internal Test()
        {
            Menu orbmenu = new Menu("Orb", "orb1");
            LXOrbwalker.AddToMenu(orbmenu);
            Settings.LibraryMenu.AddSubMenu(orbmenu);
        }
    }
}