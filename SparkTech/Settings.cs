using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace SparkTech
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    internal class Settings
    {
        internal static bool Debug = true;

        public static Menu LibraryMenu;

        internal static void LoadStuff()
        {

            Notifications.AddNotification("Loaded!", 1000);
            if (Load.MLGloaded)
            {
                Notifications.AddNotification("With MLG!", 1000);
            }

            LibraryMenu = new Menu("[ST] Core", "SparkTech", true);
            var F5settings = new Menu("F5 Settings", "F5");
            {
                F5settings.AddSubMenu(new Menu("", ""));
            }

            LibraryMenu.AddToMainMenu();
        }
    }
}
