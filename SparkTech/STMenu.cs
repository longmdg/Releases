﻿namespace SparkTech
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics.CodeAnalysis;

    using LeagueSharp;
    using LeagueSharp.Common;

    using Resources.Base;

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [SuppressMessage("ReSharper", "ObjectCreationAsStatement")]
    public static class STMenu
    {
        public static Menu LibraryMenu;

        internal static void Create()
        {
            LibraryMenu = new Menu("[ST] Core", "SparkTech", true);

            Menu F5Settings = new Menu("F5Settings", "F5Settings");
            {
                F5Settings.AddItem((new MenuItem("hack", "Load ZoomHack"))).SetValue(false);
            }
            LibraryMenu.AddSubMenu(F5Settings);

            Menu orbmenu = new Menu("BrianWalker", "lelelelell");
            BrianWalker.Init(orbmenu);
            LibraryMenu.AddSubMenu(orbmenu);

            if (Helper.IsFullHD)
            {
                new Extensions();
            }
            
            if (Settings.LoadZoomHack)
            {
                new Cheats();
            }

            LibraryMenu.AddItem(new MenuItem("onupdatedelay", "Delay in checking for menu changes")).SetValue(new Slider(300, 0, 1000));

            Utility.DelayAction.Add(150, () =>
            {
                LibraryMenu.AddToMainMenu();
                Updater.Check(null, true);
                Game.OnUpdate += Settings.OnSettingsChange;
            });
        }
    }
}