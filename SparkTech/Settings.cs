using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

namespace SparkTech
{
    class Settings
    {
        internal static bool MustDebug = true;

        public static Menu LibraryMenu;

        internal static void LoadStuff()
        {
            LibraryMenu = new Menu("[ST] Core", "SparkTech");
            //asd
            LibraryMenu.AddToMainMenu();
        }
    }
}
