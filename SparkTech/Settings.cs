using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;

namespace SparkTech
{
    class Settings
    {
        internal static bool Debug = true;

        public static Menu LibraryMenu;

        internal static void LoadStuff()
        {

            LibraryMenu = new Menu("[ST] Core", "SparkTech");
            //code
            LibraryMenu.AddToMainMenu();
        }
    }
}
