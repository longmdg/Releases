using LeagueSharp.Common;

namespace SparkTech
{
    class Test
    {
        internal Test()
        {
            var orbwalkerMenu = new Menu("Orbwalker", "orbwalker");
            Orbwalker.AddToMenu(orbwalkerMenu);
            Settings.LibraryMenu.AddSubMenu(orbwalkerMenu);
        }
    }
}
