using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace SparkTech
{
    internal class Settings
    {
        internal static bool Debug = true;

        public static Menu LibraryMenu;

        internal static void LoadStuff()
        {

            Notifications.AddNotification("Loaded!", 1000);

            LibraryMenu = new Menu("[ST] Core", "SparkTech", true);



            LibraryMenu.AddToMainMenu();
        }
    }
}
