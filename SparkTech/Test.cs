using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orbwalker = SparkTech.Orbwalker;
using LeagueSharp;
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
